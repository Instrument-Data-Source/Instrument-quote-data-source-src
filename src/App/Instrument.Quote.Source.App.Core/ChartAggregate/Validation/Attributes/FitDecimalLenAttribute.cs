using System.ComponentModel.DataAnnotations;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
using Instrument.Quote.Source.App.Shared.Validation;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Validation.Attributes;
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class PriceFitDecimalLenAttribute : FitDecimalLenAttribute
{
  public PriceFitDecimalLenAttribute(string propIdPropName) : base(propIdPropName, false) { }

  public PriceFitDecimalLenAttribute(string propIdPropName, bool useContextItems) : base(propIdPropName, useContextItems)
  {
  }

  protected override bool CheckDecimal(decimal decimalValue, IDecimalPartLongChecker checker)
  {
    return checker.IsPriceDecPartFit(decimalValue);
  }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class VolumeFitDecimalLenAttribute : FitDecimalLenAttribute
{
  public VolumeFitDecimalLenAttribute(string propIdPropName) : base(propIdPropName, false) { }

  public VolumeFitDecimalLenAttribute(string propIdPropName, bool useContextItems) : base(propIdPropName, useContextItems)
  {
  }

  protected override bool CheckDecimal(decimal decimalValue, IDecimalPartLongChecker checker)
  {
    return checker.IsVolumeDecPartFit(decimalValue);
  }
}

public abstract class FitDecimalLenAttribute : absDependableOfPropAttribute
{
  public FitDecimalLenAttribute(string propIdPropName) : base(propIdPropName, null)
  {
  }
  public FitDecimalLenAttribute(string propIdPropName, bool useContextItems) : base(propIdPropName, useContextItems)
  {
  }
  public override bool RequiresValidationContext => true;
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    var instrumentId = getId(validationContext);
    var srv = validationContext.GetService<IReadRepository<ent.Instrument>>();
    if (srv == null)
      throw new NullReferenceException($"Validation context doesn't contain service {nameof(IReadRepository<ent.Instrument>)}<{nameof(ent.Instrument)}>");

    var instrument = srv.TryGetByIdAsync(instrumentId).Result;
    if (instrument == null)
      return new ValidationResult("Decimal len cann't be checked, instrument not found", new string[] { validationContext.MemberName! });

    var checkerFactory = validationContext.GetService<IDecimalPartLongCheckerFactory>()!;
    if (checkerFactory == null)
      throw new NullReferenceException($"Validation context doesn't contain service {nameof(IDecimalPartLongCheckerFactory)}");

    var checker = checkerFactory.GetChecker(instrument);

    var checkingValue = decimal.Parse(value!.ToString()!);

    var isValid = CheckDecimal(checkingValue, checker);

    if (!isValid)
      return new ValidationResult("Decimal part of doesn't fit instrument configuration", new string[] { validationContext.MemberName! });

    return ValidationResult.Success;
  }

  protected abstract bool CheckDecimal(decimal decimalValue, IDecimalPartLongChecker checker);

}