using Domain;

namespace Application;

public interface IOutputProvider
{
    public Task NotifyMessage(string message);
    public Task NotifyEventViolations(Event pushEvent, IReadOnlyCollection<string> anomalies);
    public Task NotifyError(string message, Exception exception);
}