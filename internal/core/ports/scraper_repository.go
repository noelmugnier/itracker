package ports

import (
	"context"
	"itracker/internal/core/domain"
)

type IStoreScrapers interface {
	AddCatalogScraper(ctx context.Context, scraper *domain.CatalogScrapper) error
	GetEnabledScrapers(ctx context.Context) ([]*domain.Scrapper, error)
}