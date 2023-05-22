namespace Instrument.Quote.Source.App.Test.Tool.SeedData;

public interface ISeedContainer
{
  ISeedInstrument SeedInstrument { get; }
}

public class SeedContainer : ISeedContainer
{
  public SeedInstrument? SeedInstrument { get; set; } = null;

  ISeedInstrument ISeedContainer.SeedInstrument => SeedInstrument;
}