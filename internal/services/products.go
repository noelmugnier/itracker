package services

import (
	"itracker/internal/domain"
	"itracker/internal/ports"
	"log/slog"

	"github.com/google/uuid"
)

type ProductService struct {
	logger       *slog.Logger
	repository   ports.IProductRepository
	timeProvider ports.ITimeProvider
}

func NewProductService(logger *slog.Logger, repository ports.IProductRepository, timeProvider ports.ITimeProvider) *ProductService {
	return &ProductService{
		logger:       logger,
		repository:   repository,
		timeProvider: timeProvider,
	}
}

func (ps *ProductService) CreateProduct(product *domain.CreateProduct) (string, error) {
	id, err := uuid.NewV7()
	if err != nil {
		return "", err
	}

	productToCreate := &domain.Product{
		Id:        id.String(),
		Name:      product.Name,
		CreatedAt: ps.timeProvider.UtcNow(),
	}

	err = ps.repository.AddProduct(productToCreate)
	if err != nil {
		return "", err
	}

	return productToCreate.Id, nil
}
