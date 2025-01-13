namespace JobQueuesEfCoreDemo;

sealed class HelloEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Post("test/hello/{firstName}/{lastName}");
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
        Get("test/result/{trackingId:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken c)
    {
        var trackingId = Route<Guid>("trackingId");
        var result = await jobTracker.GetJobResultAsync<CommandCResult>(trackingId, c);
        Response = $"Your full name is: {result?.FullName}";
    }
}

sealed class JobCreateEndpoint : EndpointWithoutRequest<string>
{
    public override void Configure()
    {
        Post("job/create/{yourName}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken c)
    {
        var job = new MyJob { Name = Route<string>("yourName") ?? "john doe" };
        var trackingId = await job.QueueJobAsync(ct: c);
        await SendAsync($"Your job tracking Id is: {trackingId}");
    }
}

sealed class JobProgressEndpoint : EndpointWithoutRequest<string>
{
    public override void Configure()
    {
        Post("job/progress/{trackingId:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken c)
    {
        var trackingId = Route<Guid>("trackingId");
        var jobResult = await JobTracker<MyJob>.GetJobResultAsync<JobResult<MyEndResult>>(trackingId, c);

        if (jobResult is null)
        {
            await SendAsync("job execution hasn't begun yet!");

            return;
        }

        switch (jobResult.IsComplete)
        {
            case false:
                await SendAsync($"[{jobResult.CurrentStep} / {jobResult.TotalSteps}] | status: {jobResult.CurrentStatus}");

                break;
            case true:
                await SendAsync($"end result: {jobResult.Result.Message}");

                break;
        }
    }
}