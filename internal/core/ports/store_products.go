package ports

import (
	"itracker/internal/core/domain"
)

type IStoreProducts interface {
	AddProduct(product *domain.Product) error
}