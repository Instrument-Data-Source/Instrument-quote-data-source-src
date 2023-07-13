using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;

namespace Instrument.Quote.Source.Shared.Kernal.DataBase.Exceptions;

public class IdNotFoundException : Exception
{
  public IdNotFoundException(string entity, int id) : base($"Id {id} not found in {entity}")
  {

  }

  public static IdNotFoundException Build<TEntity>(IReadRepository<TEntity> rep, int id) where TEntity : EntityBase
  {
    return new IdNotFoundException(typeof(TEntity).Name, id);
  }
}