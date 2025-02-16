package ports

import (
	"context"
	"itracker/internal/domain"
)

type IDefinitionRepository interface {
	AddCatalogDefinition(ctx context.Context, definition *domain.CatalogDefinition) error
}