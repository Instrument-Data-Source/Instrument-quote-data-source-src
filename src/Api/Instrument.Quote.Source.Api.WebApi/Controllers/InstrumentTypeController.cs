using Ardalis.Result;
using Instrument.Quote.Source.Api.WebApi.Dto;
using Instrument.Quote.Source.Api.WebApi.Tools;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Instrument.Quote.Source.Api.WebApi.Controllers;

[ApiController]
[Route(Route)]
[Produces("application/json")]
public class InstrumentTypeController : ControllerBase
{
  public const string Route = "api/instrument-type";
  private readonly ILogger<InstrumentTypeController> logger;
  private readonly IInstrumentTypeSrv instrumentTypeSrv;

  public InstrumentTypeController(ILogger<InstrumentTypeController> logger,
                             IInstrumentTypeSrv instrumentTypeSrv)
  {
    this.logger = logger;
    this.instrumentTypeSrv = instrumentTypeSrv;
  }
  /// <summary>
  /// Get all instrumen types
  /// </summary>
  /// <returns>All Instrument Type DTO</returns>
  /// <response code="200">Instrument types getted</response>
  [HttpGet()]
  [ProducesResponseType(typeof(IEnumerable<InstrumentTypeResponseDto>), StatusCodes.Status200OK)]
  public async Task<ActionResult<IEnumerable<InstrumentTypeResponseDto>>> GetAll()
  {
    var result = await instrumentTypeSrv.GetAllAsync();
    return result.MapToActionResult();
  }
  /// <summary>
  /// Get Instrument type by Name
  /// </summary>
  /// <param name="instrumentTypeStr">Instrument Type Id or Name</param>
  /// <returns>Instrument Type DTO</returns>
  /// <response code="200">Instrument type getted</response>
  /// <response code="404">Instrument type not found</response>
  [HttpGet("{instrumentTypeStr}")]
  [ProducesResponseType(typeof(InstrumentTypeResponseDto), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<InstrumentTypeResponseDto>> GetByIdOrName(string instrumentTypeStr)
  {
    var result = await instrumentTypeSrv.GetByIdOrCodeAsync(instrumentTypeStr);
    return result.MapToActionResult();
  }
}
