package test

import (
	"database/sql"
	"itracker/internal/core/domain"
	"itracker/internal/core/services"
	"itracker/internal/inbound"
	"itracker/internal/outbound"
	"log/slog"
	"net/http"
	"testing"
	"time"

	"github.com/google/uuid"
	"github.com/stretchr/testify/require"
)

func createTestRouter(t *testing.T) (*http.ServeMux, *sql.DB, time.Time) {
	db := initInMemoryDatabase(t)

	logger := createTestLogger()
	currentTime := time.Now().UTC()
	timeProvider := &fakeTimeProvider{currentTime}

	productRepository := outbound.NewProductRepository(db, logger)
	websiteRepository := outbound.NewWebsiteRepository(logger, db)
	definitionRepository := outbound.NewDefinitionRepository(db, logger)
	scraperRepository := outbound.NewScraperConfigRepository(logger, db)

	websiteSvc := services.NewWebsiteService(websiteRepository, definitionRepository, scraperRepository, timeProvider, logger)
	productSvc := services.NewProductService(productRepository, timeProvider, logger)

	router := inbound.NewApiRouter(inbound.NewProductHandlers(productSvc, timeProvider, logger), inbound.NewWebsiteHandlers(websiteSvc, timeProvider, logger), inbound.NewWebsiteDefinitionHandlers(websiteSvc, timeProvider, logger), inbound.NewWebsiteScraperHandlers(websiteSvc, timeProvider, logger))

	return router, db, currentTime
}

func createWebsite(t *testing.T, dbConn *sql.DB, now time.Time) string {
	websiteId, err := uuid.NewV7()
	require.NoError(t, err)
	_, err = dbConn.Exec("INSERT INTO websites (id, name, host, created_at) VALUES ($1, $2, $3, $4)", websiteId.String(), "test", "test.com", now)
	require.NoError(t, err)

	return websiteId.String()
}

func createCatalogDefinition(t *testing.T, dbConn *sql.DB, definition *domain.CatalogDefinition) string {
	_, err := dbConn.Exec("INSERT INTO definitions (id, website_id, type, scraper, parser, created_at) VALUES ($1, $2, $3, $4, $5, $6)", definition.Id, definition.WebsiteId, "catalog", "{}", "{}", definition.CreatedAt)
	require.NoError(t, err)

	return definition.Id
}

func createTestLogger() *slog.Logger {
	return outbound.NewTextLogger(slog.LevelError)
}

func initInMemoryDatabase(t *testing.T) *sql.DB {
	dbConn, err := outbound.NewSqliteConnector("file::memory:")
	if err != nil {
		t.Fatal(err)
	}

	err = outbound.InitMainDatabase(dbConn)
	if err != nil {
		t.Fatal(err)
	}
	return dbConn
}