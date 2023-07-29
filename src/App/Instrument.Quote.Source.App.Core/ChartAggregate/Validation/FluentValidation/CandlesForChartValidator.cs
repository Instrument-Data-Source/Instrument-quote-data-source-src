using FluentValidation;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.ChartAggregate.Mapper;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.Validation;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Validation.FluentValidation;

public class CandlesForChartValidator : AbstractValidator<IEnumerable<Candle>>
{
  public CandlesForChartValidator(Chart chart)
  {
    RuleForEach(e => e).SetValidator(new CandleForChartValidator(chart));
    RuleFor(e => e).Must(e => e.Select(c => c.DateTime).Distinct().Count() == e.Count()).WithName(nameof(Candle.DateTime)).WithMessage("DateTime of candles in chart must be unique");
  }
}

public class CandleForChartValidator : AbstractValidator<Candle>
{
  public CandleForChartValidator(Chart chart)
  {
    if (chart.Instrument == null)
    {
      throw new NullReferenceException($"{nameof(Chart)} must have loaded relative entity {nameof(ent.Instrument)}");
    }
    IDecimalPartLongChecker checker = new DecimalToStoreIntConverter(chart.Instrument);
    var dtArr = chart.Candles != null ? chart.Candles.Select(c => c.DateTime) : new DateTime[0];
    RuleFor(e => e.DateTime)
      .Must(e => FitTimeFrameAttribute.IsValid(e, chart.TimeFrame.EnumId, out var msg)).WithMessage("Doesn't fit to Chart Timeframe")
      .GreaterThanOrEqualTo(chart.FromDate).WithMessage("DateTime must be greater or equal FromDate")
      .LessThan(chart.UntillDate).WithMessage("DateTime must be less than UntillDate")
      .Must(e => !dtArr.Contains(e)).WithMessage("DateTime already exist");
    RuleFor(e => e.Open).Must(e => checker.IsPriceDecPartFit(e)).WithMessage("Len of decimal part does not fit to instrument");
    RuleFor(e => e.High).Must(e => checker.IsPriceDecPartFit(e)).WithMessage("Len of decimal part does not fit to instrument");
    RuleFor(e => e.Low).Must(e => checker.IsPriceDecPartFit(e)).WithMessage("Len of decimal part does not fit to instrument");
    RuleFor(e => e.Close).Must(e => checker.IsPriceDecPartFit(e)).WithMessage("Len of decimal part does not fit to instrument");
    RuleFor(e => e.Volume).Must(e => checker.IsVolumeDecPartFit(e)).WithMessage("Len of decimal part does not fit to instrument");
  }
}
