package handlers

import (
	"net/http"
)

func NewApiRouter(productHandlers *ProductHttpHandlers, websiteHandlers *WebsiteHttpHandlers, definitionHandlers *ScraperDefinitionHttpHandlers, scraperHandlers *ScraperHttpHandlers) *http.ServeMux {
	handler := http.NewServeMux()

	handler.HandleFunc("POST /api/v1/products", productHandlers.CreateProduct)

	handler.HandleFunc("POST /api/v1/websites", websiteHandlers.CreateWebsite)
	handler.HandleFunc("POST /api/v1/websites/{id}/definitions/catalog", definitionHandlers.CreateCatalogDefinition)
	handler.HandleFunc("POST /api/v1/websites/{id}/definitions/product", definitionHandlers.CreateProductDefinition)
	handler.HandleFunc("POST /api/v1/websites/{id}/scrapers/catalog", scraperHandlers.CreateCatalogScraper)

	return handler
}