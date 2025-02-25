package services

import (
	"context"
	"itracker/internal/core/ports"
	"log/slog"
)

type ReconcilerService struct {
	logger               *slog.Logger
	productRepository    ports.IStoreProducts
	parsedItemRepository ports.IStoreParsedItems
}

func NewReconcilerService(parsedItemRepository ports.IStoreParsedItems, productRepository ports.IStoreProducts, logger *slog.Logger) *ReconcilerService {
	return &ReconcilerService{
		logger:               logger,
		productRepository:    productRepository,
		parsedItemRepository: parsedItemRepository,
	}
}

func (ps *ReconcilerService) Reconcile(ctx context.Context) {
	
}