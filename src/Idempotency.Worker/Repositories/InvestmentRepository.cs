using Dapper;
using Idempotency.Worker.Entities;

namespace Idempotency.Worker.Repositories;

public interface IInvestmentRepository : IRepository
{
  Task<Account?> GetAccountAsync(Guid accountId);
  Task<bool> InsertTransactionAsync(Transaction transaction);
  Task<Transaction?> GetTransactionAsync(Guid uniqueId);
  Task<bool> UpdateAccountBalanceAsync(Account account);
}

public class InvestmentRepository : Repository, IInvestmentRepository
{
  private readonly DataContext _context;

  public InvestmentRepository(DataContext context) : base(context)
  {
    _context = context;
  }

  public async Task<Account?> GetAccountAsync(Guid accountId)
  {
    const string sql = @"
      SELECT 
          id AS Id,
          owner AS Owner,
          current_balance AS CurrentBalance
      FROM public.accounts
      WHERE id = @AccountId";

    var connection = _context.GetConnection();
    var param = new { AccountId = accountId };

    return await connection.QuerySingleOrDefaultAsync<Account?>(sql, param, _context.GetTransaction());
  }

  public async Task<Transaction?> GetTransactionAsync(Guid uniqueId)
  {
    const string sql = @"
      SELECT 
          id AS Id,
          account_id AS AccountId,
          amount AS Amount,
          creation_date AS CreatedAt
      FROM public.transactions
      WHERE id = @UniqueId";

    var connection = _context.GetConnection();
    var param = new { UniqueId = uniqueId };

    return await connection.QuerySingleOrDefaultAsync<Transaction?>(sql, param, _context.GetTransaction());
  }

  public async Task<bool> InsertTransactionAsync(Transaction transaction)
  {
    const string sql = @"
      INSERT INTO public.transactions
      (id, account_id, amount)
      VALUES
      (@Id, @AccountId, @Amount)";

    var connection = _context.GetConnection();
    var param = new
    {
      transaction.Id,
      transaction.AccountId,
      transaction.Amount
    };

    var affectedRows = await connection.ExecuteAsync(sql, param, _context.GetTransaction());
    return affectedRows > 0;
  }

  public async Task<bool> UpdateAccountBalanceAsync(Account account)
  {
    const string sql = @"
      UPDATE public.accounts
      SET current_balance = @NewBalance
      WHERE id = @AccountId";

    var connection = _context.GetConnection();
    var param = new
    {
      NewBalance = account.CurrentBalance,
      AccountId = account.Id
    };

    var affectedRows = await connection.ExecuteAsync(sql, param, _context.GetTransaction());
    return affectedRows > 0;
  }
}