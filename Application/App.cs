using Application.Configurations;

namespace Application;

public class App
{
    private readonly IEventFeedListener _eventFeedListener;
    private readonly IOutputProvider _outputProvider;
    private readonly IAppConfigurations _configurations;
    public App(IEventFeedListener eventFeedListener, IOutputProvider outputProvider, IAppConfigurations configurations)
    {
        _eventFeedListener = eventFeedListener;
        _outputProvider = outputProvider;
        _configurations = configurations;
    }

    public async Task Run()
    {
        try
        {
            await _outputProvider.NotifyMessage($"Listening on {_configurations.ConnectionString} ...");
            await _eventFeedListener.Listen(_configurations.ConnectionString);
        }
        catch (Exception e)
        {
            await _outputProvider.NotifyError("Error occured", e);
            throw;
        }
    }

}