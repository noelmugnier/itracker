package ports

import (
	"context"
	"itracker/internal/domain"
)

type ISaveScrapedItems interface {
	Save(ctx context.Context, websiteId string, definitionId string, scrapedItems []domain.ScrapedItem) error
}