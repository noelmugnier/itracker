namespace ITracker.Core.Application;

public class QueryLoggingBehavior<TRequest, TResponse> : IQueryBehavior<TRequest, TResponse>
    where TRequest : IQuery<TResponse>
{
    private readonly ILogger<QueryLoggingBehavior<TRequest, TResponse>> _logger;

    public QueryLoggingBehavior(ILogger<QueryLoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<Result<TResponse>> Handle(TRequest request, MediatR.RequestHandlerDelegate<Result<TResponse>> next, CancellationToken token)
    {
        var commandName = typeof(TRequest).Name;
        var returnType = typeof(TResponse).Name;

        _logger.LogInformation($"Handling {commandName}<{returnType}>");
        var response = await next();
        _logger.LogInformation($"Handled {commandName}<{returnType}>");

        return response;
    }
}
