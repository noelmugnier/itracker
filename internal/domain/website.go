package domain

import "time"

type Website struct {
	Id        string
	Name      string
	Host      string
	CreatedAt time.Time
	UpdatedAt time.Time
}

type CatalogDefinition struct {
	Id         string
	WebsiteId  string
	CreatedAt  time.Time
	Fields     []*DefinitionField
	Pagination *PaginationDefinition
	Navigation *ProductNavigation
}

type ProductDefinition struct {
	Id        string
	WebsiteId string
	CreatedAt time.Time
	Fields    []*DefinitionField
}

type DefinitionField struct {
	Identifier  string `json:"identifier,omitempty"`
	DisplayName string `json:"display_name,omitempty"`
	Selector    string `json:"selector,omitempty"`
	Required    bool   `json:"required,omitempty"`
}

type PaginationDefinition struct {
	PageNumberParamName string `json:"page_number_param_name,omitempty"`
	MaxPage             int    `json:"max_page,omitempty"`
}

type ProductNavigation struct {
	FieldIdentifier string `json:"field_identifier,omitempty"`
	Navigate        bool   `json:"navigate,omitempty"`
}