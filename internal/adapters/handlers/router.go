package handlers

import (
	"log/slog"
	"net/http"
)

func NewRouter(logger *slog.Logger, productHandlers *ProductHttpHandlers, websiteHandlers *WebsiteHttpHandlers) *http.ServeMux {
	handler := http.NewServeMux()

	handler.HandleFunc("POST /api/v1/products", productHandlers.CreateProduct)

	handler.HandleFunc("POST /api/v1/websites", websiteHandlers.CreateWebsite)

	return handler
}
