namespace JobQueuesEfCoreDemo;

sealed class TestEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        AllowAnonymous();
        Post("test");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var taskA = Parallel.ForEachAsync(
            Enumerable.Range(1, 10),
            ct,
            async (_, c) => await new CommandA().QueueJobAsync(ct:c));

        var taskB = Parallel.ForEachAsync(
            Enumerable.Range(1, 10),
            ct,
            async (_, c) => await new CommandB().QueueJobAsync(ct: c));

        await Task.WhenAll(taskA, taskB);
    }
}