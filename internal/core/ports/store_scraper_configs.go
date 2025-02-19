package ports

import (
	"context"
	"itracker/internal/core/domain"
)

type IStoreScraperConfigs interface {
	AddCatalogScraper(ctx context.Context, scraper *domain.CatalogScrapper) error
	GetSchedulableScraperConfigs(ctx context.Context) ([]*struct{ Id, Cron string }, error)
	GetScraperConfig(ctx context.Context, id string) (*domain.ScraperConfig, error)
}