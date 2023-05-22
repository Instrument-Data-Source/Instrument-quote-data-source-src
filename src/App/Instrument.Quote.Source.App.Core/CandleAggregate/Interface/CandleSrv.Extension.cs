using Instrument.Quote.Source.App.Core.CandleAggregate.Dto;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Interface;
public static class CandleSrvExtension
{
  /// <summary>
  /// Add new candles
  /// </summary>
  /// <param name="instrumentId">Instrument Id</param>
  /// <param name="timeFrameId">TimeFrane Id</param>
  /// <param name="newCandlesDto">new candles pack</param>
  /// <exception cref="ArgumentException">One of argument has wrong value</exception>
  /// <returns></returns>
  public static async Task<int> AddAsync(this ICandleSrv candleSrv, int instrumentId, int timeFrameId, NewCandlesDto newCandlesDto, CancellationToken cancellationToken = default)
  {
    return await candleSrv.AddAsync(instrumentId, timeFrameId, newCandlesDto.From, newCandlesDto.Untill, newCandlesDto.Candles, cancellationToken);

  }
}