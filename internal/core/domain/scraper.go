package domain

import "time"

type Scrapper struct {
	Id             string
	CreatedAt      time.Time
	Enabled        bool
	Cron           string
	Urls           map[string]bool
	Scraper        *ScraperCatalogDefinition
	Parser         *ParserCatalogDefinition
	WebsiteId      string
	WebsiteName    string
	DefinitionId   string
	DefinitionType string
}

type ScrapedItem = string