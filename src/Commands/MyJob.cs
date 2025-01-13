namespace JobQueuesEfCoreDemo;

sealed class MyJob : ITrackableJob<JobResult<MyEndResult>>
{
    public Guid TrackingID { get; set; }
    public string Name { get; set; }
}

sealed class MyEndResult
{
    public string Message { get; set; }
}

sealed class MyJobHandler(IJobTracker<MyJob> tracker) : ICommandHandler<MyJob, JobResult<MyEndResult>>
{
    public async Task<JobResult<MyEndResult>> ExecuteAsync(MyJob job, CancellationToken ct)
    {
        var jobResult = new JobResult<MyEndResult>(totalSteps: 100)
        {
            CurrentStatus = "starting..."
        };

        for (var i = 0; i < 100; i++)
        {
            jobResult.CurrentStep = i;
            jobResult.CurrentStatus = $"completed step: {i}";
            await tracker.StoreJobResultAsync(job.TrackingID, jobResult, ct);
            await Task.Delay(300);
        }

        jobResult.CurrentStatus = "all done!";
        jobResult.Result = new() { Message = $"thank you {job.Name}!" };

        return jobResult;
    }
}