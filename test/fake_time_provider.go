package test

import "time"

type fakeTimeProvider struct {
	currentTime time.Time
}

func (tp *fakeTimeProvider) UtcNow() time.Time {
	return tp.currentTime
}