package services

import (
	"context"
	"fmt"
	"github.com/google/uuid"
	"itracker/internal/core/domain"
	"itracker/internal/core/ports"
	"log/slog"
	"net/url"
)

type WebsiteService struct {
	logger        *slog.Logger
	wsRepository  ports.IStoreWebsites
	scRepository  ports.IStoreScrapers
	defRepository ports.IStoreDefinitions
	timeProvider  ports.IProvideTime
}

func NewWebsiteService(websiteRepository ports.IStoreWebsites, definitionRepository ports.IStoreDefinitions, scraperRepository ports.IStoreScrapers, timeProvider ports.IProvideTime, logger *slog.Logger) *WebsiteService {
	return &WebsiteService{
		logger:        logger,
		wsRepository:  websiteRepository,
		timeProvider:  timeProvider,
		scRepository:  scraperRepository,
		defRepository: definitionRepository,
	}
}

func (ps *WebsiteService) CreateWebsite(ctx context.Context, createWebsite *domain.CreateWebsite) (string, error) {
	if createWebsite.Name == "" {
		return "", domain.CreateValidationError(domain.ErrWebsiteNameRequired)
	}

	parsedUrl, err := url.ParseRequestURI(createWebsite.Url)
	if err != nil {
		return "", domain.CreateValidationError(domain.ErrWebsiteInvalidHost)
	}

	id, err := uuid.NewV7()
	if err != nil {
		return "", err
	}

	websiteToCreate := &domain.Website{
		Id:        id.String(),
		Name:      createWebsite.Name,
		CreatedAt: ps.timeProvider.UtcNow(),
		Host:      fmt.Sprintf("%s://%s", parsedUrl.Scheme, parsedUrl.Host),
	}

	err = ps.wsRepository.AddWebsite(ctx, websiteToCreate)
	return websiteToCreate.Id, err
}

func (ps *WebsiteService) CreateCatalogDefinition(ctx context.Context, createDefinition *domain.CreateCatalogDefinition) (string, error) {
	id, err := uuid.NewV7()
	if err != nil {
		return "", err
	}

	definition := &domain.CatalogDefinition{
		Id:        id.String(),
		WebsiteId: createDefinition.WebsiteId,
		CreatedAt: ps.timeProvider.UtcNow(),
		Scraper:   createDefinition.Scraper,
		Parser:    createDefinition.Parser,
	}

	err = ps.defRepository.AddCatalogDefinition(ctx, definition)
	return definition.Id, err
}

func (ps *WebsiteService) CreateCatalogScraper(ctx context.Context, createScraper *domain.CreateCatalogScraper) (string, error) {
	id, err := uuid.NewV7()
	if err != nil {
		return "", err
	}

	urlStatuses := make(map[string]bool)
	for _, urlToScrap := range createScraper.Urls {
		urlStatuses[urlToScrap] = true
	}

	scraper := &domain.CatalogScrapper{
		Id:           id.String(),
		DefinitionId: createScraper.DefinitionId,
		CreatedAt:    ps.timeProvider.UtcNow(),
		Enabled:      true,
		Cron:         createScraper.Cron,
		Urls:         urlStatuses,
	}

	err = ps.scRepository.AddCatalogScraper(ctx, scraper)
	return scraper.Id, err
}