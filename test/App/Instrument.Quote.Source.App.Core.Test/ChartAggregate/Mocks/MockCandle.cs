using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.ChartAggregate.Mapper;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;

namespace Instrument.Quote.Source.App.Core.Test.ChartAggregate.Mocks;
public class MockCandleFactory : absMockFactory
{
  private readonly Chart chart;
  CandleMapper candleMapper;

  public MockCandleFactory() : this(new MockChartFactory().CreateChart())
  {

  }
  public MockCandleFactory(Chart chart)
  {
    this.chart = chart;
    candleMapper = new CandleMapper(chart);
  }
  /// <summary>
  /// Create Candle with
  /// </summary>
  public CandleDto CreateCandleDto(DateTime? dt = null)
  {
    var r = new Random();
    var open = r.Next(200, 500);
    var _ret = new CandleDto()
    {
      DateTime = (dt ?? DateTime.Now).ToUniversalTime(),
      Open = open,
      High = open + 100,
      Low = open - 100,
      Close = open + (r.Next(0, 100) - 50),
      Volume = r.Next(0, 100)
    };
    return _ret;
  }

  /// <summary>
  /// Create Candle with
  /// </summary>
  public Candle CreateCandle(DateTime? dt = null)
  {
    return candleMapper.map(CreateCandleDto(dt));
  }

  /// <summary>
  /// Create new list of Candles with no ID
  /// </summary>
  public IEnumerable<CandleDto> CreateCandleDtos(int count, DateTime? startDt = null)
  {
    List<CandleDto> _ret_arr = new();
    var dt = (startDt ?? new DateTime(2020, 1, 1)).ToUniversalTime();
    for (int i = 0; i < count; i++)
    {
      _ret_arr.Add(CreateCandleDto(dt.AddDays(i)));
    }
    return _ret_arr;
  }
  public IEnumerable<CandleDto> CreateCandleDtos(DateTime fromDt, DateTime untillDt)
  {
    return CreateCandleDtos(fromDt, untillDt, new TimeSpan(1, 0, 0, 0));
  }

  public IEnumerable<Candle> CreateCandles(DateTime fromDt, DateTime untillDt)
  {
    return CreateCandles(fromDt, untillDt, new TimeSpan(1, 0, 0, 0));
  }

  public (IEnumerable<Candle>, IEnumerable<CandleDto>) CreateCandlesAndDtos(DateTime fromDt, DateTime untillDt)
  {
    return CreateCandlesAndDtos(fromDt, untillDt, new TimeSpan(1, 0, 0, 0));
  }
  /// <summary>
  /// Create new list of Candles with no ID
  /// </summary>
  public IEnumerable<CandleDto> CreateCandleDtos(DateTime fromDt, DateTime untillDt, TimeSpan step)
  {
    List<CandleDto> _ret_arr = new();
    var dt = fromDt.ToUniversalTime();
    while (dt < untillDt)
    {
      _ret_arr.Add(CreateCandleDto(dt));
      dt = dt.Add(step);
    }
    return _ret_arr;
  }

  public IEnumerable<Candle> CreateCandles(DateTime fromDt, DateTime untillDt, TimeSpan step)
  {
    return CreateCandleDtos(fromDt, untillDt, step).Select(candleMapper.map).ToList();
  }

  public (IEnumerable<Candle>, IEnumerable<CandleDto>) CreateCandlesAndDtos(DateTime fromDt, DateTime untillDt, TimeSpan step)
  {
    var dtos = CreateCandleDtos(fromDt, untillDt, step).ToList();
    var candles = dtos.Select(candleMapper.map).ToList();
    return (candles, dtos);
  }
}