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

func (pr *DefinitionRepository) AddCatalogDefinition(ctx context.Context, definition *domain.CatalogDefinition) error {
	scraperDefinition, err := json.Marshal(definition.Scraper)
	if err != nil {
		pr.logger.Log(ctx, slog.LevelError, "failed to serialize catalog scraper definition", slog.Any("error", err))
		return err
	}

	parserDefinition, err := json.Marshal(definition.Parser)
	if err != nil {
		pr.logger.Log(ctx, slog.LevelError, "failed to serialize catalog parser definition", slog.Any("error", err))
		return err
	}

	scraper := string(scraperDefinition)
	parser := string(parserDefinition)

	_, err = pr.db.Exec(`INSERT INTO definitions (id, type, scraper, parser, created_at, website_id) VALUES ($1, 'catalog', $2, $3, $4, $5)`, definition.Id, scraper, parser, definition.CreatedAt.Unix(), definition.WebsiteId)

	return err
}