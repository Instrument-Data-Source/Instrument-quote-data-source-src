using Ardalis.Result;
using Instrument.Quote.Source.Api.WebApi.Dto;
using Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
using Instrument.Quote.Source.App.Core.CandleAggregate.Interface;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Instrument.Quote.Source.Api.WebApi.Controllers;

[ApiController]
[Route(Route)]
[Produces("application/json")]
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

  /// <summary>
  /// Get All Instrument
  /// </summary>
  /// <returns>All Instrument DTO</returns>
  /// <response code="200">Instrument type getted</response>
  [HttpGet()]
  [ProducesResponseType(typeof(IEnumerable<InstrumentResponseDto>), StatusCodes.Status200OK)]
  public async Task<ActionResult<IEnumerable<InstrumentResponseDto>>> GetAll()
  {
    var result_body = await instrumentSrv.GetAllAsync();
    return Ok(result_body);
  }

  /// <summary>
  /// Create new Instrument
  /// </summary>
  /// <remarks>
  /// Sample request:
  ///
  ///     POST /api/instrument
  ///     {
  ///        "Name":"EUR vs USD",
  ///        "Code":"EURUSD",
  ///        "TypeId":1,
  ///        "PriceDecimalLen": 5
  ///        "VolumeDecimalLen": 2
  ///     }
  ///
  /// </remarks>
  /// <returns>Created Instrument DTO</returns>
  /// <response code="201">Instrument created</response>
  /// <response code="400">Invalid request</response>
  [HttpPost()]
  [ProducesResponseType(typeof(InstrumentResponseDto), StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(BadRequestDto), StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<InstrumentResponseDto>> CreateInstument([FromBody] NewInstrumentRequestDto instrumentRquest,
      CancellationToken cancellationToken = new())
  {
    var createResult = await instrumentSrv.CreateAsync(instrumentRquest, cancellationToken);
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

  /// <summary>
  /// Get Instrument by Id or Code
  /// </summary>
  /// <param name="instrumentStr">Instrument Id or Name</param>
  /// <returns>Instrument DTO</returns>
  /// <response code="200">Instrument getted</response>
  /// <response code="404">Instrument not found</response>
  [HttpGet("{instrumentStr}")]
  [ProducesResponseType(typeof(InstrumentResponseDto), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
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

  /// <summary>
  /// Get all loaded periods for instrument
  /// </summary>
  /// <param name="instrumentStr">Instrument Id or Name</param>
  /// <returns>Array of Instrument Loaded Period DTO</returns>
  /// <response code="200">Instrument period getted</response>
  /// <response code="404">Instrument not found</response>
  [HttpGet("{instrumentStr}/periods")]
  [ProducesResponseType(typeof(IReadOnlyDictionary<string, PeriodResponseDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
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

  /// <summary>
  /// Delete instrument
  /// </summary>
  /// <param name="instrumentStr">Instrument Id or Name</param>
  /// <response code="200">Instrument deleted</response>
  /// <response code="404">Instrument not found</response>
  [HttpDelete("{instrumentStr}")]
  [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
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
