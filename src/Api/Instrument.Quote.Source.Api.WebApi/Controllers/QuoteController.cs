using Ardalis.Result;
using Instrument.Quote.Source.Api.WebApi.Dto;
using Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
using Instrument.Quote.Source.App.Core.CandleAggregate.Interface;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Interface;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Instrument.Quote.Source.Api.WebApi.Controllers;

[ApiController]
[Route(Route)]
[Produces("application/json")]
public class QuoteController : ControllerBase
{
  public const string Route = "api/Quote";
  private readonly ILogger<QuoteController> _logger;
  private readonly IInstrumentSrv instrumentSrv;
  private readonly ICandleSrv candleSrv;
  private readonly ITimeFrameSrv timeFrameSrv;

  public QuoteController(ILogger<QuoteController> logger,
                              IInstrumentSrv instrumentSrv,
                              ICandleSrv candleSrv,
                              ITimeFrameSrv timeFrameSrv)
  {
    _logger = logger;
    this.instrumentSrv = instrumentSrv;
    this.candleSrv = candleSrv;
    this.timeFrameSrv = timeFrameSrv;
  }

  /// <summary>
  /// Get loaded period for instrument and timeframe
  /// </summary>
  /// <param name="instrumentStr">Instrument Id or Name</param>
  /// <param name="timeframeStr">TimeFrame Id or Code</param>
  /// <returns>Array of Instrument Loaded Period DTO</returns>
  /// <response code="200">Instrument getted</response>
  /// <response code="404">Period not found</response>
  [HttpGet("{instrumentStr}/timeframe/{timeframeStr}/periods")]
  [ProducesResponseType(typeof(PeriodResponseDto), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<PeriodResponseDto>> GetPeriod(string instrumentStr, string timeframeStr, CancellationToken cancellationToken = default)
  {
    var instrumentIdResultTask = instrumentSrv.GetInstrumentByIdOrCodeAsync(instrumentStr, cancellationToken);
    var timeframeResultTask = timeFrameSrv.GetByIdOrCodeAsync(timeframeStr, cancellationToken);

    await Task.WhenAll(instrumentIdResultTask, timeframeResultTask);

    if (!instrumentIdResultTask.Result.IsSuccess || !timeframeResultTask.Result.IsSuccess)
    {
      return NotFound();
    }

    var result = await candleSrv.TryGetExistPeriodAsync(instrumentIdResultTask.Result.Value.Id, timeframeResultTask.Result.Value.Id, cancellationToken);
    switch (result.Status)
    {
      case ResultStatus.Ok:
        return Ok(result.Value);
      case ResultStatus.NotFound:
        return NotFound("Instrument not found");
      default:
        throw new ApplicationException("Unexpected result status");
    }
  }

  /// <summary>
  /// Add data to instrument and period
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
  /// <returns>Count of added candles</returns>
  /// <response code="201">Data added</response>
  /// <response code="400">Invalid request</response>
  /// <response code="404">Instument or timeframe not found</response>
  [HttpPost("{instrumentStr}/timeframe/{timeframeStr}")]
  [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(BadRequestDto), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<int>> AddData(string instrumentStr, string timeframeStr, [FromBody] NewCandlesDto newCandlesDto, CancellationToken cancellationToken = default)
  {
    var instrumentIdResultTask = instrumentSrv.GetInstrumentByIdOrCodeAsync(instrumentStr, cancellationToken);
    var timeframeResultTask = timeFrameSrv.GetByIdOrCodeAsync(timeframeStr, cancellationToken);

    await Task.WhenAll(instrumentIdResultTask, timeframeResultTask);

    if (!instrumentIdResultTask.Result.IsSuccess || !timeframeResultTask.Result.IsSuccess)
    {
      return NotFound();
    }

    var result = await candleSrv.AddAsync(instrumentIdResultTask.Result.Value.Id, timeframeResultTask.Result.Value.Id, newCandlesDto);
    switch (result.Status)
    {
      case ResultStatus.Ok:
        return Created($"~/{Route}/{instrumentStr}/timeframe/{timeframeStr}/periods", result.Value);
      case ResultStatus.Invalid:
        return BadRequest(new BadRequestDto(result.ValidationErrors));
      case ResultStatus.NotFound:
        return NotFound("Instrument not found");
      default:
        throw new ApplicationException("Unexpected result status");
    }
  }
}