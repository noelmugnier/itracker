package domain

import (
	"errors"
)

var (
	ValidationError        = errors.New("validation error")
	ErrProductNameRequired = errors.New("product name is required")
	ErrWebsiteNameRequired = errors.New("website name is required")
	ErrWebsiteUrlRequired  = errors.New("website url is required")
	ErrWebsiteInvalidUrl   = errors.New("website url is invalid")
)

func CreateValidationError(err error) error {
	return errors.Join(ValidationError, err)
}
