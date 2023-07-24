using Ardalis.Result;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Interface;


public interface IReadCandleSrv
{
  Task<Result<IEnumerable<CandleDto>>> GetAsync(int instrumentId, int timeFrameId, DateTime from, DateTime untill, CancellationToken cancellationToken = default);
  Task<Result<IEnumerable<JoinedCandleDto>>> GetAsync(int instrumentId, int baseTimeFrameId, int chartTimeFrameId, DateTime from, DateTime untill, bool addIntermediateCandles = false, CancellationToken cancellationToken = default);
}

public interface ICandleSrv : IReadCandleSrv
{
  Task<Result<int>> AddAsync(int instrumentId, int timeFrameId, UploadedCandlesDto uploadedCandlesDto, CancellationToken cancellationToken = default);
}

