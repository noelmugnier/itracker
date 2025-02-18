package outbound

import (
	"context"
	"fmt"
	"github.com/google/uuid"
	"itracker/internal/core/domain"
	"log/slog"
	"os"
	"path/filepath"
)

type ScrapedItemRepository struct {
	logger *slog.Logger
}

func NewScrapedItemRepository(logger *slog.Logger) *ScrapedItemRepository {
	return &ScrapedItemRepository{
		logger: logger,
	}
}

func (s *ScrapedItemRepository) Save(ctx context.Context, websiteId string, definitionId string, items []domain.ScrapedItem) error {
	for _, item := range items {
		directory := filepath.Join("..", "tracked_items", websiteId, definitionId)
		err := os.MkdirAll(directory, os.ModePerm)

		if err != nil && !os.IsExist(err) {
			s.logger.Error("failed to create website folder: %w", err)
			continue
		}

		fileId, err := uuid.NewV7()
		if err != nil {
			s.logger.Error("failed to generate file id: %w", err)
			continue
		}

		err = os.WriteFile(filepath.Join(directory, fmt.Sprintf("%s.html", fileId)), []byte(item), os.ModePerm)
		if err != nil {
			s.logger.Error("failed to write file: %w", err)
			continue
		}
	}

	return nil
}