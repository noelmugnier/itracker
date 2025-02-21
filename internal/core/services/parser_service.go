package services

import (
	"context"
	"fmt"
	"itracker/internal/core/domain"
	"itracker/internal/core/ports"
	"log/slog"
)

type ParserService struct {
	logger                *slog.Logger
	scrapedItemRepository ports.IStoreScrapedItems
	parsedItemRepository  ports.IStoreParsedItems
	definitionRepository  ports.IStoreDefinitions
	parseContent          ports.IParseScrapedItemContent
}

func NewParserService(parseContent ports.IParseScrapedItemContent, parsedItemRepository ports.IStoreParsedItems, definitionRepository ports.IStoreDefinitions, scrapedItemRepository ports.IStoreScrapedItems, logger *slog.Logger) *ParserService {
	return &ParserService{
		logger:                logger,
		scrapedItemRepository: scrapedItemRepository,
		definitionRepository:  definitionRepository,
		parseContent:          parseContent,
		parsedItemRepository:  parsedItemRepository,
	}
}

func (ps *ParserService) ParseScrapedItems(ctx context.Context) {
	items, err := ps.scrapedItemRepository.ListItemsToParse(ctx)
	if err != nil {
		ps.logger.Error("failed to get items to parse: %w", err)
		return
	}

	for _, item := range items {
		err := ps.ParseItem(ctx, item)
		if err != nil {
			ps.logger.Error("failed to parse item: %w", err)
			continue
		}
	}
}

func (ps *ParserService) ParseItem(ctx context.Context, item *domain.ItemToParse) error {
	ps.logger.Log(ctx, slog.LevelInfo, fmt.Sprintf("parsing item %s", item.FileName), slog.Any("item", item))
	parserDefinition, err := ps.definitionRepository.GetDefinitionParser(ctx, item.DefinitionId)
	if err != nil {
		return err
	}

	fileContent, err := ps.scrapedItemRepository.GetScrapedItemContent(ctx, item)
	if err != nil {
		return err
	}

	fields := getDefinitionFields(parserDefinition)
	parsedFields, err := ps.parseContent.Parse(ctx, fileContent, fields)
	if err != nil {
		return err
	}

	parsedProduct := &domain.ParsedProduct{
		WebsiteId: item.WebsiteId,
		Id:        parsedFields[IdentifierField],
		UnitPrice: parsedFields[UnitPriceField],
		Name:      parsedFields[NameField],
		Details:   parsedFields[DetailsField],
		ScrapedAt: item.ScrapedAt,
	}

	delete(parsedFields, IdentifierField)
	delete(parsedFields, UnitPriceField)
	delete(parsedFields, NameField)
	delete(parsedFields, DetailsField)

	parsedProduct.AdditionalFields = parsedFields

	err = ps.parsedItemRepository.Save(ctx, parsedProduct)

	return ps.scrapedItemRepository.DeleteScrapedItem(ctx, item)
}

const (
	IdentifierField = "identifier"
	NameField       = "name"
	UnitPriceField  = "unit_price"
	DetailsField    = "details"
)

func getDefinitionFields(definition *domain.ParserCatalogDefinition) []*domain.FieldDefinition {
	fields := make([]*domain.FieldDefinition, 0)

	fields = append(fields, getFieldDefinition(IdentifierField, definition.IdentifierField))
	fields = append(fields, getFieldDefinition(NameField, definition.NameField))
	fields = append(fields, getFieldDefinition(UnitPriceField, definition.UnitPriceField))
	fields = append(fields, getFieldDefinition(DetailsField, definition.DetailsField))

	fields = append(fields, definition.AdditionalFields...)
	return fields

}

func getFieldDefinition(identifier string, parserSelector *domain.ParserSelector) *domain.FieldDefinition {
	return &domain.FieldDefinition{
		Identifier:     identifier,
		Required:       true,
		ParserSelector: parserSelector,
	}
}