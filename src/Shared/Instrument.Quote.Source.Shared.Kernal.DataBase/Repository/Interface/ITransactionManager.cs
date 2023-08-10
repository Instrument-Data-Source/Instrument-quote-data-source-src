namespace Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
public interface ITransactionManager : IDisposable
{
  Task SaveChangesAsync(CancellationToken cancellationToken = default);
  void BeginTransaction();
  void CommitTransaction();
  void RollBack();
}