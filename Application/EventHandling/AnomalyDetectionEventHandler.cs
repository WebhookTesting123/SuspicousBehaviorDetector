using Application.SuspiciousBehaviorAnalyzer;
using Domain;

namespace Application.EventHandling;

public class AnomalyDetectionEventHandler : EventHandler
{
    private readonly IEventAnalyzersInvoker _analyzersInvoker;
    
    public AnomalyDetectionEventHandler(IEventHandler nextHandler, IEventAnalyzersInvoker analyzersInvoker) : base(nextHandler)
    {
        _analyzersInvoker = analyzersInvoker;
    }

    protected override async Task<bool> HandleEvent(Event pushEvent)
    {
        await _analyzersInvoker.ExecuteAnalyzers(pushEvent);
        return true;
    }
}