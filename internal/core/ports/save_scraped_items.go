package ports

import (
	"context"
	"itracker/internal/core/domain"
)

type IStoreScrapedItems interface {
	Save(ctx context.Context, websiteId string, definitionId string, scrapedItems []domain.ScrapedItem) error
}