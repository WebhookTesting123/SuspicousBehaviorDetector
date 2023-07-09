using Domain;
using Gateway.TimestampCache;
using Xunit;

namespace GatewayTests;

public class TimestampCacheTests
{
    [Fact]
    public async Task Add_HappyFlow()
    {
        var cache = new TimestampEventsCache();
        var time = new DateTime(2023, 1, 1, 12, 0, 0);
        var pushedEvent = new Event() { Repository = new Repository() { PushedAt = time } };
        
        await cache.Add(time, pushedEvent);

        var pulled = cache.GetStartingFrom(time - TimeSpan.FromHours(1)).ToList();
        Assert.Single(pulled);
        Assert.Equal(time, pulled.First().Repository.PushedAt);

    }
    
    [Fact]
    public async Task GetStartingFrom_NoEventsInRange_EmptyResults()
    {
        var cache = new TimestampEventsCache();
        var time = new DateTime(2023, 1, 1, 12, 0, 0);
        var pushedEvent = new Event() { Repository = new Repository() { PushedAt = time } };
        
        await cache.Add(time, pushedEvent);

        var pulled = cache.GetStartingFrom(time + TimeSpan.FromHours(1)).ToList();
        Assert.Empty(pulled);

    }
}