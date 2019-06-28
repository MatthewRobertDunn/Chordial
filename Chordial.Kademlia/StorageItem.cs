using System;

namespace Chordial.Kademlia
{
    public class StorageItem
    {
        private DateTime _publicationDate;
        private DateTime _expires;
        public byte[] Key { get; set; }
        public byte[] Hash { get; set; }
        public string Value { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime Expires { get; set; }
    }
}