package outbound

import (
	"context"
	"database/sql"
	"encoding/json"
	"itracker/internal/core/domain"
	"log/slog"
	"time"
)

func NewScraperConfigRepository(logger *slog.Logger, db *sql.DB) *ScraperConfigRepository {
	return &ScraperConfigRepository{
		logger: logger,
		db:     db,
	}
}

type ScraperConfigRepository struct {
	logger *slog.Logger
	db     *sql.DB
}

func (pr *ScraperConfigRepository) AddCatalogScraper(ctx context.Context, scraper *domain.CatalogScrapper) error {
	isEnabled := 1
	if !scraper.Enabled {
		isEnabled = 0
	}

	urls, err := json.Marshal(scraper.Urls)
	if err != nil {
		return err
	}

	_, err = pr.db.Exec(`INSERT INTO scraper_configs (id, cron, enabled, urls, created_at, definition_id) VALUES ($1, $2, $3, $4, $5, $6)`, scraper.Id, scraper.Cron, isEnabled, string(urls), scraper.CreatedAt.Unix(), scraper.DefinitionId)

	return err
}

func (pr *ScraperConfigRepository) GetSchedulableScraperConfigs(ctx context.Context) ([]*struct{ Id, Cron string }, error) {
	rows, err := pr.db.Query(`
		SELECT 
			s.id,
			s.cron
		FROM scraper_configs s
		WHERE s.enabled = 1`)

	if err != nil {
		return nil, err
	}
	defer rows.Close()

	var scraperConfigs = make([]*struct{ Id, Cron string }, 0)
	for rows.Next() {
		var id, cron string

		err := rows.Scan(&id, &cron)
		if err != nil {
			return nil, err
		}

		scraperConfigs = append(scraperConfigs, &struct{ Id, Cron string }{Id: id, Cron: cron})
	}

	return scraperConfigs, nil
}

func (pr *ScraperConfigRepository) GetScraperConfig(ctx context.Context, id string) (*domain.ScraperConfig, error) {
	row := pr.db.QueryRow(`
		SELECT 
			d.id as definition_id,
			d.type,
			d.scraper, 
			s.enabled,
			s.cron, 
			s.urls, 
			s.created_at, 
			w.Id as website_id,
			w.Name as website_name 
		FROM scraper_configs s
		JOIN definitions d ON s.definition_id = d.id 
		JOIN websites w ON d.website_id = w.id
		WHERE s.id = $1`)

	var definitionId, definitionType, scraper, cron, urls, websiteId, websiteName string
	var createdAt, enabled int64
	err := row.Scan(&definitionId, &definitionType, &scraper, &enabled, &cron, &urls, &createdAt, &websiteId, &websiteName)

	if err != nil {
		return nil, err
	}

	scraperConfig := &domain.ScraperConfig{
		Id:             id,
		DefinitionId:   definitionId,
		DefinitionType: definitionType,
		CreatedAt:      time.Unix(createdAt, 0),
		Enabled:        enabled == 1,
		Cron:           cron,
		WebsiteId:      websiteId,
		WebsiteName:    websiteName,
	}

	err = json.Unmarshal([]byte(scraper), &scraperConfig.ScraperDefinition)
	if err != nil {
		return nil, err
	}

	err = json.Unmarshal([]byte(urls), &scraperConfig.Urls)
	if err != nil {
		return nil, err
	}

	return scraperConfig, nil
}