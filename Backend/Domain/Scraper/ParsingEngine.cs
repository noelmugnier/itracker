namespace ITracker.Core.Domain;

public class ParsingEngine
{
	private readonly IContentParser _contentParser;
	private readonly IContentRetriever _contentRetriever;

	public ParsingEngine(
		IContentParser contentParser, 
		IContentRetriever contentRetriever)
	{
		_contentParser = contentParser;
		_contentRetriever = contentRetriever;
	}

	public async Task<ParsingResult> Parse(Scraper scraper, CancellationToken token)
	{
		var maxIterations = scraper.Pagination != null ? scraper.Pagination.MaxPages.Value : 1;
		var currentIteration = 1;
		var canProcess = true;
		var startDate = DateTimeOffset.UtcNow;
		var elements = new List<ParsedElement>();

		var errors = new List<IError>();
		while (canProcess)
		{
			var newUri = scraper.Uri.Value;
			if (scraper.Pagination != null)
				newUri = CreateNextUri(scraper.Uri.Value, scraper.Pagination, currentIteration);

			var downloadResult = await _contentRetriever.Retrieve(newUri, token);
			if (downloadResult.IsFailed)
				return ParsingResult.Failed(scraper.Id, new ParsingError("An error occured while retrieving content", ErrorCode.ParsingEngineRetrievingContentFailed), new DateRange(startDate, DateTimeOffset.UtcNow));

			var parsedContentResult = _contentParser.Parse(downloadResult.Value, scraper.Parser);
			if (parsedContentResult.IsFailed)
				return ParsingResult.Failed(scraper.Id, new ParsingError("An error occured while parsing content", ErrorCode.ParsingEngineParsingContentFailed), new DateRange(startDate, DateTimeOffset.UtcNow));

			var parsedContent = parsedContentResult.Value;
			if (parsedContent.Errors.Any())
				errors.AddRange(parsedContent.Errors);
				
			if (parsedContent.Elements.Any())
				elements.AddRange(parsedContent.Elements);
			else
				canProcess = false;

			currentIteration++;

			canProcess &= currentIteration <= maxIterations;
			canProcess &= !token.IsCancellationRequested;
		}

		if(errors.Any())				
			return ParsingResult.ReviewRequired(scraper.CatalogId, scraper.Id, new DateRange(startDate, DateTimeOffset.UtcNow), elements, errors.Select(e => new ParsingError(e.Message, ErrorCode.ParsingEnginePropertyParsingFailed)) );

		return ParsingResult.Completed(scraper.CatalogId, scraper.Id, new DateRange(startDate, DateTimeOffset.UtcNow), elements);
	}

	private Uri CreateNextUri(Uri uri, Pagination pagination, int currentIteration)
	{
		var scraperParameter = pagination.PageNumberParameterName;
		var sizeParameter = pagination.PageSizeParameterName;

		var uriBuilder = new UriBuilder(uri);
		uriBuilder.Query += $"&{scraperParameter.Value}={currentIteration}";

		if (!string.IsNullOrWhiteSpace(sizeParameter?.Value))
			uriBuilder.Query += $"&{sizeParameter.Value}={pagination.PageSize}";

		return uriBuilder.Uri;
	}
}
