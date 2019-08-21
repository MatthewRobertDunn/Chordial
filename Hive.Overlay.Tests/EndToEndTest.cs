using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Ninject.Parameters;
using Hive.Overlay.Kademlia.Network;
using Hive.Overlay.Kademlia;
using Hive.Overlay.Api;

namespace Hive.Overlay.Tests
{
    [TestClass]
    public class EndToEndTest
    {
        [TestMethod]
        public void GetExternalIP()
        {
            var result = IpUtils.GetExternalIp();
            Assert.IsTrue(result != null);
        }


        [TestMethod]
        public void Test()
        {

            var kernel = new StandardKernel();

            kernel.Bind<IKadmeliaPeer>().To<KadmeliaPeer>();

            //TODO:
            //Here we'd bind in our transport provider


            //create our first peer
            var peers = new Dictionary<string, IKadmeliaPeer>();

            kernel.Bind<Func<Uri, IKadmeliaServer>>()
                .ToMethod
                ((c) =>
                    {
                        return (uri) =>
                       {
                           return peers[uri.ToString()].Server;
                       };
                    }
                );

            var peer = kernel.Get<IKadmeliaPeer>(new ConstructorArgument("myServerUri", new Uri("mem://0")));
            peers[peer.Contact.UriDefault.ToString()] = peer;
            var rand = new Random();
            for (int i = 1; i < 10000; i++)
            {
                peer = kernel.Get<IKadmeliaPeer>(new ConstructorArgument("myServerUri", new Uri($"mem://{i}")));
                peers[peer.Contact.UriDefault.ToString()] = peer;
                peer.Client.Booststrap(new[] { "mem://0" });
            }

            byte[] key = Enumerable.Repeat<byte>(255, KadId.ID_LENGTH).ToArray();
            var keyId = new KadId(key);

            //pick a random peer and try to get the data back

            peer = peers.Values.Skip(rand.Next(peers.Count)).First();

            var result = peer.Client.ClosestContacts(key);


            var closestTenExpected = peers.Values.OrderBy(x => x.Contact.Address ^ keyId).Take(10).Select(x => x.Contact.Address).ToList();
            var closestTen = result.ClosestPeers.Select(x => x.Address).Take(10).ToList();

            var closeCount = closestTenExpected.Intersect(closestTen).Count();

            Assert.IsTrue(closeCount > 8);
        }
    }
}
