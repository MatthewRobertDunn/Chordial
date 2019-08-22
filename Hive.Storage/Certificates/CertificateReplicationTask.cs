using Hive.Cryptography.Certificates;
using Hive.Overlay.Kademlia;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hive.Storage.Certificates
{
    public class CertificateReplicationTask : ReplicationTask
    {
        private readonly ICertificateRepository repo;
        private readonly ICertificateStore store;
        private readonly IKademilaClient client;

        public CertificateReplicationTask(ICertificateRepository repo, ICertificateStore store, IKademilaClient client)
        {
            this.repo = repo;
            this.repo = repo;
            this.store = store;
            this.client = client;
        }

    }
}
