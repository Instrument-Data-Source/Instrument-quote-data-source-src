namespace Instrument.Quote.Source.App.Test.CandleAggregate;

using System.Net;
using System.Threading.Tasks;
using global::Instrument.Quote.Source.App.Core.CandleAggregate.Interface;
using global::Instrument.Quote.Source.App.Core.TimeFrameAggregate.Interface;
using global::Instrument.Quote.Source.App.Test.Tool;
using global::Instrument.Quote.Source.App.Test.Tool.SeedData;
using Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
using Instrument.Quote.Source.App.Test.Tool.SeedData.Dto;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
public class CandleSrv_Add_Test : BaseTest
{
  ISeedContainer seeds;
  public CandleSrv_Add_Test(ITestOutputHelper output) : base(nameof(CandleSrv_Add_Test), output)
  {
    using var sc = serviceProvider.CreateScope();
    seeds = new SeedDataFactory(sc.ServiceProvider).InitInstrument().Seeds;
  }

  [Fact]
  public async Task WHEN_when_add_to_new_instrument_THEN_save_candlesAsync()
  {
    // Array
    using var arrat_sc = serviceProvider.CreateScope();
    var using_inst = seeds.SeedInstrument.Instrument1;
    var using_tf = await arrat_sc.ServiceProvider.GetService<ITimeFrameSrv>()!.GetByCodeAsync("m1");
    var fromDate = new DateTime(2020, 1, 1, 1, 1, 0).ToUniversalTime();
    var untillDate = new DateTime(2020, 1, 1, 1, 21, 0).ToUniversalTime();

    var usingDto = DtoFactory.Instrument1.candleFactory["m1"];

    arrat_sc.Dispose();

    // Act
    using var act_sc = serviceProvider.CreateScope();
    var assert_return_count = act_sc.ServiceProvider.GetService<ICandleSrv>()!.AddAsync(using_inst.Id, using_tf.Id, fromDate, untillDate, usingDto).Result;

    act_sc.Dispose();


    // Assert
    Assert.Equal(usingDto.Count(), assert_return_count);

    using var assert_sc = serviceProvider.CreateScope();
    var candle_srv = assert_sc.ServiceProvider.GetService<ICandleSrv>()!;
    var asserted_data = candle_srv.GetAsync(using_inst.Id, using_tf.Id, fromDate, untillDate).Result;
    Assert.Equal(usingDto.Count(), asserted_data.Count());
    foreach (var dto in usingDto)
    {
      Assert.Contains(dto, asserted_data);
    }

    var asserted_period = candle_srv.TryGetExistPeriodAsync(using_inst.Id, using_tf.Id).Result;
    Assert.Equal(fromDate, asserted_period.FromDate);
    Assert.Equal(untillDate, asserted_period.UntillDate);
  }

  [Fact]
  public async Task WHEN_add_additional_data_THEN_add_dataAsync()
  {
    // Array
    using var arrat_sc = serviceProvider.CreateScope();
    var using_inst = seeds.SeedInstrument.Instrument1;
    var using_tf = arrat_sc.ServiceProvider.GetService<ITimeFrameSrv>()!.GetByCodeAsync("m1").Result;
    var fromDate = new DateTime(2020, 1, 1, 1, 1, 0).ToUniversalTime();
    var midDate = new DateTime(2020, 1, 1, 1, 11, 0).ToUniversalTime();
    var untillDate = new DateTime(2020, 1, 1, 1, 21, 0).ToUniversalTime();

    var usingDto = DtoFactory.Instrument1.candleFactory["m1"];
    var fisrt_part = new List<CandleDto>();
    var second_part = new List<CandleDto>();
    for (int i = 0; i < 10; i++)
    {
      fisrt_part.Add(usingDto.ElementAt(i));
    }
    for (int i = 10; i < 20; i++)
    {
      second_part.Add(usingDto.ElementAt(i));
    }

    await arrat_sc.ServiceProvider.GetService<ICandleSrv>()!.AddAsync(using_inst.Id, using_tf.Id, fromDate, midDate, fisrt_part);

    arrat_sc.Dispose();

    // Act
    using var act_sc = serviceProvider.CreateScope();
    var assert_return_count = act_sc.ServiceProvider.GetService<ICandleSrv>()!.AddAsync(using_inst.Id, using_tf.Id, midDate, untillDate, second_part).Result;

    act_sc.Dispose();


    // Assert
    Assert.Equal(second_part.Count(), assert_return_count);

    using var assert_sc = serviceProvider.CreateScope();
    var candle_srv = assert_sc.ServiceProvider.GetService<ICandleSrv>()!;
    var asserted_data = candle_srv.GetAsync(using_inst.Id, using_tf.Id, fromDate, untillDate).Result;
    Assert.Equal(usingDto.Count(), asserted_data.Count());
    foreach (var dto in usingDto)
    {
      Assert.Contains(dto, asserted_data);
    }

    var asserted_period = candle_srv.TryGetExistPeriodAsync(using_inst.Id, using_tf.Id).Result;
    Assert.Equal(fromDate, asserted_period.FromDate);
    Assert.Equal(untillDate, asserted_period.UntillDate);
  }

