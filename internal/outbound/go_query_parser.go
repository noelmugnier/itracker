package outbound

import (
	"bytes"
	"context"
	"github.com/PuerkitoBio/goquery"
	"itracker/internal/core/domain"
	"log/slog"
	"regexp"
)

type GoQueryParser struct {
	logger *slog.Logger
}

func NewGoQueryParser(logger *slog.Logger) *GoQueryParser {
	return &GoQueryParser{
		logger: logger,
	}
}

func (gqp *GoQueryParser) Parse(ctx context.Context, content []byte, parserDefinition *domain.ParserCatalogDefinition) (map[string]string, error) {
	doc, err := goquery.NewDocumentFromReader(bytes.NewReader(content))
	if err != nil {
		return nil, err
	}

	parsedFields := make(map[string]string)

	for _, field := range parserDefinition.Fields {
		doc.
			Find(field.Selector).
			Each(func(i int, s *goquery.Selection) {
				var value string
				switch field.Extract.Type {
				case domain.TextExtractor:
					value = s.Text()
					break
				case domain.AttributeExtractor:
					value, _ = s.Attr(field.Extract.Value)
					break
				}

				if field.Extract.Regex != "" {
					re, err := regexp.Compile(field.Extract.Regex)
					if err != nil {
						gqp.logger.Error("failed to match regex: %w", err)
						return
					}

					matches := re.FindStringSubmatch(value)
					valueIndex := re.SubexpIndex("value")
					if valueIndex >= 0 {
						value = matches[valueIndex]
					}
				}

				if field.Required && value == "" {
					gqp.logger.Warn("required field is missing", slog.String("field", field.Identifier))
				}

				parsedFields[field.Identifier] = value
			})
	}

	return parsedFields, nil
}