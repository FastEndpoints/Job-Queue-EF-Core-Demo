namespace JobQueuesEfCoreDemo;

sealed class CommandC : ICommand<CommandCResult>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

sealed class CommandCResult
{
    public string FullName { get; set; }
}

sealed class CommandCHandler : ICommandHandler<CommandC, CommandCResult>
{
    public Task<CommandCResult> ExecuteAsync(CommandC cmd, CancellationToken c)
        => Task.FromResult(
            new CommandCResult
            {
                FullName = $"{cmd.FirstName} {cmd.LastName}"
            });
}