  [Fact]
  public async Task WHEN_add_additional_data_cross_period_THEN_add_dataAsync()
  {
    // Array
    using var arrat_sc = serviceProvider.CreateScope();
    var using_inst = seeds.SeedInstrument.Instrument1;
    var using_tf = await arrat_sc.ServiceProvider.GetService<ITimeFrameSrv>()!.GetByCodeAsync("m1");
    var fromDate = new DateTime(2020, 1, 1, 1, 1, 0).ToUniversalTime();
    var midDate = new DateTime(2020, 1, 1, 1, 11, 0).ToUniversalTime();
    var mid2Date = new DateTime(2020, 1, 1, 1, 9, 0).ToUniversalTime();
    var untillDate = new DateTime(2020, 1, 1, 1, 21, 0).ToUniversalTime();

    var usingDto = DtoFactory.Instrument1.candleFactory["m1"];
    var fisrt_part = new List<CandleDto>();
    var second_part = new List<CandleDto>();
    for (int i = 0; i < 10; i++)
    {
      fisrt_part.Add(usingDto.ElementAt(i));
    }
    for (int i = 8; i < 20; i++)
    {
      second_part.Add(usingDto.ElementAt(i));
    }

    arrat_sc.ServiceProvider.GetService<ICandleSrv>()!.AddAsync(using_inst.Id, using_tf.Id, fromDate, midDate, fisrt_part).Wait();

    arrat_sc.Dispose();

    // Act

    // Assert
    using var assert_sc = serviceProvider.CreateScope();
    var candle_srv = assert_sc.ServiceProvider.GetService<ICandleSrv>()!;
    await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () =>
          await candle_srv.AddAsync(using_inst.Id, using_tf.Id, mid2Date, untillDate, second_part));

    var asserted_period = candle_srv.TryGetExistPeriodAsync(using_inst.Id, using_tf.Id).Result;
    Assert.Equal(fromDate, asserted_period.FromDate);
    Assert.Equal(midDate, asserted_period.UntillDate);
  }

  [Fact]
  public async void WHEN_add_additional_data_but_skip_period_THEN_exception()
  {
    // Array
    using var arrat_sc = serviceProvider.CreateScope();
    var using_inst = seeds.SeedInstrument.Instrument1;
    var using_tf = await arrat_sc.ServiceProvider.GetService<ITimeFrameSrv>()!.GetByCodeAsync("m1");
    var fromDate = new DateTime(2020, 1, 1, 1, 1, 0).ToUniversalTime();
    var midDate = new DateTime(2020, 1, 1, 1, 11, 0).ToUniversalTime();
    var mid2Date = new DateTime(2020, 1, 1, 1, 12, 0).ToUniversalTime();
    var untillDate = new DateTime(2020, 1, 1, 1, 21, 0).ToUniversalTime();

    var usingDto = DtoFactory.Instrument1.candleFactory["m1"];
    var fisrt_part = new List<CandleDto>();
    var second_part = new List<CandleDto>();
    for (int i = 0; i < 10; i++)
    {
      fisrt_part.Add(usingDto.ElementAt(i));
    }
    for (int i = 11; i < 20; i++)
    {
      second_part.Add(usingDto.ElementAt(i));
    }

    arrat_sc.ServiceProvider.GetService<ICandleSrv>()!.AddAsync(using_inst.Id, using_tf.Id, fromDate, midDate, fisrt_part).Wait();

    arrat_sc.Dispose();

    // Act

    // Assert
    using var assert_sc = serviceProvider.CreateScope();
    var candle_srv = assert_sc.ServiceProvider.GetService<ICandleSrv>()!;
    await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () =>
          await candle_srv.AddAsync(using_inst.Id, using_tf.Id, mid2Date, untillDate, second_part));

    var asserted_period = candle_srv.TryGetExistPeriodAsync(using_inst.Id, using_tf.Id).Result;
    Assert.Equal(fromDate, asserted_period.FromDate);
    Assert.Equal(midDate, asserted_period.UntillDate);
  }
}