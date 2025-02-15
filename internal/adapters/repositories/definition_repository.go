package repositories

import (
	"context"
	"database/sql"
	"encoding/json"
	"itracker/internal/domain"
	"itracker/internal/ports"
	"log/slog"
)

type DefinitionRepository struct {
	logger *slog.Logger
	db     *sql.DB
}

func NewDefinitionRepository(logger *slog.Logger, db *sql.DB) ports.IDefinitionRepository {
	return &DefinitionRepository{
		logger: logger,
		db:     db,
	}
}

func (pr *DefinitionRepository) GetWebsiteCatalogDefinitionId(ctx context.Context, websiteId string) (string, error) {
	var definitionId string
	err := pr.db.QueryRow("SELECT id FROM scraper_definitions WHERE website_id = $1 AND type = 'catalog'", websiteId).Scan(&definitionId)

	return definitionId, err
}

func (pr *DefinitionRepository) AddCatalogDefinition(ctx context.Context, definition *domain.CreateCatalogDefinition) error {
	serializedDefinition, err := json.Marshal(&domain.Definition{Fields: definition.Fields, Pagination: definition.Pagination, Navigation: definition.Navigation})

	if err != nil {
		pr.logger.Log(ctx, slog.LevelError, "failed to serialize catalog definition", slog.Any("error", err))
		return err
	}

	content := string(serializedDefinition)
	_, err = pr.db.Exec(`INSERT INTO scraper_definitions (id, website_id, type, definition, created_at) VALUES ($1, $2, $3, $4, $5)`, definition.Id, definition.WebsiteId, "catalog", content, definition.CreatedAt.Unix())

	return err
}

func (pr *DefinitionRepository) AddProductDefinition(ctx context.Context, definition *domain.CreateProductDefinition) error {
	serializedDefinition, err := json.Marshal(&domain.Definition{Fields: definition.Fields, Pagination: nil, Navigation: nil})

	if err != nil {
		pr.logger.Log(ctx, slog.LevelError, "failed to serialize product definition", slog.Any("error", err))
		return err
	}

	content := string(serializedDefinition)
	_, err = pr.db.Exec(`INSERT INTO scraper_definitions (id, website_id, type, definition, created_at) VALUES ($1, $2, $3, $4, $5)`, definition.Id, definition.WebsiteId, "product", content, definition.CreatedAt.Unix())

	return err
}