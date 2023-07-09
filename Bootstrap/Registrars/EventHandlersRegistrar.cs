using Application.EventHandling;
using Autofac;

namespace Bootstrap.Registrars;

public class EventHandlersRegistrar : Module
{
    protected override void Load(ContainerBuilder builder)
    {

        var handlers = new List<Type> { typeof(FilterEventHandler), typeof(CacheEventHandler), typeof(AnomalyDetectionEventHandler) };
        RegisterHandlers(builder, handlers);

    }
    
    
    private static void RegisterHandlers(ContainerBuilder builder, List<Type> handlerTypes)
    {

        for (var i = handlerTypes.Count - 1; i >= 0; i--)
        {
            var handlerType = handlerTypes[i];
            var index = i;   
            var parameterName = handlerType.GetConstructors().First().GetParameters()
                .FirstOrDefault(p => p.ParameterType == typeof(IEventHandler))?.Name;
            

            if (i == handlerTypes.Count - 1)
            {
                builder.RegisterType(handlerType)
                    .WithParameter(
                        (parameter, _) => parameter.Name == parameterName,
                        (_, _) => null
                    );
            }
            else if (i != 0)
            {
                
                builder.RegisterType(handlerType)
                    .WithParameter(
                        (parameter, _) => parameter.Name == parameterName,
                        (_, ctx) => ctx.Resolve(handlerTypes[index + 1]));
            }
            else
            {
                builder.RegisterType(handlerType).As<IEventHandler>().WithParameter(
                    (parameter, _) => parameter.Name == parameterName,
                    (_, ctx) => ctx.Resolve(handlerTypes[index + 1])
                );
            }
        }
        
    }
    

}