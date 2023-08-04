using Ardalis.Result;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Dto;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Interface;


public interface IJoinedCandleSrv
{
  Task<Result<IEnumerable<JoinedCandleDto>>> GetAsync(int instrumentId, int baseTimeFrameId, int chartTimeFrameId, DateTime from, DateTime untill, bool hideIntermediateCandles = false, CancellationToken cancellationToken = default);

  Task<Result<JoinedCandleResponse>> RequestAsync(int instrumentId, int baseTimeFrameId, int chartTimeFrameId, DateTime from, DateTime untill, bool hideIntermediateCandles = false, CancellationToken cancellationToken = default);

}

