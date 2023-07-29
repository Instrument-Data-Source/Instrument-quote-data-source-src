using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;

namespace Instrument.Quote.Source.Shared.Kernal.DataBase.Repository;

public class ReadRepository<TEntity> : IReadRepository<TEntity> where TEntity : EntityBase
{
  private readonly IQueryable<TEntity> queryable;

  public ReadRepository(IQueryable<TEntity> queryable)
  {
    this.queryable = queryable;
  }
  public IQueryable<TEntity> Table => queryable;

  public IReadRepository<TEntity> Include(Func<IQueryable<TEntity>, IQueryable<TEntity>> includeFunc)
  {
    return new ReadRepository<TEntity>(includeFunc(Table));
  }
}
