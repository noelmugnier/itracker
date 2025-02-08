package handlers

import (
	"bytes"
	"database/sql"
	"encoding/json"
	"itracker/internal/adapters"
	"log/slog"
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

		router, dbConn, now := CreateTestPrerequisite(t)
		defer dbConn.Close()

		content := `{"name": "test"}`
		response := httptest.NewRecorder()

		router.ServeHTTP(response, httptest.NewRequest("POST", "/products", bytes.NewBufferString(content)))

		assert.Equal(t, http.StatusCreated, response.Code)

		var id, productName, createdAt string
		err := dbConn.QueryRow("SELECT id, name, datetime(created_at,'unixepoch') as created_at FROM products").Scan(&id, &productName, &createdAt)
		require.NoError(t, err)

		var responseContent createProductResponse
		err = json.Unmarshal(response.Body.Bytes(), &responseContent)

		require.NoError(t, err)
		assert.Equal(t, responseContent.Id, id)
		assert.Equal(t, "test", productName)
		assert.Equal(t, now.Format("2006-01-02 15:04:05"), createdAt)
	})
}

type createProductResponse struct {
	Id string `json:"id"`
}

func CreateTestPrerequisite(t *testing.T) (*http.ServeMux, *sql.DB, time.Time) {
	dbConn := InitInMemoryDatabase(t)

	logger := CreateTestLogger()
	currentTime := time.Now().UTC()
	router := NewRouter(logger, NewProductHandlers(dbConn, &TestTimeProvider{currentTime}, logger), &WebsiteHttpHandlers{})

	return router, dbConn, currentTime
}

type TestTimeProvider struct {
	currentTime time.Time
}

func (tp *TestTimeProvider) UtcNow() time.Time {
	return tp.currentTime
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
