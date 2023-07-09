using Domain;

namespace Application.SuspiciousBehaviorAnalyzer.Analyzers;

public class TeamNameAnalyzer  : IEventAnalyzer
{
    private readonly string _suspiciousPrefix = "hacker";

    public string AnomalyMessage => $"Team name starts with {_suspiciousPrefix}.";

    public bool IsSuspicious(Event pushEvent)
    {
        return pushEvent.EventName == "team" &&
               pushEvent.Action == "created" &&
               pushEvent.Team.Name.StartsWith(_suspiciousPrefix);
    }
}