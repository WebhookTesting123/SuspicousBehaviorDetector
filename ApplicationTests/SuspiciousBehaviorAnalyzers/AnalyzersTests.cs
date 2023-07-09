using Application;
using Application.SuspiciousBehaviorAnalyzer.Analyzers;
using Domain;
using Moq;
using Xunit;

namespace ApplicationTests.SuspiciousBehaviorAnalyzers;

public class AnalyzersTests
{
    [Fact]
    public void DeletedRepositoryAnalyzer_SuspiciousEvent_ShouldReturnTrue()
    {
        var cacheMock = new Mock<ITimestampCache<Event>>();
        var repoId = "123";
        var baseTime = new DateTime(2023, 1, 1, 5, 5, 5);
        cacheMock.Setup(m => m.GetStartingFrom(It.IsAny<DateTime>()))
            .Returns(new List<Event>()
            {
                new()
                {
                    EventName = "repository", Action = "created",
                    Repository = new Repository() { Id = repoId, PushedAt = baseTime}
                }
            });
        var analyzer = new DeletedRepositoryAnalyzer(cacheMock.Object);

        var @event = new Event()
        {
            EventName = "repository", Action = "created",
            Repository = new Repository() { Id = repoId, PushedAt = baseTime + TimeSpan.FromMinutes(5) }
        };

        Assert.True(analyzer.IsSuspicious(@event));
    }
    
    [Fact]
    public void DeletedRepositoryAnalyzer_NoResultFromCache_ShouldReturnTrue()
    {
        var cacheMock = new Mock<ITimestampCache<Event>>();
        var repoId = "123";
        var baseTime = new DateTime(2023, 1, 1, 5, 5, 5);
        cacheMock.Setup(m => m.GetStartingFrom(It.IsAny<DateTime>()))
            .Returns(new List<Event>());
        var analyzer = new DeletedRepositoryAnalyzer(cacheMock.Object);

        var @event = new Event()
        {
            EventName = "repository", Action = "created",
            Repository = new Repository() { Id = repoId, PushedAt = baseTime + TimeSpan.FromMinutes(5) }
        };

        var result = analyzer.IsSuspicious(@event);
        
        Assert.False(result);
    }


    [Theory]
    [InlineData("hacker-123", true)]
    [InlineData("nothacker-123", false)]
    [InlineData("hello", false)]
    public void TeamNameAnalyzer_ShouldReturnCorrectResponse(string teamName, bool expectedResult)
    {
        var analyzer = new TeamNameAnalyzer();
        var pushedEvent = new Event() { Action = "created", EventName = "team", Team = new Team() { Name = teamName}};

        var result = analyzer.IsSuspicious(pushedEvent);
        
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void TimeOfDayPushAnalyzer_TimePushedOutOfRange_ShouldReturnFalse()
    {
        var analyzer = new TimeOfDayPushAnalyzer();
        var timePushed = new DateTime(2023, 1, 1, 13, 0, 0);
        var pushedEvent = new Event() { EventName = "push", Repository = new Repository() { PushedAt = timePushed } };

        var result = analyzer.IsSuspicious(pushedEvent);
        
        Assert.False(result);

    }
    
    [Fact]
    public void TimeOfDayPushAnalyzer_TimePushedInRange_ShouldReturnTrue()
    {
        var analyzer = new TimeOfDayPushAnalyzer();
        var timePushed = new DateTime(2023, 1, 1, 15, 0, 0);
        var pushedEvent = new Event() { EventName = "push", Repository = new Repository() { PushedAt = timePushed } };

        var result = analyzer.IsSuspicious(pushedEvent);
        
        Assert.True(result);

    }
}
