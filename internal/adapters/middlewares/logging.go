package middlewares

import (
	"fmt"
	"log/slog"
	"net/http"
	"time"
)

var (
	traceIdKey = "traceId"
)

func LoggingMiddleware(next http.Handler, logger *slog.Logger) http.Handler {
	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		logger.Log(r.Context(), slog.LevelInfo, fmt.Sprintf("%s: %s %s", time.Now().Format("2006-01-02T15:04:05"), r.Method, r.URL.Path), slog.Any("traceId", r.Context().Value(traceIdKey)))
		next.ServeHTTP(w, r)
	})
}