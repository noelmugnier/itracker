package services

import (
	"context"
	"github.com/google/uuid"
	"itracker/internal/domain"
	"itracker/internal/ports"
	"log/slog"
)

type ScraperDefinitionService struct {
	logger       *slog.Logger
	repository   ports.IScraperDefinitionRepository
	timeProvider ports.ITimeProvider
}

func NewScraperDefinitionService(repository ports.IScraperDefinitionRepository, timeProvider ports.ITimeProvider, logger *slog.Logger) *ScraperDefinitionService {
	return &ScraperDefinitionService{
		logger:       logger,
		repository:   repository,
		timeProvider: timeProvider,
	}
}

func (ps *ScraperDefinitionService) CreateCatalogScraperDefinition(ctx context.Context, websiteId string, fields []*domain.ScraperDefinitionField, pagination *domain.ScraperDefinitionPagination, navigation *domain.ScraperDefinitionNavigation) (string, error) {
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

	err = ps.repository.AddCatalogScraperDefinition(ctx, &domain.CreateCatalogScraperDefinition{
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

func (ps *ScraperDefinitionService) CreateProductScraperDefinition(ctx context.Context, websiteId string, fields []*domain.ScraperDefinitionField) (string, error) {
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

	err = ps.repository.AddProductScraperDefinition(ctx, &domain.CreateProductScraperDefinition{
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