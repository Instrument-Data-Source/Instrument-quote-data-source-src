namespace Instrument.Quote.Source.Api.WebApi.Tools;
public static class ApiServiceRegister
{
  public static IServiceCollection RegisterApiTool(this IServiceCollection sc)
  {
    sc.AddScoped<ParameterParser>();
    return sc;
  }
}