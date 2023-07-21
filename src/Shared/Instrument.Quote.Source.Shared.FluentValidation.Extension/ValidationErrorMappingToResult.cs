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
  /*
    public static Result<TOut> Repack<TIn, TOut>(this Result<TIn> result)
    {
      switch (result.Status)
      {
        case ResultStatus.Unauthorized:
          return Result.Unauthorized();
        case ResultStatus.NotFound:
          return Result.NotFound(result.Errors.ToArray());
        case ResultStatus.Invalid:
          return Result.Invalid(result.ValidationErrors);
        case ResultStatus.Forbidden:
          return Result.Forbidden();
        case ResultStatus.Error:
          return Result.Error(result.Errors.ToArray());
        case ResultStatus.Conflict:
          return Result.Conflict(result.Errors.ToArray());
        default:
          throw new ApplicationException("Not supported repack of result");
      }
    }
    */
  public static Result<TOut> Repack<TOut>(this IResult result)
  {
    switch (result.Status)
    {
      case ResultStatus.Unauthorized:
        return Result.Unauthorized();
      case ResultStatus.NotFound:
        return Result.NotFound(result.Errors.ToArray());
      case ResultStatus.Invalid:
        return Result.Invalid(result.ValidationErrors);
      case ResultStatus.Forbidden:
        return Result.Forbidden();
      case ResultStatus.Error:
        return Result.Error(result.Errors.ToArray());
      case ResultStatus.Conflict:
        return Result.Conflict(result.Errors.ToArray());
      default:
        throw new ApplicationException("Not supported repack of result");
    }
  }
}