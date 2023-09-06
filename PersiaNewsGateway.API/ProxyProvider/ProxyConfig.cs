namespace PersiaNewsGateway.API.ProxyProvider;

using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;

internal class ProxyConfig : IProxyConfig
{
    public IReadOnlyList<RouteConfig> Routes { get; private set; }
    public IReadOnlyList<ClusterConfig> Clusters { get; private set; }
    public IChangeToken ChangeToken { get; private set; }
    private readonly CancellationTokenSource _cancellationToken = new();

    public ProxyConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
    {
        Routes = routes;
        Clusters = clusters;
        ChangeToken = new CancellationChangeToken(_cancellationToken.Token);
    }

    public void Signal() => _cancellationToken.Cancel();
}


