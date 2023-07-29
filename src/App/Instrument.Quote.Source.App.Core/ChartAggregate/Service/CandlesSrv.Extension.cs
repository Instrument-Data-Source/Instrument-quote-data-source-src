using Ardalis.Result;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Repository;

public static class CandlesSrvExtension
{
  public static async Task<Result<IEnumerable<CandleDto>>> GetAsync(this IReadCandleSrv srv, int instrumentId, TimeFrame.Enum timeFrameEnum, DateTime from, DateTime untill, CancellationToken cancellationToken = default)
  {
    return await srv.GetAsync(instrumentId, (int)timeFrameEnum, from, untill, cancellationToken);
  }

  public static async Task<Result<IEnumerable<CandleDto>>> GetAsync(this IReadCandleSrv srv, ent.Instrument instrumentEnt, TimeFrame.Enum timeFrameEnum, DateTime from, DateTime untill, CancellationToken cancellationToken = default)
  {
    return await srv.GetAsync(instrumentEnt.Id, (int)timeFrameEnum, from, untill, cancellationToken);
  }
}