package services

import (
	"context"
	"github.com/google/uuid"
	"itracker/internal/domain"
	"itracker/internal/ports"
	"log/slog"
)

type ScraperService struct {
	logger                      *slog.Logger
	scraperRepository           ports.IScraperRepository
	scraperDefinitionRepository ports.IScraperDefinitionRepository
	timeProvider                ports.ITimeProvider
}

func NewScraperService(scraperRepository ports.IScraperRepository, scraperDefinitionRepository ports.IScraperDefinitionRepository, timeProvider ports.ITimeProvider, logger *slog.Logger) *ScraperService {
	return &ScraperService{
		logger:                      logger,
		scraperRepository:           scraperRepository,
		scraperDefinitionRepository: scraperDefinitionRepository,
		timeProvider:                timeProvider,
	}
}

func (ps *ScraperService) CreateScraper(ctx context.Context, websiteId string, urls []string, cron string) (string, error) {
	if websiteId == "" {
		return "", domain.CreateValidationError(domain.ErrWebsiteIdRequired)
	}

	scraperDefinitionId, err := ps.scraperDefinitionRepository.GetWebsiteCatalogScraperDefinitionId(ctx, websiteId)
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

	err = ps.scraperRepository.AddScraper(ctx, &domain.CreateScraper{
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