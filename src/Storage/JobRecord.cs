using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace JobQueuesEfCoreDemo;

public class JobRecord : IJobStorageRecord, IJobResultStorage
{
    public Guid Id { get; set; }
    public string QueueID { get; set; } = default!;
    public Guid TrackingID { get; set; }
    public DateTime ExecuteAfter { get; set; }
    public DateTime ExpireOn { get; set; }
    public bool IsComplete { get; set; }

    [NotMapped]
    public object Command { get; set; } = default!;

    public string CommandJson { get; set; } = default!;

    TCommand IJobStorageRecord.GetCommand<TCommand>()
        => JsonSerializer.Deserialize<TCommand>(CommandJson)!;

    void IJobStorageRecord.SetCommand<TCommand>(TCommand command)
        => CommandJson = JsonSerializer.Serialize(command);

    [NotMapped]
    public object? Result { get; set; }

    public string? ResultJson { get; set; }

    TResult? IJobResultStorage.GetResult<TResult>() where TResult : default
        => ResultJson is not null
               ? JsonSerializer.Deserialize<TResult>(ResultJson)
               : default;

    void IJobResultStorage.SetResult<TResult>(TResult result)
        => ResultJson = JsonSerializer.Serialize(result);

    // MessagePack alternative: https://gist.github.com/dj-nitehawk/02420788fb0a72c4be4752be8bd4c40b
}