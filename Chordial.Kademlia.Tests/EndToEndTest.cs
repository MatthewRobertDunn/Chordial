using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chordial.Kademlia.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Ninject.Parameters;
using Chordial.Kademlia.Network;

namespace Chordial.Kademlia.Tests
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
            List<IKadmeliaPeer> peers = new List<IKadmeliaPeer>();

            kernel.Bind<IKadmeliaServer>()
                .ToMethod
                (k =>
                    {
                        var uri = (Uri)k.Parameters.First().GetValue(k, k.Request.Target);
                        var index = int.Parse(uri.Host);
                        return peers[index].Server;
                    }
                ).Named("mem");



            var peer = kernel.Get<IKadmeliaPeer>(new ConstructorArgument("myServerUri", new Uri("mem://0")));
            peers.Add(peer);
            var rand = new Random();
            for (int i = 1; i < 1000; i++)
            {
                peer = kernel.Get<IKadmeliaPeer>(new ConstructorArgument("myServerUri", new Uri($"mem://{i}")));
                peers.Add(peer);
                peer.Client.Booststrap(new[] { "mem://0" });
            }


            byte[] key = new byte[20] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 };

            var putResult = peer.Client.Put(key, "You suck dicks", new TimeSpan(0, 15, 0), 5);


            //pick a random peer and try to get the data back

            peer = peers[rand.Next(peers.Count)];

            var result = peer.Client.Get(key);

            Assert.IsTrue(result.Values.Count == 1);
            Assert.IsTrue(result.NumberIterations <= 3);

            Assert.IsTrue(result.Values[0] == "You suck dicks");
            var closePeers = peers.Where(x => x.Myself.NodeId[0] == 255).ToList();
        }
    }
}
