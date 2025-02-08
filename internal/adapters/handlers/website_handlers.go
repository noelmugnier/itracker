package handlers

import (
	"database/sql"
	"log/slog"
	"net/http"
)

type WebsiteHttpHandlers struct {
	logger *slog.Logger
}

func NewWebsiteHandlers(dbConn *sql.DB, logger *slog.Logger) *WebsiteHttpHandlers {
	return &WebsiteHttpHandlers{
		logger: logger,
	}
}

func (ph *WebsiteHttpHandlers) CreateWebsite(w http.ResponseWriter, r *http.Request) {
	ph.logger.Info("CreateWebsite")
	w.WriteHeader(http.StatusNotImplemented)
}

func (ph *WebsiteHttpHandlers) GetWebsites(w http.ResponseWriter, r *http.Request) {
	ph.logger.Info("GetWebsites")
	w.WriteHeader(http.StatusNotImplemented)
}

func (ph *WebsiteHttpHandlers) GetWebsite(w http.ResponseWriter, r *http.Request) {
	ph.logger.Info("GetWebsite")
	w.WriteHeader(http.StatusNotImplemented)
}

func (ph *WebsiteHttpHandlers) UpdateWebsite(w http.ResponseWriter, r *http.Request) {
	ph.logger.Info("UpdateWebsite")
	w.WriteHeader(http.StatusNotImplemented)
}

func (ph *WebsiteHttpHandlers) DeleteWebsite(w http.ResponseWriter, r *http.Request) {
	ph.logger.Info("DeleteWebsite")
	w.WriteHeader(http.StatusNotImplemented)
}
