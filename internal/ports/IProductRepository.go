package ports

import "itracker/internal/domain"

type IProductRepository interface {
	AddProduct(product *domain.Product) error
}
