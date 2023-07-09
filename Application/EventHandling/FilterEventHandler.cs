using Domain;

namespace Application.EventHandling;

public class FilterEventHandler : EventHandler
{
    private readonly List<string> _allowedEvents = new List<string>() { "repository", "team", "push" };
    public FilterEventHandler(IEventHandler nextHandler) : base(nextHandler)
    {
    }

    protected override Task<bool> HandleEvent(Event pushEvent)
    {
        return Task.FromResult(_allowedEvents.Contains(pushEvent.EventName));
    }
}