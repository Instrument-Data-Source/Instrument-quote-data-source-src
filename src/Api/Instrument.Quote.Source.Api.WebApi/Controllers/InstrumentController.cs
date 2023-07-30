using Ardalis.Result;
using Instrument.Quote.Source.Api.WebApi.Dto;
using Instrument.Quote.Source.Api.WebApi.Tools;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Instrument.Quote.Source.Api.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class InstrumentController : ControllerBase
{
  private readonly ILogger<InstrumentController> _logger;
  private readonly IInstrumentSrv instrumentSrv;

  public InstrumentController(ILogger<InstrumentController> logger,
                              IInstrumentSrv instrumentSrv)
  {
    _logger = logger;
    this.instrumentSrv = instrumentSrv;
  }

  /// <summary>
  /// Get All Instrument
  /// </summary>
  /// <returns>All Instrument DTO</returns>
  /// <response code="200">Instrument getted</response>
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
  /// <response code="404">Related data not found</response>
  [HttpPost()]
  [ProducesResponseType(typeof(InstrumentResponseDto), StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(BadRequestDto), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status404NotFound)]
  [ApiExplorerSettings(GroupName = SwaggerGenOptionsInit.AdminGroup)]
  public async Task<ActionResult<InstrumentResponseDto>> CreateInstument([FromBody] NewInstrumentRequestDto instrumentRquest,
      CancellationToken cancellationToken = new())
  {
    var createResult = await instrumentSrv.CreateAsync(instrumentRquest, cancellationToken);
    return createResult.MapToActionResult();
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
  [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<InstrumentResponseDto>> GetInstrumentByIdOrCode(string instrumentStr, CancellationToken cancellationToken = default)
  {
    var result = await instrumentSrv.GetInstrumentByIdOrCodeAsync(instrumentStr, cancellationToken);
    return result.MapToActionResult();
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
  [ApiExplorerSettings(GroupName = SwaggerGenOptionsInit.AdminGroup)]
  public async Task<ActionResult> RemoveInstrument(string instrumentStr, CancellationToken cancellationToken = default)
  {
    var result = await instrumentSrv.RemoveInstrumentByIdOrStrAsync(instrumentStr, cancellationToken);
    return result.MapToActionResult();
  }


}
