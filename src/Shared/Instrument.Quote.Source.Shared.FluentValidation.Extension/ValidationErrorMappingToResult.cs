using Microsoft.Extensions.Logging;
using FluentValidation;
using FluentValidation.Results;
using Ardalis.Result;

namespace Instrument.Quote.Source.Shared.FluentValidation.Extension;
public static class ValidationErrorMappingToResult
{
  public static List<ValidationError> ToErrorList(this List<ValidationFailure> validationErrors)
  {
    return validationErrors.Select(e => new ValidationError()
    {
      Identifier = e.PropertyName,
      ErrorCode = e.ErrorCode,
      ErrorMessage = e.ErrorMessage,
      Severity = (ValidationSeverity)e.Severity
    }).ToList();
  }
}