using System.ComponentModel.DataAnnotations;
using Ardalis.Result;
using Instrument.Quote.Source.Shared.Validations.Attributes;
using MediatR;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Dto;

public class GetJoinedChartRequestDto : IRequest<Result<GetJoinedCandleResponseDto>>
{
  [Range(1, int.MaxValue)]
  public int instrumentId { get; set; }

  [Range(1, int.MaxValue)]
  public int stepTimeFrameId { get; set; }

  [Range(1, int.MaxValue)]
  public int targetTimeFrameId { get; set; }

  public bool hideIntermediateCandles { get; set; }

  [UTCKind]
  [CompareTo(CompType.GT, nameof(untill))]
  public DateTime from { get; set; }

  [UTCKind]
  [CompareTo(CompType.LT, nameof(from))]
  public DateTime untill { get; set; }
}