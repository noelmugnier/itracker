package domain

import "time"

type CreateCatalogScraperDefinition struct {
	Id         string
	WebsiteId  string
	CreatedAt  time.Time
	Fields     []*ScraperDefinitionField
	Pagination *ScraperDefinitionPagination
	Navigation *ScraperDefinitionNavigation
}

type CreateProductScraperDefinition struct {
	Id        string
	WebsiteId string
	CreatedAt time.Time
	Fields    []*ScraperDefinitionField
}

type ScraperDefinitionField struct {
	Identifier  string `json:"identifier,omitempty"`
	DisplayName string `json:"display_name,omitempty"`
	Selector    string `json:"selector,omitempty"`
	Required    bool   `json:"required,omitempty"`
}

type ScraperDefinitionPagination struct {
	PageNumberParamName string `json:"page_number_param_name,omitempty"`
	MaxPage             int    `json:"max_page,omitempty"`
}

type ScraperDefinitionNavigation struct {
	FieldIdentifier string `json:"field_identifier,omitempty"`
	Navigate        bool   `json:"navigate,omitempty"`
}