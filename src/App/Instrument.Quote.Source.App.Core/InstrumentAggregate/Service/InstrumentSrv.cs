using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Tool;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Repository;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Event;
using Microsoft.Extensions.Logging;
using Ardalis.Result;

namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Service;


public class InstrumentSrv : IInstrumentSrv
{
  private readonly IRepository<ent.Instrument> instrumentRep;
  private readonly IReadRepository<ent.InstrumentType> instrumentTypeRep;

  public InstrumentSrv(IRepository<ent.Instrument> instrumentRep, IReadRepository<ent.InstrumentType> instrumentTypeRep)
  {
    this.instrumentRep = instrumentRep;
    this.instrumentTypeRep = instrumentTypeRep;
  }

  public async Task<InstrumentResponseDto> CreateInstrumentAsync(NewInstrumentRequestDto instrumentRequest, CancellationToken cancellationToken = default)
  {
    ent.Instrument newInstrument;
    newInstrument = await instrumentRequest.ToEntityAsync(instrumentTypeRep, cancellationToken);

    try
    {
      await instrumentRep.AddAsync(newInstrument);
    }
    catch (RepositoryException ex)
    {
      throw new ApplicationException("Error when add instument", ex);
    }

    return new InstrumentResponseDto(newInstrument);
  }

  public async Task<IEnumerable<InstrumentResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    return await instrumentRep.GetAsDto(cancellationToken);
  }

  public async Task RemoveInstrumentAsync(int instrumentId, CancellationToken cancellationToken = default)
  {
    var result = await instrumentRep.TryRemoveAsync(instrumentId, cancellationToken: cancellationToken);
    if (!result)
      throw new ArgumentOutOfRangeException(nameof(instrumentId), instrumentId, "Unknown Id");
  }

  public async Task<InstrumentResponseDto?> TryGetInstrumentByCodeAsync(string instrumentCode, CancellationToken cancellationToken = default)
  {
    var findedEnt = await instrumentRep.Table.Include(e=>e.InstrumentType).SingleOrDefaultAsync(e => e.Code == instrumentCode, cancellationToken);
    return findedEnt != null ? new InstrumentResponseDto(findedEnt) : null;
  }

  public async Task<InstrumentResponseDto?> TryGetInstrumentByIdAsync(int instrumentId, CancellationToken cancellationToken = default)
  {
    var findedEnt = await instrumentRep.Table.Include(e=>e.InstrumentType).SingleOrDefaultAsync(e => e.Id == instrumentId, cancellationToken);
    return findedEnt != null ? new InstrumentResponseDto(findedEnt) : null;
  }
}