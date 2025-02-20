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
	definitionRepository  ports.IStoreDefinitions
	parseContent          ports.IParseScrapedItemContent
}

func NewParserService(parseContent ports.IParseScrapedItemContent, definitionRepository ports.IStoreDefinitions, scrapedItemRepository ports.IStoreScrapedItems, logger *slog.Logger) *ParserService {
	return &ParserService{
		logger:                logger,
		scrapedItemRepository: scrapedItemRepository,
		definitionRepository:  definitionRepository,
		parseContent:          parseContent,
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

	parsedFields, err := ps.parseContent.Parse(ctx, fileContent, parserDefinition)
	if err != nil {
		return err
	}

	parsedProduct := &domain.ParsedProduct{
		WebsiteId: item.WebsiteId,
		Id:        parsedFields["identifier"],
		Price:     parsedFields["price"],
		Name:      parsedFields["name"],
		Details:   parsedFields["link"],
		Vintage:   parsedFields["vintage"],
	}

	ps.logger.Log(ctx, slog.LevelInfo, "parsed product", slog.Any("product", parsedProduct))

	//todo save parsed product to dedicated db

	return nil
}