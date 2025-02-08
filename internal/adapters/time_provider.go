package adapters

import (
	"log/slog"
	"time"
)

type TimeProvider struct {
}

func NewTimeProvider(logger *slog.Logger) *TimeProvider {
	return &TimeProvider{}
}

func (tp *TimeProvider) UtcNow() time.Time {
	return time.Now().UTC()
}
