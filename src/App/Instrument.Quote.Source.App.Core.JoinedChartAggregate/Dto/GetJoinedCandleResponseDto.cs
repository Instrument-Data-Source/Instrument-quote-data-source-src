namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Dto;

public class GetJoinedCandleResponseDto
{
  public enum EnumStatus
  {
    Ready,
    PartlyReady,
    InProgress
  }
  public EnumStatus Status;

  public IEnumerable<JoinedCandleDto> JoinedCandles { get; set; }
}