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
		directory := filepath.Join("..", "scraped_items", websiteId, definitionId)
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

func (s *ScrapedItemRepository) ListItemsToParse(ctx context.Context) ([]*domain.ItemToParse, error) {
	directory := filepath.Join("..", "scraped_items")
	var items = make([]*domain.ItemToParse, 0)

	files, err := os.ReadDir(directory)

	if err != nil {
		return nil, err
	}

	for _, file := range files {
		if !file.IsDir() {
			continue
		}

		websiteId := file.Name()
		websiteDirectory := filepath.Join(directory, websiteId)
		websiteFiles, err := os.ReadDir(websiteDirectory)
		if err != nil {
			return nil, err
		}

		for _, websiteFile := range websiteFiles {
			if !websiteFile.IsDir() {
				continue
			}

			definitionId := websiteFile.Name()
			definitionDirectory := filepath.Join(websiteDirectory, definitionId)
			scrapedFiles, err := os.ReadDir(definitionDirectory)
			if err != nil {
				return nil, err
			}

			for _, scrapedFile := range scrapedFiles {
				if scrapedFile.IsDir() {
					continue
				}

				item := &domain.ItemToParse{
					WebsiteId:    websiteId,
					DefinitionId: definitionId,
					FileName:     scrapedFile.Name(),
				}

				items = append(items, item)
			}
		}
	}

	return items, nil
}

func (s *ScrapedItemRepository) GetScrapedItemContent(ctx context.Context, item *domain.ItemToParse) ([]byte, error) {
	filePath := filepath.Join("..", "scraped_items", item.WebsiteId, item.DefinitionId, item.FileName)
	return os.ReadFile(filePath)
}