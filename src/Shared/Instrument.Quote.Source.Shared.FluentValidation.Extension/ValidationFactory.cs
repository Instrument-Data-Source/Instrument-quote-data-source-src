using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Instrument.Quote.Source.Shared.FluentValidation.Extension;

public class ValidatorFactor
{
  private readonly IServiceProvider sp;

  public ValidatorFactor(IServiceProvider sp)
  {
    this.sp = sp;
  }

  public IValidator<T> GetValidator<T>()
  {
    var validator = sp.GetService<IValidator<T>>();
    return validator ?? new EmptyValidator<T>();
  }

  public class EmptyValidator<T> : AbstractValidator<T>
  {
  }

}
[AttributeUsage(AttributeTargets.Class)]
public class ValidatorAttribute : Attribute
{
  // Optionally, you can add properties or fields to the attribute
  public ServiceLifetime lifetime { get; }
  public ValidatorAttribute(ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
  {
    this.lifetime = serviceLifetime;
  }
}
public static class ValidatorRegistration
{
  public static IServiceCollection RegisterValidators(this IServiceCollection sc)
  {
    // Get the current assembly
    Assembly assembly = Assembly.GetCallingAssembly();

    // Get all types in the assembly that have the CustomAttribute applied
    var typesWithAttribute = assembly.GetTypes()
        .Where(type => type.GetCustomAttribute<ValidatorAttribute>() != null);

    // Process the types
    foreach (Type type in typesWithAttribute)
    {
      var validatorAttribute = type.GetCustomAttribute<ValidatorAttribute>()!;
      foreach (var intf in type.GetInterfaces())
      {
        if (intf.IsGenericType &&
            intf.GetGenericTypeDefinition() == typeof(IValidator<>))
          sc.Add(new ServiceDescriptor(intf, type, validatorAttribute.lifetime));
      }
    }
    return sc;
  }

  private static IServiceCollection AddScopedValidator<TType, TValidator>(this IServiceCollection sc) where TValidator : class, IValidator<TType>
  {
    sc.AddScoped<IValidator<TType>, TValidator>();
    return sc;
  }

}