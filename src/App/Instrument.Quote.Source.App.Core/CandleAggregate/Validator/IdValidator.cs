using FluentValidation;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Validator;

public class IdValidator<TEntity> : AbstractValidator<int> where TEntity: EntityBase
{

  public IdValidator(IReadRepository<TEntity> entRep)
  {
    RuleFor(e => e).Must(e => entRep.ContainId(e)).WithMessage("Id not found");
    RuleFor(e => e).GreaterThan(0).WithMessage("Id must > 0");
  }
}