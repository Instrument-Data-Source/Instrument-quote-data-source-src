using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Dto;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Interface;
using Microsoft.AspNetCore.Mvc;

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
  public async Task<ActionResult<IEnumerable<TimeFrameResponseDto>>> GetAll()
  {
    var result_body = await timeFrameSrv.GetAllAsync();
    return Ok(result_body);
  }

  [HttpGet("{code}")]
  public async Task<ActionResult<IEnumerable<TimeFrameResponseDto>>> GetByCode(string code)
  {
    try
    {
      var result_body = await timeFrameSrv.GetByCodeAsync(code);
      return Ok(result_body);
    }
    catch (ArgumentOutOfRangeException ex)
    {
      return NotFound(ex.Message);
    }
  }
}
