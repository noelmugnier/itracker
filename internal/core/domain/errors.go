package domain

import (
	"errors"
)

var (
	ValidationError        = errors.New("validation error")
	ErrProductNameRequired = errors.New("product name is required")
	ErrWebsiteNameRequired = errors.New("website name is required")
	ErrWebsiteInvalidHost  = errors.New("website host is invalid")
)

func CreateValidationError(err error) error {
	return errors.Join(ValidationError, err)
}