using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Dto;

namespace Instrument.Quote.Source.App.Core.TimeFrameAggregate.Interface;

public interface ITimeFrameSrv
{
  public Task<IEnumerable<TimeFrameResponseDto>> GetAllAsync();
  /// <summary>
  /// Get timeframe by code
  /// </summary>
  /// <param name="Code"></param>
  /// <exception cref="ArgumentOutOfRangeException">Code not found</exception>
  /// <returns></returns>
  public Task<TimeFrameResponseDto> GetByCodeAsync(string Code);
}