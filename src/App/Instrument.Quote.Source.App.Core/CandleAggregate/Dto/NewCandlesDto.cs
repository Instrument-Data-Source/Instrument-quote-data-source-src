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

  [ValidatorAttribute]
  public class Validator : AbstractValidator<NewCandlesDto>
  {
    public Validator(IValidator<CandleDto> candleDtoValidator)
    {
      RuleFor(e => e.From).LessThan(e => e.Untill).WithEventId(ValidationEvents.FromGeUntillEvent);
      RuleForEach(e => e.Candles)
        .Must((e, c) => c.DateTime >= e.From && c.DateTime < e.Untill).WithEventId(ValidationEvents.CandleDateIsOutOfFromAndUntillEvent)
        .Must((e, c) => c.DateTime >= e.From && c.DateTime < e.Untill).WithEventId(ValidationEvents.CandleDateIsOutOfFromAndUntillEvent)
        .SetValidator(candleDtoValidator);
    }
  }
}

