package handlers

import (
	"database/sql"
	"itracker/internal/adapters"
	"itracker/internal/adapters/repositories"
	"itracker/internal/services"
	"log/slog"
	"net/http"
	"testing"
	"time"

	"github.com/google/uuid"
	"github.com/stretchr/testify/require"
)

type TestTimeProvider struct {
	currentTime time.Time
}

func (tp *TestTimeProvider) UtcNow() time.Time {
	return tp.currentTime
}

func CreateTestRouter(t *testing.T) (*http.ServeMux, *sql.DB, time.Time) {
	db := InitInMemoryDatabase(t)

	logger := CreateTestLogger()
	currentTime := time.Now().UTC()
	timeProvider := &TestTimeProvider{currentTime}

	websiteSvc := services.NewWebsiteService(
		logger,
		repositories.NewWebsiteRepository(logger, db),
		repositories.NewScraperDefinitionRepository(logger, db),
		timeProvider)

	productSvc := services.NewProductService(logger, repositories.NewProductRepository(logger, db), timeProvider)

	router := NewRouter(
		logger,
		NewProductHandlers(productSvc, timeProvider, logger),
		NewWebsiteHandlers(websiteSvc, timeProvider, logger))

	return router, db, currentTime
}

func CreateWebsite(t *testing.T, dbConn *sql.DB, now time.Time) string {
	websiteId, err := uuid.NewV7()
	require.NoError(t, err)
	_, err = dbConn.Exec("INSERT INTO websites (id, name, host, created_at) VALUES ($1, $2, $3, $4)", websiteId.String(), "test", "test.com", now)
	require.NoError(t, err)

	return websiteId.String()
}

func CreateTestLogger() *slog.Logger {
	return adapters.NewTextLogger(slog.LevelError)
}

func InitInMemoryDatabase(t *testing.T) *sql.DB {
	dbConn, err := adapters.NewSqliteConnector("file::memory:")
	if err != nil {
		t.Fatal(err)
	}

	err = adapters.InitDatabase(dbConn)
	if err != nil {
		t.Fatal(err)
	}
	return dbConn
}