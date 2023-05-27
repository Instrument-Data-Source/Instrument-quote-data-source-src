using System.Reflection;
using Instrument.Quote.Source.App;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.InitApp();

builder.Services.Configure<KestrelServerOptions>(options =>
{
  options.Limits.MaxRequestBodySize = int.MaxValue; // if don't set default value is: 30 MB
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
  options.SwaggerDoc("v1", new OpenApiInfo
  {
    Version = "v1.2",
    Title = "Instrument Quote Source API",
    Description = "An ASP.NET Core Web API service for getting information about instrument quotes",
    Contact = new OpenApiContact
    {
      Name = "InsonusK",
      Url = new Uri("https://github.com/Instrument-Data-Source/Instrument-quote-data-source-srv")
    },
    //TermsOfService = new Uri("https://example.com/terms"),

    //License = new OpenApiLicense
    //{
    //  Name = "Example License",
    //  Url = new Uri("https://example.com/license")
    //}
  });
  var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
  options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI(options =>
{
  options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
  options.RoutePrefix = string.Empty;
});
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
