using Instrument.Quote.Source.Shared.Kernal.DataBase;
using model = Instrument.Quote.Source.App.Core.InstrumentAggregate.Model;

namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Model;

/// <summary>
/// Instrument types
/// </summary>
public class InstrumentType : EnumEntity<InstrumentType.Enum>
{
  public new const int NameLenght = 15;

  public enum Enum
  {
    Currency = 1,
    Stock = 2,
    CryptoCurrency = 3
  }

  public InstrumentType(int id) : base(id)
  {
  }

  public InstrumentType(Enum Id) : base(Id)
  {
  }

  private readonly List<model.Instrument> _instruments = new();
  public virtual IEnumerable<model.Instrument> Instruments => _instruments.AsReadOnly();
}