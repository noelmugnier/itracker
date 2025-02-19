package domain

type FieldDefinition struct {
	Identifier  string `json:"identifier,omitempty"`
	DisplayName string `json:"display_name,omitempty"`
	Selector    string `json:"selector,omitempty"`
	Required    bool   `json:"required,omitempty"`
}

type ParsedField struct {
	Identifier string `json:"identifier,omitempty"`
	Value      string `json:"value,omitempty"`
}