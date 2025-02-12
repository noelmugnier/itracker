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

func TestCreateWebsiteHandlers(t *testing.T) {
	t.Run("should return 422 if website name is missing", func(t *testing.T) {
		t.Parallel()
		router, dbConn, _ := CreateTestRouter(t)
		defer dbConn.Close()

		content := `{}`
		response := httptest.NewRecorder()

		router.ServeHTTP(response, httptest.NewRequest("POST", "/api/v1/websites", bytes.NewBufferString(content)))

		assert.Equal(t, http.StatusUnprocessableEntity, response.Code)
	})

	t.Run("should return 422 if website host is invalid", func(t *testing.T) {
		t.Parallel()
		router, dbConn, _ := CreateTestRouter(t)
		defer dbConn.Close()

		content := `{"name": "test", "host": "test"}`
		response := httptest.NewRecorder()

		router.ServeHTTP(response, httptest.NewRequest("POST", "/api/v1/websites", bytes.NewBufferString(content)))

		assert.Equal(t, http.StatusUnprocessableEntity, response.Code)
	})

	t.Run("should return 201 with created website", func(t *testing.T) {
		t.Parallel()

		router, dbConn, now := CreateTestRouter(t)
		defer dbConn.Close()

		content := `{"name": "test", "host": "https://test.com/super/test"}`
		response := httptest.NewRecorder()

		router.ServeHTTP(response, httptest.NewRequest("POST", "/api/v1/websites", bytes.NewBufferString(content)))

		assert.Equal(t, http.StatusCreated, response.Code)

		var responseContent CreateWebsiteResponse
		err := json.Unmarshal(response.Body.Bytes(), &responseContent)
		require.NoError(t, err)

		var id, websiteName, host, createdAt string
		err = dbConn.QueryRow("SELECT id, name, host, datetime(created_at,'unixepoch') as created_at FROM websites where id = ?", responseContent.Id).Scan(&id, &websiteName, &host, &createdAt)
		require.NoError(t, err)

		assert.Equal(t, responseContent.Id, id)
		assert.Equal(t, "test", websiteName)
		assert.Equal(t, "https://test.com", host)
		assert.Equal(t, now.Format("2006-01-02 15:04:05"), createdAt)
	})
}

func TestAddWebsiteScraperDefinition(t *testing.T) {
	t.Run("should return 204 when creating catalog definition", func(t *testing.T) {
		t.Parallel()

		router, dbConn, now := CreateTestRouter(t)
		defer dbConn.Close()

		websiteId := CreateWebsite(t, dbConn, now)
		expectedWebsiteId := websiteId

		request := &CreateCatalogScraperDefinitionRequest{
			Pagination: &ScraperDefinitionPagination{
				PageNumberParamName: "page",
				MaxPage:             10,
			},
			Fields: []*ScraperDefinitionField{
				{
					Identifier:  "unit_price",
					DisplayName: "Product Price",
					Selector:    "div>span",
					Required:    true,
				},
				{
					Identifier:  "details_link",
					DisplayName: "Product Details",
					Selector:    "a",
					Required:    false,
				},
			},
			ProductNavigation: &ScraperDefinitionProductNavigation{
				FieldIdentifier: "details_link",
				Navigate:        false,
			},
		}

		content, err := json.Marshal(request)
		require.NoError(t, err)

		response := httptest.NewRecorder()

		router.ServeHTTP(response, httptest.NewRequest("POST", fmt.Sprintf("/api/v1/websites/%s/definitions/catalog", websiteId), bytes.NewBuffer(content)))

		assert.Equal(t, http.StatusNoContent, response.Code)

		var id, definition, definitionType, createdAt string
		err = dbConn.QueryRow("SELECT id, website_id, type, definition, datetime(created_at,'unixepoch') as created_at FROM scraper_definitions where website_id = ?", websiteId).Scan(&id, &websiteId, &definitionType, &definition, &createdAt)
		require.NoError(t, err)

		require.NotNil(t, id)
		assert.Equal(t, expectedWebsiteId, websiteId)
		assert.Equal(t, "catalog", definitionType)
		assert.Equal(t, now.Format("2006-01-02 15:04:05"), createdAt)

		var scraperDefinition struct {
			Fields     []*domain.DefinitionField    `json:"fields"`
			Pagination *domain.PaginationDefinition `json:"pagination"`
			Navigation *domain.ProductNavigation    `json:"navigation"`
		}

		err = json.Unmarshal([]byte(definition), &scraperDefinition)
		require.NoError(t, err)

		assert.Len(t, scraperDefinition.Fields, 2)
		assert.Equal(t, request.Fields[0].Identifier, scraperDefinition.Fields[0].Identifier)
		assert.Equal(t, request.Fields[0].DisplayName, scraperDefinition.Fields[0].DisplayName)
		assert.Equal(t, request.Fields[0].Selector, scraperDefinition.Fields[0].Selector)
		assert.Equal(t, request.Fields[0].Required, scraperDefinition.Fields[0].Required)
		assert.NotNil(t, scraperDefinition.Pagination)
		assert.Equal(t, request.Pagination.PageNumberParamName, scraperDefinition.Pagination.PageNumberParamName)
		assert.Equal(t, request.Pagination.MaxPage, scraperDefinition.Pagination.MaxPage)
		assert.Equal(t, request.ProductNavigation.FieldIdentifier, scraperDefinition.Navigation.FieldIdentifier)
		assert.Equal(t, request.ProductNavigation.Navigate, scraperDefinition.Navigation.Navigate)
	})

	t.Run("should return 204 when creating product definition", func(t *testing.T) {
		t.Parallel()

		router, dbConn, now := CreateTestRouter(t)
		defer dbConn.Close()

		websiteId := CreateWebsite(t, dbConn, now)
		expectedWebsiteId := websiteId

		request := &CreateProductScraperDefinitionRequest{
			Fields: []*ScraperDefinitionField{
				{
					Identifier:  "unit_price",
					DisplayName: "Product Price",
					Selector:    "div>span",
					Required:    true,
				},
			},
		}

		content, err := json.Marshal(request)
		require.NoError(t, err)

		response := httptest.NewRecorder()

		router.ServeHTTP(response, httptest.NewRequest("POST", fmt.Sprintf("/api/v1/websites/%s/definitions/product", websiteId), bytes.NewBuffer(content)))

		assert.Equal(t, http.StatusNoContent, response.Code)

		var id, definition, definitionType, createdAt string
		err = dbConn.QueryRow("SELECT id, website_id, type, definition, datetime(created_at,'unixepoch') as created_at FROM scraper_definitions where website_id = ?", websiteId).Scan(&id, &websiteId, &definitionType, &definition, &createdAt)
		require.NoError(t, err)

		require.NotNil(t, id)
		assert.Equal(t, expectedWebsiteId, websiteId)
		assert.Equal(t, "product", definitionType)
		assert.Equal(t, now.Format("2006-01-02 15:04:05"), createdAt)

		var scraperDefinition struct {
			Fields []*domain.DefinitionField `json:"fields"`
		}

		err = json.Unmarshal([]byte(definition), &scraperDefinition)
		require.NoError(t, err)

		assert.Len(t, scraperDefinition.Fields, 1)
		assert.Equal(t, request.Fields[0].Identifier, scraperDefinition.Fields[0].Identifier)
		assert.Equal(t, request.Fields[0].DisplayName, scraperDefinition.Fields[0].DisplayName)
		assert.Equal(t, request.Fields[0].Selector, scraperDefinition.Fields[0].Selector)
		assert.Equal(t, request.Fields[0].Required, scraperDefinition.Fields[0].Required)
	})
}