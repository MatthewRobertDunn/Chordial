using System.Collections.Generic;

namespace Chordial.Kademlia
{
    public interface IBucketList
    {
        NetworkContact MySelf { get; }
        NetworkContact Blocker(KadId toAdd);
        IList<NetworkContact> CloseContacts(KadId target);
        bool Contains(KadId toCheck);
        NetworkContact Get(KadId toGet);
        int GetCount();
        void Promote(KadId toPromote);
        void Put(NetworkContact toAdd);
        void Remove(KadId toRemove);
        void Touch(KadId key);
        void AddContact(NetworkContact applicant);
    }
}