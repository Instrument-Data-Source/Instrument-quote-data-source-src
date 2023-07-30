using System.ComponentModel.DataAnnotations;
using Ardalis.Result;

namespace Instrument.Quote.Source.Shared.Result.Extension;

public static class ResultExtension
{
  public static Ardalis.Result.Result<TOut> Repack<TOut>(this IResult result)
  {
    switch (result.Status)
    {
      case ResultStatus.Unauthorized:
        return Ardalis.Result.Result.Unauthorized();
      case ResultStatus.NotFound:
        return Ardalis.Result.Result.NotFound(result.Errors.ToArray());
      case ResultStatus.Invalid:
        return Ardalis.Result.Result.Invalid(result.ValidationErrors);
      case ResultStatus.Forbidden:
        return Ardalis.Result.Result.Forbidden();
      case ResultStatus.Error:
        return Ardalis.Result.Result.Error(result.Errors.ToArray());
      case ResultStatus.Conflict:
        return Ardalis.Result.Result.Conflict(result.Errors.ToArray());
      default:
        throw new ApplicationException("Not supported repack of result");
    }
  }
  public static void Throw(this IResult result)
  {
    switch (result.Status)
    {
      case ResultStatus.Unauthorized:
        throw new UnauthorizedAccessException(getErrors(result));
      case ResultStatus.NotFound:
        throw new KeyNotFoundException(getErrors(result));
      case ResultStatus.Invalid:
        throw new ValidationException(string.Join(", ", result.ValidationErrors.Select(ve => $"{ve.Identifier}: {ve.Severity} {ve.ErrorCode} {ve.ErrorMessage}")));
      case ResultStatus.Forbidden:
        throw new ApplicationException(getErrors(result));
      case ResultStatus.Error:
        throw new ApplicationException(getErrors(result));
      case ResultStatus.Conflict:
        throw new ApplicationException(getErrors(result));
      default:
        throw new ApplicationException("Not supported repack of result");
    }
  }

  private static string getErrors(IResult result)
  {
    return string.Join(", ", result.Errors);
  }
}