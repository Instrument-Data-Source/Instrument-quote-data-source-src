using Ardalis.GuardClauses;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.ChartAggregate.Mapper;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Tools;

public class JoinedCandleMapper
{
  private readonly DecimalToStoreIntConverter decMapper;
  private readonly JoinedChart joinChart;
  public JoinedCandleMapper(JoinedChart joinChart)
  {
    Guard.Against.Null(joinChart.StepChart);
    Guard.Against.Null(joinChart.StepChart.Instrument);

    decMapper = new DecimalToStoreIntConverter(joinChart.StepChart.Instrument);
    this.joinChart = joinChart;
  }

  public JoinedCandle map(JoinedCandleDto dto)
  {
    return new JoinedCandle(
                        dto.DateTime, dto.TargetDateTime,
                        decMapper.PriceToInt(dto.Open),
                        decMapper.PriceToInt(dto.High), decMapper.PriceToInt(dto.Low),
                        decMapper.PriceToInt(dto.Close),
                        decMapper.VolumeToInt(dto.Volume),
                        dto.IsLast, dto.IsFullCalced,
                        joinChart);
  }

  public JoinedCandleDto map(JoinedCandle entity)
  {
    return new JoinedCandleDto()
    {
      DateTime = entity.StepDateTime,
      TargetDateTime = entity.TargetDateTime,
      Open = decMapper.PriceToDecimal(entity.Open),
      High = decMapper.PriceToDecimal(entity.High),
      Low = decMapper.PriceToDecimal(entity.Low),
      Close = decMapper.PriceToDecimal(entity.Close),
      Volume = decMapper.VolumeToDecimal(entity.Volume),
      IsLast = entity.IsLast,
      IsFullCalced = entity.IsFullCalc
    };
  }
}