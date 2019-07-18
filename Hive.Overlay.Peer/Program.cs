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
                .UseUrls("http://0.0.0.0:5000")
                .UseKestrel(options => options.AddServerHeader = false)
                .UseStartup<Startup>();
    }
}
