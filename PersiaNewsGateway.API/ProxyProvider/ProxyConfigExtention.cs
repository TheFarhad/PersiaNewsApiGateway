namespace PersiaNewsGateway.API.ProxyProvider;

using Yarp.ReverseProxy.Configuration;

public static class ProxyConfigExtention
{
    // Is this true?
    public static IServiceCollection LoadFromEureka(this IReverseProxyBuilder source, IServiceCollection service)
    {
        service.AddSingleton<ProxyConfigProvider>();

        service
            .AddSingleton<IProxyConfigProvider>(_ => _.GetRequiredService<ProxyConfigProvider>());

        service
            .AddSingleton<IHostedService>(_ => _.GetRequiredService<ProxyConfigProvider>());

        return service;
    }
}
