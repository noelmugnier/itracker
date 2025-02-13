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
	svc    *services.ScraperDefinitionService
}

func NewScraperDefinitionHandlers(svc *services.ScraperDefinitionService, timeProvider ports.ITimeProvider, logger *slog.Logger) *ScraperDefinitionHttpHandlers {
	return &ScraperDefinitionHttpHandlers{
		logger: logger,
		time:   timeProvider,
		svc:    svc,
	}
}

type CreateCatalogScraperDefinitionRequest struct {
	Pagination        *ScraperDefinitionPagination        `json:"pagination"`
	Fields            []*ScraperDefinitionField           `json:"fields"`
	ProductNavigation *ScraperDefinitionProductNavigation `json:"productNavigation"`
}

type CreateProductScraperDefinitionRequest struct {
	Fields []*ScraperDefinitionField `json:"fields"`
}

type ScraperDefinitionPagination struct {
	PageNumberParamName string `json:"pageNumberParamName"`
	MaxPage             int    `json:"maxPage"`
}

type ScraperDefinitionField struct {
	Identifier  string `json:"identifier"`
	DisplayName string `json:"displayName"`
	Selector    string `json:"selector"`
	Required    bool   `json:"required"`
}

type ScraperDefinitionProductNavigation struct {
	FieldIdentifier string `json:"fieldIdentifier"`
	Navigate        bool   `json:"navigate"`
}

// CreateCatalogScraperDefinition godoc
// @Summary Create a new catalog scraper definition for website
// @Tags ScraperDefinitions
// @ID create-catalog-scraper-definition
// @Accept json
// @Produce json
// @Param id path string true "ScraperDefinition ID"
// @Param body body CreateCatalogScraperDefinitionRequest true "CreateCatalogScraperDefinitionRequest"
// @Success 204
// @Failure 400 {object} string
// @Failure 422 {object} string
// @Failure 500 {object} string
// @Router /websites/{id}/definitions/catalog [post]
func (ph *ScraperDefinitionHttpHandlers) CreateCatalogScraperDefinition(w http.ResponseWriter, r *http.Request) {
	ctx := r.Context()
	ph.logger.Log(ctx, slog.LevelDebug, "CreateCatalogScraperDefinition endpoint called", slog.Any("request", r))

	var request *CreateCatalogScraperDefinitionRequest = nil

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
	fields := make([]*domain.ScraperDefinitionField, 0, len(request.Fields))
	for _, field := range request.Fields {
		fields = append(fields, &domain.ScraperDefinitionField{
			Identifier:  field.Identifier,
			DisplayName: field.DisplayName,
			Selector:    field.Selector,
			Required:    field.Required,
		})
	}

	pagination := &domain.ScraperDefinitionPagination{
		PageNumberParamName: request.Pagination.PageNumberParamName,
		MaxPage:             request.Pagination.MaxPage,
	}

	navigation := &domain.ScraperDefinitionNavigation{
		FieldIdentifier: request.ProductNavigation.FieldIdentifier,
		Navigate:        request.ProductNavigation.Navigate,
	}

	_, err = ph.svc.CreateCatalogScraperDefinition(ctx, websiteId, fields, pagination, navigation)

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

// CreateProductScraperDefinition godoc
// @Summary Create a new product scraper definition for website
// @Tags ScraperDefinitions
// @ID create-product-scraper-definition
// @Accept json
// @Produce json
// @Param id path string true "ScraperDefinition ID"
// @Param body body CreateProductScraperDefinitionRequest true "CreateProductScraperDefinitionRequest"
// @Success 204
// @Failure 400 {object} string
// @Failure 422 {object} string
// @Failure 500 {object} string
// @Router /websites/{id}/definitions/product [post]
func (ph *ScraperDefinitionHttpHandlers) CreateProductScraperDefinition(w http.ResponseWriter, r *http.Request) {
	ctx := r.Context()
	ph.logger.Log(ctx, slog.LevelDebug, "CreateProductScraperDefinition endpoint called", slog.Any("request", r))

	var request *CreateProductScraperDefinitionRequest = nil

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
	fields := make([]*domain.ScraperDefinitionField, 0, len(request.Fields))
	for _, field := range request.Fields {
		fields = append(fields, &domain.ScraperDefinitionField{
			Identifier:  field.Identifier,
			DisplayName: field.DisplayName,
			Selector:    field.Selector,
			Required:    field.Required,
		})
	}

	_, err = ph.svc.CreateProductScraperDefinition(ctx, websiteId, fields)

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