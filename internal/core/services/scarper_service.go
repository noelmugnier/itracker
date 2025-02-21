package services

import (
	"context"
	"fmt"
	"itracker/internal/core/ports"
	"log/slog"
)

type ScraperService struct {
	logger            *slog.Logger
	scraperRepository ports.IStoreScraperConfigs
	parsedRepository  ports.IStoreScrapedItems
	contentProvider   ports.IRetrieveWebsiteContent
}

func NewScraperService(scraperRepository ports.IStoreScraperConfigs, contentProvider ports.IRetrieveWebsiteContent, parsedRepository ports.IStoreScrapedItems, logger *slog.Logger) *ScraperService {
	return &ScraperService{
		logger:            logger,
		contentProvider:   contentProvider,
		parsedRepository:  parsedRepository,
		scraperRepository: scraperRepository,
	}
}

func (ps *ScraperService) Scrap(ctx context.Context, scraperId string) {
	ps.logger.Log(ctx, slog.LevelInfo, fmt.Sprintf("retrieving scraper '%s' config", scraperId))

	scraper, err := ps.scraperRepository.GetScraperConfig(ctx, scraperId)

	if err != nil {
		ps.logger.Log(ctx, slog.LevelError, fmt.Sprintf("failed to get scraper '%s' config", scraperId), slog.Any("error", err))
		return
	}

	ps.logger.Log(ctx, slog.LevelInfo, fmt.Sprintf("running website '%s' %s scraper '%s'", scraper.WebsiteName, scraper.DefinitionType, scraper.Id))

	for urlToScrap, scrap := range scraper.Urls {
		if !scrap {
			ps.logger.Log(ctx, slog.LevelInfo, fmt.Sprintf("skipping url '%s' for website '%s'", urlToScrap, scraper.WebsiteName))
			continue
		}

		page := 1
		for {
			ps.logger.Log(ctx, slog.LevelInfo, fmt.Sprintf("getting items for website '%s' page %d", scraper.WebsiteName, page))

			pagedUrl := fmt.Sprintf("%s?%s=%d", urlToScrap, scraper.ScraperDefinition.Pagination.PageNumberParamName, page)
			items, err := ps.contentProvider.Retrieve(pagedUrl, scraper.ScraperDefinition.ContentSelector)

			if err != nil {
				ps.logger.Log(ctx, slog.LevelError, fmt.Sprintf("failed to get content for website '%s' page %d", scraper.WebsiteName, page), slog.Any("error", err))
				continue
			}

			if len(items) == 0 || page >= scraper.ScraperDefinition.Pagination.MaxPage {
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
}