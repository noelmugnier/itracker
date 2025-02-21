package domain

import "time"

type ParsedProduct struct {
	WebsiteId        string            `json:"website_id,omitempty"`
	Id               string            `json:"id,omitempty"`
	Name             string            `json:"name,omitempty"`
	UnitPrice        string            `json:"unit_price,omitempty"`
	Details          string            `json:"details,omitempty"`
	AdditionalFields map[string]string `json:"additional_fields,omitempty"`
	ScrapedAt        time.Time         `json:"scraped_at,omitempty"`
}