using System;
namespace Instrument.Quote.Source.Shared.DateTimePeriod;

public struct DateTimePeriod : IEquatable<DateTimePeriod>
{
  public DateTime From;
  public DateTime Untill;
  public DateTimePeriod(DateTime from, DateTime untill) : this()
  {
    if (from > untill)
      throw new ArgumentOutOfRangeException("untill must be >= from");
    From = from;
    Untill = untill;
  }

  public bool Equals(DateTimePeriod other)
  {
    return this.From.Equals(other.From) &&
           this.Untill.Equals(other.Untill);
  }

  public bool IsEmpty() => From == Untill;

  public bool Contain(DateTime dateTime)
  {
    return dateTime >= From && dateTime < Untill;
  }
}
