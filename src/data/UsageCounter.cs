using Microsoft.EntityFrameworkCore;

namespace Cfth.Data;

[Owned]
public class UsageCounter
{
    private static readonly TimeSpan OneDay = TimeSpan.FromDays(1);
    private static readonly TimeSpan OneWeek = TimeSpan.FromDays(7);

    public UsageCounter()
    {
        this.OneDayUsageHistory = new List<DateTime>();
        this.OneWeekUsageHistory = new List<DateTime>();
        this.AllTime = 0;
    }

    public List<DateTime> OneDayUsageHistory { get; set; }

    public List<DateTime> OneWeekUsageHistory { get; set; }

    public ulong AllTime { get; set; }

    public ulong LastDay
    {
        get
        {
            TrimUsageHistory(this.OneDayUsageHistory, DateTime.Now, OneDay);
            return (ulong)this.OneDayUsageHistory.Count;
        }
    }

    public ulong LastWeek
    {
        get
        {
            TrimUsageHistory(this.OneWeekUsageHistory, DateTime.Now, OneWeek);
            return (ulong)this.OneWeekUsageHistory.Count;
        }
    }

    public void Increment()
    {
        DateTime now = DateTime.UtcNow;

        // Track this usage
        this.OneDayUsageHistory.Add(now);
        this.OneWeekUsageHistory.Add(now);
        this.AllTime++;

        // Clean up the lists
        TrimUsageHistory(this.OneDayUsageHistory, now, OneDay);
        TrimUsageHistory(this.OneWeekUsageHistory, now, OneWeek);
    }

    public override string ToString()
    {
        return $"Usage in last day={this.LastDay}, last week={this.LastWeek}, all time={this.AllTime}";
    }

    private static void TrimUsageHistory(
        List<DateTime> usageHistory,
        DateTime timestamp,
        TimeSpan maxAge)
    {
        usageHistory.RemoveAll((d) => timestamp - d > maxAge);
    }
}
