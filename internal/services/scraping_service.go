package services

import (
	"context"
	"fmt"
	"itracker/internal/ports"
	"log/slog"
)

type ScrapingService struct {
	logger            *slog.Logger
	scheduler         ports.IScheduleJobs
	scraperRepository ports.IScraperRepository
	contentProvider   ports.IProvideWebsiteContent
}

func NewScrapingService(scheduler ports.IScheduleJobs, scraperRepository ports.IScraperRepository, contentProvider ports.IProvideWebsiteContent, logger *slog.Logger) *ScrapingService {
	return &ScrapingService{
		logger:            logger,
		scraperRepository: scraperRepository,
		scheduler:         scheduler,
		contentProvider:   contentProvider,
	}
}

func (ps *ScrapingService) InitJobs(ctx context.Context) error {
	ps.logger.Log(ctx, slog.LevelInfo, "initializing scraper jobs")

	scrapers, err := ps.scraperRepository.GetEnabledScrapers(ctx)
	if err != nil {
		return err
	}

	for _, scraper := range scrapers {
		_, err := ps.scheduler.ScheduleJob(scraper.Cron, func() {
			ps.logger.Log(ctx, slog.LevelInfo, fmt.Sprintf("running website '%s' %s scraper '%s'", scraper.WebsiteName, scraper.DefinitionType, scraper.Id))

			for urlToScrap, scrap := range scraper.Urls {
				if !scrap {
					ps.logger.Log(ctx, slog.LevelInfo, fmt.Sprintf("skipping url '%s' for website '%s'", urlToScrap, scraper.WebsiteName))
					continue
				}

				content, err := ps.contentProvider.GetContent(urlToScrap, scraper.Scraper.ContentSelector)

				if err != nil {
					ps.logger.Log(ctx, slog.LevelError, fmt.Sprintf("failed to get content for website '%s'", scraper.WebsiteName), slog.Any("error", err))
					return
				}

				fmt.Println(content)
			}
		})

		if err != nil {
			ps.logger.Log(ctx, slog.LevelError, fmt.Sprintf("failed to schedule website '%s' scraper '%s'", scraper.WebsiteName, scraper.Id), slog.Any("error", err))
			continue
		}

		ps.logger.Log(ctx, slog.LevelInfo, fmt.Sprintf("website '%s' %s scraper '%s' scheduled as '%s'", scraper.WebsiteName, scraper.DefinitionType, scraper.Id, scraper.Cron))
	}

	ps.logger.Log(ctx, slog.LevelInfo, "scraper jobs initialized")
	return nil
}

func (ps *ScrapingService) Start(ctx context.Context) {
	ps.scheduler.StartScheduler()
}