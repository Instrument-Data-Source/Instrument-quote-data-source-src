using Microsoft.EntityFrameworkCore.Storage;

namespace Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;

public interface ICreateRepository<TEntity> where TEntity : EntityBase
{
  Task AddAsync(TEntity entity, IDbContextTransaction? dbContextTransaction = null, CancellationToken cancellationToken = default);
  Task AddRangeAsync(IEnumerable<TEntity> entities, IDbContextTransaction? dbContextTransaction = null, CancellationToken cancellationToken = default);
  Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

