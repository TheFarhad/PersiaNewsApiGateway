namespace PersiaNewsGateway.API.Extentions;

using Yarp.ReverseProxy.Configuration;
using ProxyProvider;

public static class ProxyConfigExtention
{
    public static IServiceCollection LoadFromEureka(this IReverseProxyBuilder source, IServiceCollection service)
    {
        service.AddSingleton<PersiaNewsProxyConfigProvider>();

        service
            .AddSingleton<IProxyConfigProvider>(_ => _.GetRequiredService<PersiaNewsProxyConfigProvider>());

        service
            .AddSingleton<IHostedService>(_ => _.GetRequiredService<PersiaNewsProxyConfigProvider>());

        return service;
    }
}
