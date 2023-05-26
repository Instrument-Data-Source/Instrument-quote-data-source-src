using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using m = Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
namespace Instrument.Quote.Source.Api.WebApi.Tools;
/*
public class ParameterParser
{
  private readonly ILogger<ParameterParser> logger;
  private readonly IInstrumentSrv instrumentSrv;

  public ParameterParser(ILogger<ParameterParser> logger, IInstrumentSrv instrumentSrv)
  {
    this.logger = logger;
    this.instrumentSrv = instrumentSrv;
  }
  public async Task<int> getInstrumentIdAsync(string instrumentStr, CancellationToken cancellationToken = default)
  {
    if (Int32.TryParse(instrumentStr, out int instrumentId))
      return instrumentId;
    else
    {
      var instrument = await instrumentSrv.TryGetInstrumentByCodeAsync(instrumentStr, cancellationToken);
      return instrument != null ? instrument.Id : -1;
    }
  }

  public int getTimeFrameId(string timeframeStr)
  {
    if (Int32.TryParse(timeframeStr, out int timeframeId))
      return timeframeId;
    else
    {
      m.TimeFrame.Enum? timeframeParseId = Enum.GetValues<m.TimeFrame.Enum>().SingleOrDefault(e => e.ToString() == timeframeStr);
      return timeframeParseId != null ? (int)timeframeParseId : -1;
    }
  }
}*/