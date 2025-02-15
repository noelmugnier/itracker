package adapters

import (
	"github.com/go-co-op/gocron/v2"
	"itracker/internal/domain"
	"itracker/internal/ports"
)

type Scheduler struct {
	scheduler gocron.Scheduler
}

func NewScheduler() ports.IScheduler {
	s, err := gocron.NewScheduler()
	if err != nil {
		panic(err)
	}

	return &Scheduler{
		scheduler: s,
	}
}

func (s Scheduler) Start() {
	s.scheduler.Start()
}

func (s Scheduler) ScheduleJob(cron string, function any, parameters ...any) (*domain.Job, error) {
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

func (s Scheduler) Shutdown() error {
	return s.scheduler.Shutdown()
}