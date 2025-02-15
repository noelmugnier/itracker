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
	GetWebsiteCatalogDefinitionId(ctx context.Context, websiteId string) (string, error)
	AddCatalogDefinition(ctx context.Context, definition *domain.CreateCatalogDefinition) error
	AddProductDefinition(ctx context.Context, definition *domain.CreateProductDefinition) error
}

type IScraperRepository interface {
	AddScraper(ctx context.Context, scraper *domain.CreateScraper) error
	GetScrapers(ctx context.Context) ([]*domain.Scrapper, error)
}