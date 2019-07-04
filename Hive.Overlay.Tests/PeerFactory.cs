/*
using Chordial.Kademlia.Network;
using Chordial.Kademlia.Storage;
using Ninject;
using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Chordial.Kademlia
{
    public static class PeerFactory
    {
        public static Dictionary<int, ServiceHost> ActiveHosts = new Dictionary<int, ServiceHost>();
        public static IKadmeliaPeer CreateTcpPeer(int port)
        {
            var kernel = new StandardKernel();
            kernel.Bind<IStorage>().To<FrameworkCacheStorage>();
            kernel.Bind<IKadmeliaPeer>().To<KadmeliaPeer>();


            kernel.Bind<IKadmeliaServer>()
                    .ToMethod
                    (k =>
                    {
                        var uri = (Uri)k.Parameters.First().GetValue(k, k.Request.Target);
                        var binding = new NetTcpBinding(SecurityMode.None);
                        var endPoint = new EndpointAddress(uri);
                        return new KadmeliaWcfServerProxy(binding, endPoint);
                    }
                    ).Named("net.tcp");

            var externalUri = new Uri($"net.tcp://{IpUtils.GetExternalIp()}:{port}");
            var peer = kernel.Get<IKadmeliaPeer>(new ConstructorArgument("myServerUri", externalUri));

            var allIpUri = new Uri($"net.tcp://{Dns.GetHostName()}:{port}");
            var serviceHost = new ServiceHost(peer.Server, allIpUri);

            var endPoints = serviceHost.AddServiceEndpoint(typeof(IKadmeliaServer), new NetTcpBinding(), "");

            serviceHost.Open();
            ActiveHosts[port] = serviceHost;
            return peer;
        }
    }
}
*/