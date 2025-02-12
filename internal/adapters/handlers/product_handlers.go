package handlers

import (
	"encoding/json"
	"errors"
	"itracker/internal/domain"
	"itracker/internal/ports"
	"itracker/internal/services"
	"log/slog"
	"net/http"
)

type ProductHttpHandlers struct {
	svc    *services.ProductService
	logger *slog.Logger
	time   ports.ITimeProvider
}

func NewProductHandlers(svc *services.ProductService, timeProvider ports.ITimeProvider, logger *slog.Logger) *ProductHttpHandlers {
	return &ProductHttpHandlers{
		svc:    svc,
		logger: logger,
		time:   timeProvider,
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
		_, err = w.Write([]byte(err.Error()))
		if err != nil {
			ph.logger.Log(ctx, slog.LevelError, "cannot write error to response", slog.Any("error", err))
		}
		return
	}
	defer r.Body.Close()

	productId, err := ph.svc.CreateProduct(request.Name, request.Websites)

	if err != nil && errors.Is(err, domain.ValidationError) {
		ph.logger.Log(ctx, slog.LevelInfo, "invalid data", slog.Any("error", err))
		w.WriteHeader(http.StatusUnprocessableEntity)
		_, err = w.Write([]byte(err.Error()))
		if err != nil {
			ph.logger.Log(ctx, slog.LevelError, "cannot write error to response", slog.Any("error", err))
		}
		return
	} else if err != nil {
		ph.logger.Log(ctx, slog.LevelError, "an unexpected error occurred", slog.Any("error", err))
		w.WriteHeader(http.StatusInternalServerError)
		_, err = w.Write([]byte(err.Error()))
		if err != nil {
			ph.logger.Log(ctx, slog.LevelError, "cannot write error to response", slog.Any("error", err))
		}
		return
	}

	w.Header().Set("Location", r.RequestURI+"/"+productId)
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusCreated)

	encoder := json.NewEncoder(w)
	err = encoder.Encode(CreateProductResponse{Id: productId})
	if err != nil {
		ph.logger.Log(ctx, slog.LevelError, "cannot write create product response", slog.Any("error", err))
	}
}