using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using FluentResults;
using ITracker.Core.Domain;

namespace ITracker.Adapters.ContentParser;

public class AngleSharpParserAdapter : IContentParser
{
	private readonly IHtmlParser _htmlParser;

	public AngleSharpParserAdapter()
	{
		_htmlParser = new HtmlParser(new HtmlParserOptions { IsNotConsumingCharacterReferences = false });
	}

	public Result<ParsedContent> Parse(string content, Parser parser)
	{
		try
		{
			var document = _htmlParser.ParseDocument(content);
			return ParseElements(document, parser);
		}
		catch (Exception e)
		{
			return Result.Fail(new UnexpectedError(e));
		}
	}

	private Result<ParsedContent> ParseElements(IHtmlDocument document, Parser parser)
	{
		try
		{
			if (document == null)
				return ParsedContent.Empty();

			var parsedElements = new List<ParsedElement>();
			var elements = document.QuerySelectorAll(parser.ElementSelector.Value);
			var errors = new List<IError>();
			foreach (var element in elements)
			{
				var parsedElementResult = ParseElement(element, parser);
				if (parsedElementResult.IsFailed)
				{
					errors.AddRange(parsedElementResult.Errors);
					continue;
				}

				parsedElements.Add(parsedElementResult.Value);
			}

			if (errors.Any())
				return ParsedContent.CompleteWithErrors(parsedElements, errors);

			return ParsedContent.Complete(parsedElements);
		}
		catch (Exception ex)
		{
			return Result.Fail(ex.Message);
		}
	}

	private Result<ParsedElement> ParseElement(IElement element, Parser parser)
	{
		var errors = new List<IError>();
		var parsedProperties = new List<PropertyParsed>();

		foreach (var propertyParser in parser.Properties)
		{
			var propertyElement = element.QuerySelector(propertyParser.ValueSelector.GetCssSelector());
			if (propertyElement == null && !propertyParser.Required)
				continue;
			else if (propertyElement == null)
			{
				errors.Add(new DomainError(ErrorCode.HtmlPropertyParserRequiredPropertyNotFound, $"Property {propertyParser.Name} is required but no matching element found with css parser: {propertyParser.ValueSelector.GetCssSelector()}"));
				continue;
			}

			Result<PropertyParsed> propertyResult;
			if(propertyParser.ValueSelector.GetSource() == PropertyValueSource.Attribute)
				propertyResult = propertyParser.ParseAttributeValue(propertyElement.GetAttribute(propertyParser.ValueSelector.GetAttribute().Name));
			else
				propertyResult = propertyParser.ParseNodeValue(propertyElement.InnerHtml);

			if (!propertyResult.IsFailed)
				parsedProperties.Add(propertyResult.Value);
			else
				errors.AddRange(propertyResult.Errors);
		}

		if (errors.Any())
			return Result.Fail<ParsedElement>(errors);

		return new ParsedElement(parsedProperties);
	}
}
