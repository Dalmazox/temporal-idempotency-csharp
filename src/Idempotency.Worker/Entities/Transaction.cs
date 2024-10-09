namespace Idempotency.Worker.Entities;

public record Transaction(Guid Id, Guid AccountId, decimal Amount)
{
  public DateTime CreatedAt { get; init; } = DateTime.MinValue;
}