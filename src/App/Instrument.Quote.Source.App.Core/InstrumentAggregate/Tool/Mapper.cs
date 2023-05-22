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
  /// <exception cref="ArgumentOutOfRangeException">One of argument has wrong value</exception>
  /// <returns></returns>
  public static async Task<ent.Instrument> ToEntityAsync(this NewInstrumentRequestDto dto, IReadRepository<ent.InstrumentType> readRepository)
  {
    InstrumentType typeEnt = await dto.GetInstrumentType(readRepository);
    return new ent.Instrument(dto.Name, dto.Code, dto.PriceDecimalLen, dto.VolumeDecimalLen, typeEnt);
  }

  private static async Task<InstrumentType> GetInstrumentType(this NewInstrumentRequestDto dto, IReadRepository<InstrumentType> readRepository)
  {
    InstrumentType? typeByCode = null;
    if (dto.TypeId != 0)
      typeByCode = await readRepository.GetByIdAsync(dto.TypeId);

    InstrumentType? typeByName = null;
    if (!string.IsNullOrEmpty(dto.Type))
      typeByName = await readRepository.GetByTypeAsync(dto.Type);

    if (typeByCode != null && typeByName != null)
    {
      if (typeByCode != typeByName)
        throw new ArgumentException("Instrument type name and id conflict to each other");

      return typeByCode;
    }

    if (typeByCode != null)
      return typeByCode;

    if (typeByName != null)
      return typeByName;

    throw new ArgumentException("Cann't define Instument type");

  }
}