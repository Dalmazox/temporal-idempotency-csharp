using Idempotency.Worker.Repositories;
using Idempotency.Worker.Workflows;
using Idempotency.Worker.Workflows.Investments.Activities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Temporalio.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.TryAddScoped<DataContext>();
builder.Services.TryAddScoped<IInvestmentRepository, InvestmentRepository>();
builder.Services.AddTemporalClient("localhost:7233", "default");
builder.Services
  .AddHostedTemporalWorker("acquire-quotas")
  .AddScopedActivities<WithdrawAccount>()
  .AddWorkflow<AcquireQuotasWorkflow>();

await builder.Build().RunAsync();