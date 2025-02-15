package handlers

import (
	"context"
	"fmt"
	"github.com/google/uuid"
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

func TracingMiddleware(next http.Handler, logger *slog.Logger) http.Handler {
	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		requestId := r.Header.Get("X-Request-Id")
		if requestId == "" {
			requestId = r.Header.Get("X-Trace-Id")
		}

		if requestId == "" {
			id, err := uuid.NewRandom()
			if err != nil {
				logger.Log(r.Context(), slog.LevelError, "cannot generate request id", slog.Any("error", err))
				next.ServeHTTP(w, r)
				return
			}

			requestId = id.String()
		}

		ctx := context.WithValue(r.Context(), traceIdKey, requestId)
		w.Header().Set("X-Request-Id", requestId)
		next.ServeHTTP(w, r.WithContext(ctx))
	})
}