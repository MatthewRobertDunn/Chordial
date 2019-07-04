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
        SearchResult FindNode(Contact senderId, byte[] key);

        /// <summary>
        /// Returns a list of values associated with a given key
        /// or contacts closest to the given key
        /// </summary>
        /// <param name="senderId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        SearchResult FindValue(Contact senderId, byte[] key);

        /// <summary>
        /// Asks the server to store a value against a given key
        /// </summary>
        /// <param name="senderId"></param>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="published"></param>
        /// <param name="expires"></param>
        bool StoreValue(Contact senderId, byte[] key, string data, DateTime published, DateTime expires);

        /// <summary>
        /// Checks if a given peer is alive.
        /// </summary>
        /// <param name="senderId"></param>
        /// <returns></returns>
        byte[] Ping(Contact senderId);
    }
}
