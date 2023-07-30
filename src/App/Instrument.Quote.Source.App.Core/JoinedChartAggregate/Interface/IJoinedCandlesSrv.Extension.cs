using Ardalis.Result;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.App.Core.Validation;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Interface;

public static class JoinedCandlesSrvExtension
{
  public static async Task<Result<IEnumerable<JoinedCandleDto>>> GetAsync(this IJoinedCandleSrv srv,
      int instrumentId, TimeFrame.Enum baseTimeFrameEnum, TimeFrame.Enum chartTimeFrameEnum,
      [UTCKind] DateTime from, [UTCKind] DateTime untill, bool hideIntermediateCandles = false, CancellationToken cancellationToken = default)
  {
    return await srv.GetAsync(instrumentId, (int)baseTimeFrameEnum, (int)chartTimeFrameEnum, from, untill, hideIntermediateCandles, cancellationToken);
  }

  public static async Task<Result<IEnumerable<JoinedCandleDto>>> GetAsync(this IJoinedCandleSrv srv,
    ent.Instrument instrumentEnt, TimeFrame.Enum baseTimeFrameEnum,
    TimeFrame.Enum chartTimeFrameEnum,
    [UTCKind] DateTime from, [UTCKind] DateTime untill,
    bool hideIntermediateCandles = false, CancellationToken cancellationToken = default)
  {
    return await srv.GetAsync(instrumentEnt.Id, (int)baseTimeFrameEnum, (int)chartTimeFrameEnum, from, untill, hideIntermediateCandles, cancellationToken);
  }
}