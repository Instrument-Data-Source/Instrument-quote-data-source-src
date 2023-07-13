//using FluentValidation;

using System.ComponentModel.DataAnnotations;
using Ardalis.Result;
using FluentValidation;
using Instrument.Quote.Source.Shared.FluentValidation.Extension;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Model;
public partial class LoadedPeriod
{
  /// <summary>
  /// Extend period by new period
  /// </summary>
  /// <param name="newPeriod"></param>
  /// <returns>Added period</returns>
  public LoadedPeriod Extend(LoadedPeriod newPeriod)
  {
    return _Extend(newPeriod, true).Value;
  }

  /// <summary>
  /// Extend period by new period
  /// </summary>
  /// <param name="newPeriod"></param>
  /// <returns>Added period</returns>
  public Result<LoadedPeriod> TryExtend(LoadedPeriod newPeriod)
  {
    return _Extend(newPeriod, false);
  }
  public Result<LoadedPeriod> _Extend(LoadedPeriod newPeriod, bool throwError)
  {
    var validationRes = new ExtensionPeriodLoadedPeriodValidator(this).Validate(newPeriod, op =>
    {
      if (throwError)
        op.ThrowOnFailures();
    });

    if (!validationRes.IsValid)
      return validationRes!.ToResult();

    if (newPeriod.FromDate < FromDate)
      FromDate = newPeriod.FromDate;
    if (newPeriod.UntillDate > UntillDate)
      UntillDate = newPeriod.UntillDate;

    _candles.AddRange(newPeriod.Candles);
    return newPeriod;
  }
  
  public LoadedPeriod AddCandles(IEnumerable<Candle> candles)
  {
    return _AddCandles(candles, true).Value;
  }

  public Result<LoadedPeriod> TryAddCandles(IEnumerable<Candle> candles)
  {
    return _AddCandles(candles, false);
  }

  private Result<LoadedPeriod> _AddCandles(IEnumerable<Candle> candles, bool throwValidate)
  {
    var validResult = new CandleArrForPeriodValidator(this).Validate(candles, opt =>
    {
      if (throwValidate)
        opt.ThrowOnFailures();
    });

    if (!validResult.IsValid)
      return validResult!.ToResult();

    this._candles.AddRange(candles);

    return Result.Success(this);
  }
}
/*
public class ExtendValidator : AbstractValidator<LoadedPeriod>
{
  public ExtendValidator(LoadedPeriod basePeriod)
  {
    RuleFor(e => e.UntillDate).Equal(basePeriod.FromDate).When(e => e.FromDate != basePeriod.UntillDate)
      .WithMessage("New period doesn't connected to the left");
    RuleFor(e => e.FromDate).Equal(basePeriod.UntillDate).When(e => e.UntillDate != basePeriod.FromDate)
      .WithMessage("New period doesn't connected to the right");
    RuleFor(e => e.Id).Equal(0)
      .WithMessage("ID of new period must be undefined");

    RuleFor(e => e).Must(e =>
    {
      var props = e.GetType().GetProperties();
      foreach (var prop in props)
      {
        var attributes = prop.GetCustomAttributes();
      }
    }
  }
}
*/
