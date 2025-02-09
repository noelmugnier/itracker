package handlers

import (
	"bytes"
	"database/sql"
	"encoding/json"
	"github.com/stretchr/testify/assert"
	"github.com/stretchr/testify/require"
	"net/http"
	"net/http/httptest"
	"testing"
	"time"
)

func TestCreateWebsiteHandlers(t *testing.T) {
	t.Run("should add a website in the database and return created", func(t *testing.T) {
		t.Parallel()

		router, dbConn, now := CreateTestWebsitePrerequisite(t)
		defer dbConn.Close()

		content := `{"name": "test", "url": "https://test.com"}`
		response := httptest.NewRecorder()

		router.ServeHTTP(response, httptest.NewRequest("POST", "/api/v1/websites", bytes.NewBufferString(content)))

		assert.Equal(t, http.StatusCreated, response.Code)

		var responseContent CreateWebsiteResponse
		err := json.Unmarshal(response.Body.Bytes(), &responseContent)
		require.NoError(t, err)

		var id, websiteName, url, createdAt string
		err = dbConn.QueryRow("SELECT id, name, url, datetime(created_at,'unixepoch') as created_at FROM websites where id = ?", responseContent.Id).Scan(&id, &websiteName, &url, &createdAt)
		require.NoError(t, err)

		assert.Equal(t, responseContent.Id, id)
		assert.Equal(t, "test", websiteName)
		assert.Equal(t, now.Format("2006-01-02 15:04:05"), createdAt)
	})

	t.Run("should return 422 if website name is missing", func(t *testing.T) {
		t.Parallel()
		router, dbConn, _ := CreateTestWebsitePrerequisite(t)
		defer dbConn.Close()

		content := `{}`
		response := httptest.NewRecorder()

		router.ServeHTTP(response, httptest.NewRequest("POST", "/api/v1/websites", bytes.NewBufferString(content)))

		assert.Equal(t, http.StatusUnprocessableEntity, response.Code)
	})

	t.Run("should return 422 if website url is missing", func(t *testing.T) {
		t.Parallel()
		router, dbConn, _ := CreateTestWebsitePrerequisite(t)
		defer dbConn.Close()

		content := `{"name": "test", "url": ""}`
		response := httptest.NewRecorder()

		router.ServeHTTP(response, httptest.NewRequest("POST", "/api/v1/websites", bytes.NewBufferString(content)))

		assert.Equal(t, http.StatusUnprocessableEntity, response.Code)
	})

	t.Run("should return 422 if website url is invalid", func(t *testing.T) {
		t.Parallel()
		router, dbConn, _ := CreateTestWebsitePrerequisite(t)
		defer dbConn.Close()

		content := `{"name": "test", "url": "test"}`
		response := httptest.NewRecorder()

		router.ServeHTTP(response, httptest.NewRequest("POST", "/api/v1/websites", bytes.NewBufferString(content)))

		assert.Equal(t, http.StatusUnprocessableEntity, response.Code)
	})

	t.Run("should create a website with a data source", func(t *testing.T) {
		t.Parallel()
		router, dbConn, _ := CreateTestWebsitePrerequisite(t)
		defer dbConn.Close()

		content := `{"name": "test", "url": "https://test.com", "sources":[{"urls":[], "pagination":{"pageNumberParamName":"", "pageSizeParamName":""}, "fields":[{"name":"price_per_box", "display_name": "Price for a box", "selector":"h1"}]}]}`
		response := httptest.NewRecorder()

		router.ServeHTTP(response, httptest.NewRequest("POST", "/api/v1/websites", bytes.NewBufferString(content)))

		var responseContent CreateWebsiteResponse
		err := json.Unmarshal(response.Body.Bytes(), &responseContent)
		require.NoError(t, err)
	})
}

func CreateTestWebsitePrerequisite(t *testing.T) (*http.ServeMux, *sql.DB, time.Time) {
	dbConn := InitInMemoryDatabase(t)

	logger := CreateTestLogger()
	currentTime := time.Now().UTC()
	router := NewRouter(logger, &ProductHttpHandlers{}, NewWebsiteHandlers(dbConn, &TestTimeProvider{currentTime}, logger))

	return router, dbConn, currentTime
}
