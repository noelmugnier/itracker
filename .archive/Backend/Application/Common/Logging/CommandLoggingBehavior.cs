namespace ITracker.Core.Application;

public class CommandLoggingBehavior<TRequest, TResponse> : ICommandBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    private readonly ILogger<CommandLoggingBehavior<TRequest, TResponse>> _logger;

    public CommandLoggingBehavior(ILogger<CommandLoggingBehavior<TRequest, TResponse>> logger)
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

public class CommandLoggingBehavior<TRequest> : ICommandBehavior<TRequest>
    where TRequest : ICommand
{
    private readonly ILogger<CommandLoggingBehavior<TRequest>> _logger;

    public CommandLoggingBehavior(ILogger<CommandLoggingBehavior<TRequest>> logger)
    {
        _logger = logger;
    }

    public async Task<Result> Handle(TRequest request, MediatR.RequestHandlerDelegate<Result> next, CancellationToken token)
    {
        var commandName = typeof(TRequest).Name;

        _logger.LogInformation($"Handling {commandName}");
        var response = await next();
        _logger.LogInformation($"Handled {commandName}");

        return response;
    }
}