using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ardalis.GuardClauses;
using Ardalis.Result;
using Instrument.Quote.Source.App.Core.ChartAggregate.Validation.Attributes;
using tfEnt = Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Model;

public partial class Chart
{
  public void AddCandles(IEnumerable<Candle> candles)
  {
    /// Don't check if Candle DateTime is Unique, because of DB conficguration set that Candle has unique index DateTime, ChartID
    var baseCandles = Candles.ToList();
    _candles.AddRange(candles);
    try
    {
      Validate();
    }
    catch (ValidationException)
    {
      _candles.RemoveAll(c => true);
      _candles.AddRange(baseCandles);
      throw;
    }
  }
  
  public Result<int> Extend(Chart extensionChart)
  {
    if (extensionChart.Id > 0)
      throw new ArgumentException("Id must be 0 in extesionChart", nameof(Chart.Id));

    if (extensionChart.InstrumentId != this.InstrumentId)
      throw new ArgumentException($"{nameof(extensionChart)} has different {nameof(Chart.InstrumentId)} then this", nameof(Chart.InstrumentId));

    if (extensionChart.TimeFrameId != this.TimeFrameId)
      throw new ArgumentException($"{nameof(extensionChart)} has different {nameof(Chart.TimeFrameId)} then this", nameof(Chart.TimeFrameId));

    if (extensionChart.FromDate >= this.FromDate && extensionChart.FromDate < this.UntillDate ||
        extensionChart.UntillDate > this.FromDate && extensionChart.UntillDate <= this.UntillDate ||
        extensionChart.FromDate <= this.FromDate && extensionChart.UntillDate >= UntillDate)
      return Result.Conflict("Uploaded data inside exist period");

    if (extensionChart.FromDate != this.UntillDate &&
        extensionChart.UntillDate != this.FromDate)
      return Result.Error("Uploaded data doesn't connected to exist period");

    if (extensionChart.FromDate < FromDate)
      FromDate = extensionChart.FromDate;
    if (extensionChart.UntillDate > UntillDate)
      UntillDate = extensionChart.UntillDate;

    AddCandles(extensionChart.Candles);
    return Result.Success(extensionChart.Candles.Count());
  }
}