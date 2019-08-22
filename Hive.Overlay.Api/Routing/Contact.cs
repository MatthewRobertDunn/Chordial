using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Hive.Overlay.Api
{
    public class Contact
    {
        /// <summary>
        /// The 32byte Hive address this contact is for
        /// </summary>
        public byte[] Address { get; set; }

        /// <summary>
        /// A list of URIs this node can be contacted on.
        /// </summary>
        public string[] Uri { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}