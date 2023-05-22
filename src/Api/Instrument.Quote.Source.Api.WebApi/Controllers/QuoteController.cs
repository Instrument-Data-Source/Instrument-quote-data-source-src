using Instrument.Quote.Source.Api.WebApi.Tools;
using Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
using Instrument.Quote.Source.App.Core.CandleAggregate.Interface;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Instrument.Quote.Source.Api.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuoteController : ControllerBase
{
  private readonly ILogger<QuoteController> _logger;
  private readonly IInstrumentSrv instrumentSrv;
  private readonly ParameterParser parameterParser;
  private readonly ICandleSrv candleSrv;

  public QuoteController(ILogger<QuoteController> logger,
                              IInstrumentSrv instrumentSrv,
                              ParameterParser parameterParser,
                              ICandleSrv candleSrv)
  {
    _logger = logger;
    this.instrumentSrv = instrumentSrv;
    this.parameterParser = parameterParser;
    this.candleSrv = candleSrv;
  }
  [HttpGet("{instrumentStr}/timeframe/{timeframeStr}/periods")]
  public async Task<ActionResult<IReadOnlyDictionary<string, PeriodResponseDto>>> GetPeriod(string instrumentStr, string timeframeStr, CancellationToken cancellationToken = default)
  {
    var instrumentId = await parameterParser.getInstrumentIdAsync(instrumentStr, cancellationToken);
    var timeframeId = parameterParser.getTimeFrameId(timeframeStr);
    var result_body = await candleSrv.TryGetExistPeriodAsync(instrumentId, timeframeId, cancellationToken);
    return Ok(result_body);
  }

  [HttpPost("{instrumentStr}/timeframe/{timeframeStr}")]
  public async Task<ActionResult<int>> AddData(string instrumentStr, string timeframeStr, [FromBody] NewCandlesDto newCandlesDto, CancellationToken cancellationToken = default)
  {
    var instrumentId = await parameterParser.getInstrumentIdAsync(instrumentStr, cancellationToken);
    var timeframeId = parameterParser.getTimeFrameId(timeframeStr);
    var result_body = await candleSrv.AddAsync(instrumentId, timeframeId, newCandlesDto);
    return Ok(result_body);
  }
}