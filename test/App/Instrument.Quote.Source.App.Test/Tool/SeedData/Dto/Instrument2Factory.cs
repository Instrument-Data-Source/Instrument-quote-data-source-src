using Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;

namespace Instrument.Quote.Source.App.Test.Tool.SeedData.Dto;

public class Instrument2Factory : IInstrumentFactory
{
  public NewInstrumentRequestDto NewInstrumentDto => new NewInstrumentRequestDto()
  {
    Name = "Instrument2",
    Code = "Inst2",
    PriceDecimalLen = 2,
    VolumeDecimalLen = 0,
    Type = "Stock"
  };

  public IReadOnlyDictionary<string, IEnumerable<CandleDto>> candleFactory => new Dictionary<string, IEnumerable<CandleDto>>(){
    {"m10", new []{
        new CandleDto()
        {
          DateTime = new DateTime(2020,1,1,1,0,0).ToUniversalTime(),
          Open = (decimal)1.01,
          High = (decimal)1.2,
          Low = (decimal) 0.09,
          Close = (decimal)1.05,
          Volume = (decimal)1
        },
        new CandleDto()
        {
          DateTime = new DateTime(2020,1,1,1,10,0).ToUniversalTime(),
          Open = (decimal)1.01,
          High = (decimal)1.1,
          Low = (decimal) 0.09,
          Close = (decimal)1.05,
          Volume = (decimal)2
        },
        new CandleDto()
        {
          DateTime = new DateTime(2020,1,1,1,20,0).ToUniversalTime(),
          Open = (decimal)1.02,
          High = (decimal)1.10,
          Low = (decimal) 0.09,
          Close = (decimal)1.05,
          Volume = (decimal)1
        },
      }
    }
  };
}