using System;

namespace Chordial.Kademlia
{
    public class StorageItem
    {
        private DateTime? _publicationDate;
        private DateTime _expires;
        public byte[] Key { get; set; }
        public byte[] Hash { get; set; }
        public string Value { get; set; }

        public DateTime? PublicationDate
        {
            get { return _publicationDate; }
            set
            {
                if (value != null)
                    _publicationDate = DateTime.SpecifyKind(value.Value.ToUniversalTime(), DateTimeKind.Utc);
                else
                    _publicationDate = null;
            }
        }

        public DateTime Expires
        {
            get { return _expires; }
            set
            {
                _expires = DateTime.SpecifyKind(value.ToUniversalTime(), DateTimeKind.Utc);
            }
        }
    }
}