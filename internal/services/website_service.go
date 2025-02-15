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
	logger        *slog.Logger
	wsRepository  ports.IWebsiteRepository
	scRepository  ports.IScraperRepository
	defRepository ports.IDefinitionRepository
	timeProvider  ports.ITimeProvider
}

func NewWebsiteService(websiteRepository ports.IWebsiteRepository, definitionRepository ports.IDefinitionRepository, scraperRepository ports.IScraperRepository, timeProvider ports.ITimeProvider, logger *slog.Logger) *WebsiteService {
	return &WebsiteService{
		logger:        logger,
		wsRepository:  websiteRepository,
		timeProvider:  timeProvider,
		scRepository:  scraperRepository,
		defRepository: definitionRepository,
	}
}

func (ps *WebsiteService) CreateWebsite(ctx context.Context, name string, websiteUrl string) (string, error) {
	if name == "" {
		return "", domain.CreateValidationError(domain.ErrWebsiteNameRequired)
	}

	parsedUrl, err := url.ParseRequestURI(websiteUrl)
	if err != nil {
		return "", domain.CreateValidationError(domain.ErrWebsiteInvalidHost)
	}

	id, err := uuid.NewV7()
	if err != nil {
		return "", err
	}

	websiteToCreate := &domain.Website{
		Id:        id.String(),
		Name:      name,
		CreatedAt: ps.timeProvider.UtcNow(),
		Host:      fmt.Sprintf("%s://%s", parsedUrl.Scheme, parsedUrl.Host),
	}

	err = ps.wsRepository.AddWebsite(ctx, websiteToCreate)
	if err != nil {
		return "", err
	}

	return websiteToCreate.Id, nil
}

func (ps *WebsiteService) CreateCatalogDefinition(ctx context.Context, websiteId string, fields []*domain.DefinitionField, pagination *domain.DefinitionPagination, navigation *domain.DefinitionNavigation) (string, error) {
	if websiteId == "" {
		return "", domain.CreateValidationError(domain.ErrWebsiteIdRequired)
	}

	if len(fields) == 0 {
		return "", domain.CreateValidationError(domain.ErrDefinitionFieldsRequired)
	}

	if pagination == nil {
		return "", domain.CreateValidationError(domain.ErrDefinitionPaginationRequired)
	}

	id, err := uuid.NewV7()
	if err != nil {
		return "", err
	}

	err = ps.defRepository.AddCatalogDefinition(ctx, &domain.CreateCatalogDefinition{
		Id:         id.String(),
		WebsiteId:  websiteId,
		Fields:     fields,
		Pagination: pagination,
		CreatedAt:  ps.timeProvider.UtcNow(),
		Navigation: navigation,
	})

	if err != nil {
		return "", err
	}

	return id.String(), nil
}

func (ps *WebsiteService) CreateProductDefinition(ctx context.Context, websiteId string, fields []*domain.DefinitionField) (string, error) {
	if websiteId == "" {
		return "", domain.CreateValidationError(domain.ErrWebsiteIdRequired)
	}

	if len(fields) == 0 {
		return "", domain.CreateValidationError(domain.ErrDefinitionFieldsRequired)
	}

	id, err := uuid.NewV7()
	if err != nil {
		return "", err
	}

	err = ps.defRepository.AddProductDefinition(ctx, &domain.CreateProductDefinition{
		Id:        id.String(),
		WebsiteId: websiteId,
		Fields:    fields,
		CreatedAt: ps.timeProvider.UtcNow(),
	})

	if err != nil {
		return "", err
	}

	return id.String(), nil
}

func (ps *WebsiteService) CreateScraper(ctx context.Context, websiteId string, urls []string, cron string) (string, error) {
	if websiteId == "" {
		return "", domain.CreateValidationError(domain.ErrWebsiteIdRequired)
	}

	scraperDefinitionId, err := ps.defRepository.GetWebsiteCatalogDefinitionId(ctx, websiteId)
	if err != nil {
		return "", err
	}

	id, err := uuid.NewV7()
	if err != nil {
		return "", err
	}

	urlStatuses := make(map[string]bool)
	for _, url := range urls {
		urlStatuses[url] = true
	}

	err = ps.scRepository.AddScraper(ctx, &domain.CreateScraper{
		Id:           id.String(),
		DefinitionId: scraperDefinitionId,
		CreatedAt:    ps.timeProvider.UtcNow(),
		Enabled:      true,
		Cron:         cron,
		Urls:         urlStatuses,
	})

	if err != nil {
		return "", err
	}

	return id.String(), nil
}