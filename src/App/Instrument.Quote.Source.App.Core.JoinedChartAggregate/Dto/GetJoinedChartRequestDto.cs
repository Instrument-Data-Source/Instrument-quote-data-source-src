using System.ComponentModel.DataAnnotations;
using Ardalis.Result;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Validations.Attributes;
using MediatR;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Dto;

public class GetJoinedChartRequestDto : IRequest<Result<GetJoinedCandleResponseDto>>
{
  [IsIdOf<ent.Instrument>()]
  public int instrumentId { get; set; }

  [IsIdOf<TimeFrame>()]
  public int stepTimeFrameId { get; set; }

  [IsIdOf<TimeFrame>()]
  public int targetTimeFrameId { get; set; }

  public bool hideIntermediateCandles { get; set; }

  [UTCKind]
  [CompareTo(CompType.LT, nameof(untill))]
  public DateTime from { get; set; }

  [UTCKind]
  [CompareTo(CompType.GT, nameof(from))]
  public DateTime untill { get; set; }

  private ValidationContext CreateValidationContext(IServiceProvider? serviceProvider = null)
  {
    return new ValidationContext(this, serviceProvider, new Dictionary<object, object?>());
  }
  public bool IsValid(out ICollection<ValidationResult>? validationResults)
  {
    var context = CreateValidationContext();
    validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
    return Validator.TryValidateObject(this, context, validationResults);
  }

  public GetJoinedChartRequestDto Validate(IServiceProvider? serviceProvider)
  {
    var context = CreateValidationContext(serviceProvider);
    Validator.ValidateObject(this, context, true);
    return this;
  }
}