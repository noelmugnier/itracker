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
	IdentifierField  *ParserSelector    `json:"identifier,omitempty"`
	NameField        *ParserSelector    `json:"name,omitempty"`
	UnitPriceField   *ParserSelector    `json:"unit_price,omitempty"`
	DetailsField     *ParserSelector    `json:"details,omitempty"`
	AdditionalFields []*FieldDefinition `json:"additional_fields,omitempty"`
}