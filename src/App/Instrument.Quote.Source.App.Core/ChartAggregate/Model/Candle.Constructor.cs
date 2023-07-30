using System.Diagnostics.CodeAnalysis;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Model;
public partial class Candle
{
  protected Candle(DateTime dateTime,
              int open,
              int high,
              int low,
              int close,
              int volume,
              int chartId)
  {

    DateTime = dateTime;
    Open = open;
    High = high;
    Low = low;
    Close = close;
    Volume = volume;
    ChartId = chartId;
  }
  public Candle(DateTime dateTime,
                int open,
                int high,
                int low,
                int close,
                int volume,
                [NotNull] Chart chart) : this(dateTime, open, high, low, close, volume, chart.Id)
  {
    Chart = chart;
    Validate();
  }
}