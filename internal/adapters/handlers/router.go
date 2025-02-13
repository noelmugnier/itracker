package handlers

import (
	"log/slog"
	"net/http"
)

func NewRouter(productHandlers *ProductHttpHandlers, websiteHandlers *WebsiteHttpHandlers, scraperDefinitionHandlers *ScraperDefinitionHttpHandlers, scraperHandlers *ScraperHttpHandlers, logger *slog.Logger) *http.ServeMux {
	handler := http.NewServeMux()

	handler.HandleFunc("POST /api/v1/products", productHandlers.CreateProduct)

	handler.HandleFunc("POST /api/v1/websites", websiteHandlers.CreateWebsite)
	handler.HandleFunc("POST /api/v1/websites/{id}/definitions/catalog", scraperDefinitionHandlers.CreateCatalogScraperDefinition)
	handler.HandleFunc("POST /api/v1/websites/{id}/definitions/product", scraperDefinitionHandlers.CreateProductScraperDefinition)
	handler.HandleFunc("POST /api/v1/websites/{id}/scrapers/catalog", scraperHandlers.CreateWebsiteCatalogScraper)

	return handler
}