namespace ITracker.Core.Application;

public sealed class CommandValidationBehavior<TRequest, TResponse> : ICommandBehavior<TRequest, TResponse>
	where TRequest : ICommand<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    public CommandValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

	public async Task<Result<TResponse>> Handle(TRequest request, MediatR.RequestHandlerDelegate<Result<TResponse>> next, CancellationToken token)
	{
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);        
        var errors = _validators
            .Select(x => x.Validate(context))
            .SelectMany(x => x.Errors)
            .Where(x => x != null)
            .DistinctBy(d => new {d.PropertyName, d.ErrorMessage});

        if (errors.Any())
            return Result.Fail(new ValidationError(new ValidationException(errors)));

        return await next();
	}
}

public sealed class CommandValidationBehavior<TRequest> : ICommandBehavior<TRequest>
	where TRequest : ICommand
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    public CommandValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

	public async Task<Result> Handle(TRequest request, MediatR.RequestHandlerDelegate<Result> next, CancellationToken token)
	{
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);        
        var errors = _validators
            .Select(x => x.Validate(context))
            .SelectMany(x => x.Errors)
            .Where(x => x != null)
            .DistinctBy(d => new {d.PropertyName, d.ErrorMessage});

        if (errors.Any())
            return Result.Fail(new ValidationError(new ValidationException(errors)));

        return await next();
	}
}
