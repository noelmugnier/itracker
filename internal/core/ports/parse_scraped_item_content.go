package ports

import (
	"context"
	"itracker/internal/core/domain"
)

type IParseScrapedItemContent interface {
	Parse(ctx context.Context, content []byte, fields []*domain.FieldDefinition) (map[string]string, error)
}