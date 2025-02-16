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
	parsedRepository  ports.ISaveScrapedItems
	contentProvider   ports.IProvideWebsiteContent
}

func NewScrapingService(scheduler ports.IScheduleJobs, scraperRepository ports.IScraperRepository, contentProvider ports.IProvideWebsiteContent, parsedRepository ports.ISaveScrapedItems, logger *slog.Logger) *ScrapingService {
	return &ScrapingService{
		logger:            logger,
		scraperRepository: scraperRepository,
		scheduler:         scheduler,
		contentProvider:   contentProvider,
		parsedRepository:  parsedRepository,
	}
}

func (ps *ScrapingService) InitJobs(ctx context.Context) error {
	ps.logger.Log(ctx, slog.LevelInfo, "initializing scraper jobs")

	scrapers, err := ps.scraperRepository.GetEnabledScrapers(ctx)
	if err != nil {
		return err
	}

	for _, scraper := range scrapers {
		scraper.Scraper.Pagination.MaxPage = 5
		_, err := ps.scheduler.ScheduleJob(scraper.Cron, func() {
			ps.logger.Log(ctx, slog.LevelInfo, fmt.Sprintf("running website '%s' %s scraper '%s'", scraper.WebsiteName, scraper.DefinitionType, scraper.Id))

			for urlToScrap, scrap := range scraper.Urls {
				if !scrap {
					ps.logger.Log(ctx, slog.LevelInfo, fmt.Sprintf("skipping url '%s' for website '%s'", urlToScrap, scraper.WebsiteName))
					continue
				}

				page := 1
				for {
					ps.logger.Log(ctx, slog.LevelInfo, fmt.Sprintf("getting items for website '%s' page %d", scraper.WebsiteName, page))

					pagedUrl := fmt.Sprintf("%s?%s=%d", urlToScrap, scraper.Scraper.Pagination.PageNumberParamName, page)
					items, err := ps.contentProvider.GetContent(pagedUrl, scraper.Scraper.ContentSelector)

					if err != nil {
						ps.logger.Log(ctx, slog.LevelError, fmt.Sprintf("failed to get content for website '%s' page %d", scraper.WebsiteName, page), slog.Any("error", err))
						continue
					}

					if len(items) == 0 || page >= scraper.Scraper.Pagination.MaxPage {
						ps.logger.Log(ctx, slog.LevelInfo, fmt.Sprintf("no items found for website '%s' page %d", scraper.WebsiteName, page))
						break
					}

					err = ps.parsedRepository.Save(ctx, scraper.WebsiteId, scraper.DefinitionId, items)
					if err != nil {
						ps.logger.Log(ctx, slog.LevelError, fmt.Sprintf("failed to save items for page %d of website '%s'", page, scraper.WebsiteName), slog.Any("error", err))
						continue
					}

					page++
				}
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