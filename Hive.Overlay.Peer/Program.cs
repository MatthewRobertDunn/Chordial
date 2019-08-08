using Hive.Overlay.Peer.Crypto;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Hive.Overlay.Peer
{
    class Program
    {
        private static X509Certificate2 Certificate;
        public static void Main(string[] args)
        {
            Console.WriteLine("Hive node starting...");
            Console.WriteLine("Starting web server");
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options =>
                {
                    options.AddServerHeader = false;
                    options.Listen(System.Net.IPAddress.Any, 5000, configure =>
                     {
                         configure.UseHttps(Certificate);

                     });
               })
                .UseStartup<Startup>();
        }
    }
}
