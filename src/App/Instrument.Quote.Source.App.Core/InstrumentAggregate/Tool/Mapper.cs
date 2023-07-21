using Ardalis.Result;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Model;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Repository;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;

namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Tool;

public static class Mapper
{

  /// <summary>
  /// Convert Dto to Entity
  /// </summary>
  /// <param name="readRepository">Entity Type Repository</param>
  /// <exception cref="FluentValidation.ValidationException">One of argument has wrong value</exception>
  /// <returns></returns>
  public static async Task<Result<ent.Instrument>> ToEntityAsync(this NewInstrumentRequestDto dto, IReadRepository<ent.InstrumentType> readRepository, CancellationToken cancellationToken = default)
  {
    var instrumentType = await readRepository.TryGetByIdAsync(dto.TypeId, cancellationToken);
    if (instrumentType == null)
      return Result.NotFound(nameof(ent.InstrumentType));

    return Result.Success(new ent.Instrument(dto.Name, dto.Code, dto.PriceDecimalLen, dto.VolumeDecimalLen, instrumentType));
  }


  public static InstrumentResponseDto ToDto(this ent.Instrument entity)
  {
    return new InstrumentResponseDto(entity);
  }

  public static InstrumentTypeResponseDto ToDto(this ent.InstrumentType entity)
  {
    return new InstrumentTypeResponseDto()
    {
      Id = entity.Id,
      Name = entity.Name
    };
  }
}