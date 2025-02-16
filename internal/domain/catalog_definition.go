package domain

import "time"

type CreateCatalogDefinition struct {
	WebsiteId string
	Scraper   *ScraperCatalogDefinition
	Parser    *ParserCatalogDefinition
}

type CatalogDefinition struct {
	Id        string
	WebsiteId string
	CreatedAt time.Time
	Scraper   *ScraperCatalogDefinition
	Parser    *ParserCatalogDefinition
}

type ScraperCatalogDefinition struct {
	ContentSelector string                `json:"content_selector,omitempty"`
	Pagination      *PaginationDefinition `json:"pagination,omitempty"`
}

type PaginationDefinition struct {
	PageNumberParamName string `json:"page_number_param_name,omitempty"`
	MaxPage             int    `json:"max_page,omitempty"`
}

type ParserCatalogDefinition struct {
	ItemSelector string             `json:"item_selector,omitempty"`
	Fields       []*FieldDefinition `json:"fields,omitempty"`
}