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

type ScraperHttpHandlers struct {
	logger *slog.Logger
	time   ports.ITimeProvider
	svc    *services.WebsiteService
}

func NewWebsiteScraperHandlers(svc *services.WebsiteService, timeProvider ports.ITimeProvider, logger *slog.Logger) *ScraperHttpHandlers {
	return &ScraperHttpHandlers{
		logger: logger,
		time:   timeProvider,
		svc:    svc,
	}
}

type CreateCatalogScraperRequest struct {
	Urls []string `json:"urls"`
	Cron string   `json:"cron"`
}

type CreateScraperResponse struct {
	Id string `json:"id"`
}

// CreateCatalogScraper godoc
// @Summary Create a new scraper for website definition
// @Tags Websites
// @ID create-catalog-definition-scraper
// @Accept json
// @Produce json
// @Param id path string true "Website ID"
// @Param definitionId path string true "Catalog Definition ID"
// @Param body body CreateCatalogScraperRequest true "CreateCatalogScraperRequest"
// @Success 201
// @Failure 400 {object} string
// @Failure 422 {object} string
// @Failure 500 {object} string
// @Router /websites/{id}/catalog/definitions/{definitionId}/scrapers [post]
func (ph *ScraperHttpHandlers) CreateCatalogScraper(w http.ResponseWriter, r *http.Request) {
	ctx := r.Context()
	ph.logger.Log(ctx, slog.LevelDebug, "CreateCatalogScraper endpoint called", slog.Any("request", r))

	var request *CreateCatalogScraperRequest = nil

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
	definitionId := r.PathValue("definitionId")

	scraperId, err := ph.svc.CreateCatalogScraper(ctx, &domain.CreateCatalogScraper{
		WebsiteId:    websiteId,
		DefinitionId: definitionId,
		Cron:         request.Cron,
		Urls:         request.Urls,
	})

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

	w.Header().Set("Location", r.RequestURI+"/"+scraperId)
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusCreated)

	encoder := json.NewEncoder(w)
	err = encoder.Encode(CreateScraperResponse{Id: scraperId})
	if err != nil {
		ph.logger.Log(ctx, slog.LevelError, "cannot write create scraper response", slog.Any("error", err))
	}
}