using Ardalis.Result;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;

public partial class JoinedChart
{
  public Result AddCandles(IEnumerable<JoinedCandle> joinedCandles)
  {
    // TODO Define validation 
    if (_joinedCandles == null)
      _joinedCandles = new();
    //var baseCandles = joinedCandles.ToList(); // TODO check why you need this
    _joinedCandles.AddRange(joinedCandles);
    return Result.Success();
  }



}