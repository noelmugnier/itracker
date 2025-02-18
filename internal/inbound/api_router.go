package inbound

import (
	"net/http"
)

func NewApiRouter(productHandlers *ProductHttpHandlers, websiteHandlers *WebsiteHttpHandlers, definitionHandlers *DefinitionHttpHandlers, scraperHandlers *ScraperHttpHandlers) *http.ServeMux {
	handler := http.NewServeMux()

	handler.HandleFunc("POST /api/v1/products", productHandlers.CreateProduct)

	handler.HandleFunc("POST /api/v1/websites", websiteHandlers.CreateWebsite)
	handler.HandleFunc("POST /api/v1/websites/{id}/catalog/definitions", definitionHandlers.CreateCatalogDefinition)
	handler.HandleFunc("POST /api/v1/websites/{id}/catalog/definitions/{definitionId}/scrapers", scraperHandlers.CreateCatalogScraper)

	return handler
}