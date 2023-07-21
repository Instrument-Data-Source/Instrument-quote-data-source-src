using System.ComponentModel.DataAnnotations;
using Ardalis.Result;

namespace Instrument.Quote.Source.Api.WebApi.Dto;

public class ErrorDto
{
  public string ErrorMessage { get; set; }
  public string ErrorCode { get; set; }
  public string Severity { get; set; }
}

public class BadRequestDto //: IErrorDto
{
/*
  public BadRequestDto(ValidationException validationException)
  {
    ErrorMessage = validationException.Message;
    if (validationException.ValidationResult != null)
    {
      ErrorMessage = validationException.ValidationResult.ErrorMessage!;
    }
    if (validationException != null)
      if (validationException.)
  }*/

  public BadRequestDto(List<ValidationError> validationErrors)
  {
    foreach (var error in validationErrors)
    {
      var field_errors = errors.GetValueOrDefault(error.Identifier, new List<ErrorDto>());
      field_errors.Add(new ErrorDto()
      {
        ErrorCode = error.ErrorCode,
        ErrorMessage = error.ErrorMessage,
        Severity = Enum.GetName<ValidationSeverity>(error.Severity)!
      });
      errors[error.Identifier] = field_errors;
    }
  }

  public Dictionary<string, List<ErrorDto>> errors { get; set; } = new Dictionary<string, List<ErrorDto>>();
}