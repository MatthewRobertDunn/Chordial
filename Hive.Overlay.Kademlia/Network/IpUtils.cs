using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hive.Overlay.Kademlia.Network
{
    public class IpUtils
    {
        public static string GetExternalIp()
        {
            var req = new WebClient();
            string[] a = req.DownloadString("http://checkip.dyndns.org").Split(':');
            string a2 = a[1].Substring(1);
            string[] a3 = a2.Split('<');
            string a4 = a3[0];
            return a4;
        }
    }
}
