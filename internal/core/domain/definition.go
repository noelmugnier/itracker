package domain

const (
	AttributeExtractor = "attribute"
	TextExtractor      = "text"
)

type FieldDefinition struct {
	Identifier string            `json:"identifier,omitempty"`
	Selector   string            `json:"selector,omitempty"`
	Required   bool              `json:"required,omitempty"`
	Extract    ExtractDefinition `json:"extract,omitempty"`
}

type ExtractDefinition struct {
	Type  string `json:"type,omitempty"`
	Value string `json:"value,omitempty"`
	Regex string `json:"regex,omitempty"`
}