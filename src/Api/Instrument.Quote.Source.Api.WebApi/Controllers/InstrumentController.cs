using Instrument.Quote.Source.Api.WebApi.Tools;
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
  private readonly ParameterParser parameterParser;
  private readonly ICandleSrv candleSrv;

  public InstrumentController(ILogger<InstrumentController> logger,
                              IInstrumentSrv instrumentSrv,
                              ParameterParser parameterParser,
                              ICandleSrv candleSrv)
  {
    _logger = logger;
    this.instrumentSrv = instrumentSrv;
    this.parameterParser = parameterParser;
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
  [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request")]
  public async Task<ActionResult<InstrumentResponseDto>> CreateInstument([FromBody] NewInstrumentRequestDto instrumentRquest,
      CancellationToken cancellationToken = new())
  {
    try
    {
      InstrumentResponseDto newInstrument = await instrumentSrv.CreateInstrumentAsync(instrumentRquest, cancellationToken);
      return Created($"~/{Route}/{newInstrument.Code}", newInstrument);
    }
    catch (FluentValidation.ValidationException ex)
    {
      return BadRequest(ex);
    }
  }

  [HttpGet("{instrumentStr}")]
  public async Task<ActionResult<InstrumentResponseDto?>> GetInstrumentByIdOrCode(string instrumentStr, CancellationToken cancellationToken = default)
  {
    var instrumentId = await parameterParser.getInstrumentIdAsync(instrumentStr, cancellationToken);
    var result_body = await instrumentSrv.TryGetInstrumentByIdAsync(instrumentId, cancellationToken);
    return Ok(result_body);
  }

  [HttpGet("{instrumentStr}/periods")]
  public async Task<ActionResult<IReadOnlyDictionary<string, PeriodResponseDto>>> GetPeriods(string instrumentStr, CancellationToken cancellationToken = default)
  {
    var instrumentId = await parameterParser.getInstrumentIdAsync(instrumentStr, cancellationToken);
    var result_body = await candleSrv.GetExistPeriodAsync(instrumentId, cancellationToken);
    return Ok(result_body);
  }

  [HttpDelete("{instrumentStr}")]
  public async Task<ActionResult> RemoveInstrument(string instrumentStr, CancellationToken cancellationToken = default)
  {
    var instrumentId = await parameterParser.getInstrumentIdAsync(instrumentStr, cancellationToken);
    await instrumentSrv.RemoveInstrumentAsync(instrumentId, cancellationToken);
    return Ok();
  }


}
