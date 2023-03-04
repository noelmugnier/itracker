using FluentValidation;

namespace ITracker.Adapters.Api;

public static class FluentResultsExtensions
{
	public static Result<TNew> MapResult<TOld, TNew>(this Result<TOld> result, Func<TOld, TNew> valueConverter) where TNew : notnull
	{
		if(result.IsSuccess)
			return result.ToResult<TNew>(valueConverter);

		return result.ToResult();
	}

	public static OneOf<EmptyResponse, IEnumerable<NotFoundException>, IEnumerable<ValidationException>, IEnumerable<Exception>> ToResponse(this Result result, string? url = null)
	{
		if(result.IsSuccess)
			return new EmptyResponse();

		return result.ToErrorResult().Match<OneOf<EmptyResponse, IEnumerable<NotFoundException>, IEnumerable<ValidationException>, IEnumerable<Exception>>>(
			notFound => notFound.ToList(),
			validation => validation.ToList(),
			unexpected => unexpected.ToList()
		);	
	}

	public static OneOf<T, IEnumerable<NotFoundException>, IEnumerable<ValidationException>, IEnumerable<Exception>> ToResponse<T>(this Result<T> result)
	{
		if(result.IsSuccess)
			return result.Value;

		return result.ToErrorResult().Match<OneOf<T, IEnumerable<NotFoundException>, IEnumerable<ValidationException>, IEnumerable<Exception>>>(
			notFound => notFound.ToList(),
			validation => validation.ToList(),
			unexpected => unexpected.ToList()
		);
	}

	private static OneOf<IEnumerable<NotFoundException>, IEnumerable<ValidationException>, IEnumerable<Exception>> ToErrorResult(this ResultBase result)
	{		
		if(result.HasException<ValidationException>(out var validationExceptions))
		{
			var _errors = new List<ValidationException>();
			foreach(ExceptionalError validationError in validationExceptions!.Where(ve => ve is ExceptionalError))
				_errors.Add((ValidationException)validationError.Exception);

			return _errors;
		}
		
		if(result.HasException<NotFoundException>(out var notFoundExceptions))
		{
			var _errors = new List<NotFoundException>();
			foreach(ExceptionalError notFoundError in notFoundExceptions!.Where(ve => ve is ExceptionalError))
				_errors.Add((NotFoundException)notFoundError.Exception);

			return _errors;
		}
		
		if(result.HasException<UnexpectedException>(out var unexpectedExceptions))
		{
			var _errors = new List<UnexpectedException>();
			foreach(ExceptionalError unexpectedError in unexpectedExceptions!.Where(ve => ve is ExceptionalError))
				_errors.Add((UnexpectedException)unexpectedError.Exception);

			return _errors;
		}
		
		var errors = new List<Exception>();		
		if(result.HasException<Exception>(out var exceptions))
		{
			foreach(ExceptionalError error in exceptions!.Where(ve => ve is ExceptionalError))
				errors.Add(error.Exception);
		}
		
		return errors;
	}
}

