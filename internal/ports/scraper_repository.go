package ports

import (
	"context"
	"itracker/internal/domain"
)

type IScraperRepository interface {
	AddCatalogScraper(ctx context.Context, scraper *domain.CatalogScrapper) error
	GetEnabledScrapers(ctx context.Context) ([]*domain.Scrapper, error)
}