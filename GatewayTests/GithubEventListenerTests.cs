using System.Text;
using Application.EventHandling;
using AutoMapper;
using Domain;
using Gateway.Github;
using Gateway.Github.Dtos;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace GatewayTests;

public class GithubEventListenerTests
{
   
    [Fact]
    public async Task Listen_WhenReceivingValidRequest_ShouldSendToEventHandler()
    {
        var localUri = "http://localhost:5099/";
        var mapperMock = new Mock<IMapper>();
        var eventHandlerMock = new Mock<IEventHandler>();
        var githubListener = new GithubEventListener(mapperMock.Object, eventHandlerMock.Object);

        var eventDto = new EventDto() { Action = "action", Repository = new RepositoryDto()
            { CreatedAt = DateTime.Now, PushedAt = DateTime.Now, Id = "id", Name = "name"} };
        var httpClient = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, localUri);
        request.Headers.Add("X-Github-Event", "repository");
        request.Content = new StringContent(JsonConvert.SerializeObject(eventDto), Encoding.UTF8, "application/json");
        
        githubListener.Listen(localUri);
        await Task.Delay(2000);
        await httpClient.SendAsync(request);
        await Task.Delay(2000);
        
        eventHandlerMock.Verify(m => m.Handle(It.IsAny<Event>()), Times.Once);
        mapperMock.Verify(m => m.Map<Event>(It.IsAny<EventDto>()), Times.Once());
    }
    
    [Fact]
    public async Task? Listen_WhenReceivingInvalidRequest_ShouldIgnore()
    {
        var localUri = "http://localhost:5077/";
        var mapperMock = new Mock<IMapper>();
        var eventHandlerMock = new Mock<IEventHandler>();
        var githubListener = new GithubEventListener(mapperMock.Object, eventHandlerMock.Object);

        var invalidRequest = "string request";
        var httpClient = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, localUri);
        request.Content = new StringContent(JsonConvert.SerializeObject(invalidRequest), Encoding.UTF8, "application/json");
        
        githubListener.Listen(localUri);
        await Task.Delay(2000);
        await httpClient.SendAsync(request);
        await Task.Delay(2000);
        
        eventHandlerMock.Verify(m => m.Handle(It.IsAny<Event>()), Times.Never);
        mapperMock.Verify(m => m.Map<Event>(It.IsAny<EventDto>()), Times.Never());
    }
    
}