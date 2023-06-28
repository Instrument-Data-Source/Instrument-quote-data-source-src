using System.ComponentModel.DataAnnotations;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Validator.Attribute;
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
class CandleInPeriodAttribute : ValidationAttribute
{
  public override bool RequiresValidationContext => true;

  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    var candles = (IEnumerable<Candle>)value!;
    var obj = (LoadedPeriod)validationContext.ObjectInstance;

    if (candles.All(c => c.DateTime >= obj.FromDate && c.DateTime < obj.UntillDate))
    {
      return ValidationResult.Success;
    }

    return new ValidationResult($"{nameof(Candle)} {nameof(Candle.DateTime)} must be >= {nameof(obj.FromDate)} and < {nameof(obj.UntillDate)}", new string[] { validationContext.MemberName! });
  }
}