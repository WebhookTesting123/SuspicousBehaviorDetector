using Autofac;
using AutoMapper;

namespace Bootstrap.Registrars;

public class AutoMapperRegistrar : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(_ => new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(Gateway.AssemblyAnchor));
        })).AsSelf().SingleInstance();

        builder.Register(c =>
        {
            var context = c.Resolve<IComponentContext>();
            var config = context.Resolve<MapperConfiguration>();
            return config.CreateMapper(context.Resolve);
        }).As<IMapper>().SingleInstance();

    }
}