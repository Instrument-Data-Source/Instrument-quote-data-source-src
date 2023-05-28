using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;

public static class CreateRepositoryTool
{
  public static async Task<bool> TryRemoveAsync<TEntity>(this IRepository<TEntity> rep,
                                          int entityId,
                                          IDbContextTransaction? dbContextTransaction = null,
                                          CancellationToken cancellationToken = default) where TEntity : EntityBase
  {
    var deleted_ent = rep.Table.SingleOrDefault(e => e.Id == entityId);
    if (deleted_ent == null) return false;
    await rep.RemoveAsync(deleted_ent, dbContextTransaction, cancellationToken);
    return true;
  }
}