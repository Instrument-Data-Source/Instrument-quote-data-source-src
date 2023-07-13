using Microsoft.EntityFrameworkCore.Storage;

namespace Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;

public interface IReadRepository<out TEntity> where TEntity : EntityBase
{
  new IQueryable<TEntity> Table { get; }
}
