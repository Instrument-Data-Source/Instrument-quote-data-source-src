using Ardalis.Result;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;

namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;

public static class IInstrumentSrvExtension
{
  /// <summary>
  /// Remove instrument from service
  /// </summary>
  /// <param name="instrumentStr">String contari Instrument Id or Instrument Code</param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public static async Task<Result> RemoveInstrumentByIdOrStrAsync(this IInstrumentSrv service, string instrumentStr, CancellationToken cancellationToken = default)
  {
    var result = await service.GetByAsync(instrumentStr, cancellationToken);

    if (result.IsSuccess)
      if (result.Value.Count() != 1)
        return Result.Error("Cann't define which instrument must be deleted");
      else
        return await service.RemoveAsync(result.Value.ElementAt(0).Id, cancellationToken);

    if (Int32.TryParse(instrumentStr, out int instrumentId))
      return await service.RemoveAsync(instrumentId, cancellationToken);

    return Result.NotFound(nameof(ent.Instrument));
  }
  /// <summary>
  /// Get instrument by code
  /// </summary>
  /// <param name="instrumentCode">Instrument Code</param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public static async Task<Result<InstrumentResponseDto>> GetInstrumentByIdOrCodeAsync(this IReadInstrumentSrv service, string instrumentStr, CancellationToken cancellationToken = default)
  {
    var result = await service.GetByAsync(instrumentStr, cancellationToken);

    if (result.IsSuccess)
      if (result.Value.Count() != 1)
        return Result.Error("Cann't define which instrument must be deleted");
      else
        return Result.Success(result.Value.ElementAt(0));

    if (Int32.TryParse(instrumentStr, out int instrumentId))
    {
      var idResult = await service.GetByAsync(instrumentId);
      return idResult;
    }
    return Result.NotFound(nameof(ent.Instrument));
  }
}