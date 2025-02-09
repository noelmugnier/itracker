package handlers

import (
	"database/sql"
	"encoding/json"
	"errors"
	"itracker/internal/adapters/repositories"
	"itracker/internal/domain"
	"itracker/internal/ports"
	"itracker/internal/services"
	"log/slog"
	"net/http"
)

type WebsiteHttpHandlers struct {
	logger       *slog.Logger
	timeProvider ports.ITimeProvider
	service      *services.WebsiteService
}

func NewWebsiteHandlers(dbConn *sql.DB, timeProvider ports.ITimeProvider, logger *slog.Logger) *WebsiteHttpHandlers {
	return &WebsiteHttpHandlers{
		logger:       logger,
		timeProvider: timeProvider,
		service:      services.NewWebsiteService(logger, repositories.NewWebsiteRepository(logger, dbConn), timeProvider),
	}
}

type CreateWebsiteRequest struct {
	Name string `json:"name"`
	Url  string `json:"url"`
}

type CreateWebsiteResponse struct {
	Id string `json:"id"`
}

// CreateWebsite godoc
// @Summary Create a new website
// @Tags Websites
// @ID create-website
// @Accept json
// @Produce json
// @Param body body CreateWebsiteRequest true "CreateWebsiteRequest"
// @Success 201 {object} CreateWebsiteResponse
// @Failure 400 {object} string
// @Failure 422 {object} string
// @Failure 500 {object} string
// @Router /websites [post]
func (ph *WebsiteHttpHandlers) CreateWebsite(w http.ResponseWriter, r *http.Request) {
	ctx := r.Context()
	ph.logger.Log(ctx, slog.LevelDebug, "CreateWebsite endpoint called", slog.Any("request", r))

	var request *CreateWebsiteRequest = nil

	decoder := json.NewDecoder(r.Body)
	decoder.DisallowUnknownFields()

	err := decoder.Decode(&request)
	if err != nil {
		ph.logger.Log(ctx, slog.LevelError, "cannot parse request content", slog.Any("error", err))
		w.WriteHeader(http.StatusBadRequest)
		w.Write([]byte(err.Error()))
		return
	}
	defer r.Body.Close()

	websiteId, err := ph.service.CreateWebsite(ctx, request.Name, request.Url)

	if err != nil && errors.Is(err, domain.ValidationError) {
		ph.logger.Log(ctx, slog.LevelInfo, "invalid data", slog.Any("error", err))
		w.WriteHeader(http.StatusUnprocessableEntity)
		w.Write([]byte(err.Error()))
		return
	} else if err != nil {
		ph.logger.Log(ctx, slog.LevelError, "an unexpected error occured", slog.Any("error", err))
		w.WriteHeader(http.StatusInternalServerError)
		w.Write([]byte(err.Error()))
		return
	}

	w.Header().Set("Location", r.RequestURI+"/"+websiteId)
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusCreated)

	encoder := json.NewEncoder(w)
	encoder.Encode(CreateWebsiteResponse{Id: websiteId})
}
