using System;
using System.Collections.Generic;
using System.Text;

namespace Hive.Overlay.Peer.Dto
{
    /// <summary>
    /// Request message sent to perform a closest nodes search
    /// </summary>
    public class ClosestNodeSearch : HiveRequest
    {
        /// <summary>
        ///  Find nodes closest to this network address.
        ///  Key should be a 256bit base64 encoded hive network address
        /// </summary>
        public byte[] Address { get; set; }
    }
}
