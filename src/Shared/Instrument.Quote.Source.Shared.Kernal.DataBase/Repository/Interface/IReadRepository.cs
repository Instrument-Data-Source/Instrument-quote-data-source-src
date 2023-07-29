using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;

namespace Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;

public interface IReadRepository<TEntity> where TEntity : EntityBase
{
  new IQueryable<TEntity> Table { get; }
  IReadRepository<TEntity> Include(Func<IQueryable<TEntity>, IQueryable<TEntity>> includeFunc);
}
