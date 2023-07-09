using System.Net;
using Application;
using AutoMapper;
using Domain;
using Gateway.Github.Dtos;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using IEventHandler = Application.EventHandling.IEventHandler;

namespace Gateway.Github;

public class GithubEventListener : IEventFeedListener, IDisposable
{
    private readonly IMapper _mapper;
    private readonly IEventHandler _eventHandler;
    private readonly HttpListener _listener;
    
    private readonly string _githubEventHeaderName = "X-Github-Event";
    
    public GithubEventListener(IMapper mapper, IEventHandler eventHandler)
    {
        _mapper = mapper;
        _eventHandler = eventHandler;
        _listener = new HttpListener();
    }
    
    public async Task Listen(string connectionString)
    {
        
        _listener.Prefixes.Add(connectionString);
        _listener.Start();

        while (true)
        {
            try
            {
                var context = await _listener.GetContextAsync();
                ParseContext(context);
                context.Response.Close();
            }
            catch (ObjectDisposedException e)
            {
                Console.WriteLine("finished");
            }
        }
    }

    private async Task ParseContext(HttpListenerContext context)
    {
        var request = context.Request;
        var stringRequestBody = await GetRequestBody(request);
        var githubEvent = GetGithubEventFromHeaders(request);
    
        var jsonConvertSettings = new JsonSerializerSettings()
            { ContractResolver = new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() },};

        var eventDto = JsonConvert.DeserializeObject<EventDto>(stringRequestBody, jsonConvertSettings);
        if (eventDto == null)
            return;
        eventDto.EventName = githubEvent;
    
        await _eventHandler.Handle(_mapper.Map<Event>(eventDto));
    }

    private string GetGithubEventFromHeaders(HttpListenerRequest request)
    {
        var headers = request.Headers;
        if (headers.AllKeys.Contains(_githubEventHeaderName))
        {
            return headers.Get(_githubEventHeaderName)!;
        }

        throw new ArgumentException($"Key {_githubEventHeaderName} not found in headers");
    }
    
    private async Task<string> GetRequestBody(HttpListenerRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (request.ContentLength64 <= 0)
            return null;

        await using var stream = request.InputStream;
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    public Task Terminate()
    {
        Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _listener.Stop();
    }
}