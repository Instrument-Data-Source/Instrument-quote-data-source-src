using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;

namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;

public interface IInstrumentSrv
{
  /// <summary>
  /// Create new instrument
  /// </summary>
  /// <param name="instrumentRquest">New instrument dto</param>
  /// <param name="cancellationToken"></param>
  /// <exception cref="ArgumentException">One of argument has wrong value</exception>
  /// <returns></returns>
  Task<InstrumentResponseDto> CreateInstrumentAsync(NewInstrumentRequestDto instrumentRquest, CancellationToken cancellationToken = default);
  /// <summary>
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
  Task<InstrumentResponseDto?> TryGetInstrumentByCodeAsync(string instrumentCode, CancellationToken cancellationToken = default);
  /// <summary>
  /// Get instrument by Id
  /// </summary>
  /// <param name="instrumentId">Instrument Id</param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  Task<InstrumentResponseDto?> TryGetInstrumentByIdAsync(int instrumentId, CancellationToken cancellationToken = default);
  /// <summary>
  /// Remove instrument from service
  /// </summary>
  /// <param name="instrumentId"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  Task RemoveInstrumentAsync(int instrumentId, CancellationToken cancellationToken = default);
}