namespace PersiaNewsGateway.API.ProxyProvider;

using Steeltoe.Discovery;
using Steeltoe.Discovery.Eureka;
using System.Threading;
using System.Threading.Tasks;
using Yarp.ReverseProxy.Configuration;

internal class ProxyConfigProvider : BackgroundService, IProxyConfigProvider
{
    private ProxyConfig _config;
    private List<RouteConfig> _routes;
    private readonly DiscoveryClient _client;

    public ProxyConfigProvider(IDiscoveryClient client)
    {
        _client = client as DiscoveryClient;
        SetRoutes();
        PopulateConfig();
    }

    public IProxyConfig GetConfig() => _config;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!stoppingToken.IsCancellationRequested)
        {
            PopulateConfig();
            await Task.Delay(30000, stoppingToken);
        }
    }

    private void SetRoutes()
    {
        _routes = new()
        {
            new RouteConfig
            {
                RouteId = "BasicInfoRoute",
                ClusterId = "BasicInfo",
                Match = new RouteMatch  { Path = "bi/{**catch-all}"  },
                Transforms = new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string> {{ "PathPattern", "{**catch-all}" }}
                }
            },
            new RouteConfig
            {
                 RouteId = "NewsCMSRoute",
                ClusterId = "NewsCMS",
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
        var clusters = new List<ClusterConfig>();

        _client
            .Applications
            .GetRegisteredApplications()
            .ToList()
            .ForEach(_ =>
            {
                clusters.Add(new ClusterConfig
                {
                    LoadBalancingPolicy = "RoundRobin",
                    ClusterId = _.Name,
                    Destinations = _.Instances.Select(e => new
                    {
                        Id = e.InstanceId,
                        config = new DestinationConfig
                        {
                            Address = $"http://{e.HostName}:{e.Port}"
                        }
                    }).ToDictionary(e => e.Id, e => e.config)
                });
            });

        var temp = _config;
        _config = new(_routes, clusters);
        temp?.Signal();
    }
}


