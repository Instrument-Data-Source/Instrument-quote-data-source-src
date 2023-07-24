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

    var baseCandles = Candles.ToList();
    _candles.AddRange(candles);
    return Result.Success();
  }

  public Result<int> Extend(Chart extensionChart)
  {
    if (extensionChart.Id > 0)
      throw new ArgumentException("Id must be 0 in extesionChart", nameof(Chart.Id));

    if (extensionChart.InstrumentId != this.InstrumentId)
      throw new ArgumentException($"{nameof(extensionChart)} has different {nameof(Chart.InstrumentId)} then this", nameof(Chart.InstrumentId));

    if (extensionChart.TimeFrameId != this.TimeFrameId)
      throw new ArgumentException($"{nameof(extensionChart)} has different {nameof(Chart.TimeFrameId)} then this", nameof(Chart.TimeFrameId));

    var validateResult = new ChartExtendChartValidator(this).Validate(extensionChart);
    if (!validateResult.IsValid)
      return validateResult.ToResult();

    if (extensionChart.FromDate < FromDate)
      FromDate = extensionChart.FromDate;
    if (extensionChart.UntillDate > UntillDate)
      UntillDate = extensionChart.UntillDate;

    var addResult = AddCandles(extensionChart.Candles);
    if (!addResult.IsSuccess)
      return addResult;
      
    return Result.Success(extensionChart.Candles.Count());
  }
}