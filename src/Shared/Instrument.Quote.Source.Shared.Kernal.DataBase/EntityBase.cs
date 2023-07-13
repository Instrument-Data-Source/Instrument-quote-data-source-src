using System.ComponentModel.DataAnnotations;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;

namespace Instrument.Quote.Source.Shared.Kernal.DataBase;

public abstract class EntityBase
{
  [Key]
  public int Id { get; protected set; }
}

public abstract class EntityBaseExt<TEntity> : EntityBase
{

  public bool IsValid(out FluentValidation.Results.ValidationResult result)
  {
    result = ValidateSelf((o) => { });
    return result.IsValid;
  }

  public void Validate()
  {
    ValidateSelf((o) => o.ThrowOnFailures());
  }
  protected abstract FluentValidation.Results.ValidationResult ValidateSelf(Action<ValidationStrategy<TEntity>> options);
  /*
    protected void ThrowValidationResults(ICollection<ValidationResult> validationResults, string validatedObjectName)
    {
      throw new ValidationException($"{validatedObjectName} invalid",
          new AggregateException(
            "Validation errors",
            validationResults.Select(v => new ValidationException(v, null, null))
          )
        );
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
    */
}
