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

	_, err = db.Exec("CREATE TABLE IF NOT EXISTS scraper_definitions (id TEXT NOT NULL PRIMARY KEY, website_id TEXT NOT NULL, type TEXT NOT NULL, definition TEXT NOT NULL, created_at INTEGER NOT NULL, UNIQUE(website_id, type), FOREIGN KEY(website_id) REFERENCES websites(id))")
	if err != nil {
		return err
	}

	_, err = db.Exec("CREATE TABLE IF NOT EXISTS scrapers (id TEXT NOT NULL PRIMARY KEY, scraper_definition_id TEXT NOT NULL, enabled BOOLEAN NOT NULL CHECK (enabled IN (0, 1)), cron TEXT NOT NULL, urls TEXT NOT NULL, created_at INTEGER NOT NULL, FOREIGN KEY(scraper_definition_id) REFERENCES scraper_definitions(id))")
	if err != nil {
		return err
	}

	return nil
}