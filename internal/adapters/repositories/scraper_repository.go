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

func (pr *ScraperRepository) AddCatalogScraper(ctx context.Context, scraper *domain.CatalogScrapper) error {
	isEnabled := 1
	if !scraper.Enabled {
		isEnabled = 0
	}

	urls, err := json.Marshal(scraper.Urls)
	if err != nil {
		return err
	}

	_, err = pr.db.Exec(`INSERT INTO scrapers (id, cron, enabled, urls, created_at, definition_id) VALUES ($1, $2, $3, $4, $5, $6)`, scraper.Id, scraper.Cron, isEnabled, string(urls), scraper.CreatedAt.Unix(), scraper.DefinitionId)

	return err
}

func (pr *ScraperRepository) GetEnabledScrapers(ctx context.Context) ([]*domain.Scrapper, error) {
	rows, err := pr.db.Query(`
		SELECT 
			s.id, 
			d.scraper, 
			d.parser, 
			d.type,
			s.cron, 
			s.urls, 
			s.created_at, 
			w.Name as website_name 
		FROM scrapers s
		JOIN definitions d ON s.definition_id = d.id 
		JOIN websites w ON d.website_id = w.id
		WHERE s.enabled = 1`)

	if err != nil {
		return nil, err
	}
	defer rows.Close()

	var scrapers []*domain.Scrapper
	for rows.Next() {
		var id, definitionType, scraper, parser, cron, urls, websiteName string
		var createdAt int64

		err := rows.Scan(&id, &definitionType, &scraper, &parser, &cron, &urls, &createdAt, &websiteName)
		if err != nil {
			return nil, err
		}

		catalogScraper := &domain.Scrapper{
			Id:             id,
			DefinitionType: definitionType,
			CreatedAt:      time.Unix(createdAt, 0),
			Enabled:        true,
			Cron:           cron,
			WebsiteName:    websiteName,
		}

		err = json.Unmarshal([]byte(scraper), &catalogScraper.Scraper)
		if err != nil {
			return nil, err
		}

		err = json.Unmarshal([]byte(parser), &catalogScraper.Parser)
		if err != nil {
			return nil, err
		}

		err = json.Unmarshal([]byte(urls), &catalogScraper.Urls)
		if err != nil {
			return nil, err
		}

		scrapers = append(scrapers, catalogScraper)
	}

	return scrapers, nil
}