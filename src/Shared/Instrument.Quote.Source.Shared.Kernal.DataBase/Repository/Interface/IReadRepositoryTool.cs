using Microsoft.EntityFrameworkCore;

namespace Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;

public static class IReadRepositoryTool
{
  /// <summary>
  /// Get element by Id
  /// </summary>
  /// <param name="id">Id of elemtnt</param>
  /// <exception cref="ArgumentOutOfRangeException">Id is unknown</exception>
  /// <returns></returns>
  public static async Task<TEntity> GetByIdAsync<TEntity>(this IReadRepository<TEntity> readRep, int id, CancellationToken cancellationToken = default) where TEntity : EntityBase
  {
    var ret = await readRep.Table.SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
    if (ret == null)
    {
      throw new ArgumentOutOfRangeException(nameof(id), id, "Unknown ID");
    }
    return ret;
  }
}
