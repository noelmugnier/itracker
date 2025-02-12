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
	db     *sql.DB
}

func NewWebsiteRepository(logger *slog.Logger, db *sql.DB) ports.IWebsiteRepository {
	return &WebsiteRepository{
		logger: logger,
		db:     db,
	}
}

func (pr *WebsiteRepository) AddWebsite(ctx context.Context, website *domain.Website) error {
	_, err := pr.db.Exec(`INSERT INTO websites (id, name, host, created_at) VALUES ($1, $2, $3, $4)`, website.Id, website.Name, website.Host, website.CreatedAt.Unix())
	return err
}