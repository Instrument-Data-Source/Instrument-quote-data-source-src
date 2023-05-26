namespace Instrument.Quote.Source.Shared.FluentValidation.Extension.Test.TestData;

public class Entity
{
  public int Value1 { get; set; }
  public SubEntity SubEntity1 { get; set; }
}

public class SubEntity
{
  public string Value2 { get; set; }
  public bool Value3 { get; set; }
}