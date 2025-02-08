package domain

import "time"

type Website struct {
	Id        string
	Name      string
	CreatedAt time.Time
	UpdatedAt time.Time
}
