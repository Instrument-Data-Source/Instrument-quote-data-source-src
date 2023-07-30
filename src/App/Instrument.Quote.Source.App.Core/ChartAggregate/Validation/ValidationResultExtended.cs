using System.ComponentModel.DataAnnotations;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Validation;

public class ValidationResultExtended : ValidationResult
{
  public string? Code { get; set; }
  public IEnumerable<ValidationResult>? SubResult { get; }
  public readonly bool ContainSubResult;

  public ValidationResultExtended(string? code, string? errorMessage, IEnumerable<string>? memberNames, IEnumerable<ValidationResult>? subResult) :
                                    base(errorMessage, memberNames)
  {
    Code = code;
    SubResult = subResult;
    ContainSubResult = SubResult != null && SubResult.Count() > 0;
  }
  public ValidationResultExtended(string? code, string? errorMessage, string memberName, IEnumerable<ValidationResult>? subResult) :
                                    this(code, errorMessage, new[] { memberName }, subResult)
  {
  }
  public ValidationResultExtended(string? errorMessage, IEnumerable<string>? memberNames, IEnumerable<ValidationResult>? subResult = null) :
                                    this(null, errorMessage, memberNames, subResult)
  {
  }

  public ValidationResultExtended(string? errorMessage, string memberName, IEnumerable<ValidationResult>? subResult = null) :
                                    this(null, errorMessage, new[] { memberName }, subResult)
  {
  }

  protected ValidationResultExtended(ValidationResult validationResult) : base(validationResult)
  {
  }
}