using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Repository;
public static class InstrumentTypeRepository
{
  public static async Task<InstrumentType> GetByTypeAsync(this IReadRepository<ent.InstrumentType> readRepository, string Type, CancellationToken cancellationToken = default)
  {
    var typeByName = await readRepository.TryGetByTypeAsync(Type, cancellationToken);
    if (typeByName == null)
      throw new ArgumentOutOfRangeException(nameof(Type), Type, "Instrument Type is unknown");

    return typeByName;
  }

  public static async Task<InstrumentType?> TryGetByTypeAsync(this IReadRepository<ent.InstrumentType> readRepository, string Type, CancellationToken cancellationToken = default)
  {
    return await readRepository.Table.SingleOrDefaultAsync(e => e.Name == Type, cancellationToken);
  }

  public static async Task<string> GetNameByIdAsync(this IReadRepository<ent.InstrumentType> readRepository,int id, CancellationToken cancellationToken = default)
  {
    var _ret = await readRepository.Table.Select(e=>new{e.Id, e.Name}).SingleOrDefaultAsync(e=>e.Id== id, cancellationToken);
    if (_ret == null){
      throw new ArgumentOutOfRangeException(nameof(id), id, "Instrument Id is unknown");
    }
    return _ret.Name;
}}