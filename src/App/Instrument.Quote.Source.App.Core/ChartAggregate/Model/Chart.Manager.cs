using Ardalis.Result;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.ChartAggregate.Validation.FluentValidation;
using Instrument.Quote.Source.Shared.FluentValidation.Extension;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Model;

public partial class Chart
{

  public class Manager
  {
    private readonly IRepository<Candle> candleRep;

    public Manager(IRepository<Candle> candleRep)
    {
      this.candleRep = candleRep;
    }

    public async Task<Result<int>> Extend(Chart extendedChart, Chart uploadedChart, CancellationToken cancellationToken = default)
    {
      if (uploadedChart.Id > 0)
        throw new ArgumentException("Id must be 0 in extesionChart", nameof(Chart.Id));

      if (uploadedChart.InstrumentId != extendedChart.InstrumentId)
        throw new ArgumentException($"{nameof(uploadedChart)} has different {nameof(Chart.InstrumentId)} then this", nameof(Chart.InstrumentId));

      if (uploadedChart.TimeFrameId != extendedChart.TimeFrameId)
        throw new ArgumentException($"{nameof(uploadedChart)} has different {nameof(Chart.TimeFrameId)} then this", nameof(Chart.TimeFrameId));

      var uploadedCandles = uploadedChart.Candles!.ToDictionary(c => c.DateTime, c => c);

      var existCandles = await candleRep.Table.Where(c => uploadedCandles.Keys.Contains(c.DateTime)).ToListAsync();
      foreach (var existCandle in existCandles)
      {
        var uploadedCandle = uploadedCandles[existCandle.DateTime];
        if (uploadedCandle.Open != existCandle.Open ||
            uploadedCandle.High != existCandle.High ||
            uploadedCandle.Low != existCandle.Low ||
            uploadedCandle.Close != existCandle.Close ||
            uploadedCandle.Volume != existCandle.Volume)
          return Result.Conflict();
      }

      var existCandleDt = existCandles.Select(c => c.DateTime).ToList();

      var validateResult = new ChartExtendChartValidator(extendedChart).Validate(uploadedChart);
      if (!validateResult.IsValid)
        return validateResult.ToResult();

      if (uploadedChart.FromDate < extendedChart.FromDate)
        extendedChart.FromDate = uploadedChart.FromDate;
      if (uploadedChart.UntillDate > extendedChart.UntillDate)
        extendedChart.UntillDate = uploadedChart.UntillDate;

      var newCandles = uploadedChart.Candles!.Where(c => !existCandleDt.Contains(c.DateTime)).ToList();
      var preparedCandles = newCandles.Select(c => new Candle(c.DateTime, c.Open, c.High, c.Low, c.Close, c.Volume, extendedChart)).ToList();
      await candleRep.AddRangeAsync(preparedCandles, cancellationToken: cancellationToken);

      return Result.Success(preparedCandles.Count());
    }
  }
}