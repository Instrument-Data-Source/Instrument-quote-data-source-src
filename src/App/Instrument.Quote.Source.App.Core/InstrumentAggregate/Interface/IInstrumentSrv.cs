using Ardalis.Result;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;

namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
public interface IReadInstrumentSrv
{/// <summary>
  /// Get all instruments
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  Task<IEnumerable<InstrumentResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
  /// <summary>
  /// Get instrument by code
  /// </summary>
  /// <param name="instrumentCode">Instrument Code</param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  Task<Result<InstrumentResponseDto>> GetByAsync(string instrumentCode, CancellationToken cancellationToken = default);
  /// <summary>
  /// Get instrument by Id
  /// </summary>
  /// <param name="instrumentId">Instrument Id</param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  Task<Result<InstrumentResponseDto>> GetByAsync(int instrumentId, CancellationToken cancellationToken = default);
}
public interface IInstrumentSrv:IReadInstrumentSrv
{
  /// <summary>
  /// Create new instrument
  /// </summary>
  /// <param name="instrumentRquest">New instrument dto</param>
  /// <param name="cancellationToken"></param>
  /// <exception cref="FluentValidation.ValidationException">One of argument has wrong value</exception>
  /// <returns></returns>
  Task<Result<InstrumentResponseDto>> CreateAsync(NewInstrumentRequestDto instrumentRquest, CancellationToken cancellationToken = default);

  /// <summary>
  /// Remove instrument from service
  /// </summary>
  /// <param name="instrumentId"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  Task<Result> RemoveAsync(int instrumentId, CancellationToken cancellationToken = default);
}
