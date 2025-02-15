package main

import (
	"context"
	"errors"
	"fmt"
	httpSwagger "github.com/swaggo/http-swagger"
	"itracker/internal/adapters"
	"itracker/internal/adapters/handlers"
	"itracker/internal/adapters/repositories"
	"itracker/internal/services"
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

	logger := adapters.NewTextLogger(slog.LevelDebug)
	logger.Debug("configuring database connection")
	db, err := adapters.NewSqliteConnector("file:itracker.db")
	if err != nil {
		panic(err)
	}
	defer db.Close()

	err = adapters.InitDatabase(db)
	if err != nil {
		panic(err)
	}

	logger.Debug("database connection configured")

	logger.Debug("configuring services")

	timeProvider := adapters.NewTimeProvider()

	productRepository := repositories.NewProductRepository(logger, db)
	websiteRepository := repositories.NewWebsiteRepository(logger, db)
	definitionRepository := repositories.NewDefinitionRepository(logger, db)
	scraperRepository := repositories.NewScraperRepository(logger, db)

	productSvc := services.NewProductService(productRepository, timeProvider, logger)
	websiteSvc := services.NewWebsiteService(websiteRepository, definitionRepository, scraperRepository, timeProvider, logger)

	productHandlers := handlers.NewProductHandlers(productSvc, timeProvider, logger)
	websiteHandlers := handlers.NewWebsiteHandlers(websiteSvc, timeProvider, logger)
	definitionHandlers := handlers.NewWebsiteDefinitionHandlers(websiteSvc, timeProvider, logger)
	scraperHandlers := handlers.NewWebsiteScraperHandlers(websiteSvc, timeProvider, logger)

	logger.Debug("services configured")

	logger.Debug("configuring http router")
	handler := handlers.NewApiRouter(productHandlers, websiteHandlers, definitionHandlers, scraperHandlers)
	handler.HandleFunc("GET /", httpSwagger.WrapHandler)

	srv := &http.Server{
		Addr:    ":8080",
		Handler: handlers.TracingMiddleware(handlers.LoggingMiddleware(handler, logger), logger),
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