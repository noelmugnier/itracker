package ports

import (
	"context"
	"itracker/internal/domain"
)

type IScraperDefinitionRepository interface {
	AddCatalogDefinition(ctx context.Context, definition *domain.CatalogDefinition) error
	AddProductDefinition(ctx context.Context, definition *domain.ProductDefinition) error
}