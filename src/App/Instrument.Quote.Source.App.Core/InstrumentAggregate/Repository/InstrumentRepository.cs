using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Repository;
public static class InstrumentRepository
{
  public static async Task<IEnumerable<InstrumentResponseDto>> GetAsDto(this IReadRepository<ent.Instrument> readRepository, CancellationToken cancellationToken = default)
  {
    return await readRepository.Table
                              .Include(e => e.InstrumentType)
                              .Select(e => new InstrumentResponseDto(e))
                              .ToArrayAsync(cancellationToken);
  }
}