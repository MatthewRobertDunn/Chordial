using System;
using System.Collections.Generic;
using System.Text;

namespace Hive.Overlay.Api.Certificates
{
    /// <summary>
    /// Public interface for a node's certificate interface
    /// </summary>
    public interface ICertificates
    {
        /// <summary>
        /// Returns all public certificates for a hive node.
        /// </summary>
        /// <param name="hiveId"></param>
        /// <returns>Byte array of the </returns>
        byte[] GetPkcs12Certificates(byte[] hiveId);

        
        /// <summary>
        /// Stores public certificates for a hive node in pkcs12 format
        /// </summary>
        /// <param name="hiveId"></param>
        /// <returns></returns>
        byte[] StorePkcs12Certificates(byte[] hiveId);
    }
}
