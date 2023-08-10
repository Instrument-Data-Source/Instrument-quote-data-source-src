using FluentValidation;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.ChartAggregate.Mapper;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;
using Instrument.Quote.Source.App.Core.Validation;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Validation;

public class JoinedCandlesForJoinedChartValidator : AbstractValidator<IEnumerable<JoinedCandle>>
{
  public JoinedCandlesForJoinedChartValidator(JoinedChart joinedChart)
  {
    RuleForEach(e => e).SetValidator(new JoinedCandleForJoinedChartValidator(joinedChart));
    RuleFor(e => e).Must(e => e.Select(c => c.StepDateTime).Distinct().Count() == e.Count()).WithName(nameof(JoinedCandle.StepDateTime)).WithMessage("DateTime of candles in chart must be unique");
  }
}

public class JoinedCandleForJoinedChartValidator : AbstractValidator<JoinedCandle>
{
  public JoinedCandleForJoinedChartValidator(JoinedChart joinedChart)
  {
    if (joinedChart.StepChart == null)
      throw new NullReferenceException($"{nameof(JoinedChart)} must have loaded relative entity {nameof(joinedChart.StepChart)}");
    if (joinedChart.StepChart.Instrument == null)
      throw new NullReferenceException($"{nameof(JoinedChart)}.{nameof(joinedChart.StepChart)} must have loaded relative entity {nameof(ent.Instrument)}");
    if (joinedChart.StepChart.TimeFrame == null)
      throw new NullReferenceException($"{nameof(JoinedChart)}.{nameof(joinedChart.StepChart)} must have loaded relative entity {nameof(joinedChart.StepChart.TimeFrame)}");
    if (joinedChart.TargetTimeFrame == null)
      throw new NullReferenceException($"{nameof(JoinedChart)} must have loaded relative entity {nameof(joinedChart.TargetTimeFrame)}");

    IDecimalPartLongChecker checker = new DecimalToStoreIntConverter(joinedChart.StepChart.Instrument);
    var dtArr = joinedChart.JoinedCandles != null ? joinedChart.JoinedCandles.Select(c => c.StepDateTime) : new DateTime[0];
    RuleFor(e => e.TargetDateTime)
      .Must(e => FitTimeFrameAttribute.IsValid(e, joinedChart.TargetTimeFrame.EnumId, out var msg)).WithMessage("Doesn't fit to JoinedChart.TargetTimeframe");
    RuleFor(e => e.StepDateTime)
      .Must(e => FitTimeFrameAttribute.IsValid(e, joinedChart.StepChart.TimeFrame.EnumId, out var msg)).WithMessage("Doesn't fit to JoinedChart.StepChart.Timeframe")
      .GreaterThanOrEqualTo(joinedChart.FromDate).WithMessage("DateTime must be greater or equal FromDate")
      .LessThan(joinedChart.UntillDate).WithMessage("DateTime must be less than UntillDate")
      .Must(e => !dtArr.Contains(e)).WithMessage("DateTime already exist");
    RuleFor(e => e.Open).Must(e => checker.IsPriceDecPartFit(e)).WithMessage("Len of decimal part does not fit to instrument");
    RuleFor(e => e.High).Must(e => checker.IsPriceDecPartFit(e)).WithMessage("Len of decimal part does not fit to instrument");
    RuleFor(e => e.Low).Must(e => checker.IsPriceDecPartFit(e)).WithMessage("Len of decimal part does not fit to instrument");
    RuleFor(e => e.Close).Must(e => checker.IsPriceDecPartFit(e)).WithMessage("Len of decimal part does not fit to instrument");
    RuleFor(e => e.Volume).Must(e => checker.IsVolumeDecPartFit(e)).WithMessage("Len of decimal part does not fit to instrument");
  }
}
