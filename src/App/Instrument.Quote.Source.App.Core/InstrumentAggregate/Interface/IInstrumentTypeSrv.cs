using Ardalis.Result;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;

namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
public interface IInstrumentTypeSrv
{
  Task<Result<IEnumerable<InstrumentTypeResponseDto>>> GetAllAsync(CancellationToken cancellationToken = default);
  Task<Result<InstrumentTypeResponseDto>> GetByAsync(int Id, CancellationToken cancellationToken = default);
  Task<Result<InstrumentTypeResponseDto>> GetByAsync(string Name, CancellationToken cancellationToken = default);
}