package services

import (
	"context"
	"fmt"
	"itracker/internal/domain"
	"itracker/internal/ports"
	"log/slog"
	"net/url"

	"github.com/google/uuid"
)

type WebsiteService struct {
	logger       *slog.Logger
	repository   ports.IWebsiteRepository
	timeProvider ports.ITimeProvider
}

func NewWebsiteService(logger *slog.Logger, repository ports.IWebsiteRepository, timeProvider ports.ITimeProvider) *WebsiteService {
	return &WebsiteService{
		logger:       logger,
		repository:   repository,
		timeProvider: timeProvider,
	}
}

func (ps *WebsiteService) CreateWebsite(ctx context.Context, name string, websiteUrl string) (string, error) {
	if name == "" {
		return "", domain.CreateValidationError(domain.ErrWebsiteNameRequired)
	}

	if websiteUrl == "" {
		return "", domain.CreateValidationError(domain.ErrWebsiteUrlRequired)
	}

	parsedUrl, err := url.ParseRequestURI(websiteUrl)
	if err != nil {
		return "", domain.CreateValidationError(domain.ErrWebsiteInvalidUrl)
	}

	id, err := uuid.NewV7()
	if err != nil {
		return "", err
	}

	websiteToCreate := &domain.Website{
		Id:        id.String(),
		Name:      name,
		CreatedAt: ps.timeProvider.UtcNow(),
		Url:       fmt.Sprintf("%s://%s", parsedUrl.Scheme, parsedUrl.Host),
	}

	err = ps.repository.AddWebsite(ctx, websiteToCreate)
	if err != nil {
		return "", err
	}

	return websiteToCreate.Id, nil
}
