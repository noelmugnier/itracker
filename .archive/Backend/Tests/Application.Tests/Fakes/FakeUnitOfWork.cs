namespace ITracker.Core.Application.Tests;

public class FakeUnitOfWork : IUnitOfWork
{
    private List<FakeRepository> _repositories = new List<FakeRepository>{
        new FakeBrandRepository(),
        new FakeScraperRepository(),
        new FakeParsingResultRepository(),
        new FakeCatalogRepository(),
        new FakeProductRepository(),
        new FakeProductHistoryRepository(),
        new FakeCatalogProductMappingRepository()
    };

    public FakeUnitOfWork()
    {
    }

	public List<DomainEvent> RaisedDomainEvents { get; private set; } = new List<DomainEvent>();
	public List<IntegrationEvent> RaisedIntegrationEvents { get; private set; } = new List<IntegrationEvent>();

	public Task<Result> Commit(CancellationToken token)
    {
        RaisedDomainEvents.Clear();
        RaisedIntegrationEvents.Clear();
        
        var domainEvents = _repositories.SelectMany(r => r.GetDomainEvents()).ToList();
        RaisedDomainEvents.AddRange(domainEvents);  

        domainEvents.ForEach(de => {
            var integrationEvent = IntegrationEventMapper.MapDomainEvent(de);
            if(integrationEvent != null)
                RaisedIntegrationEvents.Add(integrationEvent);
        });

        foreach (FakeRepository repository in _repositories)
            repository.Commit();

        return Task.FromResult(Result.Ok());
    }

    public T Get<T>() where T: IRepository
    {
        return _repositories.OfType<T>().Single();
    }

    public Task<Result> Rollback(CancellationToken token)
    {
        foreach (FakeRepository repository in _repositories)
            repository.Rollback();

        return Task.FromResult(Result.Ok());
    }
}
