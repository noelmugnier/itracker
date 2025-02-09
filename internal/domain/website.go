package domain

import "time"

type Website struct {
	Id        string
	Name      string
	Url       string
	CreatedAt time.Time
	UpdatedAt time.Time
}
