using Ardalis.Result;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Repository;

public static class JoinedCandlesSrvExtension
{
  public static async Task<Result<IEnumerable<JoinedCandleDto>>> GetAsync(this IReadJoinedCandleSrv srv, int instrumentId, TimeFrame.Enum baseTimeFrameEnum, TimeFrame.Enum chartTimeFrameEnum, DateTime from, DateTime untill, bool addIntermediateCandles = false, CancellationToken cancellationToken = default)
  {
    return await srv.GetAsync(instrumentId, (int)baseTimeFrameEnum, (int)chartTimeFrameEnum, from, untill, addIntermediateCandles, cancellationToken);
  }

  public static async Task<Result<IEnumerable<JoinedCandleDto>>> GetAsync(this IReadJoinedCandleSrv srv, ent.Instrument instrumentEnt, TimeFrame.Enum baseTimeFrameEnum, TimeFrame.Enum chartTimeFrameEnum, DateTime from, DateTime untill, bool addIntermediateCandles = false, CancellationToken cancellationToken = default)
  {
    return await srv.GetAsync(instrumentEnt.Id, (int)baseTimeFrameEnum, (int)chartTimeFrameEnum, from, untill, addIntermediateCandles, cancellationToken);
  }
}