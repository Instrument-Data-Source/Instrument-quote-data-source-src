using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Ardalis.Specification.EntityFrameworkCore;
using Ardalis.Specification;
using Microsoft.Extensions.Logging;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Event;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Exceptions;

namespace Instrument.Quote.Source.Shared.Kernal.DataBase.Repository;

public class Repository<TEntity, TDbContext> : IRepository<TEntity> where TEntity : EntityBase where TDbContext : DbContext
{
  private readonly TDbContext dbContext;
  protected readonly ILogger logger;

  public Repository(TDbContext dbContext, ILogger logger)
  {
    this.dbContext = dbContext;
    this.logger = logger;
  }

  protected void ProcessingExceptionOnSave(Exception ex)
  {
    logger.LogCritical(EventEnum.AddFail.GetEventId(), ex, "Get exception when save data to Database");
    throw new RepositoryException("Get exception when add entity to Database", ex);
  }
  protected void ProcessingExceptionOnAdd(Exception ex)
  {
    logger.LogCritical(EventEnum.AddFail.GetEventId(), ex, "Get exception when add data to DB context");
    throw new RepositoryException("Get exception when add entity to context", ex);
  }

  public async Task AddAsync(TEntity enity, IDbContextTransaction? dbContextTransaction = null, CancellationToken cancellationToken = default)
  {
    try
    {
      await dbContext.Set<TEntity>().AddAsync(enity);
    }
    catch (Exception ex)
    {
      ProcessingExceptionOnAdd(ex);
    }
    await SaveChangesAsync(cancellationToken);
  }

  public async Task AddRangeAsync(IEnumerable<TEntity> entities, IDbContextTransaction? dbContextTransaction = null, CancellationToken cancellationToken = default)
  {
    try
    {
      await dbContext.Set<TEntity>().AddRangeAsync(entities);
    }
    catch (Exception ex)
    {
      ProcessingExceptionOnAdd(ex);
    }

    await SaveChangesAsync(cancellationToken);

  }

  public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      await dbContext.SaveChangesAsync(cancellationToken);
    }
    catch (Exception ex)
    {
      ProcessingExceptionOnSave(ex);
    }
  }

  public async Task RemoveAsync(TEntity entity, IDbContextTransaction? dbContextTransaction = null, CancellationToken cancellationToken = default)
  {
    try
    {
      dbContext.Set<TEntity>().Remove(entity);
    }
    catch (Exception ex)
    {
      ProcessingExceptionOnSave(ex);
    }
    await SaveChangesAsync(cancellationToken);
  }

  public IQueryable<TEntity> Table => dbContext.Set<TEntity>().AsQueryable();
}
