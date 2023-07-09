using Domain;

namespace Application.SuspiciousBehaviorAnalyzer.Analyzers;

public class TimeOfDayPushAnalyzer : IEventAnalyzer
{
    private readonly TimeSpan _fromHour = new(14, 0, 0);
    private readonly TimeSpan _toHour = new(16, 0, 0);
    
    public string AnomalyMessage => "Code pushed between 14:00 and 16:00";

    public bool IsSuspicious(Event pushEvent)
    {
        if (pushEvent.EventName != "push")
            return false;
        var pushedAt = pushEvent.Repository.PushedAt.TimeOfDay;
        return pushedAt > _fromHour && pushedAt < _toHour;
    }
}