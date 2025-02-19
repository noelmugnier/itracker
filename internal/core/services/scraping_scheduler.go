package services

import (
	"context"
	"fmt"
	"itracker/internal/core/ports"
	"log/slog"
)

type SchedulerService struct {
	logger                  *slog.Logger
	scheduler               ports.IScheduleJobs
	scraperConfigRepository ports.IStoreScraperConfigs
	scraper                 *ScraperService
}

func NewSchedulerService(scheduler ports.IScheduleJobs, scraperConfigRepository ports.IStoreScraperConfigs, scraper *ScraperService, logger *slog.Logger) *SchedulerService {
	return &SchedulerService{
		logger:                  logger,
		scraperConfigRepository: scraperConfigRepository,
		scheduler:               scheduler,
		scraper:                 scraper,
	}
}

func (ps *SchedulerService) ScheduleScrapers(ctx context.Context) error {
	ps.logger.Log(ctx, slog.LevelInfo, "initializing scraper jobs")

	scraperConfigs, err := ps.scraperConfigRepository.GetSchedulableScraperConfigs(ctx)
	if err != nil {
		return err
	}

	for _, scraperConfig := range scraperConfigs {
		_, err := ps.scheduler.ScheduleJob(scraperConfig.Cron, func(scraper *ScraperService) {
			scraper.Scrap(ctx, scraperConfig.Id)
		}, ps.scraper)

		if err != nil {
			ps.logger.Log(ctx, slog.LevelError, fmt.Sprintf("failed to schedule scraper '%s'", scraperConfig.Id), slog.Any("error", err))
			continue
		}

		ps.logger.Log(ctx, slog.LevelInfo, fmt.Sprintf("scraper '%s' scheduled as '%s'", scraperConfig.Id, scraperConfig.Cron))
	}

	ps.logger.Log(ctx, slog.LevelInfo, "scraper jobs initialized")
	return nil
}