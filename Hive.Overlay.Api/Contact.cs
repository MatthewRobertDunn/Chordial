using System.Runtime.Serialization;

namespace Hive.Overlay.Api
{
    public class Contact
    {
        /// <summary>
        /// 256bit Hive Address of the node this contact details are
        /// </summary>
        public byte[] Address { get; set; }

        public string[] Uri { get; set; }
    }
}