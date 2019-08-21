using Hive.Cryptography.Primitives;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Hive.Storage.Certificates
{
    public class CertificateRepository : ICertificateRepository
    {
        private const int ExpireDays = 365;
        private const int GraceDays = 30;
        private const int ReplicateHours = 24;

        private readonly LiteCollection<StoredPk12Cert> store;
        public CertificateRepository(LiteDatabase database)
        {
            this.store = database.GetCollection<StoredPk12Cert>();
        }

        /// <summary>
        /// Adds a certificate to this nodes store
        /// </summary>
        /// <param name="hiveId"></param>
        /// <param name="pk12Bytes"></param>
        public void AddCertificate(byte[] hiveId, byte[] pk12Bytes)
        {
            //Check if cert is already in this store
            var document = store.FindById(hiveId);
            if (document == null)
            {
                //create new
                document = new StoredPk12Cert()
                {
                    HiveId = hiveId,
                    Pk12Bytes = pk12Bytes
                };
            }
            else
            {
                //Update last access
                document.LastReplicated = DateTime.UtcNow;
            }

            //save in litedb
            store.Upsert(hiveId, document);
        }

        //Items are candidates for replication if they've been requested relatively recently and they haven't been
        //replicated by someone else in the past 24 hours.
        public List<StoredPk12Cert> GetItemsForReplication()
        {
            return store.Find(x => DateTime.UtcNow.AddDays(GraceDays - ExpireDays) < x.LastRequested)
                        .Where(x => DateTime.UtcNow.AddHours(-ReplicateHours) > x.LastReplicated)
                        .ToList();
        }

        public byte[] GetCertificate(byte[] hiveId)
        {
            var result = store.FindById(hiveId);
            if (result == null)
                return null;
            result.LastRequested = DateTime.UtcNow;

            //We use upsert here to avoid race conditions with RemoveOld
            store.Upsert(hiveId, result);
            return result.Pk12Bytes;
        }

        public void RemoveOld()
        {
            store.Delete(x => DateTime.UtcNow.AddDays(-365) > x.LastRequested);
        }
    }
}
