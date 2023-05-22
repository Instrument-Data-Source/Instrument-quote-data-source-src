using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;

namespace Instrument.Quote.Source.App.Test.Tool.SeedData;

public interface ISeedInstrument
{
  IEnumerable<InstrumentResponseDto> All { get; }
  InstrumentResponseDto Instrument1 { get; }
  InstrumentResponseDto Instrument2 { get; }
}

public class SeedInstrument : ISeedInstrument
{

  public IEnumerable<InstrumentResponseDto> All => new[] { Instrument1, Instrument2 };

  public InstrumentResponseDto Instrument1 { get; set; }

  public InstrumentResponseDto Instrument2 { get; set; }
}