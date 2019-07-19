using Hive.Overlay.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hive.Overlay.Peer.Dto
{
    /// <summary>
    /// Base class all hive overly requests inherit from
    /// </summary>
    public class HiveRequest
    {
        /// <summary>
        /// Contact details for the node submitting the request
        /// </summary>
        public Contact RequestedBy { get; set; }
    }
}
