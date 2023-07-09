using Application;
using Domain;

namespace Gateway.ConsolePrinter;

public class ConsoleOutput : IOutputProvider
{
    public Task NotifyMessage(string message)
    {
        Console.WriteLine(message);
        return Task.CompletedTask;
    }
    
    public Task NotifyEventViolations(Event pushEvent, IReadOnlyCollection<string> anomalies)
    {
        var message =
            $" ~ Anomalies detected in event of type {pushEvent.EventName}:\n";
        foreach (var anomaly in anomalies)
        {
            message += $"\t{anomaly}\n";
        }
        NotifyMessage(message);
        return Task.CompletedTask;
    }

    public Task NotifyError(string message, Exception exception)
    {
        Console.WriteLine($"{message}\n{exception}");
        return Task.CompletedTask;
    }
}