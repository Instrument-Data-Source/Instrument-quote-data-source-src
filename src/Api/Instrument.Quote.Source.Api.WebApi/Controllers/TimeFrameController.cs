using Ardalis.Result;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Dto;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Interface;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Instrument.Quote.Source.Api.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimeFrameController : ControllerBase
{

  private readonly ILogger<TimeFrameController> _logger;
  private readonly ITimeFrameSrv timeFrameSrv;

  public TimeFrameController(ILogger<TimeFrameController> logger, ITimeFrameSrv timeFrameSrv)
  {
    _logger = logger;
    this.timeFrameSrv = timeFrameSrv;
  }

  [HttpGet()]
  [SwaggerOperation("Get all timeframes")]
  [SwaggerResponse(StatusCodes.Status200OK, "All timeframe getted", typeof(IEnumerable<TimeFrameResponseDto>))]
  public async Task<ActionResult<IEnumerable<TimeFrameResponseDto>>> GetAll()
  {
    var result = await timeFrameSrv.GetAllAsync();
    switch (result.Status)
    {
      case ResultStatus.Ok:
        return Ok(result.Value);
      default:
        throw new ApplicationException("Unexpected result status");
    }
  }

  [HttpGet("{timeFrameStr}")]
  [SwaggerOperation("Get Instrument by Code")]
  [SwaggerResponse(StatusCodes.Status200OK, "Instrument getted", typeof(TimeFrameResponseDto))]
  [SwaggerResponse(StatusCodes.Status404NotFound, "Timeframe not found")]
  public async Task<ActionResult<TimeFrameResponseDto>> GetByCode(string timeframeStr)
  {
    var result = await timeFrameSrv.GetByIdOrCodeAsync(timeframeStr);
    switch (result.Status)
    {
      case ResultStatus.Ok:
        return Ok(result.Value);
      case ResultStatus.NotFound:
        return NotFound("Timeframe not found");
      default:
        throw new ApplicationException("Unexpected result status");
    }
  }
}
