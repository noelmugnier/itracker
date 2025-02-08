package repositories

import (
	"database/sql"
	"errors"
	"itracker/internal/domain"
	"itracker/internal/ports"
	"log/slog"
)

type ProductRepository struct {
	logger *slog.Logger
	dbConn *sql.DB
}

func NewProductRepository(logger *slog.Logger, dbConn *sql.DB) ports.IProductRepository {
	return &ProductRepository{
		logger: logger,
		dbConn: dbConn,
	}
}

func (pr *ProductRepository) GetProducts() ([]*domain.Product, error) {
	return nil, errors.New("not implemented")
}

func (pr *ProductRepository) GetProduct(id string) (*domain.Product, error) {
	return nil, errors.New("not implemented")
}

func (pr *ProductRepository) AddProduct(product *domain.Product) error {
	_, err := pr.dbConn.Exec(`INSERT INTO products (id, name, created_at) VALUES ($1, $2, $3)`, product.Id, product.Name, product.CreatedAt.Unix())
	return err
}

func (pr *ProductRepository) UpdateProduct(product *domain.Product) error {
	return errors.New("not implemented")
}

func (pr *ProductRepository) DeleteProduct(id string) error {
	return errors.New("not implemented")
}
