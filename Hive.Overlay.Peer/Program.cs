using Hive.Cryptography.Certificates;
using Hive.Cryptography.Primitives;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Open.Nat;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Hive.Overlay.Peer
{
    class Program
    {
        public static int Port { get; private set; }
        public static void Main(string[] args)
        {
            Port = 5000;
            if (args.Length > 0)
            {

                if (int.TryParse(args[0], out int port))
                {
                    Port = port;
                }

            }


            Console.WriteLine("Hive node starting...");
            Console.WriteLine("Forwarding port");
            PortForward();

            Console.WriteLine("Starting web server");
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options =>
                {
                    options.AddServerHeader = false;
                    options.Listen(System.Net.IPAddress.Any, Program.Port, configure =>
                     {
                         configure.UseHttps(Startup.CertificateStore.Transport.ToMicrosoftPrivate());

                     });
                })
                .UseStartup<Startup>();
        }

        public static async Task PortForward()
        {
            var discoverer = new NatDiscoverer();
            var cts = new CancellationTokenSource(10000);
            var device = await discoverer.DiscoverDeviceAsync(PortMapper.Upnp, cts);

            await device.CreatePortMapAsync(new Mapping(Protocol.Tcp, Program.Port, Program.Port, "The mapping name"));
        }
    }
}
