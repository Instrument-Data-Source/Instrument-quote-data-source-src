using FluentValidation;
using FluentValidation.Results;
using Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
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
  public static Candle ToEntity(this CandleDto dto, ent.Instrument instrument, TimeFrame timeFrame)
  {

    return new Candle(
      dateTime: dto.DateTime,
      open: ToStoreValue(dto.Open, instrument.PriceDecimalLen, nameof(dto.Open)),
      high: ToStoreValue(dto.High, instrument.PriceDecimalLen, nameof(dto.High)),
      low: ToStoreValue(dto.Low, instrument.PriceDecimalLen, nameof(dto.Low)),
      close: ToStoreValue(dto.Close, instrument.PriceDecimalLen, nameof(dto.Close)),
      volume: ToStoreValue(dto.Volume, instrument.VolumeDecimalLen, nameof(dto.Volume)),
      timeFrame: timeFrame,
      instrument: instrument
    );

  }

  public static bool TryConvertToEntity(this IEnumerable<CandleDto> candleDtos, ent.Instrument instrument, TimeFrame timeFrame,
                                        out IEnumerable<Candle> candles, out ValidationResult validationResult)
  {
    var isValid = true;
    List<ValidationFailure> _validationFailures = new List<ValidationFailure>();
    var _ret_candles = new List<Candle>();
    foreach (var candleDto in candleDtos)
    {
      Candle? entity = null;
      try
      {
        var newCandle = new Candle(dateTime: candleDto.DateTime,
                                             open: ToStoreValue(candleDto.Open, instrument.PriceDecimalLen, nameof(candleDto.Open)),
                                             high: ToStoreValue(candleDto.High, instrument.PriceDecimalLen, nameof(candleDto.High)),
                                             low: ToStoreValue(candleDto.Low, instrument.PriceDecimalLen, nameof(candleDto.Low)),
                                             close: ToStoreValue(candleDto.Close, instrument.PriceDecimalLen, nameof(candleDto.Close)),
                                             volume: ToStoreValue(candleDto.Volume, instrument.VolumeDecimalLen, nameof(candleDto.Volume)),
                                             instrument: instrument,
                                             timeFrame: timeFrame);
        _ret_candles.Add(newCandle);
      }
      catch (ValidationException ex)
      {
        _validationFailures.AddRange(ex.Errors);
        throw;
      }
    }
    candles = _ret_candles;
    validationResult = new ValidationResult(_validationFailures);
    return isValid;
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