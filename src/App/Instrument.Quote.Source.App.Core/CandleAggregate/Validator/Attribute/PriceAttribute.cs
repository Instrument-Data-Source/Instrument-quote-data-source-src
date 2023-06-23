using System.ComponentModel.DataAnnotations;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Validator.Attribute;

public abstract class PriceAttribute : ValidationAttribute
{
  public override bool RequiresValidationContext => true;
  protected bool GetCandle(ValidationContext validationContext, out Candle? candle, out ValidationResult? validationResult)
  {
    if ((validationContext.ObjectInstance is Candle candleInstance))
    {
      candle = candleInstance;
      validationResult = null;
      return true;
    }
    else
    {
      validationResult = new ValidationResult($"{validationContext.MemberName} must be part of Candle", new string[] { validationContext.MemberName! });
      candle = null;
      return false;
    }

  }
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class PriceGeLowAttribute : PriceAttribute
{
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    if (!GetCandle(validationContext, out var candle, out var validationResult))
      return validationResult;

    if (value is int price && price >= candle!.LowStore)
      return ValidationResult.Success;

    return new ValidationResult($"{validationContext.MemberName} must be GE Low", new string[] { validationContext.MemberName! });
  }
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class PriceLeHighAttribute : PriceAttribute
{
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    if (!GetCandle(validationContext, out var candle, out var validationResult))
      return validationResult;

    if (value is int price && price <= candle.HighStore)
      return ValidationResult.Success;

    return new ValidationResult($"{validationContext.MemberName} must be LE High", new string[] { validationContext.MemberName! });
  }
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class HighIsMaxAttribute : PriceAttribute
{
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    if (!GetCandle(validationContext, out var candle, out var validationResult))
      return validationResult;

    if (value is int price && price >= new[] { candle!.OpenStore, candle.LowStore, candle.CloseStore }.Max())
      return ValidationResult.Success;

    return new ValidationResult($"{validationContext.MemberName} must be GE other prices", new string[] { validationContext.MemberName! });
  }
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class LowIsMinAttribute : PriceAttribute
{
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    if (!GetCandle(validationContext, out var candle, out var validationResult))
      return validationResult;

    if (value is int price && price <= new[] { candle!.OpenStore, candle.HighStore, candle.CloseStore }.Min())
      return ValidationResult.Success;

    return new ValidationResult($"{validationContext.MemberName} must be LE other prices", new string[] { validationContext.MemberName! });
  }
}