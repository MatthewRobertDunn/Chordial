using System;
using System.Collections.Generic;
using System.Text;

namespace Hive.Overlay.Peer.Dto
{
    public class FindRequest : KadRequest
    {
        /// <summary>
        /// The key to search for
        /// </summary>
        public byte[] Key { get; set; }
    }
}
