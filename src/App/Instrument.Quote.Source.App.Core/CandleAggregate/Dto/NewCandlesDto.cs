using Instrument.Quote.Source.App.Core.CandleAggregate.Tool;
namespace Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
public class NewCandlesDto
{
  public DateTime From { get; set; }
  public DateTime Untill { get; set; }
  public IEnumerable<CandleDto> Candles { get; set; }
}