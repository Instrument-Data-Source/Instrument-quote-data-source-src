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
}