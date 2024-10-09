namespace Idempotency.Worker.Repositories;

public interface IRepository
{
  void BeginTransaction();
  void Commit();
  void Rollback();
}

public abstract class Repository(DataContext context) : IRepository
{
  public void BeginTransaction()
  {
    context.BeginTransaction();
  }

  public void Commit()
  {
    context.Commit();
  }

  public void Rollback()
  {
    context.Rollback();
  }
}