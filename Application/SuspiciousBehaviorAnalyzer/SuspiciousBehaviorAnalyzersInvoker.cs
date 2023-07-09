using Domain;

namespace Application.SuspiciousBehaviorAnalyzer;

public class SuspiciousBehaviorAnalyzersInvoker: IEventAnalyzersInvoker
{
    private readonly IReadOnlyCollection<IEventAnalyzer> _eventAnalyzers;
    private readonly IOutputProvider _outputProvider;

    public SuspiciousBehaviorAnalyzersInvoker(IReadOnlyCollection<IEventAnalyzer> eventAnalyzers, IOutputProvider outputProvider)
    {
        _eventAnalyzers = eventAnalyzers;
        _outputProvider = outputProvider;
    }

    public async Task ExecuteAnalyzers(Event pushEvent)
    {
        var anomalies = _eventAnalyzers.Where(eventAnalyzer => eventAnalyzer.IsSuspicious(pushEvent))
            .Select(eventAnalyzer => eventAnalyzer.AnomalyMessage).ToList();

        if (anomalies.Any())
        {
            await _outputProvider.NotifyEventViolations(pushEvent, anomalies.ToList());
        }
    }
}