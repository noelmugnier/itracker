package ports

import (
	"context"
	"itracker/internal/domain"
)

type IScraperRepository interface {
	AddScraper(ctx context.Context, scraper *domain.CreateScraper) error
}