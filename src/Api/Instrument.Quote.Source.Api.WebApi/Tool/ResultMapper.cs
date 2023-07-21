using Ardalis.Result;
using Instrument.Quote.Source.Api.WebApi.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Instrument.Quote.Source.Api.WebApi.Tools;
public static class ResultMapper
{
  public static ActionResult MapToActionResult(this Result result)
  {
    switch (result.Status)
    {
      case ResultStatus.Ok:
        return new OkResult();
      default:
        return result.MapFailToActionResult();
    }
  }

  public static ActionResult<TRetModel> MapToActionResult<TRetModel>(this Result<TRetModel> result)
  {
    switch (result.Status)
    {
      case ResultStatus.Ok:
        return new OkObjectResult(result.Value);
      default:
        return result.MapFailToActionResult();
    }
  }

  public static ActionResult MapFailToActionResult(this Ardalis.Result.IResult result)
  {
    switch (result.Status)
    {
      case ResultStatus.NotFound:
        return new NotFoundObjectResult(result.Errors);
      case ResultStatus.Invalid:
        return new BadRequestObjectResult(new BadRequestDto(result.ValidationErrors));
      default:
        throw new NotImplementedException($"Mapping to result status {result.Status} hasn't been realised");
    }
  }
}