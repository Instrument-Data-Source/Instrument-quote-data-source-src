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
  public async Task<IEnumerable<TimeFrameResponseDto>> GetAllAsync()
  {
    IEnumerable<TimeFrameResponseDto> tf_dto = await tfRep.Table.Select(e => new TimeFrameResponseDto(e)).ToArrayAsync();
    return tf_dto;
  }

  public async Task<TimeFrameResponseDto> GetByCodeAsync(string Code)
  {
    var ret_tf = await tfRep.Table.SingleOrDefaultAsync(e => e.Name == Code);
    if (ret_tf == null) throw new ArgumentOutOfRangeException(nameof(Code), Code, "Uknown TimeFrame code");

    return ret_tf.ToDto();
  }
}