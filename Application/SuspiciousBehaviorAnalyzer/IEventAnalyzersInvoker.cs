using Domain;

namespace Application.SuspiciousBehaviorAnalyzer;

public interface IEventAnalyzersInvoker
{
    public Task ExecuteAnalyzers(Event pushEvent);
}