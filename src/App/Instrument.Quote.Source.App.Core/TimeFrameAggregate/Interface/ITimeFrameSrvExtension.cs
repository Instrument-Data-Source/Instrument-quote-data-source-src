using Ardalis.Result;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Dto;
using m = Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
namespace Instrument.Quote.Source.App.Core.TimeFrameAggregate.Interface;

public static class ITimeFrameSrvExtension
{
  public static async Task<Result<TimeFrameResponseDto>> GetByIdOrCodeAsync(this ITimeFrameSrv service, string timeFrameStr, CancellationToken cancellationToken = default)
  {
    var timeframeId = getTimeFrameId(timeFrameStr);
    return await service.GetByIdAsync(timeframeId, cancellationToken);
  }

  private static int getTimeFrameId(string timeframeStr)
  {
    if (Int32.TryParse(timeframeStr, out int timeframeId))
      return timeframeId;
    else
    {
      m.TimeFrame.Enum? timeframeParseId = Enum.GetValues<m.TimeFrame.Enum>().SingleOrDefault(e => e.ToString() == timeframeStr);
      return timeframeParseId != null ? (int)timeframeParseId : -1;
    }
  }
}