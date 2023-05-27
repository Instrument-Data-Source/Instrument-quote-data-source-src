using Ardalis.Result;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Model;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Tool;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Service;
public class InstrumentTypeSrv : IInstrumentTypeSrv
{
  private readonly ILogger<InstrumentTypeSrv> logger;
  private readonly IReadRepository<InstrumentType> rep;

  public InstrumentTypeSrv(ILogger<InstrumentTypeSrv> logger, IReadRepository<InstrumentType> rep)
  {
    this.logger = logger;
    this.rep = rep;
  }

  public async Task<Result<IEnumerable<InstrumentTypeResponseDto>>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    return await rep.Table.Select(e => e.ToDto()).ToArrayAsync(cancellationToken);
  }

  public async Task<Result<InstrumentTypeResponseDto>> GetByAsync(int Id, CancellationToken cancellationToken = default)
  {
    var result = await rep.Table.Select(e => e.ToDto()).SingleOrDefaultAsync(e => e.Id == Id, cancellationToken);
    if (result == null)
      return Result.NotFound();
    return Result.Success(result);
  }

  public async Task<Result<InstrumentTypeResponseDto>> GetByAsync(string Code, CancellationToken cancellationToken = default)
  {
    var lower_code = Code.ToLower();
    var result = await rep.Table.Select(e => e.ToDto()).SingleOrDefaultAsync(e => e.Name.ToLower() == lower_code, cancellationToken);
    if (result == null)
      return Result.NotFound();
    return Result.Success(result);
  }
}