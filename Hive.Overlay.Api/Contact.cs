using System.Runtime.Serialization;

namespace Hive.Overlay.Api
{
    [DataContract]
    public class Contact
    {
        [DataMember]
        public byte[] NodeId { get; set; }

        [DataMember]
        public string Uri { get; set; }
    }
}