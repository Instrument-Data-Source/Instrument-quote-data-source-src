namespace Instrument.Quote.Source.App.Core.CandleAggregate.Validator.Attribute;

using System.ComponentModel.DataAnnotations;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.Extensions.DependencyInjection;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
class ExistIdAttribute<TEntity> : ValidationAttribute where TEntity : EntityBase
{
  public override bool RequiresValidationContext => true;

  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    var id = (int)value!;
    var entityRep = validationContext.GetService<IReadRepository<TEntity>>()!;


    if (!entityRep.ContainId(id))
    {
      return new ValidationResult($"Id must exist in repository", new string[] { validationContext.MemberName! });
    }

    return ValidationResult.Success;
  }
}