package outbound

import "database/sql"
import _ "github.com/mattn/go-sqlite3"

func NewSqliteConnector(cs string) (*sql.DB, error) {
	return sql.Open("sqlite3", cs)
}

func InitMainDatabase(db *sql.DB) error {
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

	return err
}

func InitParsedItemsDatabase(db *sql.DB) error {
	_, err := db.Exec("CREATE TABLE IF NOT EXISTS parsed_items (id TEXT NOT NULL PRIMARY KEY, item_id TEXT NOT NULL, name TEXT NOT NULL, unit_price TEXT NOT NULL, details TEXT NOT NULL, additional_fields TEXT NOT NULL, scraped_at INTEGER NOT NULL, website_id TEXT NOT NULL)")
	if err != nil {
		return err
	}

	_, err = db.Exec("CREATE VIRTUAL TABLE IF NOT EXISTS search USING fts5(parsed_item_id UNINDEXED, name, website_id UNINDEXED)")
	return err
}