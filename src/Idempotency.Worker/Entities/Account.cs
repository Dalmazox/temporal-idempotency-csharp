namespace Idempotency.Worker.Entities;

public record Account(Guid Id, string Owner, decimal CurrentBalance)
{
  public decimal CurrentBalance { get; private set; } = CurrentBalance;
  public void Withdraw(decimal amount)
  {
    if (CurrentBalance < amount)
      throw new InvalidOperationException("Withdraw operation invalid due to current balance");

    CurrentBalance -= amount;
  }
}