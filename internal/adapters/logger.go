package adapters

import (
	"log/slog"
	"os"
)

func NewTextLogger(level slog.Level) *slog.Logger {
	opts := &slog.HandlerOptions{
		Level: level,
	}

	handler := slog.NewTextHandler(os.Stderr, opts)
	logger := slog.New(handler)
	return logger
}
