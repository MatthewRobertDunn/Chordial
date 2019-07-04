using Hive.Overlay.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Hive.Overlay.Kademlia
{
    /// <summary>
    /// A list of contacts.
    /// Also responsible for storing last lookup times for buckets, so we can refresh them.
    /// Not thread safe for multiple people writing at once, since you can't enforce preconditions.
    /// </summary>
    public class BucketList : IBucketList
    {
        private const int BUCKET_SIZE = 20; // "K" in the spec
        private const int NUM_BUCKETS = 8 * KadId.ID_LENGTH; // One per bit in an ID

        private readonly List<List<NetworkContact>> buckets;
        private readonly List<DateTime> accessTimes; // last bucket write or explicit touch
        private Func<Uri, IKadmeliaServer> serverFactory;
        public NetworkContact MySelf { get; }

        /// <summary>
        /// Make a new bucket list, for holding node contacts.
        /// </summary>
        /// <param name="mySelf">The ID to center the list on.</param>
        public BucketList(NetworkContact mySelf, Func<Uri, IKadmeliaServer> serverFactory)
        {
            this.MySelf = mySelf;
            buckets = new List<List<NetworkContact>>(NUM_BUCKETS);
            accessTimes = new List<DateTime>();

            // Set up each bucket
            for (int i = 0; i < NUM_BUCKETS; i++)
            {
                buckets.Add(new List<NetworkContact>(BUCKET_SIZE));
                accessTimes.Add(default(DateTime));
            }
            this.serverFactory = serverFactory;
        }



        /// <summary>
        /// Returns what contact is blocking insertion (least promoted), or null if no contact is.
        /// </summary>
        /// <param name="toAdd"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public NetworkContact Blocker(KadId toAdd)
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
        public bool Contains(KadId toCheck)
        {
            return this.Get(toCheck) != null;
        }

        /// <summary>
        /// Add the given contact at the end of its bucket.
        /// PRECONDITION: Won't over-fill bucket.
        /// </summary>
        /// <param name="toAdd"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Put(NetworkContact toAdd)
        {
            if (toAdd == null)
            {
                return; // Don't be silly.
            }

            int bucket = BucketFor(toAdd.Id);
            buckets[bucket].Add(toAdd); // No lock: people can read while we do this.
            accessTimes[bucket] = DateTime.Now;
        }

        /// <summary>
        /// Report that a lookup was done for the given key.
        /// Key must not match our ID.
        /// </summary>
        /// <param name="key"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Touch(KadId key)
        {
            accessTimes[BucketFor(key)] = DateTime.Now;
        }

        /// <summary>
        /// Return the contact with the given ID, or null if it's not found.
        /// </summary>
        /// <param name="toGet"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public NetworkContact Get(KadId toGet)
        {
            int bucket = BucketFor(toGet);
            for (int i = 0; i < buckets[bucket].Count; i++)
            {
                if (buckets[bucket][i].Id == toGet)
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
        public void Promote(KadId toPromote)
        {
            NetworkContact promotee = Get(toPromote);
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
        public void Remove(KadId toRemove)
        {
            int bucket = BucketFor(toRemove);
            for (int i = 0; i < buckets[bucket].Count; i++)
            {
                if (buckets[bucket][i].Id == toRemove)
                {
                    buckets[bucket].RemoveAt(i);
                    return;
                }
            }
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public IList<NetworkContact> CloseContacts(KadId target)
        {
            return AllContacts.OrderBy(x => x.Id ^ target)
                              .ToList();
        }

        private IEnumerable<NetworkContact> AllContacts
        {
            get
            {
                return buckets.SelectMany(x => x).Concat(new[] { this.MySelf });
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddContact(NetworkContact applicant)
        {
            //Never add myself
            if (applicant.Id == MySelf.Id)
                return;
            // If we already know about them
            if (Contains(applicant.Id))
            {
                // If they have a new address, record that
                if (Get(applicant.Id).Uri
                   != applicant.Uri)
                {
                    // Replace old one
                    Remove(applicant.Id);
                    Put(applicant);
                }
                else
                { // Just promote them
                    Promote(applicant.Id);
                }
                return;
            }

            // If we can fit them, do so
            NetworkContact blocker = Blocker(applicant.Id);
            if (blocker == null)
            {
                Put(applicant);
                return;
            }

            //has the blocker been pinged recently?
            Task.Factory.StartNew<bool>(() => PingBlocker(blocker))
                .ContinueWith
                (
                    x =>
                    {
                        // If the blocker doesn't respond, pick the applicant.
                        if (!x.Result)
                            ReplaceBlocker(applicant, blocker);
                    }
                );
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void ReplaceBlocker(NetworkContact applicant, NetworkContact blocker)
        {
            Remove(blocker.Id);
            Put(applicant);
        }

        private bool PingBlocker(NetworkContact blocker)
        {
            // We can't fit them. We have to choose between blocker and applicant
            var remotePeerUri = blocker.Uri;
            var peer = serverFactory(remotePeerUri);
            var pingResult = peer.Ping(MySelf.ToContact());
            return pingResult != null;
        }

        /// <summary>
        /// Returns what bucket an ID maps to.
        /// PRECONDITION: ourID not passed.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private int BucketFor(KadId id)
        {
            return (this.MySelf.Id.DifferingBit(id));
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
                List<NetworkContact> bucket = buckets[i];
                lock (bucket)
                {
                    if (bucket.Count > 0)
                    {
                        toReturn += "\nBucket " + i.ToString() + ":";
                    }
                    foreach (NetworkContact c in bucket)
                    {
                        toReturn += "\n" + c.Id.ToString() + "@" + c.Uri.ToString();
                    }
                }
            }
            return toReturn;
        }



    }
}
