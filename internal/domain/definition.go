package domain

import "time"

type CreateCatalogDefinition struct {
	Id         string
	WebsiteId  string
	CreatedAt  time.Time
	Fields     []*DefinitionField
	Pagination *DefinitionPagination
	Navigation *DefinitionNavigation
}

type CreateProductDefinition struct {
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

type DefinitionPagination struct {
	PageNumberParamName string `json:"page_number_param_name,omitempty"`
	MaxPage             int    `json:"max_page,omitempty"`
}

type DefinitionNavigation struct {
	FieldIdentifier string `json:"field_identifier,omitempty"`
	Navigate        bool   `json:"navigate,omitempty"`
}

type Definition struct {
	Fields     []*DefinitionField    `json:"fields,omitempty"`
	Pagination *DefinitionPagination `json:"pagination,omitempty"`
	Navigation *DefinitionNavigation `json:"navigation,omitempty"`
}