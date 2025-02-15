package domain

import "time"

type Job struct {
	Id             string
	Name           string
	LastExecutedAt time.Time
	NextRunAt      time.Time
}