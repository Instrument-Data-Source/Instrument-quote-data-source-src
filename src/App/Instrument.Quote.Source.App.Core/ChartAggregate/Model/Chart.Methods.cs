using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ardalis.GuardClauses;
using Ardalis.Result;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.Validation;
using Instrument.Quote.Source.App.Core.ChartAggregate.Validation.FluentValidation;
using Instrument.Quote.Source.Shared.FluentValidation.Extension;
using tfEnt = Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Model;

public partial class Chart
{
  public Result AddCandles(IEnumerable<Candle> candles)
  {
    /// Don't check if Candle DateTime is Unique, because of DB conficguration set that Candle has unique index DateTime, ChartID
    var validateResult = new CandlesForChartValidator(this).Validate(candles);
    if (!validateResult.IsValid)
      return validateResult.ToResult();
    if (_candles == null)
      _candles = new();

    //var baseCandles = Candles!.ToList();
    _candles.AddRange(candles);
    return Result.Success();
  }
}