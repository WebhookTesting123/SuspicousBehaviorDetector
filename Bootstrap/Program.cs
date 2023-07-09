using Application;
using Application.Configurations;
using Application.SuspiciousBehaviorAnalyzer;
using Application.SuspiciousBehaviorAnalyzer.Analyzers;
using Autofac;
using Bootstrap.Registrars;
using Domain;
using Gateway.ConsolePrinter;
using Gateway.Github;
using Gateway.TimestampCache;

namespace Bootstrap;

static class Program
{
    private static IContainer ContainerBuilder(string[] args)
    {
        var builder = new ContainerBuilder();
        var connectionString = args.Length > 0 ? args[0] : "http://localhost:80/";

        builder.RegisterInstance(new AppConfigurations() { ConnectionString = connectionString })
            .As<IAppConfigurations>();

        builder.RegisterType<App>();
        builder.RegisterType<TimestampEventsCache>().As<ITimestampCache<Event>>().SingleInstance();
        builder.RegisterType<GithubEventListener>().As<IEventFeedListener>();
        builder.RegisterType<ConsoleOutput>().As<IOutputProvider>().SingleInstance();
        builder.RegisterType<SuspiciousBehaviorAnalyzersInvoker>().As<IEventAnalyzersInvoker>();

        builder.RegisterType<DeletedRepositoryAnalyzer>().As<IEventAnalyzer>();
        builder.RegisterType<TeamNameAnalyzer>().As<IEventAnalyzer>();
        builder.RegisterType<TimeOfDayPushAnalyzer>().As<IEventAnalyzer>();

        builder.RegisterModule<AutoMapperRegistrar>();
        builder.RegisterModule<EventHandlersRegistrar>();
        
        return builder.Build();
    }

    public static async Task Main(string[] args)
    {
        await ContainerBuilder(args).Resolve<App>().Run();
    }
}