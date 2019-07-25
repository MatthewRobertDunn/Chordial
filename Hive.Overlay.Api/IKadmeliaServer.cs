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
        Contact[] CloseContacts(byte[] key, Contact senderId = null);

        /// <summary>
        /// Returns this server's hive address.
        /// </summary>
        /// <param name="senderId"></param>
        /// <returns></returns>
        byte[] Address(Contact senderId = null);
    }
}
