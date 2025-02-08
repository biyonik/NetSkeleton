namespace Infrastructure.BackgroundJobs.Attributes;

/// <summary>
/// Background job attribute'u
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class BackgroundJobAttribute : Attribute
{
    /// <summary>
    /// Job'ın çalışacağı queue
    /// </summary>
    public string Queue { get; set; } = "default";

    /// <summary>
    /// Job'ın tekrar deneme sayısı
    /// </summary>
    public int RetryCount { get; set; } = 3;

    public BackgroundJobAttribute(string queue = "default", int retryCount = 3)
    {
        Queue = queue;
        RetryCount = retryCount;
    }
}