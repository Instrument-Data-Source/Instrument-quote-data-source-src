using Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;

namespace Instrument.Quote.Source.App.Test.Tool.SeedData.Dto;

public class Instrument1Factory : IInstrumentFactory
{
  public NewInstrumentRequestDto NewInstrumentDto => new NewInstrumentRequestDto()
  {
    Name = "Instrument1",
    Code = "Inst1",
    PriceDecimalLen = 5,
    VolumeDecimalLen = 2,
    Type = "Currency"
  };

  private IEnumerable<CandleDto> GenerateDtoM1()
  {
    var dtos = new List<CandleDto>();
    for (int i = 0; i < 20; i++)
    {
      dtos.Add(
      new CandleDto()
      {
        DateTime = new DateTime(2020, 1, 1, 1, 1 + i, 0).ToUniversalTime(),
        Open = (decimal)1.00253 + i,
        High = (decimal)1.10000 + i,
        Low = (decimal)0.09000 + i,
        Close = (decimal)1.05 + i,
        Volume = (decimal)1.23 + i
      });
    }
    return dtos;
  }
  public IReadOnlyDictionary<string, IEnumerable<CandleDto>> candleFactory => new Dictionary<string, IEnumerable<CandleDto>>(){
    {"m1", GenerateDtoM1()}
  };

}