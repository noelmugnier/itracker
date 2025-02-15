package main

import (
	"context"
	"itracker/internal/adapters"
	"itracker/internal/adapters/repositories"
	"itracker/internal/services"
	"log/slog"
	"os/signal"
	"syscall"
)

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

	logger.Debug("database configured")

	logger.Debug("configuring services")

	scheduler := adapters.NewScheduler()
	defer scheduler.Shutdown()

	scraperSvc := services.NewScrapingService(
		scheduler,
		repositories.NewScraperRepository(logger, db),
		logger)

	logger.Debug("services configured")

	logger.Info("starting scraper...")

	err = scraperSvc.InitJobs(ctx)
	if err != nil {
		panic(err)
	}

	scraperSvc.Start(ctx)
	logger.Info("scraper started")

	<-ctx.Done()

	logger.Info("scraper stopped")
}