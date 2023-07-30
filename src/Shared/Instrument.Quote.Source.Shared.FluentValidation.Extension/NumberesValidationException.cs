using System.ComponentModel.DataAnnotations;

namespace Instrument.Quote.Source.Shared.FluentValidation.Extension;

public class NumberesValidationResult : ValidationResult
{
  public readonly string ErrorCode;

  public NumberesValidationResult(string ErrorCode, string? errorMessage) : this(ErrorCode, new ValidationResult(errorMessage))
  {
  }

  public NumberesValidationResult(string ErrorCode, string? errorMessage, IEnumerable<string>? memberNames) : this(ErrorCode, new ValidationResult(errorMessage, memberNames))
  {
  }

  protected NumberesValidationResult(string ErrorCode, ValidationResult validationResult) : base(validationResult)
  {
    this.ErrorCode = ErrorCode;
  }
}
