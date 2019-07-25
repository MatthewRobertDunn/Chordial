using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hive.Overlay.Api
{
    public interface IKadmeliaServer
    {
        /// <summary>
        /// Returns list of contacts closest to a given key
        /// </summary>
        /// <param name="senderId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        SearchResult CloseContacts(byte[] key, Contact senderId = null);

        /// <summary>
        /// Checks if a given peer is alive.
        /// </summary>
        /// <param name="senderId"></param>
        /// <returns></returns>
        byte[] Address(Contact senderId = null);
    }
}
