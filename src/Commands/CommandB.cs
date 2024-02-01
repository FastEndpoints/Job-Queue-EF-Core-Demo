namespace JobQueuesEfCoreDemo;

sealed class CommandB : ICommand
{
    public string Name => "B";
    public int Id { get; set; }
}

sealed class CommandBHandler(ILogger<CommandBHandler> logger) : ICommandHandler<CommandB>
{
    static int _id;

    public Task ExecuteAsync(CommandB commandB, CancellationToken ct)
    {
        logger.LogInformation("COMMAND B EXECUTED: {id}", _id++);

        return Task.CompletedTask;
    }
}