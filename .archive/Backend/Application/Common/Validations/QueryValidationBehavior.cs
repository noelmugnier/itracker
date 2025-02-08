namespace ITracker.Core.Application;

public sealed class QueryValidationBehavior<TRequest, TResponse> : IQueryBehavior<TRequest, TResponse>
	where TRequest : IQuery<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    public QueryValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

	public async Task<Result<TResponse>> Handle(TRequest request, MediatR.RequestHandlerDelegate<Result<TResponse>> next, CancellationToken cancellationToken)
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