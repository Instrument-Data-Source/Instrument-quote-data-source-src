using Ardalis.Result;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Dto;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Interface;


public interface IJoinedCandleSrv
{
  Task<Result<IEnumerable<JoinedCandleDto>>> GetAsync(int instrumentId, int baseTimeFrameId, int chartTimeFrameId, DateTime from, DateTime untill, bool addIntermediateCandles = false, CancellationToken cancellationToken = default);
}

