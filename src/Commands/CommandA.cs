namespace JobQueuesEfCoreDemo;

sealed class CommandA : ICommand
{
    public string Name => "A";
    public int Id { get; set; }
}

sealed class CommandAHandler(ILogger<CommandAHandler> logger) : ICommandHandler<CommandA>
{
    static int _id;

    public Task ExecuteAsync(CommandA commandA, CancellationToken ct)
    {
        logger.LogInformation("COMMAND A EXECUTED: {id}", _id++);

        return Task.CompletedTask;
    }
}