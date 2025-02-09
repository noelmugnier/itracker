package handlers

import (
	"database/sql"
	"itracker/internal/adapters"
	"log/slog"
	"testing"
	"time"
)

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
