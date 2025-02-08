namespace ITracker.Core.Domain;

public interface IContentRetriever
{
	Task<Result<string>> Retrieve(Uri uri, CancellationToken token);
}

