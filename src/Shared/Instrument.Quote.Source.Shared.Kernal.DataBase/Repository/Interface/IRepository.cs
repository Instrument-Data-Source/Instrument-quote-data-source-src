namespace Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;

public interface IRepository<TEntity> : ICreateRepository<TEntity>, IReadRepository<TEntity> where TEntity : EntityBase
{

}