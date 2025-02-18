package outbound

import (
	"fmt"
	"github.com/playwright-community/playwright-go"
	"itracker/internal/core/domain"
	"log/slog"
	"net/url"
	"os"
)

type PlaywrightWebsiteContentRetriever struct {
	pw          *playwright.Playwright
	browser     playwright.BrowserContext
	logger      *slog.Logger
	initialized bool
}

func NewPlaywrightWebsiteContentRetriever(logger *slog.Logger) *PlaywrightWebsiteContentRetriever {
	return &PlaywrightWebsiteContentRetriever{
		logger: logger,
	}
}

func (w *PlaywrightWebsiteContentRetriever) Retrieve(urlToScrap string, contentSelector string) ([]domain.ScrapedItem, error) {
	err := w.init()
	if err != nil {
		return nil, err
	}

	page, err := w.browser.NewPage()
	defer page.Close()

	if err != nil {
		w.logger.Error("could not create new page: %v", err)
		return nil, err
	}

	err = page.Route("**/*", func(route playwright.Route) {
		request := route.Request()
		resourceType := request.ResourceType()

		requestUrl, err := url.Parse(request.URL())
		if err != nil {
			w.logger.Error("could not parse url: %v", err)
		}

		urlToScrapParsed, err := url.Parse(urlToScrap)
		if err != nil {
			w.logger.Error("could not parse url: %v", err)
		}

		if requestUrl.Host != urlToScrapParsed.Host || resourceType == "image" || resourceType == "media" || resourceType == "font" || resourceType == "stylesheet" || resourceType == "other" {
			w.logger.Debug(fmt.Sprintf("blocking request: %s", request.URL()))
			err := route.Abort("aborted")
			if err != nil {
				w.logger.Error("could not abort route: %v", err)
			}
		} else {
			w.logger.Debug(fmt.Sprintf("allowing request: %s", request.URL()))
			_ = route.Continue()
		}
	})

	pageOptions := playwright.PageGotoOptions{
		Referer:   playwright.String("https://www.google.com/search?q=vinatis"),
		Timeout:   playwright.Float(5000),
		WaitUntil: playwright.WaitUntilStateNetworkidle,
	}

	if _, err = page.Goto(urlToScrap, pageOptions); err != nil {
		w.logger.Error("could not goto: %v", err)
		return nil, err
	}

	elements, err := page.QuerySelectorAll(contentSelector)
	if err != nil {
		w.logger.Error("could not query selector: %v", err)
		return nil, err
	}

	items := make([]domain.ScrapedItem, 0)
	for _, element := range elements {
		htmlContent, err := element.InnerHTML()
		if err != nil {
			w.logger.Error("could not get text content: %v", err)
			return nil, err
		}

		items = append(items, fmt.Sprintf("<div>%s</div>", htmlContent))
	}

	return items, nil
}

func (w *PlaywrightWebsiteContentRetriever) Close() error {
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

func (w *PlaywrightWebsiteContentRetriever) init() error {
	if w.initialized {
		return nil
	}

	options := &playwright.RunOptions{
		OnlyInstallShell: true,
		Browsers:         []string{"chromium"},
		Stdout:           os.Stdout,
		Stderr:           os.Stderr,
		Logger:           w.logger,
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
		w.browser, err = w.pw.Chromium.LaunchPersistentContext("", playwright.BrowserTypeLaunchPersistentContextOptions{
			AcceptDownloads:   playwright.Bool(false),
			Args:              nil,
			BypassCSP:         playwright.Bool(true),
			Headless:          playwright.Bool(true),
			IgnoreHttpsErrors: playwright.Bool(true),
			Locale:            playwright.String("fr-FR"),
			ServiceWorkers:    playwright.ServiceWorkerPolicyBlock,
			Proxy:             nil,
			UserAgent:         playwright.String("Mozilla/5.0 (X11; Linux x86_64; rv:135.0) Gecko/20100101 Firefox/135.0"),
		})

		if err != nil {
			w.logger.Error("could not launch browser: %v", err)
			return err
		}
	}

	w.initialized = true
	return nil
}