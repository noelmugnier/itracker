package handlers

import (
	"encoding/json"
	"errors"
	"itracker/internal/domain"
	"itracker/internal/ports"
	"itracker/internal/services"
	"log/slog"
	"net/http"
)

type ScraperDefinitionHttpHandlers struct {
	logger *slog.Logger
	time   ports.ITimeProvider
	svc    *services.WebsiteService
}

func NewWebsiteDefinitionHandlers(svc *services.WebsiteService, timeProvider ports.ITimeProvider, logger *slog.Logger) *ScraperDefinitionHttpHandlers {
	return &ScraperDefinitionHttpHandlers{
		logger: logger,
		time:   timeProvider,
		svc:    svc,
	}
}

type CreateCatalogDefinitionRequest struct {
	Pagination        *DefinitionPaginationRequest `json:"pagination"`
	Fields            []*DefinitionFieldRequest    `json:"fields"`
	ProductNavigation *DefinitionNavigationRequest `json:"productNavigation"`
}

type CreateProductDefinitionRequest struct {
	Fields []*DefinitionFieldRequest `json:"fields"`
}

type DefinitionPaginationRequest struct {
	PageNumberParamName string `json:"pageNumberParamName"`
	MaxPage             int    `json:"maxPage"`
}

type DefinitionFieldRequest struct {
	Identifier  string `json:"identifier"`
	DisplayName string `json:"displayName"`
	Selector    string `json:"selector"`
	Required    bool   `json:"required"`
}

type DefinitionNavigationRequest struct {
	FieldIdentifier string `json:"fieldIdentifier"`
	Navigate        bool   `json:"navigate"`
}

// CreateCatalogDefinition godoc
// @Summary Create a new catalog scraper definition for website
// @Tags Websites
// @ID create-website-catalog-definition
// @Accept json
// @Produce json
// @Param id path string true "Website ID"
// @Param body body CreateCatalogDefinitionRequest true "CreateCatalogDefinitionRequest"
// @Success 204
// @Failure 400 {object} string
// @Failure 422 {object} string
// @Failure 500 {object} string
// @Router /websites/{id}/definitions/catalog [post]
func (ph *ScraperDefinitionHttpHandlers) CreateCatalogDefinition(w http.ResponseWriter, r *http.Request) {
	ctx := r.Context()
	ph.logger.Log(ctx, slog.LevelDebug, "CreateCatalogDefinition endpoint called", slog.Any("request", r))

	var request *CreateCatalogDefinitionRequest = nil

	decoder := json.NewDecoder(r.Body)
	decoder.DisallowUnknownFields()

	err := decoder.Decode(&request)
	if err != nil {
		ph.logger.Log(ctx, slog.LevelError, "cannot parse request content", slog.Any("error", err))
		w.WriteHeader(http.StatusBadRequest)
		_, err = w.Write([]byte(err.Error()))
		if err != nil {
			ph.logger.Log(ctx, slog.LevelError, "cannot write error to response", slog.Any("error", err))
		}
		return
	}
	defer r.Body.Close()

	websiteId := r.PathValue("id")
	fields := make([]*domain.DefinitionField, 0, len(request.Fields))
	for _, field := range request.Fields {
		fields = append(fields, &domain.DefinitionField{
			Identifier:  field.Identifier,
			DisplayName: field.DisplayName,
			Selector:    field.Selector,
			Required:    field.Required,
		})
	}

	pagination := &domain.DefinitionPagination{
		PageNumberParamName: request.Pagination.PageNumberParamName,
		MaxPage:             request.Pagination.MaxPage,
	}

	navigation := &domain.DefinitionNavigation{
		FieldIdentifier: request.ProductNavigation.FieldIdentifier,
		Navigate:        request.ProductNavigation.Navigate,
	}

	_, err = ph.svc.CreateCatalogDefinition(ctx, websiteId, fields, pagination, navigation)

	if err != nil && errors.Is(err, domain.ValidationError) {
		ph.logger.Log(ctx, slog.LevelInfo, "invalid data", slog.Any("error", err))
		w.WriteHeader(http.StatusUnprocessableEntity)
		_, err = w.Write([]byte(err.Error()))
		if err != nil {
			ph.logger.Log(ctx, slog.LevelError, "cannot write error to response", slog.Any("error", err))
		}
		return
	} else if err != nil {
		ph.logger.Log(ctx, slog.LevelError, "an unexpected error occurred", slog.Any("error", err))
		w.WriteHeader(http.StatusInternalServerError)
		_, err = w.Write([]byte(err.Error()))
		if err != nil {
			ph.logger.Log(ctx, slog.LevelError, "cannot write error to response", slog.Any("error", err))
		}
		return
	}

	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusNoContent)
}

// CreateProductDefinition godoc
// @Summary Create a new product scraper definition for website
// @Tags Websites
// @ID create-website-product-definition
// @Accept json
// @Produce json
// @Param id path string true "Website ID"
// @Param body body CreateProductDefinitionRequest true "CreateProductDefinitionRequest"
// @Success 204
// @Failure 400 {object} string
// @Failure 422 {object} string
// @Failure 500 {object} string
// @Router /websites/{id}/definitions/product [post]
func (ph *ScraperDefinitionHttpHandlers) CreateProductDefinition(w http.ResponseWriter, r *http.Request) {
	ctx := r.Context()
	ph.logger.Log(ctx, slog.LevelDebug, "CreateProductDefinition endpoint called", slog.Any("request", r))

	var request *CreateProductDefinitionRequest = nil

	decoder := json.NewDecoder(r.Body)
	decoder.DisallowUnknownFields()

	err := decoder.Decode(&request)
	if err != nil {
		ph.logger.Log(ctx, slog.LevelError, "cannot parse request content", slog.Any("error", err))
		w.WriteHeader(http.StatusBadRequest)
		_, err = w.Write([]byte(err.Error()))
		if err != nil {
			ph.logger.Log(ctx, slog.LevelError, "cannot write error to response", slog.Any("error", err))
		}
		return
	}
	defer r.Body.Close()

	websiteId := r.PathValue("id")
	fields := make([]*domain.DefinitionField, 0, len(request.Fields))
	for _, field := range request.Fields {
		fields = append(fields, &domain.DefinitionField{
			Identifier:  field.Identifier,
			DisplayName: field.DisplayName,
			Selector:    field.Selector,
			Required:    field.Required,
		})
	}

	_, err = ph.svc.CreateProductDefinition(ctx, websiteId, fields)

	if err != nil && errors.Is(err, domain.ValidationError) {
		ph.logger.Log(ctx, slog.LevelInfo, "invalid data", slog.Any("error", err))
		w.WriteHeader(http.StatusUnprocessableEntity)
		_, err = w.Write([]byte(err.Error()))
		if err != nil {
			ph.logger.Log(ctx, slog.LevelError, "cannot write error to response", slog.Any("error", err))
		}
		return
	} else if err != nil {
		ph.logger.Log(ctx, slog.LevelError, "an unexpected error occurred", slog.Any("error", err))
		w.WriteHeader(http.StatusInternalServerError)
		_, err = w.Write([]byte(err.Error()))
		if err != nil {
			ph.logger.Log(ctx, slog.LevelError, "cannot write error to response", slog.Any("error", err))
		}
		return
	}

	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusNoContent)
}