package adapters

import (
	"context"
	"fmt"
	"itracker/internal/domain"
	"log/slog"
)

type SaveScrapedItemsOnFile struct {
	logger *slog.Logger
}

func NewSaveScrapedItemsOnFile(logger *slog.Logger) *SaveScrapedItemsOnFile {
	return &SaveScrapedItemsOnFile{
		logger: logger,
	}
}

func (s *SaveScrapedItemsOnFile) Save(ctx context.Context, websiteId string, definitionId string, scrapedItems []domain.ScrapedItem) error {
	for _, item := range scrapedItems {
		s.logger.Log(ctx, slog.LevelInfo, fmt.Sprintf("saving scraped items on file: %s", item), slog.Any("websiteId", websiteId), slog.Any("definitionId", definitionId))
	}

	return nil
}