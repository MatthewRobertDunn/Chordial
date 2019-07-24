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
using Hive.Overlay.Kademlia.Storage;
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

            kernel.Bind<IStorage>().To<SimpleStorage>();
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
            peers[peer.Myself.UriDefault.ToString()] = peer;
            var rand = new Random();
            for (int i = 1; i < 1000; i++)
            {
                peer = kernel.Get<IKadmeliaPeer>(new ConstructorArgument("myServerUri", new Uri($"mem://{i}")));
                peers[peer.Myself.UriDefault.ToString()] = peer;
                peer.Client.Booststrap(new[] { "mem://0" });
            }

            byte[] key = Enumerable.Repeat<byte>(255, KadId.ID_LENGTH).ToArray();


            //pick a random peer and try to get the data back

            peer = peers.Values.Skip(rand.Next(peers.Count)).First();

            var result = peer.Client.ClosestContacts(key);

            var closeCount = result.ClosestPeers.Where(x => x.Id.Data[0] == 255).Count();
            var closeExpected = peers.Values.Where(x => x.Myself.Id.Data[0] == 255).Count();

            Assert.AreEqual(closeExpected, closeCount);
        }
    }
}
