package ports

import (
	"context"
	"itracker/internal/domain"
)

type IProductRepository interface {
	AddProduct(product *domain.Product) error
}

type IWebsiteRepository interface {
	AddWebsite(ctx context.Context, website *domain.Website) error
}

type IDefinitionRepository interface {
	AddCatalogDefinition(ctx context.Context, definition *domain.CatalogDefinition) error
}

type IScraperRepository interface {
	AddCatalogScraper(ctx context.Context, scraper *domain.CatalogScrapper) error
	GetEnabledScrapers(ctx context.Context) ([]*domain.Scrapper, error)
}