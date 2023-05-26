using Ardalis.Result;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Dto;

namespace Instrument.Quote.Source.App.Core.TimeFrameAggregate.Interface;

public interface ITimeFrameSrv
{
  public Task<Result<IEnumerable<TimeFrameResponseDto>>> GetAllAsync(CancellationToken cancellationToken = default);
  /// <summary>
  /// Get timeframe by code
  /// </summary>
  /// <param name="Code"></param>
  /// <returns></returns>
  public Task<Result<TimeFrameResponseDto>> GetByCodeAsync(string Code, CancellationToken cancellationToken = default);
  /// <summary>
  /// Get timeframe by id
  /// </summary>
  /// <param name="Id"></param>
  /// <returns></returns>
  public Task<Result<TimeFrameResponseDto>> GetByIdAsync(int Id, CancellationToken cancellationToken = default);
}