using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.App.Test.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace Instrument.Quote.Source.App.Test.ChartAggregate.Mocks;


public static class InitExtension
{
  private static int year = 2020;
  public static async Task<UploadedCandlesDto> InitChartData(this BaseDbTest baseDbTest, InstrumentResponseDto instrument, TimeFrame.Enum timeframe)
  {

    var expectedFrom = new DateTime(year, 3, 1).ToUniversalTime();
    var expectedUntill = new DateTime(year, 4, 1).ToUniversalTime();


    Interlocked.Increment(ref year);

    return await baseDbTest.InitChartData(instrument, timeframe, expectedFrom, expectedUntill);
  }

  public static async Task<UploadedCandlesDto> InitChartData(this BaseDbTest baseDbTest, InstrumentResponseDto instrument, TimeFrame.Enum timeframe, DateTime fromDt, DateTime untillDt)
  {

    var expectedFrom = fromDt.ToUniversalTime();
    var expectedUntill = untillDt.ToUniversalTime();
    var expectedCandles = new MockCandleDtoFactory().CreateCandleDtos(expectedFrom, expectedUntill);
    var uploadedData = new UploadedCandlesDto()
    {
      FromDate = expectedFrom,
      UntillDate = expectedUntill,
      Candles = expectedCandles
    };

    using var act_scope = baseDbTest.global_sp.CreateScope();

    var sp = act_scope.ServiceProvider;
    var usedSrv = sp.GetRequiredService<ICandleSrv>();
    var assertedResult = await usedSrv.AddAsync(instrument.Id, (int)timeframe, uploadedData);
    if (!assertedResult.IsSuccess)
      throw new ApplicationException("unexpected behavior on test init");

    return uploadedData;
  }
}