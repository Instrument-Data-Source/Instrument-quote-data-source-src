using Instrument.Quote.Source.App.Core.InstrumentAggregate.Model;

namespace Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Mocks;

public class MockInstrument : ent.Instrument
{
  public MockInstrument(string name, string code, byte priceDecimalLen, byte volumeDecimalLen, int instrumentTypeId) :
  base(name, code, priceDecimalLen, volumeDecimalLen, instrumentTypeId)
  {
  }

  public MockInstrument(string name, string code, byte priceDecimalLen, byte volumeDecimalLen, InstrumentType instrumentType) : base(name, code, priceDecimalLen, volumeDecimalLen, instrumentType)
  {
  }

  public ent.Instrument InitId(int id)
  {
    if (this.Id != 0)
      throw new Exception("Cann't init ID if it has already setted");

    this.Id = id;
    return this;
  }

  private static HashSet<int> usedId = new HashSet<int>();
  public static ent.Instrument Create(byte pliceDecimalLen = 1, byte volumeDecimalLen = 1)
  {
    var random = new Random();
    var id = random.Next(1, 1000);
    while (usedId.Contains(id))
    {
      id = random.Next(1, 1000);
    }
    usedId.Add(id);
    return new MockInstrument($"Mock {id}", $"M{id}", pliceDecimalLen, volumeDecimalLen, 1).InitId(id);
  }
}