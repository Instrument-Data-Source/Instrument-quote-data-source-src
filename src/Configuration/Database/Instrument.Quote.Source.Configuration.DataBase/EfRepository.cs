using Instrument.Quote.Source.Shared.Kernal.DataBase;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository;
using Microsoft.Extensions.Logging;

namespace Instrument.Quote.Source.Configuration.DataBase;

public class EfRepository<TEntity> : Repository<TEntity, SrvDbContext> where TEntity : EntityBase
{
  public EfRepository(SrvDbContext dbContext, ILogger<EfRepository<TEntity>> logger) : base(dbContext, logger)
  {
  }
}