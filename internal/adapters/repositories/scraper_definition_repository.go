package repositories

import (
	"context"
	"database/sql"
	"encoding/json"
	"itracker/internal/domain"
	"itracker/internal/ports"
	"log/slog"
)

type ScraperDefinitionRepository struct {
	logger *slog.Logger
	db     *sql.DB
}

func NewScraperDefinitionRepository(logger *slog.Logger, db *sql.DB) ports.IScraperDefinitionRepository {
	return &ScraperDefinitionRepository{
		logger: logger,
		db:     db,
	}
}

func (pr *ScraperDefinitionRepository) AddCatalogDefinition(ctx context.Context, definition *domain.CatalogDefinition) error {
	serializedDefinition, err := json.Marshal(struct {
		Fields     []*domain.DefinitionField    `json:"fields"`
		Pagination *domain.PaginationDefinition `json:"pagination"`
		Navigation *domain.ProductNavigation    `json:"navigation"`
	}{definition.Fields, definition.Pagination, definition.Navigation})

	if err != nil {
		pr.logger.Log(ctx, slog.LevelError, "failed to serialize catalog definition", slog.Any("error", err))
	}

	content := string(serializedDefinition)
	_, err = pr.db.Exec(`INSERT INTO scraper_definitions (id, website_id, type, definition, created_at) VALUES ($1, $2, $3, $4, $5)`, definition.Id, definition.WebsiteId, "catalog", content, definition.CreatedAt.Unix())

	return err
}

func (pr *ScraperDefinitionRepository) AddProductDefinition(ctx context.Context, definition *domain.ProductDefinition) error {
	serializedDefinition, err := json.Marshal(struct {
		Fields []*domain.DefinitionField `json:"fields"`
	}{definition.Fields})

	if err != nil {
		pr.logger.Log(ctx, slog.LevelError, "failed to serialize product definition", slog.Any("error", err))
	}

	content := string(serializedDefinition)
	_, err = pr.db.Exec(`INSERT INTO scraper_definitions (id, website_id, type, definition, created_at) VALUES ($1, $2, $3, $4, $5)`, definition.Id, definition.WebsiteId, "product", content, definition.CreatedAt.Unix())

	return err
}