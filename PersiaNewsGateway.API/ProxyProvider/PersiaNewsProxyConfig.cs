namespace PersiaNewsGateway.API.ProxyProvider;

using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;

internal class PersiaNewsProxyConfig : IProxyConfig
{
    public IReadOnlyList<RouteConfig> Routes { get; }
    public IReadOnlyList<ClusterConfig> Clusters { get; }
    public IChangeToken ChangeToken { get; private set; }
    private readonly CancellationTokenSource _cancellationToken = new();

    public PersiaNewsProxyConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
    {
        Routes = routes;
        Clusters = clusters;
        ChangeToken = new CancellationChangeToken(_cancellationToken.Token);
    }

    public void Signal() => _cancellationToken.Cancel();
}


