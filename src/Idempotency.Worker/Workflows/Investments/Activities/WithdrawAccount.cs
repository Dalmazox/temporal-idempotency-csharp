using Idempotency.Worker.Entities;
using Idempotency.Worker.Repositories;
using Temporalio.Activities;

namespace Idempotency.Worker.Workflows.Investments.Activities;

public class WithdrawAccount(IInvestmentRepository repository)
{
  [Activity("WithdrawAccount")]
  public async Task<Guid> HandleAsync(Guid accountId, Guid uniqueId, decimal amount)
  {
    var transaction = await repository.GetTransactionAsync(uniqueId);

    if (transaction is not null)
      return transaction.Id;

    repository.BeginTransaction();

    var account = await repository.GetAccountAsync(accountId) ?? throw new AccountNotFoundException();
    account.Withdraw(amount);
    transaction = new Transaction(uniqueId, accountId, -amount);
    var createTransactionSuccess = await repository.InsertTransactionAsync(transaction);
    var updateBalanceSuccess = await repository.UpdateAccountBalanceAsync(account);

    repository.Commit();

    if (!createTransactionSuccess || !updateBalanceSuccess)
      throw new WithdrawUnsuccessfullyException();

    return transaction.Id;
  }
}

public class AccountNotFoundException() : Exception("Account not found");

public class WithdrawUnsuccessfullyException() : Exception();