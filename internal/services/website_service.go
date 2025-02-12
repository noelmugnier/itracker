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
	logger               *slog.Logger
	websiteRepository    ports.IWebsiteRepository
	definitionRepository ports.IScraperDefinitionRepository
	timeProvider         ports.ITimeProvider
}

func NewWebsiteService(logger *slog.Logger, websiteRepository ports.IWebsiteRepository, definitionRepository ports.IScraperDefinitionRepository, timeProvider ports.ITimeProvider) *WebsiteService {
	return &WebsiteService{
		logger:               logger,
		websiteRepository:    websiteRepository,
		definitionRepository: definitionRepository,
		timeProvider:         timeProvider,
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

	err = ps.websiteRepository.AddWebsite(ctx, websiteToCreate)
	if err != nil {
		return "", err
	}

	return websiteToCreate.Id, nil
}

func (ps *WebsiteService) CreateCatalogScraperDefinitionForWebsite(ctx context.Context, websiteId string, fields []*domain.DefinitionField, pagination *domain.PaginationDefinition, navigation *domain.ProductNavigation) (string, error) {
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

	err = ps.definitionRepository.AddCatalogDefinition(ctx, &domain.CatalogDefinition{
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

func (ps *WebsiteService) CreateProductScraperDefinitionForWebsite(ctx context.Context, websiteId string, fields []*domain.DefinitionField) (string, error) {
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

	err = ps.definitionRepository.AddProductDefinition(ctx, &domain.ProductDefinition{
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