package ports

import (
	"time"
)

type IProvideTime interface {
	UtcNow() time.Time
}