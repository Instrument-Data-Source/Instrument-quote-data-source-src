using FluentValidation;
using Instrument.Quote.Source.App.Core.CandleAggregate.Tool;
using Instrument.Quote.Source.App.Core.CandleAggregate.Validator;
using Instrument.Quote.Source.App.Core.Event;
using Instrument.Quote.Source.Shared.FluentValidation.Extension;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
public class NewCandlesDto
{
  public DateTime From { get; set; }
  public DateTime Untill { get; set; }
  public IEnumerable<CandleDto> Candles { get; set; }

}

