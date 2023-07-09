using Application;
using Application.SuspiciousBehaviorAnalyzer;
using Domain;
using Moq;
using Xunit;

namespace ApplicationTests.SuspiciousBehaviorAnalyzers;

public class SuspiciousBehaviorAnalyzersInvokerTests
{
    [Fact]
    public async Task ExecuteAnalyzers_ProvidedAnalyzers_ExecutesAll()
    {
        var eventAnalyzerMock1 = new Mock<IEventAnalyzer>();
        var eventAnalyzerMock2 = new Mock<IEventAnalyzer>();
        
        var eventAnalyzers = new IEventAnalyzer[] { eventAnalyzerMock1.Object, eventAnalyzerMock2.Object };
        var invoker = new SuspiciousBehaviorAnalyzersInvoker(eventAnalyzers, Mock.Of<IOutputProvider>());

        await invoker.ExecuteAnalyzers(new Event());
        
        eventAnalyzerMock1.Verify(m => m.IsSuspicious(It.IsAny<Event>()), Times.Once);
        eventAnalyzerMock2.Verify(m => m.IsSuspicious(It.IsAny<Event>()), Times.Once);
    }
    
    [Fact]
    public async Task ExecuteAnalyzers_ProvidedAnalyzersWhenOneSuspicious_ExecutesAll()
    {
        var eventAnalyzerMock1 = new Mock<IEventAnalyzer>();
        var message = "message";
        var eventAnalyzerMock2 = new Mock<IEventAnalyzer>();

        var outputProviderMock = new Mock<IOutputProvider>();

        eventAnalyzerMock1.Setup(m => m.IsSuspicious(It.IsAny<Event>())).Returns(true);
        eventAnalyzerMock1.SetupGet(m => m.AnomalyMessage).Returns(message);
        eventAnalyzerMock2.Setup(m => m.IsSuspicious(It.IsAny<Event>())).Returns(false);

        var eventAnalyzers = new[] { eventAnalyzerMock1.Object, eventAnalyzerMock2.Object };
        var invoker = new SuspiciousBehaviorAnalyzersInvoker(eventAnalyzers, outputProviderMock.Object);

        await invoker.ExecuteAnalyzers(new Event());
        
        eventAnalyzerMock1.Verify(m => m.IsSuspicious(It.IsAny<Event>()), Times.Once);
        eventAnalyzerMock2.Verify(m => m.IsSuspicious(It.IsAny<Event>()), Times.Once);
        outputProviderMock.Verify(m => m.NotifyEventViolations(It.IsAny<Event>(),
            It.Is<IReadOnlyCollection<string>>(l => l.Count == 1 && l.First() == message)), Times.Once());
    }
    
}