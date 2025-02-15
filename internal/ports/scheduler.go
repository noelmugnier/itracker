package ports

import (
	"itracker/internal/domain"
)

type IScheduler interface {
	Start()
	ScheduleJob(cron string, function any, parameters ...any) (*domain.Job, error)
	Shutdown() error
}