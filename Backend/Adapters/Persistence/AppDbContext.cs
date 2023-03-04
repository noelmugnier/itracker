using System.Diagnostics;
using FluentResults;
using ITracker.Core.Application;
using ITracker.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace ITracker.Adapters.Persistence;

public class AppDbContext : DbContext, IUnitOfWork
{
	private readonly IDomainEventDispatcher _eventDispatcher;
	private readonly List<IRepository> _repositories = new List<IRepository>();

	public AppDbContext(DbContextOptions<AppDbContext> options, IDomainEventDispatcher eventDispatcher)
		: base(options)
	{
		_eventDispatcher = eventDispatcher;

		_repositories.Add(new BrandRepository(this));
		
		_repositories.Add(new ScraperRepository(this));
		_repositories.Add(new ParsingResultRepository(this));
		
		_repositories.Add(new CatalogRepository(this));
		_repositories.Add(new ProductRepository(this));
		_repositories.Add(new ProductHistoryRepository(this));
		_repositories.Add(new CatalogProductMappingRepository(this));
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		=> optionsBuilder.UseSnakeCaseNamingConvention();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
	}

	public async Task<Result> Commit(CancellationToken token)
	{
		try
		{
			ChangeTracker.DetectChanges();
			var entities = ChangeTracker.Entries<Entity>().Where(e => e.Entity.DomainEvents.Any()).Select(e => e.Entity);
			var domainEvents = entities.SelectMany(po => po.DomainEvents).ToList();

			foreach (var entity in entities)
				entity.ClearDomainEvents();

			var domainEventResult = await DispatchDomainEvents(domainEvents, token);
			if (domainEventResult.IsFailed)
				return domainEventResult;

			var integrationEventResult = await DispatchIntegrationEvents(domainEvents, token);
			if (integrationEventResult.IsFailed)
				return integrationEventResult;

			await SaveChangesAsync(token);


			return Result.Ok();
		}
		catch (DbUpdateConcurrencyException exc)
		{
			return new PersistenceError(exc);
		}
		catch (DbUpdateException exc)
		{
			return new PersistenceError(exc);
		}
		catch (Exception exc)
		{
			return new UnexpectedError(exc);
		}
	}

	public T Get<T>() where T : IRepository
	{
		Debug.Assert(_repositories.OfType<T>().Any(), $"Repository {typeof(T).Name} not found, please register it in the UnitOfWork constructor");
		return _repositories.OfType<T>().Single();
	}

	private async Task<Result> DispatchDomainEvents<T>(IEnumerable<T> domainEvents, CancellationToken token) where T : DomainEvent
	{
		return await _eventDispatcher.DispatchEvents(domainEvents, token);
	}

	private async Task<Result> DispatchIntegrationEvents<T>(IEnumerable<T> domainEvents, CancellationToken token) where T : DomainEvent
	{
		try
		{
			var mappedEvents = OutboxIntegrationEventMapper.MapDomainEvents(domainEvents);
			await Set<OutboxIntegrationEvent>().AddRangeAsync(mappedEvents, token);
			return Result.Ok();
		}
		catch (Exception e)
		{
			return Result.Fail(new UnexpectedError(e));
		}
	}
}
