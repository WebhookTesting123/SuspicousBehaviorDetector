using Application;
using Application.EventHandling;
using Application.SuspiciousBehaviorAnalyzer;
using Domain;
using Moq;
using Xunit;

namespace ApplicationTests.EventHandlers;

public class EventHandlersTests
{
    [Fact]
    public async Task FilterEventHandler_EventShouldBeKept_DoesntFilterEvent()
    {
        var nextHandler = new Mock<IEventHandler>();
        nextHandler.Setup(m => m.Handle(It.IsAny<Event>())).ReturnsAsync(true);
        
        var eventHandler = new FilterEventHandler(nextHandler.Object);
        var pushedEvent = new Event() { EventName = "repository" };
        
        var result = await eventHandler.Handle(pushedEvent);
        
        Assert.True(result);
        nextHandler.Verify(m => m.Handle(It.Is<Event>(e => e != null)));

    }
    
    [Fact]
    public async Task FilterEventHandler_IrrelevantEvent_BreakChainReturnFalse()
    {
        var nextHandler = new Mock<IEventHandler>();
        nextHandler.Setup(m => m.Handle(It.IsAny<Event>())).ReturnsAsync(true);
        
        var eventHandler = new FilterEventHandler(nextHandler.Object);
        var pushedEvent = new Event() { EventName = "irrelevant" };
        
        var result = await eventHandler.Handle(pushedEvent);
        
        Assert.False(result);
        nextHandler.Verify(m => m.Handle(It.IsAny<Event>()), Times.Never);

    }
    
    [Fact]
    public async Task CacheEventHandler_EventShouldBeKept_DoesntFilterEvent()
    {
        var nextHandler = new Mock<IEventHandler>();
        nextHandler.Setup(m => m.Handle(It.IsAny<Event>())).ReturnsAsync(true);
        var cacheMock = new Mock<ITimestampCache<Event>>();
        var pushedAt = new DateTime(2023, 1, 1);
        
        var eventHandler = new CacheEventHandler(nextHandler.Object, cacheMock.Object);
        var pushedEvent = new Event() { EventName = "repository", Repository = new Repository() { PushedAt = pushedAt }};
        
        var result = await eventHandler.Handle(pushedEvent);
        
        Assert.True(result);
        cacheMock.Verify(m => m.Add(It.Is<DateTime>(dt => dt == pushedAt), It.IsAny<Event>()));
        nextHandler.Verify(m => m.Handle(It.Is<Event>(e => e != null)));

    }
    
    [Fact]
    public async Task AnomalyDetectionEventHandler_EventShouldBeKept_DoesntFilterEvent()
    {
        var nextHandler = new Mock<IEventHandler>();
        nextHandler.Setup(m => m.Handle(It.IsAny<Event>())).ReturnsAsync(true);

        var invokerMock = new Mock<IEventAnalyzersInvoker>();
        
        var eventHandler = new AnomalyDetectionEventHandler(nextHandler.Object, invokerMock.Object);
        var pushedEvent = new Event();
        
        var result = await eventHandler.Handle(pushedEvent);
        
        Assert.True(result);
        nextHandler.Verify(m => m.Handle(It.Is<Event>(e => e != null)));
        invokerMock.Verify(m => m.ExecuteAnalyzers(It.IsAny<Event>()), Times.Once);

    }
    
}