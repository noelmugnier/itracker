package adapters

import (
	"github.com/go-co-op/gocron/v2"
	"itracker/internal/domain"
	"itracker/internal/ports"
)

type GoCronScheduler struct {
	scheduler gocron.Scheduler
}

func NewGoCronScheduler() ports.IScheduleJobs {
	s, err := gocron.NewScheduler()
	if err != nil {
		panic(err)
	}

	return &GoCronScheduler{
		scheduler: s,
	}
}

func (s GoCronScheduler) StartScheduler() {
	s.scheduler.Start()
}

func (s GoCronScheduler) ScheduleJob(cron string, function any, parameters ...any) (*domain.Job, error) {
	job, err := s.scheduler.NewJob(gocron.CronJob(cron, true), gocron.NewTask(function, parameters...))
	if err != nil {
		return nil, err
	}

	nextRun, _ := job.NextRun()
	lastRun, _ := job.LastRun()

	return &domain.Job{
		Id:             job.ID().String(),
		Name:           job.Name(),
		NextRunAt:      nextRun,
		LastExecutedAt: lastRun,
	}, nil
}

func (s GoCronScheduler) Shutdown() error {
	return s.scheduler.Shutdown()
}