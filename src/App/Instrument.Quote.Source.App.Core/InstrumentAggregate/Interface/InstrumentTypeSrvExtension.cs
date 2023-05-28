using Ardalis.Result;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Model;

namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
public static class InstrumentTypeSrvExtension
{
  public static async Task<Result<InstrumentTypeResponseDto>> GetByIdOrCodeAsync(this IInstrumentTypeSrv service, string instrumentTypeStr, CancellationToken cancellationToken = default)
  {
    if (Int32.TryParse(instrumentTypeStr, out int instrumentTypeId))
      return await service.GetByAsync(instrumentTypeId, cancellationToken);
    else
      return await service.GetByAsync(instrumentTypeStr, cancellationToken);
  }
}