using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Instrument.Quote.Source.Shared.Kernal.DataBase;

public class EnumEntity<TEnum> : EntityBase, IEquatable<EnumEntity<TEnum>>, IComparable<EnumEntity<TEnum>>
  where TEnum : struct, Enum
{
  public const int NameLenght = 50;
  [NotMapped]
  public virtual TEnum EnumId
  {
    get => (TEnum)Enum.ToObject(typeof(TEnum), Id);
    set
    {
      var new_name = Enum.GetName<TEnum>(value);
      if (new_name == null)
      {
        throw new ArgumentOutOfRangeException(nameof(EnumId), value, "Given Id is out of range");
      }
      __name = new_name;
      Id = Convert.ToInt32(value);
    }
  }
  private string __name;
  [Required]
  [MaxLength(NameLenght)]
  public string Name
  {
    get => this.__name;
    set
    {
      try
      {
        EnumId = Enum.GetValues<TEnum>().Single(e => e.ToString() == value);
      }
      catch (InvalidOperationException ex)
      {
        throw new ArgumentOutOfRangeException(nameof(Name), value, "Given Name is out of range");
      }
    }
  }

  public EnumEntity(int id) : this((TEnum)Enum.ToObject(typeof(TEnum), id))
  {
  }

  public EnumEntity(TEnum Id)
  {
    this.EnumId = Id;
  }

  public EnumEntity(string Name)
  {
    this.Name = Name;
  }

  public static IEnumerable<EnumEntity<TEnum>> ToList()
  {
    List<EnumEntity<TEnum>> _ret = new List<EnumEntity<TEnum>>();
    foreach (var v in Enum.GetValues<TEnum>())
    {
      _ret.Add(new EnumEntity<TEnum>(v));
    }
    return _ret;
  }

  public int CompareTo(EnumEntity<TEnum> other)
  {
    return this.EnumId.CompareTo(other.EnumId);
  }

  public bool Equals(EnumEntity<TEnum>? other)
  {
    // check if same instance
    if (Object.ReferenceEquals(this, other))
      return true;

    // it's not same instance so 
    // check if it's not null and is same value
    if (other is null)
      return false;
    return this.EnumId.Equals(other.EnumId);
  }

  public override int GetHashCode()
  {
    return this.EnumId.GetHashCode();
  }
}