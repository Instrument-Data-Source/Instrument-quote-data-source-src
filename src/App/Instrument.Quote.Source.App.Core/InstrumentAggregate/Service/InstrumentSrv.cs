using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Tool;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Repository;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Validator.Instrument;
using Ardalis.Result;
using Instrument.Quote.Source.Shared.FluentValidation.Extension;

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

  public async Task<Result<InstrumentResponseDto>> CreateInstrumentAsync(NewInstrumentRequestDto instrumentRequest, CancellationToken cancellationToken = default)
  {
    if (!instrumentRequest.IsValid(out var validationResult))
    {
      return Result.Invalid(validationResult.Errors.ToErrorList());
    }
    ent.Instrument newInstrument = await instrumentRequest.ToEntityAsync(instrumentTypeRep, cancellationToken);

    try
    {
      await instrumentRep.AddAsync(newInstrument);
    }
    catch (RepositoryException ex)
    {
      throw new ApplicationException("Error when add instument", ex);
    }

    return Result.Success(await newInstrument.ToDtoAsync(instrumentTypeRep, cancellationToken));
  }

  public async Task<IEnumerable<InstrumentResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    return await instrumentRep.GetAsDto(cancellationToken);
  }

  public async Task<Result> RemoveInstrumentAsync(int instrumentId, CancellationToken cancellationToken = default)
  {
    var result = await instrumentRep.TryRemoveAsync(instrumentId, cancellationToken: cancellationToken);
    if (!result)
      return Result.Success();
    return !result ? Result.Success() : Result.NotFound();
  }

  public async Task<InstrumentResponseDto?> TryGetInstrumentByCodeAsync(string instrumentCode, CancellationToken cancellationToken = default)
  {
    var findedEnt = await instrumentRep.Table.Include(e=>e.InstrumentType).SingleOrDefaultAsync(e => e.Code == instrumentCode, cancellationToken);
    return findedEnt != null ? await findedEnt.ToDtoAsync(instrumentTypeRep, cancellationToken) : null;
  }

  public async Task<Result<InstrumentResponseDto>> GetInstrumentByIdAsync(int instrumentId, CancellationToken cancellationToken = default)
  {
    var findedEnt = await instrumentRep.Table.Include(e=>e.InstrumentType).SingleOrDefaultAsync(e => e.Id == instrumentId, cancellationToken);
    if (findedEnt == null)
      return Result.NotFound();
    var _ret_dto = await findedEnt.ToDtoAsync(instrumentTypeRep, cancellationToken);
    return Result.Success(_ret_dto);
  }
}