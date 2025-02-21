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

	err = outbound.InitMainDatabase(db)
	if err != nil {
		panic(err)
	}

	logger.Debug("database configured")

	logger.Debug("configuring services")

	scheduler := outbound.NewGoCronScheduler()
	defer scheduler.Shutdown()

	timeProvider := outbound.NewTimeProvider()
	playwrightContentProvider := outbound.NewPlaywrightWebsiteContentRetriever(timeProvider, logger)
	defer playwrightContentProvider.Close()

	scraperRepository := outbound.NewScraperConfigRepository(logger, db)
	scrapedItemRepository := outbound.NewScrapedItemRepository(logger)

	scraper := services.NewScraperService(
		scraperRepository,
		playwrightContentProvider,
		scrapedItemRepository,
		logger)

	scrapingScheduler := services.NewSchedulerService(
		scheduler,
		scraperRepository,
		scraper,
		logger)

	logger.Debug("services configured")

	logger.Info("starting scraper service...")

	err = scrapingScheduler.ScheduleScrapers(ctx)
	if err != nil {
		panic(err)
	}

	scheduler.StartScheduler()
	logger.Info("scraper service started")

	<-ctx.Done()

	logger.Info("scraper service stopped")
}