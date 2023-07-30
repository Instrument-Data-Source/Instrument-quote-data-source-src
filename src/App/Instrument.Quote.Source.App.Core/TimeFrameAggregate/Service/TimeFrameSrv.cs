using Ardalis.Result;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Dto;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Interface;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Instrument.Quote.Source.App.Core.TimeFrameAggregate.Service;

public class TimeFrameSrv : ITimeFrameSrv
{
  private readonly IReadRepository<TimeFrame> tfRep;

  public TimeFrameSrv(IReadRepository<TimeFrame> tfRep)
  {
    this.tfRep = tfRep;
  }
  public async Task<Result<IEnumerable<TimeFrameResponseDto>>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    IEnumerable<TimeFrameResponseDto> tf_dto = await tfRep.Table.Select(e => new TimeFrameResponseDto(e)).ToArrayAsync(cancellationToken);

    return Result.Success(tf_dto);
  }

  public async Task<Result<TimeFrameResponseDto>> GetByCodeAsync(string Code, CancellationToken cancellationToken = default)
  {
    var ret_tf = await tfRep.Table.SingleOrDefaultAsync(e => e.Name == Code, cancellationToken);

    if (ret_tf == null) return Result.NotFound(nameof(TimeFrame));

    return Result.Success(ret_tf.ToDto());
  }

  public async Task<Result<TimeFrameResponseDto>> GetByIdAsync(int Id, CancellationToken cancellationToken = default)
  {
    var ret_tf = await tfRep.Table.SingleOrDefaultAsync(e => e.Id == Id, cancellationToken);

    if (ret_tf == null) return Result.NotFound(nameof(TimeFrame));

    return Result.Success(ret_tf.ToDto());
  }
}