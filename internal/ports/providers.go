package ports

import (
	"itracker/internal/domain"
	"time"
)

type ITimeProvider interface {
	UtcNow() time.Time
}

type IProvideWebsiteContent interface {
	GetContent(url string, contentSelector string) (*domain.ScrapedContent, error)
}