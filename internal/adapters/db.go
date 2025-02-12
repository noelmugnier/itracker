package adapters

import "database/sql"
import _ "github.com/ncruces/go-sqlite3/driver"
import _ "github.com/ncruces/go-sqlite3/embed"

func NewSqliteConnector(cs string) (*sql.DB, error) {
	return sql.Open("sqlite3", cs)
}

func InitDatabase(db *sql.DB) error {
	_, err := db.Exec("CREATE TABLE IF NOT EXISTS products (id TEXT PRIMARY KEY, name TEXT NOT NULL, created_at INTEGER NOT NULL)")
	if err != nil {
		return err
	}

	_, err = db.Exec("CREATE TABLE IF NOT EXISTS websites (id TEXT PRIMARY KEY, name TEXT NOT NULL, host TEXT NOT NULL, created_at INTEGER NOT NULL)")
	if err != nil {
		return err
	}

	_, err = db.Exec("CREATE TABLE IF NOT EXISTS scraper_definitions (id TEXT PRIMARY KEY, website_id TEXT NOT NULL, type TEXT NOT NULL, definition TEXT NOT NULL, created_at INTEGER NOT NULL)")
	if err != nil {
		return err
	}

	return nil
}