using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Instrument.Quote.Source.Api.WebApi;
public static class SwaggerGenOptionsInit
{
  public const string Version = "v2.1";
  public const string UserGroup = "User";
  public const string AdminGroup = "Admin";

  public static void Init(this SwaggerGenOptions options)
  {
    options.SwaggerDoc(UserGroup, new OpenApiInfo
    {
      Version = Version,
      Title = "Instrument Quote Source API",
      Description = "An ASP.NET Core Web API service for getting information about instrument quotes",
      Contact = new OpenApiContact
      {
        Name = "InsonusK",
        Url = new Uri("https://github.com/Instrument-Data-Source/Instrument-quote-data-source-srv")
      }
    });
    options.SwaggerDoc(AdminGroup, new OpenApiInfo
    {
      Version = Version,
      Title = "Instrument Quote Source API (administrator)",
      Description = "An ASP.NET Core Web API service for getting information about instrument quotes. Administrator tools to extend avaliable data",
      Contact = new OpenApiContact
      {
        Name = "InsonusK",
        Url = new Uri("https://github.com/Instrument-Data-Source/Instrument-quote-data-source-srv")
      }
    });
  }

  public static void Init(this SwaggerUIOptions options)
  {
    options.SwaggerEndpoint($"/swagger/{UserGroup}/swagger.json", UserGroup);
    options.SwaggerEndpoint($"/swagger/{AdminGroup}/swagger.json", AdminGroup);
  }
}