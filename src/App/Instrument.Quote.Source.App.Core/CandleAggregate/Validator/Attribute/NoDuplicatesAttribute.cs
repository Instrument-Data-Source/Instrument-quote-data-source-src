using System.ComponentModel.DataAnnotations;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Validator.Attribute;
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
class NoDuplicatesAttribute : ValidationAttribute
{
  public override bool RequiresValidationContext => false;

  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    var candles = (IEnumerable<Candle>)value;

    var dates = candles.Select(c => c.DateTime).ToArray();
    if (dates.Distinct().Count() != dates.Count())
      return new ValidationResult($"{nameof(LoadedPeriod.Candles)} must contain only unique dates", new string[] { validationContext.MemberName! });

    return ValidationResult.Success;
  }
}