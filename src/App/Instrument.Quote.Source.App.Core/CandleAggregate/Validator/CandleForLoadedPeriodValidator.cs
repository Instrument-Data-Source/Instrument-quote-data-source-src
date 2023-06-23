using FluentValidation;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Validator;

public class CandleForLoadedPeriodValidator : AbstractValidator<Candle>
{
  /*
  public CandleForLoadedPeriodValidator(LoadedPeriod loadedPeriod)
  {
    var dt_validator = new CandleDateTimeValidator((TimeFrame.Enum)loadedPeriod.TimeFrameId);
    var added_dts = loadedPeriod.Candles
                                .Where(e => e.DateTime >= loadedPeriod.FromDate && e.DateTime < loadedPeriod.UntillDate)
                                .Select(e => e.DateTime).ToArray();
    RuleLevelCascadeMode = CascadeMode.Continue;
    RuleFor(e => e.DateTime)
      .GreaterThanOrEqualTo(loadedPeriod.FromDate).WithMessage(e => $"DateTime {e.DateTime} of candle must be GE then From DateTime {loadedPeriod.FromDate}.")
      .LessThan(loadedPeriod.UntillDate).WithMessage(e => $"DateTime {e.DateTime} of candle must be LT then Untill DateTime {loadedPeriod.UntillDate}.")
      .SetValidator(dt_validator).WithMessage(e => $"DateTime {e.DateTime} must be correct to TimeFrame {TfName(loadedPeriod.TimeFrameId)}")
      .Must(e => !added_dts.Contains(e)).WithMessage(e => $"DateTime {e.DateTime} must be unique for adding");
    RuleFor(e => e.InstrumentId)
      .Equal(loadedPeriod.InstrumentId).WithMessage(e => $"Instument Id {e.InstrumentId} must be EQ Loaded Period Instument Id {loadedPeriod.InstrumentId}");
    RuleFor(e => e.TimeFrameId)
      .Equal(loadedPeriod.TimeFrameId).WithMessage(e => $"TimeFrame Id {e.TimeFrameId} must be EQ Period TimeFrame Id {loadedPeriod.TimeFrameId}");
  }

  private string TfName(int tfId)
  {
    return Enum.GetName<TimeFrame.Enum>((TimeFrame.Enum)tfId);
  }
  */
}
