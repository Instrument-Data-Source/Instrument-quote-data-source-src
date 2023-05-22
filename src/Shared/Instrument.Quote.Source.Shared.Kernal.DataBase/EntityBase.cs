using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Instrument.Quote.Source.Shared.Kernal.DataBase;

public abstract class EntityBase
{
  [Key]
  public int Id { get; protected set; }
}
