using Ardalis.Result;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Repository;

public static class CandleRepositoryExtension
{
  public static async Task<Result<IEnumerable<CandleDto>>> GetAsync(this ICandleSrv srv, int instrumentId, TimeFrame.Enum timeFrameEnum, DateTime from, DateTime untill, CancellationToken cancellationToken = default)
  {
    return await srv.GetAsync(instrumentId, (int)timeFrameEnum, from, untill, cancellationToken);
  }

  public static async Task<Result<IEnumerable<CandleDto>>> GetAsync(this ICandleSrv srv, ent.Instrument instrumentEnt, TimeFrame.Enum timeFrameEnum, DateTime from, DateTime untill, CancellationToken cancellationToken = default)
  {
    return await srv.GetAsync(instrumentEnt.Id, (int)timeFrameEnum, from, untill, cancellationToken);
  }

  public static async Task<Result<IEnumerable<JoinedCandleDto>>> GetAsync(this ICandleSrv srv, int instrumentId, TimeFrame.Enum baseTimeFrameEnum, TimeFrame.Enum chartTimeFrameEnum, DateTime from, DateTime untill, bool addIntermediateCandles = false, CancellationToken cancellationToken = default)
  {
    return await srv.GetAsync(instrumentId, (int)baseTimeFrameEnum, (int)chartTimeFrameEnum, from, untill, addIntermediateCandles, cancellationToken);
  }

  public static async Task<Result<IEnumerable<JoinedCandleDto>>> GetAsync(this ICandleSrv srv, ent.Instrument instrumentEnt, TimeFrame.Enum baseTimeFrameEnum, TimeFrame.Enum chartTimeFrameEnum, DateTime from, DateTime untill, bool addIntermediateCandles = false, CancellationToken cancellationToken = default)
  {
    return await srv.GetAsync(instrumentEnt.Id, (int)baseTimeFrameEnum, (int)chartTimeFrameEnum, from, untill, addIntermediateCandles, cancellationToken);
  }
}