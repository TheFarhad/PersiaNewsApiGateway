namespace PersiaNewsGateway.API.ProxyProvider;

using System.Threading;
using System.Threading.Tasks;
using Steeltoe.Discovery;
using Steeltoe.Discovery.Eureka;
using Yarp.ReverseProxy.Configuration;

internal class PersiaNewsProxyConfigProvider : BackgroundService, IProxyConfigProvider
{
    private PersiaNewsProxyConfig _proxyConfig;
    private List<RouteConfig> _routesConfig;
    private readonly DiscoveryClient _discoveryClient;

    public PersiaNewsProxyConfigProvider(IDiscoveryClient client)
    {
        _discoveryClient = (DiscoveryClient)client; //client as DiscoveryClient
        RoutesConfig();
        PopulateConfig();
    }

    public IProxyConfig GetConfig() => _proxyConfig;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            PopulateConfig();
            await Task.Delay(30000, stoppingToken);
        }
    }

    private void RoutesConfig()
    {
        _routesConfig = new()
        {
            new RouteConfig
            {
                RouteId = "BasicInfoRoute",
                ClusterId = "BASICINFO",
                Match = new RouteMatch  { Path = "bi/{**catch-all}"  },
                Transforms = new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string> {{ "PathPattern", "{**catch-all}" }}
                }
            },
            new RouteConfig
            {
                 RouteId = "NewsCMSRoute",
                ClusterId = "NEWSCMS",
                Match = new RouteMatch { Path = "news/{**catch-all}" },
                Transforms = new List<Dictionary<string, string>>
                {
                   new Dictionary<string, string> {{ "PathPattern", "{**catch-all}" }}
                }
            }
        };
    }

    private void PopulateConfig()
    {
        var _clustersConfig = new List<ClusterConfig>();

        _discoveryClient
            .Applications
            .GetRegisteredApplications()
            .ToList()
            .ForEach(_ =>
            {
                _clustersConfig.Add(new ClusterConfig
                {
                    LoadBalancingPolicy = LoadBalancingPolicy.RoundRobin,
                    ClusterId = _.Name,
                    Destinations = _.Instances.Select(__ => new
                    {
                        Id = __.InstanceId,
                        config = new DestinationConfig
                        {
                            Address = $"http://{__.HostName}:{__.Port}"
                        }
                    }).ToDictionary(e => e.Id, e => e.config)
                });
            });

        var temp = _proxyConfig;
        _proxyConfig = new(_routesConfig, _clustersConfig);
        temp?.Signal();
    }
}
