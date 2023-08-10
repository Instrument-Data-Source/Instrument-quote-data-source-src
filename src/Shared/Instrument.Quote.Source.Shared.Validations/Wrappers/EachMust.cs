using System.ComponentModel.DataAnnotations;

namespace Instrument.Quote.Source.Shared.Validations.Wrapper;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
public class EachMust<TEachValidationAttribute> : Each<TEachValidationAttribute> where TEachValidationAttribute : ValidationAttribute
{
  // private readonly string idName;
  public EachMust(TEachValidationAttribute eachValidationAttribute) : base(true, eachValidationAttribute)
  {
    //this.idName = idName;
  }
  public EachMust(params object[] eachValidationAtrCostrPar) : base(true, eachValidationAtrCostrPar)
  {
    //this.idName = idName;
  }

}
