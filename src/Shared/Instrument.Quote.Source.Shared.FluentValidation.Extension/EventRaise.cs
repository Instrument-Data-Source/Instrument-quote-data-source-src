using Microsoft.Extensions.Logging;
using FluentValidation;
namespace Instrument.Quote.Source.Shared.FluentValidation.Extension;
public static class EventRaise
{
  public static IRuleBuilderOptions<T, TProperty> WithEventId<T, TProperty>(this IRuleBuilderOptions<T, TProperty> ruleBuilder, EventId eventId)
  {
    return ruleBuilder.WithErrorCode(eventId.Id.ToString()).WithMessage(eventId.Name);
  }
}