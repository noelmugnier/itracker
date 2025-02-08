package handlers

import (
	"log/slog"
	"net/http"
)

func NewRouter(logger *slog.Logger, productHandlers *ProductHttpHandlers, websiteHandlers *WebsiteHttpHandlers) *http.ServeMux {
	handler := http.NewServeMux()

	handler.HandleFunc("GET /products", productHandlers.GetProducts)
	handler.HandleFunc("GET /products/{id}", productHandlers.GetProduct)
	handler.HandleFunc("POST /products", productHandlers.CreateProduct)
	handler.HandleFunc("PUT /products/{id}", productHandlers.UpdateProduct)
	handler.HandleFunc("DELETE /products/{id}", productHandlers.DeleteProduct)
	handler.HandleFunc("POST /products/{id}/websites", productHandlers.AddWebsite)
	handler.HandleFunc("DELETE /products/{id}/websites/{websiteId}", productHandlers.RemoveWebsite)

	handler.HandleFunc("GET /websites", websiteHandlers.GetWebsites)
	handler.HandleFunc("GET /websites/{id}", websiteHandlers.GetWebsite)
	handler.HandleFunc("POST /websites", websiteHandlers.CreateWebsite)
	handler.HandleFunc("PUT /websites/{id}", websiteHandlers.UpdateWebsite)
	handler.HandleFunc("DELETE /websites/{id}", websiteHandlers.DeleteWebsite)

	return handler
}
