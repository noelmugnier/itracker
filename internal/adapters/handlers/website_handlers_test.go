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