using Ardalis.Result;
using Instrument.Quote.Source.Api.WebApi.Dto;
using Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
using Instrument.Quote.Source.App.Core.CandleAggregate.Interface;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Instrument.Quote.Source.Api.WebApi.Controllers;

[ApiController]
[Route(Route)]
public class InstrumentController : ControllerBase
{
  public const string Route = "api/instrument";
  private readonly ILogger<InstrumentController> _logger;
  private readonly IInstrumentSrv instrumentSrv;
  private readonly ICandleSrv candleSrv;

  public InstrumentController(ILogger<InstrumentController> logger,
                              IInstrumentSrv instrumentSrv,
                              ICandleSrv candleSrv)
  {
    _logger = logger;
    this.instrumentSrv = instrumentSrv;
    this.candleSrv = candleSrv;
  }

  [HttpGet()]
  public async Task<ActionResult<IEnumerable<InstrumentResponseDto>>> GetAll()
  {
    var result_body = await instrumentSrv.GetAllAsync();
    return Ok(result_body);
  }

  [HttpPost()]
  [SwaggerOperation("Create new Instrument")]
  [SwaggerResponse(StatusCodes.Status201Created, "Instrument created", typeof(InstrumentResponseDto))]
  [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request", typeof(BadRequestDto))]
  public async Task<ActionResult<InstrumentResponseDto>> CreateInstument([FromBody] NewInstrumentRequestDto instrumentRquest,
      CancellationToken cancellationToken = new())
  {
    var createResult = await instrumentSrv.CreateInstrumentAsync(instrumentRquest, cancellationToken);
    switch (createResult.Status)
    {
      case ResultStatus.Ok:
        return Created($"~/{Route}/{createResult.Value.Code}", createResult.Value);
      case ResultStatus.Invalid:
        return BadRequest(new BadRequestDto(createResult.ValidationErrors));
      default:
        throw new ApplicationException("Unexpected result status");
    }
  }

  [HttpGet("{instrumentStr}")]
  [SwaggerOperation("Get Instrument by Id or Code")]
  [SwaggerResponse(StatusCodes.Status200OK, "Instrument getted", typeof(InstrumentResponseDto))]
  [SwaggerResponse(StatusCodes.Status404NotFound, "Instrument not found")]
  public async Task<ActionResult<InstrumentResponseDto?>> GetInstrumentByIdOrCode(string instrumentStr, CancellationToken cancellationToken = default)
  {
    var result = await instrumentSrv.GetInstrumentByIdOrCodeAsync(instrumentStr, cancellationToken);
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

  [HttpGet("{instrumentStr}/periods")]
  [SwaggerOperation("Get all loaded periods for instrument")]
  [SwaggerResponse(StatusCodes.Status200OK, "Instrument period getted", typeof(IReadOnlyDictionary<string, PeriodResponseDto>))]
  [SwaggerResponse(StatusCodes.Status404NotFound, "Instrument not found")]
  public async Task<ActionResult<IReadOnlyDictionary<string, PeriodResponseDto>>> GetPeriods(string instrumentStr, CancellationToken cancellationToken = default)
  {
    var instrumentResult = await instrumentSrv.GetInstrumentByIdOrCodeAsync(instrumentStr, cancellationToken);
    switch (instrumentResult.Status)
    {
      case ResultStatus.Ok:
        break;
      case ResultStatus.NotFound:
        return NotFound("Instrument not found");
      default:
        throw new ApplicationException("Unexpected result status");
    }

    var result = await candleSrv.GetExistPeriodAsync(instrumentResult.Value.Id, cancellationToken);
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

  [HttpDelete("{instrumentStr}")]
  [SwaggerOperation("Delete instrument")]
  [SwaggerResponse(StatusCodes.Status200OK, "Instrument deleted")]
  [SwaggerResponse(StatusCodes.Status404NotFound, "Instrument not found")]
  public async Task<ActionResult> RemoveInstrument(string instrumentStr, CancellationToken cancellationToken = default)
  {

    var result = await instrumentSrv.RemoveInstrumentByIdOrStrAsync(instrumentStr, cancellationToken);
    switch (result.Status)
    {
      case ResultStatus.Ok:
        return Ok("Instrument deleted");
      case ResultStatus.NotFound:
        return NotFound("Instrument not found");
      default:
        throw new ApplicationException("Unexpected result status");
    }
  }


}
