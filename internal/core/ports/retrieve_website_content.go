package ports

import (
	"itracker/internal/core/domain"
)

type IRetrieveWebsiteContent interface {
	Retrieve(url string, contentSelector string) ([]domain.ScrapedItem, error)
}