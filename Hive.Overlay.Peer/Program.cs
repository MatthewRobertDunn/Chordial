using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;

namespace Hive.Overlay.Peer
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hive node starting...");
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options => options.AddServerHeader = false)
                .UseStartup<Startup>();
    }
}
