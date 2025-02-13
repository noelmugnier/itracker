package handlers

import (
	"bytes"
	"encoding/json"
	"fmt"
	"github.com/stretchr/testify/assert"
	"github.com/stretchr/testify/require"
	"itracker/internal/domain"
	"net/http"
	"net/http/httptest"
	"testing"
)

func TestCreateWebsiteCatalogScraper(t *testing.T) {
	t.Run("should return 201 when creating website catalog scraper", func(t *testing.T) {
		t.Parallel()

		router, dbConn, now := CreateTestRouter(t)
		defer dbConn.Close()

		websiteId := CreateWebsite(t, dbConn, now)
		catalogScraperDefinition := &domain.CreateCatalogScraperDefinition{
			WebsiteId: websiteId,
			CreatedAt: now,
		}

		scraperDefinitionId := CreateWebsiteCatalogScraperDefinition(t, dbConn, catalogScraperDefinition)

		request := &CreateScraperRequest{
			Urls: []string{"https://test.com"},
			Cron: "0 1/* * * *",
		}

		content, err := json.Marshal(request)
		require.NoError(t, err)

		response := httptest.NewRecorder()

		router.ServeHTTP(response, httptest.NewRequest("POST", fmt.Sprintf("/api/v1/websites/%s/scrapers/catalog", websiteId), bytes.NewBuffer(content)))

		assert.Equal(t, http.StatusCreated, response.Code)

		var id, enabled, urls, cron, createdAt string
		err = dbConn.QueryRow("SELECT id, enabled, cron, urls, datetime(created_at,'unixepoch') as created_at FROM scrapers where scraper_definition_id = ?", scraperDefinitionId).Scan(&id, &enabled, &cron, &urls, &createdAt)
		require.NoError(t, err)

		assert.Equal(t, "1", enabled)
		assert.Equal(t, "0 1/* * * *", cron)
		assert.Equal(t, `{"https://test.com":true}`, urls)
	})
}