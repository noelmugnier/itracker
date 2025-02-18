package ports

import (
	"context"
	"itracker/internal/core/domain"
)

type IStoreWebsites interface {
	AddWebsite(ctx context.Context, website *domain.Website) error
}