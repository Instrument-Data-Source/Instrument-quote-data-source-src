using Ardalis.GuardClauses;
using Ardalis.Result;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.FluentValidation.Extension;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Mapper;

public static class ChartMapper
{

  public static Result<Chart> ToEntity(this UploadedCandlesDto dto, ent.Instrument instrument, TimeFrame timeFrame)
  {
    Guard.Against.Null(instrument);
    Guard.Against.Null(timeFrame);
    var newChart = new Chart(dto.FromDate, dto.UntillDate, instrument, timeFrame);

    var mapper = new CandleMapper(newChart);

    var candles = dto.Candles.Select(mapper.map).ToArray();

    var result = newChart.AddCandles(candles);
    if (!result.IsSuccess)
      return result;

    return Result.Success(newChart);
  }


}

public static class ChartMapperExtension
{
  public static ChartDto ToDto(this Chart entity)
  {
    return new ChartDto()
    {
      InstrumentId = entity.InstrumentId,
      TimeFrameId = entity.TimeFrameId,
      FromDate = entity.FromDate,
      UntillDate = entity.UntillDate
    };
  }
}