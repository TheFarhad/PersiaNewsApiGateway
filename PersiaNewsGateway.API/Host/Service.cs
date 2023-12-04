namespace PersiaNewsGateway.API.Host;

using PersiaNewsGateway.API.Extentions;
using ProxyProvider;
using Steeltoe.Discovery.Client;

public static class Service
{
    public static void Host(string[] args) => WebApplication.CreateBuilder(args).Services().Pipeline();

    public static WebApplication Services(this WebApplicationBuilder source)
    {
        source
            .Services
            .AddDiscoveryClient() //source.Configuration
            .AddReverseProxy()
           .LoadFromEureka(source.Services);

        return source.Build();
    }

    public static void Pipeline(this WebApplication source)
    {
        source.MapReverseProxy();
        source.Run();
    }
}
