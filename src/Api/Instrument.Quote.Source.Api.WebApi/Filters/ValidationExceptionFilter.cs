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
    if (context.Exception is ValidationException exception)
    {
      context.Result = new ObjectResult(exception.Message) //new BadRequestDto(exception)
      {
        StatusCode = StatusCodes.Status508LoopDetected,
      };
      context.ExceptionHandled = true;// BadRequest()
    }
  }
}