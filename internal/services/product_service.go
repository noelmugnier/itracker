package services

import (
	"context"
	"github.com/google/uuid"
	"itracker/internal/domain"
	"itracker/internal/ports"
	"log/slog"
)

type ProductService struct {
	logger       *slog.Logger
	repository   ports.IProductRepository
	timeProvider ports.ITimeProvider
}

func NewProductService(repository ports.IProductRepository, timeProvider ports.ITimeProvider, logger *slog.Logger) *ProductService {
	return &ProductService{
		logger:       logger,
		repository:   repository,
		timeProvider: timeProvider,
	}
}

func (ps *ProductService) CreateProduct(ctx context.Context, createProduct *domain.CreateProduct) (string, error) {
	if createProduct.Name == "" {
		return "", domain.CreateValidationError(domain.ErrProductNameRequired)
	}

	id, err := uuid.NewV7()
	if err != nil {
		return "", err
	}

	product := &domain.Product{
		Id:        id.String(),
		Name:      createProduct.Name,
		CreatedAt: ps.timeProvider.UtcNow(),
	}

	err = ps.repository.AddProduct(product)
	return product.Id, err
}