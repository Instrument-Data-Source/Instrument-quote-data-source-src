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
  public static async Task<ent.Instrument> ToEntityAsync(this NewInstrumentRequestDto dto, IReadRepository<ent.InstrumentType> readRepository, CancellationToken cancellationToken = default)
  {
    int typeEnt = await dto.GetInstrumentTypeId(readRepository, cancellationToken);
    return new ent.Instrument(dto.Name, dto.Code, dto.PriceDecimalLen, dto.VolumeDecimalLen, typeEnt);
  }

  private static async Task<int> GetInstrumentTypeId(this NewInstrumentRequestDto dto, IReadRepository<InstrumentType> readRepository, CancellationToken cancellationToken = default)
  {
    InstrumentType? typeByCode = null;
    if (dto.TypeId != 0)
      typeByCode = await readRepository.TryGetByIdAsync(dto.TypeId, cancellationToken);


    if (typeByCode != null)
      return typeByCode.Id;

    return -1;

  }
  public static async Task<InstrumentResponseDto> ToDtoAsync(this ent.Instrument entity, IReadRepository<ent.InstrumentType> instrumentTypeRep, CancellationToken cancellationToken = default)
  {
    var dto = new InstrumentResponseDto();
    dto.Id = entity.Id;
    dto.Name = entity.Name;
    dto.Code = entity.Code;
    if (entity.InstrumentType == null)
      dto.Type = await instrumentTypeRep.GetNameByIdAsync(entity.InstrumentTypeId, cancellationToken);
    else
      dto.Type = entity.InstrumentType.Name;
    dto.PriceDecimalLen = entity.PriceDecimalLen;
    dto.VolumeDecimalLen = entity.VolumeDecimalLen;
    return dto;
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