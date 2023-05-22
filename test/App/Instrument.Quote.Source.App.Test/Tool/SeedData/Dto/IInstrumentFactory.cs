using Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;

namespace Instrument.Quote.Source.App.Test.Tool.SeedData.Dto;

public interface IInstrumentFactory
{
  NewInstrumentRequestDto NewInstrumentDto { get; }
  IReadOnlyDictionary<string, IEnumerable<CandleDto>> candleFactory { get; }
}
