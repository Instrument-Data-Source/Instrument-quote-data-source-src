using Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Tool;

public static class CandleMapper
{

  public static CandleDto ToDto(this Candle entity)
  {

    return new CandleDto()
    {
      DateTime = entity.DateTime,
      Open = entity.Open,
      High = entity.High,
      Low = entity.Low,
      Close = entity.Close,
      Volume = entity.Volume
    };
  }

  /// <summary>
  /// Convert Dto to Entity
  /// </summary>
  /// <param name="dto">Data transfer object</param>
  /// <param name="InstrumentId">Instrument id</param>
  /// <param name="timeFrameId">timeframe id</param>
  /// <param name="instrumentRep">instrument repository</param>
  /// <exception cref="ArgumentOutOfRangeException">One of argument has wrong value</exception>
  /// <returns></returns>
  public static async Task<Candle> ToEntityAsync(this CandleDto dto, int InstrumentId, int timeFrameId, IReadRepository<ent.Instrument> instrumentRep)
  {
    var instrument = await instrumentRep.GetByIdAsync(InstrumentId);

    return dto.ToEntity(instrument, timeFrameId);
  }

  /// <summary>
  /// Convert Dto to Entity
  /// </summary>
  /// <param name="dto">Data transfer object</param>
  /// <param name="InstrumentId">Instrument id</param>
  /// <param name="timeFrameId">timeframe id</param>
  /// <param name="instrumentRep">instrument repository</param>
  /// <exception cref="ArgumentOutOfRangeException">One of argument has wrong value</exception>
  /// <returns></returns>
  public static Candle ToEntity(this CandleDto dto, ent.Instrument instrument, int timeFrameId)
  {
    return new Candle(
      dateTime: dto.DateTime,
      openStore: ToStoreValue(dto.Open, instrument.PriceDecimalLen, nameof(dto.Open)),
      highStore: ToStoreValue(dto.High, instrument.PriceDecimalLen, nameof(dto.High)),
      lowStore: ToStoreValue(dto.Low, instrument.PriceDecimalLen, nameof(dto.Low)),
      closeStore: ToStoreValue(dto.Close, instrument.PriceDecimalLen, nameof(dto.Close)),
      volumeStore: ToStoreValue(dto.Volume, instrument.VolumeDecimalLen, nameof(dto.Volume)),
      timeFrameId: timeFrameId,
      instrument: instrument
    );
  }

  public static int ToStoreValue(decimal value, int decimalLen, string name = "")
  {
    var storeValue = value * (int)Math.Pow(10, decimalLen);
    if (storeValue % 1 != 0)
      throw new ArgumentOutOfRangeException(name, $"Decimal part in {name} longer than expected {decimalLen}.");
    return (int)storeValue;
  }

  public static int DecimalPart(decimal value, int decimalLen, string name = "")
  {
    var decimalPart = value % 1;
    var decimalValue = Math.Round(decimalPart, decimalLen);
    if (Math.Abs(decimalValue - decimalPart) > (1 / (decimal)Math.Pow(10, decimalLen + 5)))
      throw new ArgumentOutOfRangeException(name, $"Too big missing while extracting decimal part ({decimalValue - decimalPart}) when decimal part len is {decimalLen}.");
    return (int)(decimalValue * (decimal)Math.Pow(10, decimalLen));
  }
}