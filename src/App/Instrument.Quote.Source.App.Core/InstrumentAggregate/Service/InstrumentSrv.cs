using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Tool;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Repository;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Validator.Instrument;
using Ardalis.Result;
using Instrument.Quote.Source.Shared.FluentValidation.Extension;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Exceptions;

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

  public async Task<Result<InstrumentResponseDto>> CreateAsync(NewInstrumentRequestDto instrumentRequest, CancellationToken cancellationToken = default)
  {
    var newInstrumentResult = await instrumentRequest.ToEntityAsync(instrumentTypeRep, cancellationToken);
    if (!newInstrumentResult.IsSuccess)
      return newInstrumentResult.Repack<InstrumentResponseDto>();
    var newInstrument = newInstrumentResult.Value;

    try
    {
      await instrumentRep.AddAsync(newInstrument);
    }
    catch (RepositoryException ex)
    {
      throw new ApplicationException("Error when add instument", ex);
    }

    return Result.Success(newInstrument.ToDto());
  }

  public async Task<IEnumerable<InstrumentResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    return await instrumentRep.GetAsDto(cancellationToken);
  }

  public async Task<Result> RemoveAsync(int instrumentId, CancellationToken cancellationToken = default)
  {
    var result = await instrumentRep.TryRemoveAsync(instrumentId, cancellationToken: cancellationToken);
    return result ? Result.Success() : Result.NotFound();
  }

  public async Task<Result<IEnumerable<InstrumentResponseDto>>> GetByAsync(string instrumentCode, CancellationToken cancellationToken = default)
  {
    var lowerCode = instrumentCode.ToLower();
    var findedEnt = await instrumentRep.Table
                .Include(e => e.InstrumentType)
                .Where(e => e.Code.ToLower() == lowerCode)
                .ToArrayAsync(cancellationToken);
    if (findedEnt == null || findedEnt.Count() == 0)
      return Result.NotFound(nameof(ent.Instrument));
    IEnumerable<InstrumentResponseDto> res_dto = findedEnt.Select(e => e.ToDto()).ToArray();
    return Result.Success(res_dto);
  }

  public async Task<Result<InstrumentResponseDto>> GetByAsync(int instrumentId, CancellationToken cancellationToken = default)
  {
    var findedEnt = await instrumentRep.Table.Include(e => e.InstrumentType).SingleOrDefaultAsync(e => e.Id == instrumentId, cancellationToken);
    if (findedEnt == null)
      return Result.NotFound(nameof(ent.Instrument));
    var _ret_dto = findedEnt.ToDto();
    return Result.Success(_ret_dto);
  }
}