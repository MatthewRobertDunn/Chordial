using System.Collections.Generic;

namespace Hive.Storage.Certificates
{
    public interface ICertificateRepository
    {
        void AddCertificate(byte[] hiveId, byte[] pk12Bytes);
        byte[] GetCertificate(byte[] hiveId);
        List<StoredPk12Cert> GetItemsForReplication();
        void RemoveOld();
    }
}