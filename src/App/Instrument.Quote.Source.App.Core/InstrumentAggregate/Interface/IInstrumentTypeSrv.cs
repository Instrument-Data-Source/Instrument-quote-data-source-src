using Ardalis.Result;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;

namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
public interface IInstrumentTypeSrv
{
  Task<Result<IEnumerable<InstrumentTypeDto>>> GetAllAsync(CancellationToken cancellationToken = default);
  Task<Result<InstrumentTypeDto>> GetByAsync(int Id, CancellationToken cancellationToken = default);
  Task<Result<InstrumentTypeDto>> GetByAsync(string Code, CancellationToken cancellationToken = default);
}