namespace Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
public interface ITransactionManager : IDisposable
{
  void BeginTransaction();
  void CommitTransaction();
  void RollBack();
}