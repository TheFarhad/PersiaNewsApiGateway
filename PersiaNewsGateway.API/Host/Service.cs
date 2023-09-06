namespace PersiaNewsGateway.API.Host;

using Steeltoe.Discovery.Client;
using ProxyProvider;

public static class Service
{
    public static void Host(string[] args) => WebApplication.CreateBuilder(args).Services().Middlewares();

    public static WebApplication Services(this WebApplicationBuilder source)
    {
        var configuration = source.Configuration;
        source
            .Services
            .AddDiscoveryClient(configuration)
            .AddReverseProxy()
        //.LoadFromConfig(configuration.GetSection("ReverseProxy"))
        .LoadFromEureka(source.Services);

        return source.Build();
    }

    public static void Middlewares(this WebApplication source)
    {
        source.MapReverseProxy();
        source.Run();
    }
}
