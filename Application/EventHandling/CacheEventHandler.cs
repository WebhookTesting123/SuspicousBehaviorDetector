using Domain;

namespace Application.EventHandling;

public class CacheEventHandler : EventHandler
{
    private readonly ITimestampCache<Event> _cache;
    private readonly IReadOnlyCollection<string> _eventNamesToCache = new[] { "repository" };
    public CacheEventHandler(IEventHandler nextHandler, ITimestampCache<Event> cache) : base(nextHandler)
    {
        _cache = cache;
    }

    protected override async Task<bool> HandleEvent(Event pushEvent)
    {
        if (_eventNamesToCache.Contains(pushEvent.EventName))
            await _cache.Add(pushEvent.Repository.PushedAt, pushEvent);
        
        return true;
    }
}