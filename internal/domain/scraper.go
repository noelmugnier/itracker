package domain

import "time"

type CreateScraper struct {
	Id           string
	DefinitionId string
	CreatedAt    time.Time
	Enabled      bool
	Cron         string
	Urls         map[string]bool
}

type Scrapper struct {
	Id         string
	Type       string
	Website    WebsiteInfo
	Definition DefinitionInfo
	CreatedAt  time.Time
	Enabled    bool
	Cron       string
	Urls       map[string]bool
}

type WebsiteInfo struct {
	Id   string
	Name string
}

type DefinitionInfo struct {
	Id string
	*Definition
}