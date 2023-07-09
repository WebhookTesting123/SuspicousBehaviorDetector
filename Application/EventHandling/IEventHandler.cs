using Domain;

namespace Application.EventHandling;

public interface IEventHandler
{
    public Task<bool> Handle(Event pushEvent);
}