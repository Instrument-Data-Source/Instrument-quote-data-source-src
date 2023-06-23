using System.ComponentModel.DataAnnotations;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Validator.Attribute;
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
class NotEmptyAttribute : ValidationAttribute
{
  public override bool RequiresValidationContext => false;

  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    var candles = (IEnumerable<Candle>)value;

    if (candles.Count() > 0)
      return ValidationResult.Success;

    return new ValidationResult($"{nameof(LoadedPeriod.Candles)} must contain at least 1 element", new string[] { validationContext.MemberName! });


  }
}