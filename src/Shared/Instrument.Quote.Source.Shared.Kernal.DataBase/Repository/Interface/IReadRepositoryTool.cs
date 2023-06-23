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
    var ret = await readRep.TryGetByIdAsync(id, cancellationToken);
    if (ret == null)
    {
      throw new ArgumentOutOfRangeException(nameof(id), id, "Unknown ID");
    }
    return ret;
  }

  /// <summary>
  /// Get element by Id
  /// </summary>
  /// <param name="id">Id of elemtnt</param>
  /// <returns></returns>
  public static async Task<TEntity?> TryGetByIdAsync<TEntity>(this IReadRepository<TEntity> readRep, int id, CancellationToken cancellationToken = default) where TEntity : EntityBase
  {
    return await readRep.Table.SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
  }

  /// <summary>
  /// Contain entity with Id
  /// </summary>
  /// <param name="id">Id of elemtnt</param>
  /// <returns></returns>
  public static async Task<bool> ContainIdAsync<TEntity>(this IReadRepository<TEntity> readRep, int id, CancellationToken cancellationToken = default) where TEntity : EntityBase
  {
    return (await readRep.Table.Select(e => new { e.Id }).SingleOrDefaultAsync(e => e.Id == id, cancellationToken)) != null;
  }

  /// <summary>
  /// Contain entity with Id
  /// </summary>
  /// <param name="id">Id of elemtnt</param>
  /// <returns></returns>
  public static bool ContainId<TEntity>(this IReadRepository<TEntity> readRep, int id) where TEntity : EntityBase
  {
    return readRep.ContainIdAsync(id).Result;
  }
}
