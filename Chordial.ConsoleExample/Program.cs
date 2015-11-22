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

            var peer = PeerFactory.CreateTcpPeer(2456);


            Console.ReadLine();
        }
    }
}
