package services

import (
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

func (ps *ProductService) CreateProduct(name string, websites []string) (string, error) {
	if name == "" {
		return "", domain.CreateValidationError(domain.ErrProductNameRequired)
	}

	id, err := uuid.NewV7()
	if err != nil {
		return "", err
	}

	productToCreate := &domain.Product{
		Id:        id.String(),
		Name:      name,
		CreatedAt: ps.timeProvider.UtcNow(),
	}

	err = ps.repository.AddProduct(productToCreate)
	if err != nil {
		return "", err
	}

	return productToCreate.Id, nil
}