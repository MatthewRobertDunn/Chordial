using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Chordial.Kademlia
{
    /// <summary>
    /// A list of contacts.
    /// Also responsible for storing last lookup times for buckets, so we can refresh them.
    /// Not thread safe for multiple people writing at once, since you can't enforce preconditions.
    /// </summary>
    public class BucketList : IBucketList
    {
        private const int BUCKET_SIZE = 20; // "K" in the spec
        private const int NUM_BUCKETS = 8 * ID.ID_LENGTH; // One per bit in an ID

        private List<List<Contact>> buckets;
        private List<DateTime> accessTimes; // last bucket write or explicit touch
        private ID ourID;
        private Contact myself;
        /// <summary>
        /// Make a new bucket list, for holding node contacts.
        /// </summary>
        /// <param name="ourID">The ID to center the list on.</param>
        public BucketList(Contact ourID)
        {
            this.ourID = ourID.GetID();
            this.myself = ourID;
            buckets = new List<List<Contact>>(NUM_BUCKETS);
            accessTimes = new List<DateTime>();

            // Set up each bucket
            for (int i = 0; i < NUM_BUCKETS; i++)
            {
                buckets.Add(new List<Contact>(BUCKET_SIZE));
                accessTimes.Add(default(DateTime));
            }
        }

        private ID OurId
        {
            get { return ourID; }
        }

        public Contact OurContact
        {
            get { return this.myself; }
        }

        /// <summary>
        /// Returns what contact is blocking insertion (least promoted), or null if no contact is.
        /// </summary>
        /// <param name="toAdd"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Contact Blocker(ID toAdd)
        {
            int bucket = BucketFor(toAdd);
            if (buckets[bucket].Count < BUCKET_SIZE)
            {
                return null;
            }
            else
            {
                return buckets[bucket][0];
            }
        }

        /// <summary>
        /// See if we have a contact with the given ID.
        /// </summary>
        /// <param name="toCheck"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool Contains(ID toCheck)
        {
            return this.Get(toCheck) != null;
        }

        /// <summary>
        /// Add the given contact at the end of its bucket.
        /// PRECONDITION: Won't over-fill bucket.
        /// </summary>
        /// <param name="toAdd"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Put(Contact toAdd)
        {
            if (toAdd == null)
            {
                return; // Don't be silly.
            }

            int bucket = BucketFor(toAdd.GetID());
            buckets[bucket].Add(toAdd); // No lock: people can read while we do this.
            accessTimes[bucket] = DateTime.Now;
        }

        /// <summary>
        /// Report that a lookup was done for the given key.
        /// Key must not match our ID.
        /// </summary>
        /// <param name="key"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Touch(ID key)
        {
            accessTimes[BucketFor(key)] = DateTime.Now;
        }

        /// <summary>
        /// Return the contact with the given ID, or null if it's not found.
        /// </summary>
        /// <param name="toGet"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Contact Get(ID toGet)
        {
            int bucket = BucketFor(toGet);
            for (int i = 0; i < buckets[bucket].Count; i++)
            {
                if (buckets[bucket][i].GetID() == toGet)
                {
                    return buckets[bucket][i];
                }
            }
            return null;
        }

        /// <summary>
        /// Return how many contacts are cached.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int GetCount()
        {
            int found = 0;
            // Just enumerate all the buckets and sum counts
            for (int i = 0; i < NUM_BUCKETS; i++)
            {
                found = found + buckets[i].Count;
            }

            return found;
        }

        /// <summary>
        /// Move the contact with the given ID to the front of its bucket.
        /// </summary>
        /// <param name="toPromote"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Promote(ID toPromote)
        {
            Contact promotee = Get(toPromote);
            int bucket = BucketFor(toPromote);
            buckets[bucket].Remove(promotee); // Take out
            buckets[bucket].Add(promotee); // And put in at end
            accessTimes[bucket] = DateTime.Now;
        }

        /// <summary>
        /// Remove a contact.
        /// </summary>
        /// <param name="toRemove"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Remove(ID toRemove)
        {
            int bucket = BucketFor(toRemove);
            for (int i = 0; i < buckets[bucket].Count; i++)
            {
                if (buckets[bucket][i].GetID() == toRemove)
                {
                    buckets[bucket].RemoveAt(i);
                    return;
                }
            }
        }

        
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IList<Contact> CloseContacts(ID target)
        {
            return AllContacts.OrderBy(x => x.GetID() ^ target)
                              .ToList();
        }

        private IEnumerable<Contact> AllContacts
        {
            get
            {
                return buckets.SelectMany(x => x).Concat(new[] { this.myself });
            }
        }


        /// <summary>
        /// Returns what bucket an ID maps to.
        /// PRECONDITION: ourID not passed.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private int BucketFor(ID id)
        {
            return (OurId.DifferingBit(id));
        }

        /// <summary>
        /// A ToString for debugging.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string toReturn = "BucketList:";
            for (int i = 0; i < NUM_BUCKETS; i++)
            {
                List<Contact> bucket = buckets[i];
                lock (bucket)
                {
                    if (bucket.Count > 0)
                    {
                        toReturn += "\nBucket " + i.ToString() + ":";
                    }
                    foreach (Contact c in bucket)
                    {
                        toReturn += "\n" + c.GetID().ToString() + "@" + c.Uri.ToString();
                    }
                }
            }
            return toReturn;
        }


    }
}
