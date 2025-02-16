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

func TestCreateCatalogDefinition(t *testing.T) {
	t.Run("should return 201 when creating catalog definition", func(t *testing.T) {
		t.Parallel()

		router, dbConn, now := CreateTestRouter(t)
		defer dbConn.Close()

		websiteId := CreateWebsite(t, dbConn, now)

		request := createCatalogDefinitionRequest()

		content, err := json.Marshal(request)
		require.NoError(t, err)

		response := httptest.NewRecorder()

		router.ServeHTTP(response, httptest.NewRequest("POST", fmt.Sprintf("/api/v1/websites/%s/catalog/definitions", websiteId), bytes.NewBuffer(content)))

		assert.Equal(t, http.StatusCreated, response.Code)

		var scraper, parser, definitionType, createdAt string
		err = dbConn.QueryRow("SELECT type, scraper, parser, datetime(created_at,'unixepoch') as created_at FROM definitions where website_id = ?", websiteId).Scan(&definitionType, &scraper, &parser, &createdAt)
		require.NoError(t, err)

		assert.Equal(t, "catalog", definitionType)
		assert.Equal(t, now.Format("2006-01-02 15:04:05"), createdAt)

		var scraperDefinition domain.ScraperCatalogDefinition
		var parserDefinition domain.ParserCatalogDefinition

		err = json.Unmarshal([]byte(scraper), &scraperDefinition)
		require.NoError(t, err)

		err = json.Unmarshal([]byte(parser), &parserDefinition)
		require.NoError(t, err)

		assert.Len(t, parserDefinition.Fields, 2)
		assert.Equal(t, request.Parser.Fields[0].Identifier, parserDefinition.Fields[0].Identifier)
		assert.Equal(t, request.Parser.Fields[0].DisplayName, parserDefinition.Fields[0].DisplayName)
		assert.Equal(t, request.Parser.Fields[0].Selector, parserDefinition.Fields[0].Selector)
		assert.Equal(t, request.Parser.Fields[0].Required, parserDefinition.Fields[0].Required)
		assert.NotNil(t, scraperDefinition.Pagination)
		assert.Equal(t, request.Scraper.Pagination.PageNumberParamName, scraperDefinition.Pagination.PageNumberParamName)
		assert.Equal(t, request.Scraper.Pagination.MaxPage, scraperDefinition.Pagination.MaxPage)
	})
}

func createCatalogDefinitionRequest() *CreateCatalogDefinitionRequest {
	return &CreateCatalogDefinitionRequest{
		Parser: &ParserCatalogDefinitionRequest{
			ItemSelector: "div.product",
			Fields: []*FieldDefinitionRequest{
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
			}},
		Scraper: &ScraperCatalogDefinitionRequest{
			ContentSelector: "div.product-list",
			Pagination: &PaginationDefinitionRequest{
				PageNumberParamName: "page",
				MaxPage:             10,
			},
		},
	}
}