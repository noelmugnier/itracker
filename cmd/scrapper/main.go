package main

import (
	"context"
	"itracker/internal/core/services"
	"itracker/internal/outbound"
	"log/slog"
	"os/signal"
	"syscall"
)

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

	logger.Debug("database configured")

	logger.Debug("configuring services")

	scheduler := outbound.NewGoCronScheduler()
	defer scheduler.Shutdown()

	playwrightContentProvider := outbound.NewPlaywrightWebsiteContentRetriever(logger)
	defer playwrightContentProvider.Close()

	scraperRepository := outbound.NewScraperRepository(logger, db)
	scraperSvc := services.NewScrapingService(
		scheduler,
		scraperRepository,
		playwrightContentProvider,
		outbound.NewScrapedItemRepository(logger),
		logger)

	logger.Debug("services configured")

	logger.Info("starting scraper service...")

	err = scraperSvc.InitJobs(ctx)
	if err != nil {
		panic(err)
	}

	scraperSvc.Start(ctx)
	logger.Info("scraper service started")

	<-ctx.Done()

	logger.Info("scraper service stopped")
}