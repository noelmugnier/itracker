package outbound

import (
	"database/sql"
	"itracker/internal/core/domain"
	"log/slog"
)

type ProductRepository struct {
	logger *slog.Logger
	db     *sql.DB
}

func NewProductRepository(db *sql.DB, logger *slog.Logger) *ProductRepository {
	return &ProductRepository{
		logger: logger,
		db:     db,
	}
}

func (pr *ProductRepository) AddProduct(product *domain.Product) error {
	_, err := pr.db.Exec(`INSERT INTO products (id, name, created_at) VALUES ($1, $2, $3)`, product.Id, product.Name, product.CreatedAt.Unix())
	return err
}