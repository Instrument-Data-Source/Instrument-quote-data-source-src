using FluentValidation;
using Instrument.Quote.Source.App.Core.CandleAggregate.Validator.Attribute;
using Instrument.Quote.Source.App.Core.Event;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.FluentValidation.Extension;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
public class AddCandlesDto
{

  [ExistId<ent.Instrument>]
  public int instrumentId { get; set; }
  [ExistId<TimeFrame>]
  public int timeFrameId { get; set; }
  public DateTime From { get; set; }
  public DateTime Untill { get; set; }
  public IEnumerable<CandleDto> Candles { get; set; }

  [ValidatorAttribute]
  public class Validator : AbstractValidator<AddCandlesDto>
  {
    public Validator(IReadRepository<ent.Instrument> instrumentRep, IReadRepository<TimeFrame> timeFrameRep)
    {

      RuleFor(e => e.instrumentId)
        .Must(e => instrumentRep.ContainIdAsync(e).Result).WithEventId(ValidationEvents.IdNotFoundEvent);
      RuleFor(e => e.timeFrameId)
        .Must(e => timeFrameRep.ContainIdAsync(e).Result).WithEventId(ValidationEvents.IdNotFoundEvent)
        .DependentRules(() =>
        {
          RuleFor(e => e).Custom(
            (dto, context) =>
            {
              var validator = new CandleDto.Validator(timeFrameRep.GetByIdAsync(dto.timeFrameId).Result.EnumId);
              RuleForEach(e => e.Candles).ChildRules(e => e.RuleFor(e => e).SetValidator(validator));
            });
        });
      RuleFor(e => e.From)
        .LessThan(e => e.Untill).WithEventId(ValidationEvents.FromGeUntillEvent);
    }
  }
}