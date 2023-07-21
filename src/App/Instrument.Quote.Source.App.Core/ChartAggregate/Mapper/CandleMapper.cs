using Ardalis.GuardClauses;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Mapper;

public class CandleMapper
{
  private readonly DecimalToStoreIntConverter decMapper;
  private readonly Chart chart;
  public CandleMapper(Chart chart)
  {
    Guard.Against.Null(chart.Instrument);

    decMapper = new DecimalToStoreIntConverter(chart.Instrument);
    this.chart = chart;
  }

  public Candle map(CandleDto dto)
  {
    return new Candle(
                        dto.DateTime,
                        decMapper.PriceToInt(dto.Open),
                        decMapper.PriceToInt(dto.High), decMapper.PriceToInt(dto.Low),
                        decMapper.PriceToInt(dto.Close),
                        decMapper.VolumeToInt(dto.Volume),
                        chart);
  }

  public CandleDto map(Candle entity)
  {
    return new CandleDto()
    {
      DateTime = entity.DateTime,
      Open = decMapper.PriceToDecimal(entity.Open),
      High = decMapper.PriceToDecimal(entity.High),
      Low = decMapper.PriceToDecimal(entity.Low),
      Close = decMapper.PriceToDecimal(entity.Close),
      Volume = decMapper.VolumeToDecimal(entity.Volume)
    };
  }

}