using Ardalis.Result;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Validation.FluentValidation;
using Instrument.Quote.Source.Shared.FluentValidation.Extension;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;

public partial class JoinedChart
{
  public Result AddCandles(IEnumerable<JoinedCandle> joinedCandles)
  {
    var validateResult = new JoinedCandlesForJoinedChartValidator(this).Validate(joinedCandles);
    if (!validateResult.IsValid)
      return validateResult.ToResult();

    if (_joinedCandles == null)
      _joinedCandles = new();
    _joinedCandles.AddRange(joinedCandles);
    return Result.Success();
  }



}