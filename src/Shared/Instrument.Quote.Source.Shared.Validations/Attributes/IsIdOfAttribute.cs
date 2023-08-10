using System.ComponentModel.DataAnnotations;
using Instrument.Quote.Source.Shared.Kernal.DataBase;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Instrument.Quote.Source.Shared.Validations.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class IsIdOfAttribute<TEntity> : ValidationAttribute where TEntity : EntityBase
{
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    if (value is not int idValue)
      return new ValidationResult("Is nod id value", new[] { validationContext.DisplayName });

    var entityRep = validationContext.GetRequiredService<IReadRepository<TEntity>>();

    if (entityRep.ContainId(idValue))
      return ValidationResult.Success;

    return new ValidationResult($"Entity with ID {idValue} doesn't exist", new[] { validationContext.DisplayName });
  }
}