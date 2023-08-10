using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Instrument.Quote.Source.Configuration.DataBase.PostreSQL;

public class PGConnectionStringSource : IConnectionStringSource
{
  private readonly DbConnectionStringBuilder dbConnectionStringBuilder;

  public PGConnectionStringSource(IConfiguration configuration)
  {
    dbConnectionStringBuilder = GetConnectionStringBuilder(configuration);
  }
  public string DataBase => dbConnectionStringBuilder["Database"].ToString()!;

  string IConnectionStringSource.ConnectionString => dbConnectionStringBuilder.ConnectionString;

  string IConnectionStringSource.Host => dbConnectionStringBuilder["Host"].ToString()!;

  private DbConnectionStringBuilder GetConnectionStringBuilder(IConfiguration configuration)
  {
    var _defConnection = configuration.GetConnectionString("DefaultConnection");
    var dbSuffix = configuration["ConnectionStrings:DbSuffix"];
    DbConnectionStringBuilder _connectionStringBuilder = new NpgsqlConnectionStringBuilder(_defConnection);
    if (dbSuffix != null)
      _connectionStringBuilder["Database"] += $"_{dbSuffix}";
    return _connectionStringBuilder;
  }
}