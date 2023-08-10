using System.Diagnostics.CodeAnalysis;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;
public partial class JoinedCandle
{
  internal void Update(JoinedCandle candle)
  {
    Open = candle.Open;
    High = candle.High;
    Low = candle.Low;
    Close = candle.Close;
    Volume = candle.Volume;
    IsLast = candle.IsLast;
    IsFullCalc = candle.IsFullCalc;
  }

}
