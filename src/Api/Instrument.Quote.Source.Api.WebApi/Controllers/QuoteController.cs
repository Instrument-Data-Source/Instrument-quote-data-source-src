using Ardalis.Result;
using Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
using Instrument.Quote.Source.App.Core.CandleAggregate.Interface;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Interface;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Instrument.Quote.Source.Api.WebApi.Controllers;

[ApiController]
[Route(Route)]
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
  [HttpGet("{instrumentStr}/timeframe/{timeframeStr}/periods")]
  [SwaggerOperation("Get loaded period for instrument and timeframe")]
  [SwaggerResponse(StatusCodes.Status200OK, "Instrument getted", typeof(PeriodResponseDto))]
  [SwaggerResponse(StatusCodes.Status404NotFound, "Period not found")]
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

  [HttpPost("{instrumentStr}/timeframe/{timeframeStr}")]
  [SwaggerOperation("Add data to instrument and period")]
  [SwaggerResponse(StatusCodes.Status201Created, "Data added", typeof(int))]
  [SwaggerResponse(StatusCodes.Status404NotFound, "Instument or timeframe not found")]
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
      case ResultStatus.NotFound:
        return NotFound("Instrument not found");
      default:
        throw new ApplicationException("Unexpected result status");
    }
  }
}