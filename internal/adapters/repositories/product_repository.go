package repositories

import (
	"database/sql"
	"itracker/internal/domain"
	"itracker/internal/ports"
	"log/slog"
)

type ProductRepository struct {
	logger *slog.Logger
	db     *sql.DB
}

func NewProductRepository(logger *slog.Logger, db *sql.DB) ports.IProductRepository {
	return &ProductRepository{
		logger: logger,
		db:     db,
	}
}

func (pr *ProductRepository) AddProduct(product *domain.Product) error {
	_, err := pr.db.Exec(`INSERT INTO products (id, name, created_at) VALUES ($1, $2, $3)`, product.Id, product.Name, product.CreatedAt.Unix())
	return err
}