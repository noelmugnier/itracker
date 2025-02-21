package outbound

import (
	"context"
	"database/sql"
	"encoding/json"
	"github.com/google/uuid"
	"itracker/internal/core/domain"
	"log/slog"
)

type ParsedItemRepository struct {
	logger *slog.Logger
	db     *sql.DB
}

func NewParsedItemRepository(db *sql.DB, logger *slog.Logger) *ParsedItemRepository {
	return &ParsedItemRepository{
		logger: logger,
		db:     db,
	}
}

func (pir *ParsedItemRepository) Save(ctx context.Context, item *domain.ParsedProduct) error {
	additionalFields, err := json.Marshal(item.AdditionalFields)
	if err != nil {
		pir.logger.Log(ctx, slog.LevelError, "failed to marshal item's additional fields", slog.Any("error", err))
		return err
	}

	id, err := uuid.NewV7()
	if err != nil {
		pir.logger.Log(ctx, slog.LevelError, "failed to generate item id", slog.Any("error", err))
		return err
	}

	_, err = pir.db.Exec("INSERT INTO parsed_items (id, item_id, name, unit_price, details, additional_fields, scraped_at, website_id) VALUES ($1, $2, $3, $4, $5, $6, $7, $8)", id.String(), item.Id, item.Name, item.UnitPrice, item.Details, additionalFields, item.ScrapedAt.Unix(), item.WebsiteId)

	if err != nil {
		pir.logger.Log(ctx, slog.LevelError, "failed to save parsed item", slog.Any("error", err))
		return err
	}

	row := pir.db.QueryRow("SELECT EXISTS(SELECT 1 FROM search WHERE parsed_item_id = $1)", item.Id)
	var exists int
	err = row.Scan(&exists)
	if err != nil {
		pir.logger.Log(ctx, slog.LevelError, "failed to check if item already exists in search table", slog.Any("error", err))
		return err
	}

	if exists == 0 {
		_, err = pir.db.Exec("INSERT INTO search (parsed_item_id, name, website_id) VALUES ($1, $2, $3)", item.Id, item.Name, item.WebsiteId)
	} else {
		_, err = pir.db.Exec("UPDATE search SET name = $1 WHERE parsed_item_id = $2", item.Name, item.Id)
	}

	return err
}