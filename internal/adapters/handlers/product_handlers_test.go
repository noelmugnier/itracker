package handlers

import (
	"bytes"
	"database/sql"
	"encoding/json"
	"net/http"
	"net/http/httptest"
	"testing"
	"time"

	"github.com/stretchr/testify/assert"
	"github.com/stretchr/testify/require"
)

func TestProductHandlers(t *testing.T) {
	t.Run("should add a product in the database and return created", func(t *testing.T) {
		t.Parallel()

		router, dbConn, now := CreateTestProductPrerequisite(t)
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

	t.Run("should return 422 if product name is missing", func(t *testing.T) {
		t.Parallel()
		router, dbConn, _ := CreateTestProductPrerequisite(t)
		defer dbConn.Close()

		content := `{}`
		response := httptest.NewRecorder()

		router.ServeHTTP(response, httptest.NewRequest("POST", "/api/v1/products", bytes.NewBufferString(content)))

		assert.Equal(t, http.StatusUnprocessableEntity, response.Code)
	})
}

func CreateTestProductPrerequisite(t *testing.T) (*http.ServeMux, *sql.DB, time.Time) {
	dbConn := InitInMemoryDatabase(t)

	logger := CreateTestLogger()
	currentTime := time.Now().UTC()
	router := NewRouter(logger, NewProductHandlers(dbConn, &TestTimeProvider{currentTime}, logger), &WebsiteHttpHandlers{})

	return router, dbConn, currentTime
}
