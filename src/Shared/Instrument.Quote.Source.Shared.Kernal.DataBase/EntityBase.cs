using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Instrument.Quote.Source.Shared.Kernal.DataBase;

public abstract class EntityBase
{
  [Key]
  public int Id { get; protected set; }
}

public abstract class EntityBaseExt : EntityBase
{
  public bool IsValid(out ICollection<ValidationResult> validationResults, ValidationContext? validationContext = null)
  {
    if (validationContext == null)
      validationContext = new ValidationContext(this);
    validationResults = new List<ValidationResult>();

    return System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, validationContext, validationResults, true);
  }

  public void Validate()
  {
    if (!IsValid(out var validationResults))
    {
      throw new ValidationException($"{this.GetType().Name} invalid",
        new AggregateException(
          "Validation errors",
          validationResults.Select(v => new ValidationException(v, null, null))
        )
      );
    }
  }

  protected void SetProperty<T>(T value, string propName, ref T prop)
  {
    var validationResults = new List<ValidationResult>();
    if (!TrySetProperty(value, propName, ref prop, validationResults))
    {
      throw new System.ComponentModel.DataAnnotations.ValidationException(
        "DateTime invalid",
        new AggregateException(
          "Validation errors",
          validationResults.Select(v => new System.ComponentModel.DataAnnotations.ValidationException(v, null, null))
        )
      );
    }
  }

  protected void TrySetProperty<T>(T value,
    string propName,
    ref T prop,
    ref bool isValid,
    ICollection<ValidationResult>? validationResults = null,
    ValidationContext? validationContext = null)
  {
    var fieldIsValid = TrySetProperty(value, propName, ref prop, validationResults, validationContext);
    isValid = isValid && fieldIsValid;
  }

  protected bool TrySetProperty<T>(T value,
    string propName,
    ref T prop,
    ICollection<ValidationResult>? validationResults = null,
    ValidationContext? validationContext = null)
  {
    if (validationContext == null)
      validationContext = new ValidationContext(this);
    if (validationResults == null)
      validationResults = new List<ValidationResult>();

    validationContext.MemberName = propName;
    var fieldIsValid = System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, validationContext, validationResults);
    validationContext.MemberName = null;

    if (fieldIsValid) prop = value;
    return fieldIsValid;
  }
}
