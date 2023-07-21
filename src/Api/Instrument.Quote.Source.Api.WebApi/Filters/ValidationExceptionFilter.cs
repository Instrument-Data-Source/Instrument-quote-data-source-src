using System.ComponentModel.DataAnnotations;
using Instrument.Quote.Source.Api.WebApi.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Instrument.Quote.Source.Api.WebApi;
public class ValidationExceptionFilter : IActionFilter, IOrderedFilter
{
  public int Order { get; } = int.MaxValue - 10;

  public void OnActionExecuting(ActionExecutingContext context)
  {
  }

  public void OnActionExecuted(ActionExecutedContext context)
  {
    if (context.Exception != null)
    {
      if (context.Exception is ValidationException exception)
        context.Result = new BadRequestObjectResult(exception.Message);
      else
        context.Result = new StatusCodeResult(500);
      context.ExceptionHandled = true;
    }
  }
}