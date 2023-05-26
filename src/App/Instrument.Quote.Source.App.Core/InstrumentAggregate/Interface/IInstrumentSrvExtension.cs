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
      return await service.RemoveAsync(result.Value.Id, cancellationToken);

    if (Int32.TryParse(instrumentStr, out int instrumentId))
      return await service.RemoveAsync(instrumentId, cancellationToken);

    return Result.NotFound();
  }
  /// <summary>
  /// Get instrument by code
  /// </summary>
  /// <param name="instrumentCode">Instrument Code</param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public static async Task<Result<InstrumentResponseDto>> GetInstrumentByIdOrCodeAsync(this IInstrumentSrv service, string instrumentStr, CancellationToken cancellationToken = default)
  {
    var result = await service.GetByAsync(instrumentStr, cancellationToken);

    if (result.IsSuccess)
      return result;

    if (Int32.TryParse(instrumentStr, out int instrumentId))
      return await service.GetByAsync(instrumentId);

    return Result.NotFound();
  }
}