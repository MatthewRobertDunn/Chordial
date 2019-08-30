using System;
using System.Collections.Generic;
using System.Text;

namespace Hive.Overlay.Peer.Dto
{
    public class StorePfxCertificate
    {
        public byte[] HiveId { get; set; }
        public byte[] Certificate { get; set; }
    }
}
