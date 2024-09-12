namespace JobQueuesEfCoreDemo;

sealed class HelloEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Post("hello/{firstName}/{lastName}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var taskA = Parallel.ForEachAsync(
            Enumerable.Range(1, 10),
            ct,
            async (_, c) => await new CommandA().QueueJobAsync(ct: c));

        var taskB = Parallel.ForEachAsync(
            Enumerable.Range(1, 10),
            ct,
            async (_, c) => await new CommandB().QueueJobAsync(ct: c));

        await Task.WhenAll(taskA, taskB);

        var commandCTrackingId = await new CommandC
        {
            FirstName = Route<string>("firstName")!,
            LastName = Route<string>("lastName")!
        }.QueueJobAsync(ct: ct);

        await SendCreatedAtAsync<ResultEndpoint>(
            routeValues: new { trackingId = commandCTrackingId },
            responseBody: $"Your job tracking Id is: {commandCTrackingId}",
            cancellation: ct);
    }
}

sealed class ResultEndpoint(IJobTracker<CommandC> jobTracker) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("result/{trackingId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken c)
    {
        var trackingId = Route<Guid>("trackingId");
        var result = await jobTracker.GetJobResultAsync<CommandCResult>(trackingId, c);
        Response = $"Your full name is: {result?.FullName}";
    }
}