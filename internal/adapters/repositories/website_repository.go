package repositories

import (
	"context"
	"database/sql"
	"itracker/internal/domain"
	"itracker/internal/ports"
	"log/slog"
)

type WebsiteRepository struct {
	logger *slog.Logger
	dbConn *sql.DB
}

func NewWebsiteRepository(logger *slog.Logger, dbConn *sql.DB) ports.IWebsiteRepository {
	return &WebsiteRepository{
		logger: logger,
		dbConn: dbConn,
	}
}

func (pr *WebsiteRepository) AddWebsite(ctx context.Context, website *domain.Website) error {
	_, err := pr.dbConn.Exec(`INSERT INTO websites (id, name, url, created_at) VALUES ($1, $2, $3, $4)`, website.Id, website.Name, website.Url, website.CreatedAt.Unix())
	return err
}
