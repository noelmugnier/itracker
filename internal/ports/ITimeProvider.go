package ports

import "time"

type ITimeProvider interface {
	UtcNow() time.Time
}
