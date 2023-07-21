
using Instrument.Quote.Source.Api.WebApi.Dto;
using Instrument.Quote.Source.Api.WebApi.Tools;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Instrument.Quote.Source.Api.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ChartController : ControllerBase
{
  private readonly IChartSrv chartSrv;
  private readonly IReadInstrumentSrv instrumentSrv;
  private readonly ICandleSrv candleSrv;
  private readonly ITimeFrameSrv timeFrameSrv;

  public ChartController(IChartSrv chartSrv, IReadInstrumentSrv instrumentSrv, ICandleSrv candleSrv, ITimeFrameSrv timeFrameSrv)
  {
    this.chartSrv = chartSrv;
    this.instrumentSrv = instrumentSrv;
    this.candleSrv = candleSrv;
    this.timeFrameSrv = timeFrameSrv;
  }

  /// <summary>
  /// Get All Charts
  /// </summary>
  /// <returns>All charts DTO</returns>
  /// <response code="200">Charts getted</response>
  [HttpGet()]
  [ProducesResponseType(typeof(IEnumerable<ChartDto>), StatusCodes.Status200OK)]
  public async Task<ActionResult<IEnumerable<ChartDto>>> GetAll()
  {
    var result = await chartSrv.GetAllAsync();
    return result.MapToActionResult();
  }

  /// <summary>
  /// Get All Charts for instrument
  /// </summary>
  /// <returns>All charts DTO for instrumnent</returns>
  /// <response code="200">Charts getted</response>
  [HttpGet("{instrumentStr}")]
  [ProducesResponseType(typeof(IEnumerable<ChartDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<IEnumerable<ChartDto>>> GetForInstrument(string instrumentStr, CancellationToken cancellationToken = default)
  {
    var instrumentResult = await instrumentSrv.GetInstrumentByIdOrCodeAsync(instrumentStr, cancellationToken);
    if (!instrumentResult.IsSuccess)
      return instrumentResult.MapFailToActionResult();

    var result = await chartSrv.GetAsync(instrumentResult.Value.Id, cancellationToken);
    return result.MapToActionResult();
  }

  /// <summary>
  /// Get Candles from instrument|timeframe Chart
  /// </summary>
  /// <returns>All charts DTO for instrumnent</returns>
  /// <response code="200">Charts getted</response>
  [HttpGet("{instrumentStr}/{timeframeStr}")]
  [ProducesResponseType(typeof(IEnumerable<CandleDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<IEnumerable<CandleDto>>> GetCandles(string instrumentStr, string timeframeStr, [FromQuery] DateTime from, [FromQuery] DateTime untill, CancellationToken cancellationToken = default)
  {
    var instrumentResult = await instrumentSrv.GetInstrumentByIdOrCodeAsync(instrumentStr, cancellationToken);
    if (!instrumentResult.IsSuccess)
      return instrumentResult.MapFailToActionResult();

    var timeFrameResult = await timeFrameSrv.GetByIdOrCodeAsync(timeframeStr, cancellationToken);
    if (!timeFrameResult.IsSuccess)
      return timeFrameResult.MapFailToActionResult();

    var result = await candleSrv.GetAsync(instrumentResult.Value.Id, timeFrameResult.Value.Id, from, untill, cancellationToken);
    return result.MapToActionResult();
  }

  /// <summary>
  /// Post candle into instument|timeframe chart
  /// </summary>
  /// <remarks>
  /// Sample request:
  ///
  ///     POST api/Quote/EURUSD/timeframe/D1
  ///     {
  ///        "From": "2023-01-01T00:00:00.000Z",
  ///        "Untill":"2023-01-10T00:00:00.000Z",
  ///        "candles": [
  ///          {
  ///            "dateTime": "2023-01-01T00:00:00.000Z",
  ///            "open": 10,
  ///            "high": 20,
  ///            "low": 5,
  ///            "close": 8,
  ///            "volume": 10
  ///          },
  ///          {
  ///            "dateTime": "2023-01-09T00:00:00.000Z",
  ///            "open": 15,
  ///            "high": 20,
  ///            "low": 10,
  ///            "close": 13,
  ///            "volume": 5
  ///          }
  ///        ]
  ///     }
  ///
  /// </remarks>
  /// <returns>All charts DTO for instrumnent</returns>
  /// <response code="200">Charts getted</response>
  [HttpPost("{instrumentStr}/{timeframeStr}")]
  [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(BadRequestDto), StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<int>> PostCandles(string instrumentStr, string timeframeStr, [FromBody] UploadedCandlesDto uploadedCandlesDto, CancellationToken cancellationToken = default)
  {
    var instrumentResult = await instrumentSrv.GetInstrumentByIdOrCodeAsync(instrumentStr, cancellationToken);
    if (!instrumentResult.IsSuccess)
      return instrumentResult.MapFailToActionResult();

    var timeFrameResult = await timeFrameSrv.GetByIdOrCodeAsync(timeframeStr, cancellationToken);
    if (!timeFrameResult.IsSuccess)
      return timeFrameResult.MapFailToActionResult();

    var result = await candleSrv.AddAsync(instrumentResult.Value.Id, timeFrameResult.Value.Id, uploadedCandlesDto, cancellationToken);
    return result.MapToActionResult();
  }
}