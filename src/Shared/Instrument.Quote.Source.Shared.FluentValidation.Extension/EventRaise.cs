using Microsoft.Extensions.Logging;
using FluentValidation;
namespace Instrument.Quote.Source.Shared.FluentValidation.Extension;
public static class EventRaise
{
  public static void WithEventId<T, TProperty>(this IRuleBuilderOptions<T, TProperty> ruleBuilder, EventId eventId)
  {
    ruleBuilder.WithErrorCode(eventId.Id.ToString()).WithMessage(eventId.Name);
  }
}