using FluentValidation;
using FluentValidation.Results;
using Ardalis.Result;

namespace Instrument.Quote.Source.Shared.FluentValidation.Extension;
public static class ValidationErrorMappingToResult
{
  public static Result ToResult(this ValidationResult result)
  {
    if (result.IsValid)
      return Result.Success();
    return Result.Invalid(result.Errors.ToResultErrorList());
  }
  public static List<ValidationError> ToResultErrorList(this IEnumerable<ValidationFailure> validationErrors)
  {
    return validationErrors.Select(e => new ValidationError()
    {
      Identifier = e.PropertyName,
      ErrorCode = e.ErrorCode,
      ErrorMessage = e.ErrorMessage,
      Severity = (ValidationSeverity)e.Severity
    }).ToList();
  }

  public static List<ValidationError> ToResultErrorList(this ICollection<System.ComponentModel.DataAnnotations.ValidationResult> validationErrors)
  {
    return validationErrors.Select(e => new ValidationError()
    {
      Identifier = string.Join(", ", e.MemberNames),
      //ErrorCode = e.ErrorCode,
      ErrorMessage = e.ErrorMessage,
      //Severity = (ValidationSeverity)e
    }).ToList();
  }
  public static Result ToResult(this ValidationException exception)
  {
    return Result.Invalid(exception.ToResultErrorList());
  }
  public static List<ValidationError> ToResultErrorList(this ValidationException exception)
  {
    return exception.Errors.Select(vf => new ValidationError()
    {
      Identifier = vf.PropertyName,
      ErrorCode = vf.ErrorCode,
      ErrorMessage = vf.ErrorMessage,
      Severity = (ValidationSeverity)vf.Severity
    }).ToList();
  }
}