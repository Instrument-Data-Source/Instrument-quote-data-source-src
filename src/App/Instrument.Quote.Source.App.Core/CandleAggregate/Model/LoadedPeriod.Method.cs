//using FluentValidation;

using System.ComponentModel.DataAnnotations;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Model;
public partial class LoadedPeriod
{
  /// <summary>
  /// Extend period by new period
  /// </summary>
  /// <param name="newPeriod"></param>
  /// <returns>Self instance</returns>
  public LoadedPeriod Extend(LoadedPeriod newPeriod)
  {
    //new ExtendValidator(this).ValidateAndThrow(newPeriod);
    if (!ValidateExtedPeriod(newPeriod, out var validationResults))
      ThrowValidationResults(validationResults, $"New {nameof(LoadedPeriod)}");

    if (newPeriod.FromDate < FromDate)
      FromDate = newPeriod.FromDate;
    if (newPeriod.UntillDate > UntillDate)
      UntillDate = newPeriod.UntillDate;

    _candles.AddRange(newPeriod.Candles);
    Validate();
    return this;
  }

  private bool ValidateExtedPeriod(LoadedPeriod newPeriod, out ICollection<ValidationResult> validationResults)
  {
    validationResults = new List<ValidationResult>();

    if (newPeriod.FromDate != this.UntillDate && newPeriod.UntillDate != this.FromDate)
      validationResults.Add(new ValidationResult("New period is not connected", new[] { nameof(this.FromDate), nameof(this.UntillDate) }));
    if (newPeriod.Id > 0)
      validationResults.Add(new ValidationResult("New period has Id", new[] { nameof(Id) }));

    return validationResults.Count == 0;
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
