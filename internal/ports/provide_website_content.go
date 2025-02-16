package ports

import "itracker/internal/domain"

type IProvideWebsiteContent interface {
	GetContent(url string, contentSelector string) ([]domain.ScrapedItem, error)
}