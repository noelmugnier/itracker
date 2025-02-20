package ports

import (
	"context"
	"itracker/internal/core/domain"
)

type IParseScrapedItemContent interface {
	Parse(ctx context.Context, content []byte, parserDefinition *domain.ParserCatalogDefinition) (map[string]string, error)
}