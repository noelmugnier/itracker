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

func TestCreateScraperDefinition(t *testing.T) {
	t.Run("should return 204 when creating catalog definition", func(t *testing.T) {
		t.Parallel()

		router, dbConn, now := CreateTestRouter(t)
		defer dbConn.Close()

		websiteId := CreateWebsite(t, dbConn, now)
		expectedWebsiteId := websiteId

		request := createCatalogScraperDefinitionRequest()

		content, err := json.Marshal(request)
		require.NoError(t, err)

		response := httptest.NewRecorder()

		router.ServeHTTP(response, httptest.NewRequest("POST", fmt.Sprintf("/api/v1/websites/%s/definitions/catalog", websiteId), bytes.NewBuffer(content)))

		assert.Equal(t, http.StatusNoContent, response.Code)

		var definition, definitionType, createdAt string
		err = dbConn.QueryRow("SELECT website_id, type, definition, datetime(created_at,'unixepoch') as created_at FROM scraper_definitions where website_id = ?", websiteId).Scan(&websiteId, &definitionType, &definition, &createdAt)
		require.NoError(t, err)

		assert.Equal(t, expectedWebsiteId, websiteId)
		assert.Equal(t, "catalog", definitionType)
		assert.Equal(t, now.Format("2006-01-02 15:04:05"), createdAt)

		var scraperDefinition struct {
			Fields     []*domain.DefinitionField    `json:"fields"`
			Pagination *domain.DefinitionPagination `json:"pagination"`
			Navigation *domain.DefinitionNavigation `json:"navigation"`
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

		request := createProductScraperDefinitionRequest()
		response := httptest.NewRecorder()

		content, err := json.Marshal(request)
		require.NoError(t, err)

		router.ServeHTTP(response, httptest.NewRequest("POST", fmt.Sprintf("/api/v1/websites/%s/definitions/product", websiteId), bytes.NewBuffer(content)))

		assert.Equal(t, http.StatusNoContent, response.Code)

		var definition, definitionType, createdAt string
		err = dbConn.QueryRow("SELECT website_id, type, definition, datetime(created_at,'unixepoch') as created_at FROM scraper_definitions where website_id = ?", websiteId).Scan(&websiteId, &definitionType, &definition, &createdAt)
		require.NoError(t, err)

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

func createProductScraperDefinitionRequest() *CreateProductDefinitionRequest {
	return &CreateProductDefinitionRequest{
		Fields: []*DefinitionFieldRequest{
			{
				Identifier:  "unit_price",
				DisplayName: "Product Price",
				Selector:    "div>span",
				Required:    true,
			},
		},
	}
}

func createCatalogScraperDefinitionRequest() *CreateCatalogDefinitionRequest {
	return &CreateCatalogDefinitionRequest{
		Pagination: &DefinitionPaginationRequest{
			PageNumberParamName: "page",
			MaxPage:             10,
		},
		Fields: []*DefinitionFieldRequest{
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
		ProductNavigation: &DefinitionNavigationRequest{
			FieldIdentifier: "details_link",
			Navigate:        false,
		},
	}
}