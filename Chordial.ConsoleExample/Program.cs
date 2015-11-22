using Chordial.Kademlia;
using Chordial.Kademlia.Storage;
using Ninject;
using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chordial.ConsoleExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var kernel = new StandardKernel();

            kernel.Bind<IStorage>()
                .To<FrameworkCacheStorage>();

            //Because Kadmelia peer takes an IStorage as a constructor arguement ninject will auto provide it
            kernel.Bind<IKadmeliaPeer>().To<KadmeliaPeer>();


            var peer = kernel.Get<IKadmeliaPeer>(new ConstructorArgument("myServerUri", new Uri("mem://0")));




        }
    }
}
