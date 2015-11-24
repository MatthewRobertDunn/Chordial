using System.Collections.Generic;

namespace Chordial.Kademlia
{
    public interface IBucketList
    {
        Contact OurContact { get; }
        Contact Blocker(ID toAdd);
        IList<Contact> CloseContacts(ID target);
        bool Contains(ID toCheck);
        Contact Get(ID toGet);
        int GetCount();
        void Promote(ID toPromote);
        void Put(Contact toAdd);
        void Remove(ID toRemove);
        void Touch(ID key);
        void AddContact(Contact applicant);
    }
}