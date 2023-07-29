using Ardalis.GuardClauses;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Instrument.Quote.Source.Shared.Kernal.DataBase.Repository;

public class TransactionManager<TDbContext> : ITransactionManager where TDbContext : DbContext
{
  private readonly TDbContext dbContext;
  private readonly ILogger logger;
  private IDbContextTransaction? transaction;
  private int transactionWrapLevel = 0;

  public TransactionManager(TDbContext dbContext, ILogger logger)
  {
    this.dbContext = dbContext;
    this.logger = logger;
  }

  public void BeginTransaction()
  {
    lock (this)
    {
      if (transaction == null)
        transaction = dbContext.Database.BeginTransaction();
      transactionWrapLevel++;
    }
  }

  public void CommitTransaction()
  {
    lock (this)
    {
      transactionWrapLevel--;
      if (transactionWrapLevel == 0)
      {
        transaction!.Commit();
        transaction = null;
      }
    }
  }

  public void Dispose()
  {
    lock (this)
    {
      if (transaction != null)
        throw new ApplicationException("Transaction manager has opened transaction while disposing");
    }
  }

  public void RollBack()
  {
    transaction?.Rollback();
    transaction = null;
    transactionWrapLevel = 0;
  }
}