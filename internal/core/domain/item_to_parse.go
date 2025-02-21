package domain

import "time"

type ItemToParse struct {
	WebsiteId    string
	DefinitionId string
	FileName     string
	ScrapedAt    time.Time
}