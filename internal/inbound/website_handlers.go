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

type WebsiteHttpHandlers struct {
	logger *slog.Logger
	time   ports.IProvideTime
	svc    *services.WebsiteService
}

func NewWebsiteHandlers(svc *services.WebsiteService, timeProvider ports.IProvideTime, logger *slog.Logger) *WebsiteHttpHandlers {
	return &WebsiteHttpHandlers{
		logger: logger,
		time:   timeProvider,
		svc:    svc,
	}
}

type CreateWebsiteRequest struct {
	Name string `json:"name"`
	Url  string `json:"host"`
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
		_, err = w.Write([]byte(err.Error()))
		if err != nil {
			ph.logger.Log(ctx, slog.LevelError, "cannot write error to response", slog.Any("error", err))
		}
		return
	}
	defer r.Body.Close()

	websiteId, err := ph.svc.CreateWebsite(ctx, &domain.CreateWebsite{
		Name: request.Name,
		Url:  request.Url,
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

	w.Header().Set("Location", r.RequestURI+"/"+websiteId)
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusCreated)

	encoder := json.NewEncoder(w)
	err = encoder.Encode(CreateWebsiteResponse{Id: websiteId})
	if err != nil {
		ph.logger.Log(ctx, slog.LevelError, "cannot write create website response", slog.Any("error", err))
	}
}