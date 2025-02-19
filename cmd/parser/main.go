package main

import (
	"context"
	"itracker/internal/core/services"
	"itracker/internal/outbound"
	"log/slog"
	"os/signal"
	"syscall"
	"time"
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

	parserSvc := services.NewParserService(
		outbound.NewGoQueryParser(logger),
		outbound.NewDefinitionRepository(logger, db),
		outbound.NewScrapedItemRepository(logger),
		logger)

	logger.Debug("services configured")

	logger.Info("starting parser service...")

	timerTicker := time.NewTicker(5 * time.Second)

	go func() {
		for {
			select {
			case <-timerTicker.C:
				parserSvc.ParseScrapedItems(ctx)
			}
		}
	}()

	logger.Info("parser service started")

	<-ctx.Done()

	logger.Info("parser service stopped")
}