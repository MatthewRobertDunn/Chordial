using Hive.Cryptography.Certificates;
using Hive.Overlay.Kademlia;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hive.Storage.Certificates
{
    public class CertificateReplicationTask : ReplicationTask
    {
        public CertificateReplicationTask(ICertificateRepository repo, ICertificateStore store, IKademilaClient client)
        {
        }
    }
}
