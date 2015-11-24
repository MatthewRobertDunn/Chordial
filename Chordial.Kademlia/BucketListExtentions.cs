using Ninject;
using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chordial.Kademlia
{
    public static class BucketListExtentions
    {

        public static List<Contact> CloseContacts(this IBucketList _contactCache, int count, ID target)
        {
            return _contactCache.CloseContacts(target)
                    .Take(count).ToList();
        }


        public static List<Contact> CloseContacts(this IBucketList _contactCache, ID target, ID excluded)
        {
            return _contactCache.CloseContacts(target)
                    .Take(8 * ID.ID_LENGTH)
                    .Where(x => x.GetID() != excluded)
                    .ToList();
        }

               

        public static void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
