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
	WebsiteName    string
	DefinitionType string
}

type ScrapedContent struct {
	Value    string
	BaseHref string
}