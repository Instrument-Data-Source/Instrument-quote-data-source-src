using FluentValidation;
using FluentValidation.Results;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Validator.Instrument;

public class NewInstrumentRequestDtoValidator : AbstractValidator<NewInstrumentRequestDto>
{
  public NewInstrumentRequestDtoValidator()
  {
    RuleFor(e => e.Name).NotNull().SetValidator(e => new NameValidator());
    RuleFor(e => e.Code).NotNull().SetValidator(e => new CodeValidator());
    RuleFor(e => e.TypeId).SetValidator(e => new TypeValidator());
  }
}

public static class NewInstrumentRequestDtoValidatorBuilder
{
  public static ValidationResult Validate(this NewInstrumentRequestDto dto)
  {
    return new NewInstrumentRequestDtoValidator().Validate(dto);
  }
  public static bool IsValid(this NewInstrumentRequestDto dto, out ValidationResult validationResult)
  {
    validationResult = dto.Validate();
    return validationResult.IsValid;
  }
}