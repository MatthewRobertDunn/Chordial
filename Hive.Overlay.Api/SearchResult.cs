using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Hive.Overlay.Api
{
    [DataContract]
    public class SearchResult
    {
        [DataMember]
        public Contact[] Contacts { get; set; }
        [DataMember]
        public string[] Values { get; set; }
    }
}