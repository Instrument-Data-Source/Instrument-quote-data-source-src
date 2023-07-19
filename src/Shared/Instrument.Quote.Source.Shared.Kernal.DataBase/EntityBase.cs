using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;

namespace Instrument.Quote.Source.Shared.Kernal.DataBase;

public abstract class EntityBase
{
  [Key]
  public int Id { get; protected set; }
}

public abstract class EntityBaseValidation : EntityBase
{
  private IServiceProvider? _sp;
  public void SetServiceProvider(IServiceProvider sp)
  {
    this._sp = sp;
  }
  private ValidationContext CreateValidationContext()
  {
    return new ValidationContext(this, _sp, new Dictionary<object, object?>());
  }
  public bool IsValid(out ICollection<System.ComponentModel.DataAnnotations.ValidationResult>? validationResults)
  {
    var context = CreateValidationContext();
    validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
    return Validator.TryValidateObject(this, context, validationResults);
  }

  public void Validate()
  {
    var context = CreateValidationContext();
    Validator.ValidateObject(this, context, true);
  }
  /*
   public void ValidateProps(Expression<Func<object>> validatedProps)
   {
     var context = new ValidationContext(this);
     var names = GetNames(validatedProps);
     //foreach (var propObj in validatedProps.Distinct())
     //{
     //  context.MemberName = 
     //  Validator.ValidateProperty(propObj, context);
     //}
   }

   private string GetName<T>(Expression<Func<T>> propertyExpression)
   {
     var memberExpression = (MemberExpression)propertyExpression.Body;
     var propertyName = memberExpression.Member.Name;

     return propertyName;
   }

   private IEnumerable<string> GetNames(Expression<Func<object>> propertyExpression)
   {
     if (propertyExpression.Body is NewExpression newExpression)
     {
       return newExpression.Members.Select(member => member.Name);
     }
     else if (propertyExpression.Body is MemberExpression memberExpression)
     {
       return new[] { memberExpression.Member.Name };
     }

     throw new ArgumentException("Invalid property expression");
   }
   */
}

public abstract class EntityBaseFluentValidation<TEntity> : EntityBase
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
