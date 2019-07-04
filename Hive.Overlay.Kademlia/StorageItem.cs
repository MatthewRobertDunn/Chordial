using System;

namespace Hive.Overlay.Kademlia
{
    public class StorageItem
    {
        public byte[] Key { get; set; }
        public byte[] Hash { get; set; }
        public string Value { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime Expires { get; set; }
    }
}