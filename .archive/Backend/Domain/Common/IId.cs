namespace ITracker.Core.Domain;

public interface IId<T> 
{
	T Id { get; }
}