
namespace Application;

public interface ITimestampCache<T>
{
    public Task Add(DateTime pushTime, T item);

    public IEnumerable<T> GetStartingFrom(DateTime pushedAfter);
}