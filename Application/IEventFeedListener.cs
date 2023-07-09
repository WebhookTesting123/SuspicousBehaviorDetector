namespace Application;

public interface IEventFeedListener
{
    public Task Listen(string connectionString);
}