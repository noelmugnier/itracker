package repositories

import (
	"context"
	"database/sql"
	"encoding/json"
	"itracker/internal/domain"
	"itracker/internal/ports"
	"log/slog"
)

type ScraperRepository struct {
	logger *slog.Logger
	db     *sql.DB
}

func NewScraperRepository(logger *slog.Logger, db *sql.DB) ports.IScraperRepository {
	return &ScraperRepository{
		logger: logger,
		db:     db,
	}
}

func (pr *ScraperRepository) AddScraper(ctx context.Context, scraper *domain.CreateScraper) error {
	isEnabled := 1
	if !scraper.Enabled {
		isEnabled = 0
	}

	urls, err := json.Marshal(scraper.Urls)
	if err != nil {
		return err
	}

	_, err = pr.db.Exec(`INSERT INTO scrapers (id, scraper_definition_id, enabled, cron, urls, created_at) VALUES ($1, $2, $3, $4, $5, $6)`, scraper.Id, scraper.DefinitionId, isEnabled, scraper.Cron, string(urls), scraper.CreatedAt.Unix())

	return err
}