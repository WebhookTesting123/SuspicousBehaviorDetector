using Domain;

namespace Application.SuspiciousBehaviorAnalyzer.Analyzers;

public class DeletedRepositoryAnalyzer : IEventAnalyzer
{
    private readonly ITimestampCache<Event> _eventsTimestampCache;
    private readonly int _suspiciousTimeInMinutes = 10;

    public DeletedRepositoryAnalyzer(ITimestampCache<Event> eventsTimestampCache)
    {
        _eventsTimestampCache = eventsTimestampCache;
    }

    public string AnomalyMessage => $"Repository created and deleted in less than {_suspiciousTimeInMinutes} minutes.";

    public bool IsSuspicious(Event pushEvent)
    {
        if (pushEvent.Action != "deleted" || pushEvent.EventName != "repository")
            return false;
        var relevantEvents =
            _eventsTimestampCache.GetStartingFrom(pushEvent.Repository.PushedAt - TimeSpan.FromMinutes(_suspiciousTimeInMinutes));

        return relevantEvents.Any(
            @event => @event.EventName == "repository" &&
                      @event.Repository.Id == pushEvent.Repository.Id &&
                      @event.Action == "created");


    }
}