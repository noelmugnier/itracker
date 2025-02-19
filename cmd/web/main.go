package main

import (
	"context"
	"errors"
	"fmt"
	httpSwagger "github.com/swaggo/http-swagger"
	"itracker/internal/core/services"
	"itracker/internal/inbound"
	"itracker/internal/outbound"
	"log/slog"
	"net/http"
	"os/signal"
	"syscall"

	_ "itracker/cmd/web/docs"
)

// @title iTracker API
// @version 1.0
// @description This is a sample server for iTracker API.
// @BasePath /api/v1
func main() {
	ctx, cancel := signal.NotifyContext(context.Background(), syscall.SIGINT, syscall.SIGTERM)
	defer cancel()

	logger := outbound.NewTextLogger(slog.LevelDebug)
	logger.Debug("configuring database connection")
	db, err := outbound.NewSqliteConnector("file:../itracker.db")
	if err != nil {
		panic(err)
	}
	defer db.Close()

	err = outbound.InitDatabase(db)
	if err != nil {
		panic(err)
	}

	logger.Debug("database connection configured")

	logger.Debug("configuring services")

	timeProvider := outbound.NewTimeProvider()

	productRepository := outbound.NewProductRepository(logger, db)
	websiteRepository := outbound.NewWebsiteRepository(logger, db)
	definitionRepository := outbound.NewDefinitionRepository(logger, db)
	scraperRepository := outbound.NewScraperConfigRepository(logger, db)

	productSvc := services.NewProductService(productRepository, timeProvider, logger)
	websiteSvc := services.NewWebsiteService(websiteRepository, definitionRepository, scraperRepository, timeProvider, logger)

	productHandlers := inbound.NewProductHandlers(productSvc, timeProvider, logger)
	websiteHandlers := inbound.NewWebsiteHandlers(websiteSvc, timeProvider, logger)
	definitionHandlers := inbound.NewWebsiteDefinitionHandlers(websiteSvc, timeProvider, logger)
	scraperHandlers := inbound.NewWebsiteScraperHandlers(websiteSvc, timeProvider, logger)

	logger.Debug("services configured")

	logger.Debug("configuring http router")
	handler := inbound.NewApiRouter(productHandlers, websiteHandlers, definitionHandlers, scraperHandlers)
	handler.HandleFunc("GET /", httpSwagger.WrapHandler)

	srv := &http.Server{
		Addr:    ":8080",
		Handler: inbound.TracingMiddleware(inbound.LoggingMiddleware(handler, logger), logger),
	}
	logger.Debug("http server configured")

	go func() {
		logger.Info(fmt.Sprintf("server listening on http://localhost%s", srv.Addr))
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