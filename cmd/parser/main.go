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

	logger.Debug("configuring databases connection")
	mainDb, err := outbound.NewSqliteConnector("file:../itracker.db")
	if err != nil {
		panic(err)
	}
	defer mainDb.Close()

	parsedDb, err := outbound.NewSqliteConnector("file:../parsed_items.db")
	if err != nil {
		panic(err)
	}
	defer parsedDb.Close()

	err = outbound.InitMainDatabase(mainDb)
	if err != nil {
		panic(err)
	}

	err = outbound.InitParsedItemsDatabase(parsedDb)
	if err != nil {
		panic(err)
	}

	logger.Debug("databases configured")

	logger.Debug("configuring services")

	parserSvc := services.NewParserService(
		outbound.NewGoQueryParser(logger),
		outbound.NewParsedItemRepository(parsedDb, logger),
		outbound.NewDefinitionRepository(mainDb, logger),
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