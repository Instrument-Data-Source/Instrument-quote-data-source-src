using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Repository;
public static class InstrumentTypeRepository
{
  public static async Task<InstrumentType> GetByTypeAsync(this IReadRepository<ent.InstrumentType> readRepository, string Type)
  {
    var typeByName = await readRepository.Table.SingleOrDefaultAsync(e => e.Name == Type);
    if (typeByName == null)
      throw new ArgumentOutOfRangeException(nameof(Type), Type, "Instrument Type is unknown");

    return typeByName;
  }
}