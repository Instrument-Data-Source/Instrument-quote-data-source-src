using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Interface;

public interface IJoindeCandleMapper
{
  JoinedCandle map(JoinedCandleDto dto);
  JoinedCandleDto map(JoinedCandle entity);
}