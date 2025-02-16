package adapters

import (
	"github.com/playwright-community/playwright-go"
	"itracker/internal/domain"
	"log/slog"
	"os"
)

type PlaywrightContentProvider struct {
	pw          *playwright.Playwright
	browser     playwright.Browser
	logger      *slog.Logger
	initialized bool
}

func NewPlaywrightContentProvider(logger *slog.Logger) *PlaywrightContentProvider {
	return &PlaywrightContentProvider{
		logger: logger,
	}
}

func (w *PlaywrightContentProvider) GetContent(urlToScrap string, contentSelector string) ([]domain.ScrapedItem, error) {
	err := w.init()
	if err != nil {
		return nil, err
	}

	context, err := w.browser.NewContext(playwright.BrowserNewContextOptions{
		AcceptDownloads:   playwright.Bool(false),
		BypassCSP:         playwright.Bool(true),
		IgnoreHttpsErrors: playwright.Bool(true),
		UserAgent:         playwright.String("Mozilla/5.0 (X11; Linux x86_64; rv:135.0) Gecko/20100101 Firefox/135.0"),
	})
	defer context.Close()

	if err != nil {
		w.logger.Error("could not create new context: %v", err)
		return nil, err
	}

	page, err := context.NewPage()
	defer page.Close()

	if err != nil {
		w.logger.Error("could not create new page: %v", err)
		return nil, err
	}

	if _, err = page.Goto(urlToScrap); err != nil {
		w.logger.Error("could not goto: %v", err)
	}

	elements, err := page.QuerySelectorAll(contentSelector)
	if err != nil {
		w.logger.Error("could not query selector: %v", err)
		return nil, err
	}

	items := make([]domain.ScrapedItem, 0)
	for _, element := range elements {
		text, err := element.TextContent()
		if err != nil {
			w.logger.Error("could not get text content: %v", err)
			return nil, err
		}

		items = append(items, text)
	}

	return items, nil
}

func (w *PlaywrightContentProvider) Close() error {
	if w.browser != nil {
		if err := w.browser.Close(); err != nil {
			w.logger.Error("could not close browser: %v", err)
		}
	}

	if w.pw != nil {
		if err := w.pw.Stop(); err != nil {
			w.logger.Error("could not stop Playwright: %v", err)
		}
	}

	w.pw = nil
	w.browser = nil
	w.initialized = false

	return nil
}

func (w *PlaywrightContentProvider) init() error {
	if w.initialized {
		return nil
	}

	options := &playwright.RunOptions{
		Browsers: []string{"chromium"},
		Stdout:   os.Stdout,
		Stderr:   os.Stderr,
		Logger:   w.logger,
	}

	err := playwright.Install(options)
	if err != nil {
		w.logger.Error("could not start playwright: %v", err)
		return err
	}

	if w.pw == nil {
		pw, err := playwright.Run(options)
		if err != nil {
			w.logger.Error("could not start playwright: %v", err)
			return err
		}

		w.pw = pw
	}

	if w.browser == nil {
		w.browser, err = w.pw.Chromium.Launch(playwright.BrowserTypeLaunchOptions{
			Headless: playwright.Bool(true),
		})

		if err != nil {
			w.logger.Error("could not launch browser: %v", err)
			return err
		}
	}

	w.initialized = true
	return nil
}