using System;
using System.Collections.Generic;
using System.Text;

namespace Hive.Storage.Certificates
{
    public class StoredPk12Cert
    {
        public StoredPk12Cert()
        {
            Created = DateTime.UtcNow;
            LastRequested = DateTime.UtcNow;
            LastReplicated = DateTime.UtcNow;
        }
        public byte[] HiveId { get; set; }
        public byte[] Pk12Bytes { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastRequested { get; set; }
        public DateTime LastReplicated { get; set; }
    }
}
