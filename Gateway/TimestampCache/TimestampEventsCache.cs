using Application;
using Domain;

namespace Gateway.TimestampCache;

public class TimestampEventsCache : ITimestampCache<Event>
{
    private readonly Dictionary<DateTime, Event> _cache = new Dictionary<DateTime, Event>();
    
    public Task Add(DateTime pushTime, Event item)
    {
        _cache[pushTime] = item;
        return Task.CompletedTask;
    }

    public IEnumerable<Event> GetStartingFrom(DateTime pushedAfter)
    {
        return _cache.Where(kv => kv.Key > pushedAfter).Select(kv => kv.Value);
    }
}