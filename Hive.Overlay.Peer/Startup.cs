using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hive.Cryptography.Certificates;
using Hive.Overlay.Api;
using Hive.Overlay.Kademlia;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Hive.Cryptography.Primitives;
using Hive.Overlay.Peer.Crypto;
using Hive.Overlay.Kademlia.Network;
using Hive.Overlay.Peer.Settings;
using Microsoft.Extensions.Hosting;
using Hive.Overlay.Peer.Tasks;
using LiteDB;
using Hive.Storage.Certificates;

namespace Hive.Overlay.Peer
{
    public class Startup
    {
        public static CertificateStore CertificateStore { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });

                var filePath = Path.Combine(System.AppContext.BaseDirectory, "Hive.Overlay.Peer.xml");
                c.IncludeXmlComments(filePath);
            });

            ConfigureSettings(services);
            ConfigureCertificateStore(services);
            ConfigureMyAddress(services);

            services.AddSingleton<IRoutingTable, RoutingTable>();
            services.AddSingleton<IKadmeliaServer, KademliaServer>();

            //rest client proxy
            Func<Uri, IKadmeliaServer> serverFactory = uri => new RestClient(uri);
            services.AddSingleton(serverFactory);

            services.AddSingleton<IKademilaClient, KademliaClient>();

            //database
            services.AddSingleton<LiteDatabase>(new LiteDatabase(@"Hive.db"));

            services.AddSingleton<ICertificateRepository, CertificateRepository>();

            //background tasks
            services.AddSingleton<IHostedService, BootsrapPeer>();
            services.AddSingleton<IHostedService, CertificateReplication>();
        }

        private void ConfigureSettings(IServiceCollection services)
        {
            var peerSetting = Configuration.GetSection("Peer").Get<PeerSettings>();
            services.AddSingleton(peerSetting);
        }

        private void ConfigureMyAddress(IServiceCollection services)
        {
            //network contact for local server instance
            var kadId = new KadId(CertificateStore.HiveAddress);
            var myUrl = new Uri($"http://{IpUtils.GetExternalIp()}:{Program.Port}/");
            var myself = new NetworkContact(kadId, new[] { myUrl });
            services.AddSingleton(myself);
            
        }

        private void ConfigureCertificateStore(IServiceCollection services)
        {
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
                CertificateStore.Save();
                Console.WriteLine($"Generated, your new Hive ID is {CertificateStore.HiveAddress.ToBase64()}");
            }

            services.AddSingleton<ICertificateStore>(CertificateStore);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }
    }
}
