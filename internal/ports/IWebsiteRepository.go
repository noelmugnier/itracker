package ports

import (
	"context"
	"itracker/internal/domain"
)

type IWebsiteRepository interface {
	AddWebsite(ctx context.Context, website *domain.Website) error
}
