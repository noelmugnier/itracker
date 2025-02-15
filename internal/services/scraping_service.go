package services

import (
	"context"
	"fmt"
	"itracker/internal/ports"
	"log/slog"
)

type ScrapingService struct {
	logger            *slog.Logger
	scheduler         ports.IScheduler
	scraperRepository ports.IScraperRepository
}

func NewScrapingService(scheduler ports.IScheduler, scraperRepository ports.IScraperRepository, logger *slog.Logger) *ScrapingService {
	return &ScrapingService{
		logger:            logger,
		scraperRepository: scraperRepository,
		scheduler:         scheduler,
	}
}

func (ps *ScrapingService) InitJobs(ctx context.Context) error {
	ps.logger.Log(ctx, slog.LevelInfo, "initializing scraper jobs")

	scrapers, err := ps.scraperRepository.GetScrapers(ctx)
	if err != nil {
		return err
	}

	for _, scraper := range scrapers {
		_, err := ps.scheduler.ScheduleJob(scraper.Cron, func() {
			ps.logger.Log(ctx, slog.LevelInfo, fmt.Sprintf("running website '%s' %s scraper %s", scraper.Website.Name, scraper.Type, scraper.Id))
		})

		if err != nil {
			ps.logger.Log(ctx, slog.LevelError, fmt.Sprintf("failed to schedule website '%s' scraper '%s'", scraper.Website.Name, scraper.Id), slog.Any("error", err))
			continue
		}

		ps.logger.Log(ctx, slog.LevelInfo, fmt.Sprintf("website '%s' %s scraper '%s' scheduled as '%s'", scraper.Website.Name, scraper.Type, scraper.Id, scraper.Cron))
	}

	ps.logger.Log(ctx, slog.LevelInfo, "scraper jobs initialized")
	return nil
}

func (ps *ScrapingService) Start(ctx context.Context) {
	ps.scheduler.Start()
}