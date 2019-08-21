using Hive.Cryptography.Certificates;
using Hive.Storage.Certificates;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hive.Overlay.Peer.Tasks
{
    public class CertificateReplication : BackgroundService
    {
        private readonly ICertificateRepository repo;
        private readonly ICertificateStore store;

        public CertificateReplication(ICertificateRepository repo, ICertificateStore store)
        {
            this.repo = repo;
            this.store = store;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //Ensure our own certificate is in the repo.
            repo.AddCertificate(store.HiveAddress, store.ToPublicPfxBytes());


            return Task.CompletedTask;
        }
    }
}
