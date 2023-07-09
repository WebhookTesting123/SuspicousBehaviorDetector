using Domain;

namespace Application;

public interface IEventAnalyzer
{
    public string AnomalyMessage { get; }
    public bool IsSuspicious(Event pushEvent);

}