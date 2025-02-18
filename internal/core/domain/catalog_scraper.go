package domain

import "time"

type CreateCatalogScraper struct {
	WebsiteId    string
	DefinitionId string
	Cron         string
	Urls         []string
}

type CatalogScrapper struct {
	Id           string
	DefinitionId string
	CreatedAt    time.Time
	Enabled      bool
	Cron         string
	Urls         map[string]bool
}