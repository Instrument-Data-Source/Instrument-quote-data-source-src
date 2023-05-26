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
public class InstrumentTypeController : ControllerBase
{
  public const string Route = "api/instrument-type";/*
   public InstrumentController(ILogger<InstrumentController> logger,
                              IInstrumentSrv instrumentSrv,
                              ICandleSrv candleSrv)
  {
    _logger = logger;
    this.instrumentSrv = instrumentSrv;
    this.candleSrv = candleSrv;
  }*/
}