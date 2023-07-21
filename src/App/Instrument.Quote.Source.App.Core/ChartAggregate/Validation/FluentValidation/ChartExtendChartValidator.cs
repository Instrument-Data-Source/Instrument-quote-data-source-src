using FluentValidation;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Validation.FluentValidation;

public class ChartExtendChartValidator : AbstractValidator<Chart>
{
  public ChartExtendChartValidator(Chart extendedChart)
  {
    RuleFor(e => e)
      .Must(e => !IsCrossed(e, extendedChart)).WithMessage("Uploaded data inside exist period")
      .Must(e=>IsJoined(e, extendedChart)).WithMessage("Uploaded data doesn't connected to exist period");
  }
  private static bool IsCrossed(Chart newChart, Chart extendedChart)
  {
    return newChart.FromDate >= extendedChart.FromDate && newChart.FromDate < extendedChart.UntillDate ||
      newChart.UntillDate > extendedChart.FromDate && newChart.UntillDate <= extendedChart.UntillDate ||
      newChart.FromDate <= extendedChart.FromDate && newChart.UntillDate >= extendedChart.UntillDate;
  }

  private static bool IsJoined(Chart newChart, Chart extendedChart)
  {
    return newChart.FromDate == extendedChart.UntillDate ||
        newChart.UntillDate == extendedChart.FromDate;
  }
}