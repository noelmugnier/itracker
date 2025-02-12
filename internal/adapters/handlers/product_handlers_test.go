package handlers

import (
	"bytes"
	"encoding/json"
	"github.com/stretchr/testify/assert"
	"github.com/stretchr/testify/require"
	"net/http"
	"net/http/httptest"
	"testing"
)

func TestProductHandlers(t *testing.T) {
	t.Run("should return 422 if product name is missing", func(t *testing.T) {
		t.Parallel()
		router, dbConn, _ := CreateTestRouter(t)
		defer dbConn.Close()

		content := `{}`
		response := httptest.NewRecorder()

		router.ServeHTTP(response, httptest.NewRequest("POST", "/api/v1/products", bytes.NewBufferString(content)))

		assert.Equal(t, http.StatusUnprocessableEntity, response.Code)
	})

	t.Run("should return 201 with created product", func(t *testing.T) {
		t.Parallel()

		router, dbConn, now := CreateTestRouter(t)
		defer dbConn.Close()

		content := `{"name": "test"}`
		response := httptest.NewRecorder()

		router.ServeHTTP(response, httptest.NewRequest("POST", "/api/v1/products", bytes.NewBufferString(content)))

		assert.Equal(t, http.StatusCreated, response.Code)

		var id, productName, createdAt string
		err := dbConn.QueryRow("SELECT id, name, datetime(created_at,'unixepoch') as created_at FROM products").Scan(&id, &productName, &createdAt)
		require.NoError(t, err)

		var responseContent CreateProductResponse
		err = json.Unmarshal(response.Body.Bytes(), &responseContent)

		require.NoError(t, err)
		assert.Equal(t, responseContent.Id, id)
		assert.Equal(t, "test", productName)
		assert.Equal(t, now.Format("2006-01-02 15:04:05"), createdAt)
	})
}
