package ports

import (
	"context"
	"itracker/internal/core/domain"
)

type IStoreDefinitions interface {
	AddCatalogDefinition(ctx context.Context, definition *domain.CatalogDefinition) error
	GetDefinitionParser(ctx context.Context, definitionId string) (*domain.ParserCatalogDefinition, error)
}