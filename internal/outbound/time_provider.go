package outbound

import (
	"time"
)

type TimeProvider struct {
}

func NewTimeProvider() *TimeProvider {
	return &TimeProvider{}
}

func (tp *TimeProvider) UtcNow() time.Time {
	return time.Now().UTC()
}