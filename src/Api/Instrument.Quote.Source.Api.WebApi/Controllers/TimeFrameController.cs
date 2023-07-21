using Ardalis.Result;
using Instrument.Quote.Source.Api.WebApi.Tools;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Dto;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Instrument.Quote.Source.Api.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TimeFrameController : ControllerBase
{

  private readonly ILogger<TimeFrameController> _logger;
  private readonly ITimeFrameSrv timeFrameSrv;

  public TimeFrameController(ILogger<TimeFrameController> logger, ITimeFrameSrv timeFrameSrv)
  {
    _logger = logger;
    this.timeFrameSrv = timeFrameSrv;
  }

  /// <summary>
  /// Get all timeframes
  /// </summary>
  /// <response code="200">All timeframe getted</response>
  [HttpGet()]
  [ProducesResponseType(typeof(IEnumerable<TimeFrameResponseDto>), StatusCodes.Status200OK)]
  public async Task<ActionResult<IEnumerable<TimeFrameResponseDto>>> GetAll()
  {
    var result = await timeFrameSrv.GetAllAsync();
    return result.MapToActionResult();
  }

  /// <summary>
  /// Get TimeFrame by Code
  /// </summary>
  /// <param name="instrumentStr">Instrument Id or Name</param>
  /// <returns>Instrument DTO</returns>
  /// <response code="200">TimeFrame getted</response>
  /// <response code="404">Timeframe not found</response>
  [HttpGet("{timeframeStr}")]
  [ProducesResponseType(typeof(TimeFrameResponseDto), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<TimeFrameResponseDto>> GetByIdOrCode(string timeframeStr)
  {
    var result = await timeFrameSrv.GetByIdOrCodeAsync(timeframeStr);
    return result.MapToActionResult();
  }
}
