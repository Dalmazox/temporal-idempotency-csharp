using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Idempotency.Worker.Repositories;

public class DataContext(IConfiguration configuration) : IDisposable
{
  private bool _disposed = false;
  private IDbConnection? _connection;
  private IDbTransaction? _transaction;

  public IDbConnection GetConnection()
  {
    _connection ??= new NpgsqlConnection(configuration.GetConnectionString("Default"));

    if (_connection.State != ConnectionState.Open)
      _connection.Open();

    return _connection;
  }

  public IDbTransaction? GetTransaction() => _transaction;

  public IDbTransaction BeginTransaction()
  {
    if (_transaction == null)
    {
      _transaction = GetConnection().BeginTransaction();
    }
    return _transaction;
  }

  public void Commit()
  {
    try { _transaction?.Commit(); }
    catch
    {
      _transaction?.Rollback();
    }
    finally
    {
      DisposeTransaction();
    }
  }

  public void Rollback()
  {
    _transaction?.Rollback();
    DisposeTransaction();
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  private void DisposeTransaction()
  {
    _transaction?.Dispose();
    _transaction = null;
  }

  private void Dispose(bool disposing)
  {
    if (_disposed)
      return;

    if (disposing)
    {
      DisposeTransaction();
      _connection?.Close();
      _connection?.Dispose();
      _connection = null;
    }

    _disposed = true;
  }
}