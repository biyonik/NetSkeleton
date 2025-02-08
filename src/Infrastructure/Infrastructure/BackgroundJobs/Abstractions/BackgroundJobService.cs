using System.Linq.Expressions;
using Hangfire;
using Hangfire.States;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundJobs.Abstractions;

/// <summary>
/// Background job servisi implementasyonu
/// </summary>
public class BackgroundJobService(
    IBackgroundJobClient backgroundJobs,
    IRecurringJobManager recurringJobs,
    ILogger<BackgroundJobService> logger)
    : IBackgroundJobService
{
    /// <summary>
    /// Fire-and-forget job ekler
    /// </summary>
    public string Enqueue<T>(Expression<Action<T>> methodCall) where T : class
    {
        try
        {
            return backgroundJobs.Enqueue(methodCall);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error enqueueing job for {Type}", typeof(T).Name);
            throw;
        }
    }

    /// <summary>
    /// Fire-and-forget job ekler (belirli queue'ya)
    /// </summary>
    public string Enqueue<T>(Expression<Action<T>> methodCall, string queue) where T : class
    {
        try
        {
            return backgroundJobs.Create(methodCall, new EnqueuedState(queue));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error enqueueing job for {Type} to queue {Queue}", typeof(T).Name, queue);
            throw;
        }
    }

    /// <summary>
    /// Zamanlanmış job ekler
    /// </summary>
    public string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay) where T : class
    {
        try
        {
            return backgroundJobs.Schedule(methodCall, delay);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error scheduling job for {Type} with delay {Delay}", typeof(T).Name, delay);
            throw;
        }
    }

    /// <summary>
    /// Zamanlanmış job ekler (belirli queue'ya)
    /// </summary>
    public string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay, string queue) where T : class
    {
        try
        {
            return backgroundJobs.Create(methodCall, new ScheduledState(delay)
            {
                Reason = queue
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error scheduling job for {Type} with delay {Delay} to queue {Queue}", 
                typeof(T).Name, delay, queue);
            throw;
        }
    }

    /// <summary>
    /// Belirli bir zamanda çalışacak job ekler
    /// </summary>
    public string Schedule<T>(Expression<Action<T>> methodCall, DateTimeOffset enqueueAt) where T : class
    {
        try
        {
            return backgroundJobs.Schedule(methodCall, enqueueAt);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error scheduling job for {Type} at {EnqueueAt}", typeof(T).Name, enqueueAt);
            throw;
        }
    }

    /// <summary>
    /// Recurring job ekler
    /// </summary>
    public void RecurringJob<T>(string jobId, Expression<Action<T>> methodCall, string cronExpression, string queue = "default") where T : class
    {
        try
        {
            recurringJobs.AddOrUpdate(jobId, methodCall, cronExpression, new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc,
                QueueName = queue
            });

            logger.LogInformation("Added recurring job {JobId} for {Type} with cron {Cron} to queue {Queue}", 
                jobId, typeof(T).Name, cronExpression, queue);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding recurring job {JobId} for {Type}", jobId, typeof(T).Name);
            throw;
        }
    }

    /// <summary>
    /// Recurring job'ı kaldırır
    /// </summary>
    public void RemoveRecurringJob(string jobId)
    {
        try
        {
            recurringJobs.RemoveIfExists(jobId);
            logger.LogInformation("Removed recurring job {JobId}", jobId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing recurring job {JobId}", jobId);
            throw;
        }
    }

    /// <summary>
    /// Job'ı iptal eder
    /// </summary>
    public bool Delete(string jobId)
    {
        try
        {
            return backgroundJobs.Delete(jobId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting job {JobId}", jobId);
            throw;
        }
    }

    /// <summary>
    /// Job'ı yeniden kuyruğa ekler
    /// </summary>
    public bool Requeue(string jobId)
    {
        try
        {
            return backgroundJobs.Requeue(jobId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error requeueing job {JobId}", jobId);
            throw;
        }
    }
}