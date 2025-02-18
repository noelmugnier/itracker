package domain

import "time"

type CreateWebsite struct {
	Name string
	Url  string
}

type Website struct {
	Id        string
	Name      string
	Host      string
	CreatedAt time.Time
	UpdatedAt time.Time
}