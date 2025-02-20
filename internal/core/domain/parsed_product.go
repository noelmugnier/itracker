package domain

type ParsedProduct struct {
	WebsiteId string `json:"website_id,omitempty"`
	Id        string `json:"id,omitempty"`
	Price     string `json:"price,omitempty"`
	Name      string `json:"name,omitempty"`
	Details   string `json:"details,omitempty"`
	Vintage   string `json:"vintage,omitempty"`
}