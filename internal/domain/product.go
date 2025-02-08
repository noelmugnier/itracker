package domain

import "time"

type CreateProduct struct {
	Name     string
	Websites []*Website
}

type Product struct {
	Id        string
	Name      string
	Websites  []*Website
	CreatedAt time.Time
	UpdatedAt time.Time
}
