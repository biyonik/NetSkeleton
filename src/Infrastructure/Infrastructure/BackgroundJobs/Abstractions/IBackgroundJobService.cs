using System.Linq.Expressions;

namespace Infrastructure.BackgroundJobs.Abstractions;

/// <summary>
/// Background job servisi için interface
/// </summary>
public interface IBackgroundJobService
{
    /// <summary>
    /// Fire-and-forget job ekler
    /// </summary>
    string Enqueue<T>(Expression<Action<T>> methodCall) where T : class;

    /// <summary>
    /// Fire-and-forget job ekler (belirli queue'ya)
    /// </summary>
    string Enqueue<T>(Expression<Action<T>> methodCall, string queue) where T : class;

    /// <summary>
    /// Zamanlanmış job ekler
    /// </summary>
    string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay) where T : class;

    /// <summary>
    /// Zamanlanmış job ekler (belirli queue'ya)
    /// </summary>
    string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay, string queue) where T : class;

    /// <summary>
    /// Belirli bir zamanda çalışacak job ekler
    /// </summary>
    string Schedule<T>(Expression<Action<T>> methodCall, DateTimeOffset enqueueAt) where T : class;

    /// <summary>
    /// Recurring job ekler
    /// </summary>
    void RecurringJob<T>(string jobId, Expression<Action<T>> methodCall, string cronExpression, string queue = "default") where T : class;

    /// <summary>
    /// Recurring job'ı kaldırır
    /// </summary>
    void RemoveRecurringJob(string jobId);

    /// <summary>
    /// Job'ı iptal eder
    /// </summary>
    bool Delete(string jobId);

    /// <summary>
    /// Job'ı yeniden kuyruğa ekler
    /// </summary>
    bool Requeue(string jobId);
}