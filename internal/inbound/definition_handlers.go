package inbound

import (
	"encoding/json"
	"errors"
	"itracker/internal/core/domain"
	"itracker/internal/core/ports"
	"itracker/internal/core/services"
	"log/slog"
	"net/http"
)

type DefinitionHttpHandlers struct {
	logger *slog.Logger
	time   ports.IProvideTime
	svc    *services.WebsiteService
}

func NewWebsiteDefinitionHandlers(svc *services.WebsiteService, timeProvider ports.IProvideTime, logger *slog.Logger) *DefinitionHttpHandlers {
	return &DefinitionHttpHandlers{
		logger: logger,
		time:   timeProvider,
		svc:    svc,
	}
}

type CreateCatalogDefinitionRequest struct {
	Scraper *ScraperCatalogDefinitionRequest `json:"scraper,omitempty"`
	Parser  *ParserCatalogDefinitionRequest  `json:"parser,omitempty"`
}

type ScraperCatalogDefinitionRequest struct {
	ContentSelector string                       `json:"contentSelector,omitempty"`
	Pagination      *PaginationDefinitionRequest `json:"pagination,omitempty"`
}

type PaginationDefinitionRequest struct {
	PageNumberParamName string `json:"pageNumberParamName,omitempty"`
	MaxPage             int    `json:"maxPage,omitempty"`
}

type ParserCatalogDefinitionRequest struct {
	Fields []*FieldDefinitionRequest `json:"fields,omitempty"`
}

type FieldDefinitionRequest struct {
	Identifier string                   `json:"identifier,omitempty"`
	Selector   string                   `json:"selector,omitempty"`
	Required   bool                     `json:"required,omitempty"`
	Extract    ExtractDefinitionRequest `json:"extract,omitempty"`
}

type ExtractDefinitionRequest struct {
	Type  string `json:"type,omitempty"`
	Value string `json:"value,omitempty"`
	Regex string `json:"regex,omitempty"`
}

type CreateDefinitionResponse struct {
	Id string `json:"id"`
}

// CreateCatalogDefinition godoc
// @Summary Create a new catalog scraper definition for website
// @Tags Websites
// @ID create-catalog-definition
// @Accept json
// @Produce json
// @Param id path string true "Website ID"
// @Param body body CreateCatalogDefinitionRequest true "CreateCatalogDefinitionRequest"
// @Success 201
// @Failure 400 {object} string
// @Failure 422 {object} string
// @Failure 500 {object} string
// @Router /websites/{id}/catalog/definitions [post]
func (ph *DefinitionHttpHandlers) CreateCatalogDefinition(w http.ResponseWriter, r *http.Request) {
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
	fields := make([]*domain.FieldDefinition, 0, len(request.Parser.Fields))
	for _, field := range request.Parser.Fields {
		fields = append(fields, &domain.FieldDefinition{
			Identifier: field.Identifier,
			Selector:   field.Selector,
			Required:   field.Required,
			Extract: domain.ExtractDefinition{
				Type:  field.Extract.Type,
				Value: field.Extract.Value,
				Regex: field.Extract.Regex,
			},
		})
	}

	definition := &domain.CreateCatalogDefinition{
		WebsiteId: websiteId,
		Scraper: &domain.ScraperCatalogDefinition{
			ContentSelector: request.Scraper.ContentSelector,
			Pagination: &domain.PaginationDefinition{
				PageNumberParamName: request.Scraper.Pagination.PageNumberParamName,
				MaxPage:             request.Scraper.Pagination.MaxPage,
			},
		},
		Parser: &domain.ParserCatalogDefinition{
			Fields: fields,
		},
	}

	definitionId, err := ph.svc.CreateCatalogDefinition(ctx, definition)

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

	w.Header().Set("Location", r.RequestURI+"/"+definitionId)
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusCreated)

	encoder := json.NewEncoder(w)
	err = encoder.Encode(CreateDefinitionResponse{Id: definitionId})
	if err != nil {
		ph.logger.Log(ctx, slog.LevelError, "cannot write create definition response", slog.Any("error", err))
	}
}