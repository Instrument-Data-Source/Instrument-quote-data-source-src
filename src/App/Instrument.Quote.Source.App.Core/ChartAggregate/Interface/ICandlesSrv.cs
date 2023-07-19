using Ardalis.Result;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Interface;

public interface ICandlesSrv
{
  Task<Result<int>> AddCandlesAsync(int instrumentId, int timeFrameId, UploadedCandlesDto uploadedCandlesDto, CancellationToken cancellationToken = default);
  Task<Result<IEnumerable<CandleDto>>> GetCandlesAsync(int instrumentId, int timeFrameId, DateTime from, DateTime untill, CancellationToken cancellationToken = default);
}

