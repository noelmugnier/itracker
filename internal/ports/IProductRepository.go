package ports

import "itracker/internal/domain"

type IProductRepository interface {
	GetProducts() ([]*domain.Product, error)
	GetProduct(id string) (*domain.Product, error)
	AddProduct(product *domain.Product) error
	UpdateProduct(product *domain.Product) error
	DeleteProduct(id string) error
}
