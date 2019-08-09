using Hive.Cryptography.Certificates;
using Hive.Cryptography.Primitives;
using Hive.Overlay.Peer.Crypto;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Hive.Overlay.Peer
{
    class Program
    {
        public static CertificateStore CertificateStore { get; private set; }

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

            CertificateStore = new CertificateStore();


            try
            {
                CertificateStore.Load();
                Console.WriteLine($"Certificate store loaded, your Hive ID is {CertificateStore.HiveAddress.ToBase64()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Couldn't open certificate store {0}", ex.Message);
            }

            if (!CertificateStore.IsLoaded)
            {
                Console.WriteLine("Certificate store not loaded, generating new keys");
                CertificateStore.Generate();
                Console.WriteLine($"Generated, your new Hive ID is {CertificateStore.HiveAddress.ToBase64()}");
            }





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
                         configure.UseHttps(CertificateStore.Transport.ToMicrosoftPrivate());

                     });
                })
                .UseStartup<Startup>();
        }
    }
}
