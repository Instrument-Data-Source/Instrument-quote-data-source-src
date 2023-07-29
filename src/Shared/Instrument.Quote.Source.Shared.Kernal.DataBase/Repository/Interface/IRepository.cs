using Microsoft.EntityFrameworkCore.Storage;

namespace Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;

public interface IRepository<TEntity> : IReadRepository<TEntity> where TEntity : EntityBase
{
  Task AddAsync(TEntity entity, IDbContextTransaction? dbContextTransaction = null, CancellationToken cancellationToken = default);
  Task AddRangeAsync(IEnumerable<TEntity> entities, IDbContextTransaction? dbContextTransaction = null, CancellationToken cancellationToken = default);
  Task SaveChangesAsync(CancellationToken cancellationToken = default);
  Task RemoveAsync(TEntity entity, IDbContextTransaction? dbContextTransaction = null, CancellationToken cancellationToken = default);
  Task RemoveRangeAsync(IEnumerable<TEntity> entities, IDbContextTransaction? dbContextTransaction = null, CancellationToken cancellationToken = default);
}