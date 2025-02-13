package ports

import (
	"context"
	"itracker/internal/domain"
)

type IScraperDefinitionRepository interface {
	GetWebsiteCatalogScraperDefinitionId(ctx context.Context, websiteId string) (string, error)
	AddCatalogScraperDefinition(ctx context.Context, definition *domain.CreateCatalogScraperDefinition) error
	AddProductScraperDefinition(ctx context.Context, definition *domain.CreateProductScraperDefinition) error
}