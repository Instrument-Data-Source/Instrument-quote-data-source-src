using Microsoft.EntityFrameworkCore.Storage;

namespace Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;

public interface IReadRepository<TEntity> where TEntity : EntityBase
{
  IQueryable<TEntity> Table { get; }
}
