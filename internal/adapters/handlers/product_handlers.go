package handlers

import (
	"database/sql"
	"encoding/json"
	"errors"
	"itracker/internal/adapters/repositories"
	"itracker/internal/domain"
	"itracker/internal/ports"
	"itracker/internal/services"
	"log/slog"
	"net/http"
)

type ProductHttpHandlers struct {
	service      *services.ProductService
	logger       *slog.Logger
	timeProvider ports.ITimeProvider
}

func NewProductHandlers(dbConn *sql.DB, timeProvider ports.ITimeProvider, logger *slog.Logger) *ProductHttpHandlers {
	productService := services.NewProductService(logger, repositories.NewProductRepository(logger, dbConn), timeProvider)
	return &ProductHttpHandlers{
		service:      productService,
		logger:       logger,
		timeProvider: timeProvider,
	}
}

type CreateProductRequest struct {
	Name     string   `json:"name"`
	Websites []string `json:"websites"`
}

type CreateProductResponse struct {
	Id string `json:"id"`
}

// CreateProduct godoc
// @Summary Create a new product
// @Tags Products
// @ID create-product
// @Accept json
// @Produce json
// @param body body CreateProductRequest true "CreateProductRequest"
// @Success 201 {object} CreateProductResponse
// @Failure 400 {object} string
// @Failure 422 {object} string
// @Failure 500 {object} string
// @Router /products [post]
func (ph *ProductHttpHandlers) CreateProduct(w http.ResponseWriter, r *http.Request) {
	ctx := r.Context()
	ph.logger.Log(ctx, slog.LevelDebug, "CreateProduct endpoint called", slog.Any("request", r))

	var request *CreateProductRequest = nil

	decoder := json.NewDecoder(r.Body)
	decoder.DisallowUnknownFields()

	err := decoder.Decode(&request)
	if err != nil {
		ph.logger.Log(ctx, slog.LevelError, "cannot parse request content", slog.Any("error", err))
		w.WriteHeader(http.StatusBadRequest)
		w.Write([]byte(err.Error()))
		return
	}
	defer r.Body.Close()

	productId, err := ph.service.CreateProduct(request.Name, request.Websites)

	if err != nil && errors.Is(err, domain.ValidationError) {
		ph.logger.Log(ctx, slog.LevelInfo, "invalid data", slog.Any("error", err))
		w.WriteHeader(http.StatusUnprocessableEntity)
		w.Write([]byte(err.Error()))
		return
	} else if err != nil {
		ph.logger.Log(ctx, slog.LevelError, "an unexpected error occured", slog.Any("error", err))
		w.WriteHeader(http.StatusInternalServerError)
		w.Write([]byte(err.Error()))
		return
	}

	w.Header().Set("Location", r.RequestURI+"/"+productId)
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusCreated)

	encoder := json.NewEncoder(w)
	encoder.Encode(CreateProductResponse{Id: productId})
}
