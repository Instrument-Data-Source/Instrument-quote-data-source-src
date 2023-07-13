using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Instrument.Quote.Source.Api.WebApi.Controllers;

[ApiController]
[Route(Route)]
[Produces("application/json")]
public class TestController : ControllerBase
{
  public const string Route = "api/test";
  private readonly ILogger<TestController> _logger;

  public TestController(ILogger<TestController> logger)
  {
    _logger = logger;
  }

  [HttpPost("throwError")]
  [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
  public async Task<ActionResult<string>> PostThrow(bool needThrow)
  {
    if (needThrow)
    {
      throw new ValidationException("Some validation error");
    }

    return Ok("No need throw");
  }
}