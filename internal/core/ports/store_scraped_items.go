package ports

import (
	"context"
	"itracker/internal/core/domain"
)

type IStoreScrapedItems interface {
	Save(ctx context.Context, websiteId string, definitionId string, scrapedItems []*domain.ScrapedItem) error
	ListItemsToParse(ctx context.Context) ([]*domain.ItemToParse, error)
	GetScrapedItemContent(ctx context.Context, item *domain.ItemToParse) ([]byte, error)
	DeleteScrapedItem(ctx context.Context, item *domain.ItemToParse) error
}