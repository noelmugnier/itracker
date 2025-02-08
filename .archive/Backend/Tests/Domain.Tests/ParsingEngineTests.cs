namespace ITracker.Core.Domain.Tests;

public class ParsingEngineTests
{
    [Fact]
    public async Task Should_ReturnAllElementsOfRequestedPage_When_PaginationIsDisabled()
    {
        var pageDownloaderMocked = new Mock<IContentRetriever>();
        var parserEngine = new ParsingEngine(new FakeContentParser(contentToParse => GetParsedElements(2)), pageDownloaderMocked.Object);

        var content = @"
          <section>
            <article><h1>p1</h1><h5>Product1</h5><div>12.5</div></article>
            <article><h1>p1</h1><h5>Product1</h5><div>12.5</div></article>
          </section>";

        pageDownloaderMocked.Setup(c => c.Retrieve(It.IsAny<Uri>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(Result.Ok(content)));

        var (_, _, scraper) = new TestScraperBuilder().Build();

        var results = await parserEngine.Parse(scraper, CancellationToken.None);

        results.Status.Should().Be(ParsingStatus.Completed);
        results.Elements.Should().HaveCount(2);
    }

	[Fact]
    public async Task Should_ReturnElementsUntilMaxPage_When_PaginationIsEnabled()
    {
        var pageDownloaderMocked = new Mock<IContentRetriever>();
        var parserEngine = new ParsingEngine(new FakeContentParser(contentToParse => GetParsedElements(1)), pageDownloaderMocked.Object);

        var content = @"
          <section>
            <article><h1>p1</h1><h5>Product1</h5><div>12.5</div></article>
          </section>";

        pageDownloaderMocked.Setup(c => c.Retrieve(It.IsAny<Uri>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(Result.Ok(content)));
        
        var (_, _, scraper) = new TestScraperBuilder()
          .WithPagination(3)
          .Build();

        var results = await parserEngine.Parse(scraper, CancellationToken.None);

        results.Status.Should().Be(ParsingStatus.Completed);
        results.Elements.Should().HaveCount(3);
    }

    [Fact]
    public async Task Should_ReturnElementsWhileAvailable_When_PaginationIsEnabled()
    {
        var content = @"
          <section>
            <article><h1>p1</h1><h5>Product1</h5><div>12.5</div></article>
          </section>";

        var emptyContent = "<section></section>";
        
        var pageDownloaderMocked = new Mock<IContentRetriever>();
        pageDownloaderMocked.Setup(c => c.Retrieve(It.Is<Uri>(uri => !uri.ToString().Contains("p=3")), It.IsAny<CancellationToken>())).Returns(Task.FromResult(Result.Ok(content)));
        pageDownloaderMocked.Setup(c => c.Retrieve(It.Is<Uri>(uri => uri.ToString().Contains("p=3")), It.IsAny<CancellationToken>())).Returns(Task.FromResult(Result.Ok(emptyContent)));

        var parserEngine = new ParsingEngine(new FakeContentParser(contentToParse => {
          return contentToParse == emptyContent 
            ? ParsedContent.Empty()
            : GetParsedElements(1);
          }), pageDownloaderMocked.Object);

        var (_, _, scraper) = new TestScraperBuilder()
          .WithPagination(5)
          .Build();

        var results = await parserEngine.Parse(scraper, CancellationToken.None);

        results.Status.Should().Be(ParsingStatus.Completed);
        results.Elements.Should().HaveCount(2);
    }

    [Fact]
    public async Task Should_ReturnStatusFailed_When_DownloadErrorOccured()
    {
        var pageDownloaderMocked = new Mock<IContentRetriever>();
        var parserEngine = new ParsingEngine(new FakeContentParser(), pageDownloaderMocked.Object);

        pageDownloaderMocked.Setup(c => c.Retrieve(It.IsAny<Uri>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(Result.Fail<string>(new UnexpectedError(""))));

        var (_, _, scraper) = new TestScraperBuilder()
          .WithPagination(5)
          .Build();

        var results = await parserEngine.Parse(scraper, CancellationToken.None);

        results.Status.Should().Be(ParsingStatus.Failed);
        results.Errors.Should().HaveCount(1);
        results.Errors.Single().Code.Should().Be(ErrorCode.ParsingEngineRetrievingContentFailed);
    }

    [Fact]
    public async Task Should_ReturnStatusFailed_When_ParsingErrorOccured()
    {
        var pageDownloaderMocked = new Mock<IContentRetriever>();
        var parserEngine = new ParsingEngine(new FakeContentParser(content => {
            throw new Exception();
        }), pageDownloaderMocked.Object);

        pageDownloaderMocked.Setup(c => c.Retrieve(It.IsAny<Uri>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(Result.Ok("<section></section>")));

        var (_, _, scraper) = new TestScraperBuilder()
          .WithPagination(5)
          .Build();

        var results = await parserEngine.Parse(scraper, CancellationToken.None);

        results.Status.Should().Be(ParsingStatus.Failed);
        results.Errors.Should().HaveCount(1);
        results.Errors.Single().Code.Should().Be(ErrorCode.ParsingEngineParsingContentFailed);
    }

    private ParsedContent GetParsedElements(int v)
    {
        var props = new List<ParsedElement>();
        for(int i = 0; i < v; i++)
            props.Add(new ParsedElement(GetProperties()));

        return ParsedContent.Complete(props);
    }

    private IEnumerable<PropertyParsed> GetProperties()
    {
        return new List<PropertyParsed>
        {
            new TextParsed(PropertyName.Identifier, new Text("11")),
            new TextParsed(PropertyName.DisplayName, new Text("test")),
        };
    }
}
