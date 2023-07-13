using Ardalis.Result;
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
  public static async Task<Result<int>> AddAsync(this ICandleSrv candleSrv, int instrumentId, int timeFrameId, NewCandlesDto newCandlesDto, CancellationToken cancellationToken = default)
  {
    NewPeriodDto addCandlesDto = new NewPeriodDto()
    {
      InstrumentId = instrumentId,
      TimeFrameId = timeFrameId,
      FromDate = newCandlesDto.From,
      UntillDate = newCandlesDto.Untill,
      Candles = newCandlesDto.Candles
    };
    return await candleSrv.AddAsync(addCandlesDto, cancellationToken);

  }

  public static async Task<Result<int>> AddAsync(this ICandleSrv candleSrv, int instrumentId, int timeFrameId, DateTime From, DateTime Untill,
IEnumerable<CandleDto> Candles, CancellationToken cancellationToken = default)
  {
    NewPeriodDto addCandlesDto = new NewPeriodDto()
    {
      InstrumentId = instrumentId,
      TimeFrameId = timeFrameId,
      FromDate = From,
      UntillDate = Untill,
      Candles = Candles
    };
    return await candleSrv.AddAsync(addCandlesDto, cancellationToken);

  }
  /*
    public async Task<Result<PeriodResponseDto>> TryGetExistPeriodAsync(string instrumentStr, string timeframeStr, CancellationToken cancellationToken = default)
    {
      var findedEnt = await instrumentRep.Table.Include(e => e.InstrumentType).SingleOrDefaultAsync(e => e.Code == instrumentStr, cancellationToken);
      int instrumentId = -1;
      if (findedEnt != null)
        instrumentId = findedEnt.Id;
      else if (!Int32.TryParse(instrumentStr, out instrumentId))
        return Result.NotFound();

      int timeframeId = -1;
      if (!Int32.TryParse(timeframeStr, out timeframeId))
      {
        TimeFrame.Enum? timeframeParseId = Enum.GetValues<TimeFrame.Enum>().SingleOrDefault(e => e.ToString() == timeframeStr);
        if (timeframeParseId != null)
        {
          timeframeId = (int)timeframeParseId;
        }
        else
          return Result.NotFound();
      }

      return await TryGetExistPeriodAsync(instrumentId, timeframeId, cancellationToken);
    }*/
}