package ports

import (
	"itracker/internal/core/domain"
)

type IScheduleJobs interface {
	StartScheduler()
	ScheduleJob(cron string, function any, parameters ...any) (*domain.Job, error)
	Shutdown() error
}