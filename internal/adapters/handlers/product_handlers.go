package handlers

import (
	"database/sql"
	"encoding/json"
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

// CreateProduct godoc
// @Summary Create a new product
// @Tags Products
// @ID create-product
// @Accept  json
// @Produce  json
// @Success 201 {object} domain.Product
// @Router /products [post]
func (ph *ProductHttpHandlers) CreateProduct(w http.ResponseWriter, r *http.Request) {
	ph.logger.Info("CreateProduct")

	var product *domain.CreateProduct = nil

	decoder := json.NewDecoder(r.Body)
	decoder.DisallowUnknownFields()

	err := decoder.Decode(&product)
	if err != nil {
		ph.logger.Error("CreateProduct", "error", err)
		w.WriteHeader(http.StatusBadRequest)
		return
	}
	defer r.Body.Close()

	productId, err := ph.service.CreateProduct(product)
	if err != nil {
		ph.logger.Error("CreateProduct", "error", err)
		w.WriteHeader(http.StatusInternalServerError)
		return
	}

	w.Header().Set("Location", r.RequestURI+"/"+productId)
	w.WriteHeader(http.StatusCreated)
	w.Write([]byte(`{"id": "` + productId + `"}`))
}

// GetProducts godoc
// @Summary Get all products
// @Tags Products
// @ID get-products
// @Accept  json
// @Produce  json
// @Success 200 {object} []domain.Product
// @Router /products [get]
func (ph *ProductHttpHandlers) GetProducts(w http.ResponseWriter, r *http.Request) {
	ph.logger.Info("GetProducts")
	w.WriteHeader(http.StatusNotImplemented)
}

// GetProduct godoc
// @Summary Get a product
// @Tags Products
// @ID get-product
// @Accept  json
// @Produce  json
// @Success 200 {object} domain.Product
// @Router /products/{id} [get]
func (ph *ProductHttpHandlers) GetProduct(w http.ResponseWriter, r *http.Request) {
	ph.logger.Info("GetProduct")
	w.WriteHeader(http.StatusNotImplemented)
}

// UpdateProduct godoc
// @Summary Update a product
// @Tags Products
// @ID update-product
// @Accept  json
// @Produce  json
// @Success 200 {object} domain.Product
// @Router /products/{id} [put]
func (ph *ProductHttpHandlers) UpdateProduct(w http.ResponseWriter, r *http.Request) {
	ph.logger.Info("UpdateProduct")
	w.WriteHeader(http.StatusNotImplemented)
}

// DeleteProduct godoc
// @Summary Delete a product
// @Tags Products
// @ID delete-product
// @Accept  json
// @Produce  json
// @Success 204
// @Router /products/{id} [delete]
func (ph *ProductHttpHandlers) DeleteProduct(w http.ResponseWriter, r *http.Request) {
	ph.logger.Info("DeleteProduct")
	w.WriteHeader(http.StatusNotImplemented)
}

// AddProductWebsite godoc
// @Summary Add a website to a product
// @Tags Products
// @ID add-product-website
// @Accept  json
// @Produce  json
// @Success 200 {object} domain.Product
// @Router /products/{id}/websites [post]
func (ph *ProductHttpHandlers) AddWebsite(w http.ResponseWriter, r *http.Request) {
	ph.logger.Info("AddWebsite")
	w.WriteHeader(http.StatusNotImplemented)
}

// RemoveProductWebsite godoc
// @Summary Remove a website from a product
// @Tags Products
// @ID delete-product-website
// @Accept  json
// @Produce  json
// @Success 204
// @Router /products/{id}/websites/{websiteId} [delete]
func (ph *ProductHttpHandlers) RemoveWebsite(w http.ResponseWriter, r *http.Request) {
	ph.logger.Info("DeleteWebsite")
	w.WriteHeader(http.StatusNotImplemented)
}
