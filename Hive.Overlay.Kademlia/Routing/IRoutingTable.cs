using System.Collections.Generic;

namespace Hive.Overlay.Kademlia
{
    /// <summary>
    /// Implements kademlia routing table scheme.
    /// </summary>
    public interface IRoutingTable
    {
        NetworkContact MySelf { get; }
        NetworkContact Blocker(KadId toAdd);
        IList<NetworkContact> CloseContacts(KadId target);
        bool Contains(KadId toCheck);
        NetworkContact Get(KadId toGet);
        int GetCount();
        void Promote(KadId toPromote);
        void Remove(KadId toRemove);
        void AddContact(NetworkContact applicant);
    }
}