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