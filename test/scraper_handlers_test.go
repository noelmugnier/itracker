package test

import (
	"bytes"
	"encoding/json"
	"fmt"
	"github.com/stretchr/testify/assert"
	"github.com/stretchr/testify/require"
	"itracker/internal/core/domain"
	"itracker/internal/inbound"
	"net/http"
	"net/http/httptest"
	"testing"
)

func TestCreateCatalogScraper(t *testing.T) {
	t.Run("should return 201 when creating catalog scraper", func(t *testing.T) {
		t.Parallel()

		router, dbConn, now := createTestRouter(t)
		defer dbConn.Close()

		websiteId := createWebsite(t, dbConn, now)

		definitionId := createCatalogDefinition(t, dbConn, &domain.CatalogDefinition{
			Id:        "test-id",
			WebsiteId: websiteId,
			CreatedAt: now,
		})

		request := &inbound.CreateCatalogScraperRequest{
			Urls: []string{"https://test.com"},
			Cron: "0 1/* * * *",
		}

		content, err := json.Marshal(request)
		require.NoError(t, err)

		response := httptest.NewRecorder()

		router.ServeHTTP(response, httptest.NewRequest("POST", fmt.Sprintf("/api/v1/websites/%s/catalog/definitions/%s/scrapers", websiteId, definitionId), bytes.NewBuffer(content)))

		assert.Equal(t, http.StatusCreated, response.Code)

		var id, enabled, urls, cron, createdAt string
		err = dbConn.QueryRow("SELECT id, enabled, cron, urls, datetime(created_at,'unixepoch') as created_at FROM scrapers where definition_id = ?", definitionId).Scan(&id, &enabled, &cron, &urls, &createdAt)

		require.NoError(t, err)

		assert.Equal(t, "1", enabled)
		assert.Equal(t, "0 1/* * * *", cron)
		assert.Equal(t, `{"https://test.com":true}`, urls)
	})
}