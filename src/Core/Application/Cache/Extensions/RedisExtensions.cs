namespace Application.Cache.Extensions;

/// <summary>
/// Redis için extension methodlar
/// </summary>
public static class RedisExtensions
{
    /// <summary>
    /// Redis'ten gelen bellek değerini parse eder
    /// </summary>
    public static long ParseMemory(this string memoryValue)
    {
        if (string.IsNullOrEmpty(memoryValue))
            return 0;

        return long.TryParse(memoryValue, out var result) ? result : 0;
    }
}