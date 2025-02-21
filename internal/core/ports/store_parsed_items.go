package ports

import (
	"context"
	"itracker/internal/core/domain"
)

type IStoreParsedItems interface {
	Save(ctx context.Context, item *domain.ParsedProduct) error
}