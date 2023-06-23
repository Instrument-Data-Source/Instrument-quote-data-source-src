using FluentValidation;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Validator;

public class CandlesForLoadedPeriodValidator : AbstractValidator<IEnumerable<Candle>>
{
  /*
  public CandlesForLoadedPeriodValidator(LoadedPeriod loadedPeriod)
  {
    var validator = new CandleForLoadedPeriodValidator(loadedPeriod);

    RuleLevelCascadeMode = CascadeMode.Continue;
    RuleFor(e => e)
      .Must(e => e.Select(el => el.DateTime).Distinct().Count() == e.Count())
      .WithMessage("DateTime must be unique in candle list");
    RuleForEach(e => e)
      .SetValidator(validator).WithMessage("All DateTime of Candles must be in period (from <= datatime < untill).");
  }
  */
}