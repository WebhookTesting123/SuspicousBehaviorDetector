using Domain;

namespace Application.EventHandling;

public abstract class EventHandler : IEventHandler
{
    private readonly IEventHandler? _nextHandler;

    public EventHandler(IEventHandler nextHandler)
    {
        _nextHandler = nextHandler;
    }

    protected abstract Task<bool> HandleEvent(Event pushEvent);

    public async Task<bool> Handle(Event pushEvent)
    {
        if (!await HandleEvent(pushEvent))
            return false;
        if (_nextHandler != null)
            return await _nextHandler.Handle(pushEvent);
        return true;
    }
    
}