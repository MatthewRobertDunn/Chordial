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



        public static void AddContact(this IBucketList _contactCache, Contact applicant, Contact myself, IKernel kernel)
        {
            Log("Processing contact for " + applicant.GetID().ToString());

            //Never add myself
            if (applicant.GetID() == myself.GetID())
                return;

            // If we already know about them
            if (_contactCache.Contains(applicant.GetID()))
            {
                // If they have a new address, record that
                if (_contactCache.Get(applicant.GetID()).Uri
                   != applicant.Uri)
                {
                    // Replace old one
                    _contactCache.Remove(applicant.GetID());
                    _contactCache.Put(applicant);
                }
                else
                { // Just promote them
                    _contactCache.Promote(applicant.GetID());
                }
                return;
            }

            // If we can fit them, do so
            Contact blocker = _contactCache.Blocker(applicant.GetID());
            if (blocker == null)
            {
                _contactCache.Put(applicant);
                return;
            }

            //has the blocker been pinged recently?
                        
            
            // We can't fit them. We have to choose between blocker and applicant
            var remotePeerUri = blocker.ToUri();
            var peer = kernel.Get<IKadmeliaServer>(remotePeerUri.Scheme, new ConstructorArgument("uri", remotePeerUri));

            // If the blocker doesn't respond, pick the applicant.
            var pingResult = peer.Ping(myself);
            if (pingResult == null)
            {
                _contactCache.Remove(blocker.GetID());
                _contactCache.Put(applicant);
                Log("Chose applicant");
            }
            //Log(contactCache.ToString());
        }

        public static void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
