package outbound

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

	_, err = db.Exec("CREATE TABLE IF NOT EXISTS definitions (id TEXT NOT NULL PRIMARY KEY, type TEXT NOT NULL, scraper TEXT NOT NULL, parser TEXT NOT NULL, created_at INTEGER NOT NULL, website_id TEXT NOT NULL, UNIQUE(website_id, type), FOREIGN KEY(website_id) REFERENCES websites(id))")
	if err != nil {
		return err
	}

	_, err = db.Exec("CREATE TABLE IF NOT EXISTS scraper_configs (id TEXT NOT NULL PRIMARY KEY, cron TEXT NOT NULL, urls TEXT NOT NULL, created_at INTEGER NOT NULL, enabled BOOLEAN NOT NULL CHECK (enabled IN (0, 1)), definition_id TEXT NOT NULL, FOREIGN KEY(definition_id) REFERENCES definitions(id))")
	if err != nil {
		return err
	}

	return nil
}