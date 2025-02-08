package main

import (
	"context"
	"errors"
	httpSwagger "github.com/swaggo/http-swagger"
	"itracker/internal/adapters"
	"itracker/internal/adapters/handlers"
	"log/slog"
	"net/http"
	"os/signal"
	"syscall"

	_ "itracker/cmd/web/docs"
)

// @title iTracker API
// @version 1.0
// @description This is a sample server for iTracker API.
// @host itrack.it.com
// @BasePath /v1
func main() {
	ctx, cancel := signal.NotifyContext(context.Background(), syscall.SIGINT, syscall.SIGTERM)
	defer cancel()

	logger := adapters.NewTextLogger(slog.LevelDebug)
	logger.Debug("configuring database connection")
	dbConn, err := adapters.NewSqliteConnector("file:itracker.db")
	if err != nil {
		panic(err)
	}
	defer dbConn.Close()

	err = adapters.InitDatabase(dbConn)
	if err != nil {
		panic(err)
	}

	logger.Debug("database connection configured")

	logger.Debug("configuring services")
	timeProvider := adapters.NewTimeProvider(logger)
	productHandlers := handlers.NewProductHandlers(dbConn, timeProvider, logger)
	websiteHandlers := handlers.NewWebsiteHandlers(dbConn, logger)
	logger.Debug("services configured")

	logger.Debug("configuring http router")
	handler := handlers.NewRouter(logger, productHandlers, websiteHandlers)
	handler.HandleFunc("GET /swagger/", httpSwagger.WrapHandler)
	srv := &http.Server{
		Addr:    ":8080",
		Handler: handler,
	}
	logger.Debug("http server configured")

	go func() {
		logger.Info("server listening...")
		if err := srv.ListenAndServe(); err != nil && !errors.Is(err, http.ErrServerClosed) {
			logger.Error("server error", "error", err)
		}

		logger.Debug("stopping server...")
	}()

	<-ctx.Done()
	if err := srv.Shutdown(ctx); err != nil {
		logger.Error("server shutdown error", "error", err)
	}

	logger.Info("server stopped")
}
