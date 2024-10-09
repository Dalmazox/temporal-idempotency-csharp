using Idempotency.Worker.Workflows.Investments.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Idempotency.Worker.Workflows;

public record AcquireQuotasArgs(Guid AccountId, decimal Amount);

[Workflow]
public interface IAcquireQuotasWorkflow
{
  [WorkflowRun]
  Task<bool> RunAsync(AcquireQuotasArgs args);
}

[Workflow]
public class AcquireQuotasWorkflow : IAcquireQuotasWorkflow
{
  [WorkflowRun]
  public async Task<bool> RunAsync(AcquireQuotasArgs args)
  {
    var uniqueId = Guid.Parse(Workflow.Info.RunId);
    var options = GetActivityOptions();

    var transactionId = await Workflow.ExecuteActivityAsync<WithdrawAccount, Guid>(
      act => act.HandleAsync(args.AccountId, uniqueId, args.Amount),
      options);

    return true;
  }

  private static ActivityOptions GetActivityOptions()
  {
    return new ActivityOptions
    {
      RetryPolicy = new RetryPolicy
      {
        MaximumAttempts = 2,
        InitialInterval = TimeSpan.FromSeconds(1)
      },
      StartToCloseTimeout = TimeSpan.FromMinutes(100)
    };
  }
}