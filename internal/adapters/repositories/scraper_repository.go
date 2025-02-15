package repositories

import (
	"context"
	"database/sql"
	"encoding/json"
	"itracker/internal/domain"
	"itracker/internal/ports"
	"log/slog"
	"time"
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

func (pr *ScraperRepository) GetScrapers(ctx context.Context) ([]*domain.Scrapper, error) {
	rows, err := pr.db.Query(`
		SELECT 
			s.id, 
			sd.id as scraper_definition_id,
			sd.type,
			sd.definition, 
			s.enabled, 
			s.cron, 
			s.urls, 
			s.created_at, 
			w.Id as website_id, 
			w.Name as website_name 
		FROM scrapers s 
		JOIN scraper_definitions sd ON s.scraper_definition_id = sd.id 
		JOIN websites w ON sd.website_id = w.id`)

	if err != nil {
		return nil, err
	}
	defer rows.Close()

	var scrapers []*domain.Scrapper
	for rows.Next() {
		scraper := &domain.Scrapper{
			Website:    domain.WebsiteInfo{},
			Definition: domain.DefinitionInfo{},
		}
		var urls string
		var definition string
		var createdAt int64
		err := rows.Scan(&scraper.Id, &scraper.Definition.Id, &scraper.Type, &definition, &scraper.Enabled, &scraper.Cron, &urls, &createdAt, &scraper.Website.Id, &scraper.Website.Name)
		if err != nil {
			return nil, err
		}

		scraper.CreatedAt = time.Unix(createdAt, 0)

		err = json.Unmarshal([]byte(definition), &scraper.Definition.Definition)
		if err != nil {
			return nil, err
		}

		err = json.Unmarshal([]byte(urls), &scraper.Urls)
		if err != nil {
			return nil, err
		}

		scrapers = append(scrapers, scraper)
	}

	return scrapers, nil
}