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

func (pr *ScraperDefinitionRepository) GetWebsiteCatalogScraperDefinitionId(ctx context.Context, websiteId string) (string, error) {
	var definitionId string
	err := pr.db.QueryRow("SELECT id FROM scraper_definitions WHERE website_id = $1 AND type = 'catalog'", websiteId).Scan(&definitionId)

	return definitionId, err
}

func (pr *ScraperDefinitionRepository) AddCatalogScraperDefinition(ctx context.Context, definition *domain.CreateCatalogScraperDefinition) error {
	serializedDefinition, err := json.Marshal(struct {
		Fields     []*domain.ScraperDefinitionField    `json:"fields"`
		Pagination *domain.ScraperDefinitionPagination `json:"pagination"`
		Navigation *domain.ScraperDefinitionNavigation `json:"navigation"`
	}{definition.Fields, definition.Pagination, definition.Navigation})

	if err != nil {
		pr.logger.Log(ctx, slog.LevelError, "failed to serialize catalog definition", slog.Any("error", err))
	}

	content := string(serializedDefinition)
	_, err = pr.db.Exec(`INSERT INTO scraper_definitions (id, website_id, type, definition, created_at) VALUES ($1, $2, $3, $4, $5)`, definition.Id, definition.WebsiteId, "catalog", content, definition.CreatedAt.Unix())

	return err
}

func (pr *ScraperDefinitionRepository) AddProductScraperDefinition(ctx context.Context, definition *domain.CreateProductScraperDefinition) error {
	serializedDefinition, err := json.Marshal(struct {
		Fields []*domain.ScraperDefinitionField `json:"fields"`
	}{definition.Fields})

	if err != nil {
		pr.logger.Log(ctx, slog.LevelError, "failed to serialize product definition", slog.Any("error", err))
	}

	content := string(serializedDefinition)
	_, err = pr.db.Exec(`INSERT INTO scraper_definitions (id, website_id, type, definition, created_at) VALUES ($1, $2, $3, $4, $5)`, definition.Id, definition.WebsiteId, "product", content, definition.CreatedAt.Unix())

	return err
}