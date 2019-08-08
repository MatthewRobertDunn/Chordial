using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Hive.Cryptography.Certificates
{
    /// <summary>
    /// Interface to provide access to this node's cryptographic keys
    /// </summary>
    public interface ICertificateStore
    {
        /// <summary>
        /// Hive address of the owner of this store
        /// </summary>
        byte[] HiveAddress { get; }

        /// <summary>
        /// Provides access to the key intended for the transport layer
        /// </summary>
        X509Certificate2 Transport { get; }
        /// <summary>
        /// Provides access to the key intended for public signing
        /// </summary>
        X509Certificate Signing { get; }
        /// <summary>
        /// Provides access to the key intended for private messages.
        /// </summary>
        X509Certificate Private { get; }
    }
}
