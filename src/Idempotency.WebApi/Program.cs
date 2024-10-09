using Idempotency.Worker.Workflows;
using Microsoft.AspNetCore.Mvc;
using Temporalio.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTemporalClient("localhost:7233", "default");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/quotas/buy", async ([FromBody] BuyQuotasRequest request) =>
{
    var workflowId = string.Format("buy-quotas-{0}", request.AccountId);
    var temporal = app.Services.GetRequiredService<ITemporalClient>();
    var handle = await temporal.StartWorkflowAsync<IAcquireQuotasWorkflow>(
        act => act.RunAsync(new AcquireQuotasArgs(request.AccountId, request.Amount)),
        new WorkflowOptions
        {
            Id = workflowId,
            TaskQueue = "acquire-quotas"
        }
    );

    return Results.Ok(new { executionId = handle.Id });

});

app.UseHttpsRedirection();
app.Run();

record BuyQuotasRequest(Guid AccountId, decimal Amount);