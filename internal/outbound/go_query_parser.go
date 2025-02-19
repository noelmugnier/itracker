package outbound

import (
	"bytes"
	"context"
	"github.com/PuerkitoBio/goquery"
	"itracker/internal/core/domain"
	"log/slog"
)

type GoQueryParser struct {
	logger *slog.Logger
}

func NewGoQueryParser(logger *slog.Logger) *GoQueryParser {
	return &GoQueryParser{
		logger: logger,
	}
}

func (gqp *GoQueryParser) Parse(ctx context.Context, content []byte, parserDefinition *domain.ParserCatalogDefinition) ([]*domain.ParsedField, error) {
	doc, err := goquery.NewDocumentFromReader(bytes.NewReader(content))
	if err != nil {
		return nil, err
	}

	var parsedFields []*domain.ParsedField

	for _, field := range parserDefinition.Fields {
		doc.
			Find(field.Selector).
			Each(func(i int, s *goquery.Selection) {
				txt := s.Text()
				parsedFields = append(parsedFields, &domain.ParsedField{
					Identifier: field.Identifier,
					Value:      txt,
				})
			})
	}

	return parsedFields, nil
}