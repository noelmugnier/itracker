package domain

import (
	"errors"
)

var (
	ValidationError                 = errors.New("validation error")
	ErrProductNameRequired          = errors.New("product name is required")
	ErrWebsiteNameRequired          = errors.New("website name is required")
	ErrWebsiteInvalidHost           = errors.New("website host is invalid")
	ErrWebsiteIdRequired            = errors.New("website is required")
	ErrDefinitionFieldsRequired     = errors.New("definition fields are required")
	ErrDefinitionPaginationRequired = errors.New("definition pagination is required")
)

func CreateValidationError(err error) error {
	return errors.Join(ValidationError, err)
}