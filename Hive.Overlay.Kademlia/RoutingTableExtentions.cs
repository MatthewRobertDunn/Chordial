using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hive.Overlay.Kademlia
{
    public static class RoutingTableExtentions
    {

        public static List<NetworkContact> CloseContacts(this IRoutingTable _contactCache, int count, KadId target)
        {
            return _contactCache.CloseContacts(target)
                    .Take(count).ToList();
        }

        public static List<NetworkContact> CloseContacts(this IRoutingTable _contactCache, KadId target, KadId excluded)
        {
            return _contactCache.CloseContacts(target)
                    .Take(8 * KadId.ID_LENGTH)
                    .Where(x => x.Address != excluded)
                    .ToList();
        }

        public static void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
