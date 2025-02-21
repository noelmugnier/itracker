package domain

import "time"

type ScraperConfig struct {
	Id                string
	CreatedAt         time.Time
	Enabled           bool
	Cron              string
	Urls              map[string]bool
	ScraperDefinition *ScraperCatalogDefinition
	WebsiteId         string
	WebsiteName       string
	DefinitionId      string
	DefinitionType    string
}

type ScrapedItem struct {
	Id        string
	ScrapedAt time.Time
	Content   []byte
}