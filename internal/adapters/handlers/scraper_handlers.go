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
	svc    *services.ScraperService
}

func NewScraperHandlers(svc *services.ScraperService, timeProvider ports.ITimeProvider, logger *slog.Logger) *ScraperHttpHandlers {
	return &ScraperHttpHandlers{
		logger: logger,
		time:   timeProvider,
		svc:    svc,
	}
}

type CreateScraperRequest struct {
	Urls []string `json:"urls"`
	Cron string   `json:"cron"`
}

// CreateWebsiteCatalogScraper godoc
// @Summary Create a new scraper for website
// @Tags Scrapers
// @ID create-website-scraper
// @Accept json
// @Produce json
// @Param id path string true "Website ID"
// @Param body body CreateScraperRequest true "CreateScraperRequest"
// @Success 204
// @Failure 400 {object} string
// @Failure 422 {object} string
// @Failure 500 {object} string
// @Router /websites/{id}/scrapers/catalog [post]
func (ph *ScraperHttpHandlers) CreateWebsiteCatalogScraper(w http.ResponseWriter, r *http.Request) {
	ctx := r.Context()
	ph.logger.Log(ctx, slog.LevelDebug, "CreateScraper endpoint called", slog.Any("request", r))

	var request *CreateScraperRequest = nil

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

	_, err = ph.svc.CreateScraper(ctx, websiteId, request.Urls, request.Cron)

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
	w.WriteHeader(http.StatusCreated)
}