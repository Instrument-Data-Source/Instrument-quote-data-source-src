using FluentValidation;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.CandleAggregate.Validator;
using Instrument.Quote.Source.App.Core.CandleAggregate.Validator.Attribute;
using Instrument.Quote.Source.App.Core.Event;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.FluentValidation.Extension;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
public class NewPeriodDto : LoadedPeriod.IPayload
{
  public int InstrumentId { get; set; }
  public int TimeFrameId { get; set; }
  public DateTime FromDate { get; set; }
  public DateTime UntillDate { get; set; }
  public IEnumerable<CandleDto> Candles { get; set; }


  public class Validator : AbstractValidator<NewPeriodDto>
  {
    public Validator(IReadRepository<ent.Instrument> instrumentRep, IReadRepository<TimeFrame> timeFrameRep)
    {
      RuleFor(e => e.InstrumentId).SetValidator(new IdValidator<ent.Instrument>(instrumentRep));
      RuleFor(e => e.TimeFrameId).SetValidator(new IdValidator<TimeFrame>(timeFrameRep));
      RuleFor(e => e.FromDate).SetValidator((dto) => new LoadedPeriod.FromDateValidator(dto));
      RuleFor(e => e.UntillDate).SetValidator((dto) => new LoadedPeriod.UntillDateValidator(dto));
      RuleForEach(e => e.Candles).SetValidator(dto => new LoadedPeriod.CandlePayloadForPeriodValidator(dto));
    }
  }
}