using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chordial.Kademlia
{
    /// <summary>
    /// Represents a 160-bit number which is used both as a nodeID and as a key for the DHT.
    /// The number is stored big-endian (most-significant-byte first).
    /// IDs are immutable.
    /// </summary>
    public class ID : IComparable
    {
        public const int ID_LENGTH = 20; // This is how long IDs should be, in bytes.
        private byte[] data;

        // We want to be able to generate random IDs without timing issues.
        private static Random rnd = new Random();

        // We need to have a mutex to control access to the hash-based host ID.
        // Once one process on the machine under the current user gets it, no others can.
        private static Mutex mutex;

        /// <summary>
        /// Make a new ID from a byte array.
        /// </summary>
        /// <param name="data">An array of exactly 20 bytes.</param>
        public ID(byte[] data)
        {
            if (data.Length == ID_LENGTH)
            {
                this.data = data;
            }
            else
            {
                throw new Exception("An ID must be exactly " + ID_LENGTH + " bytes.");
            }
        }

        public byte[] Data
        {
            get { return data; }
        }

        /// <summary>
        /// Hash a string to produce an ID
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static ID Hash(string key)
        {
            HashAlgorithm hasher = new SHA1Managed(); // Keeping this around results in exceptions
            return new ID(hasher.ComputeHash(Encoding.UTF8.GetBytes(key)));
        }

        /// <summary>
        /// XOR operator.
        /// This is our distance metric in the DHT.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static ID operator ^(ID a, ID b)
        {
            byte[] xoredData = new byte[ID_LENGTH];
            // Do each byte in turn
            for (int i = 0; i < ID_LENGTH; i++)
            {
                xoredData[i] = (byte)(a.Data[i] ^ b.Data[i]);
            }
            return new ID(xoredData);
        }

        // We need to compare these when measuring distance
        public static bool operator <(ID a, ID b)
        {
            for (int i = 0; i < ID_LENGTH; i++)
            {
                if (a.Data[i] < b.Data[i])
                {
                    return true; // If first mismatch is a < b, a < b
                }
                else if (a.Data[i] > b.Data[i])
                {
                    return false; // If first mismatch is a > b, a > b
                }
            }
            return false; // No mismatches
        }

        public static bool operator >(ID a, ID b)
        {
            for (int i = 0; i < ID_LENGTH; i++)
            {
                if (a.Data[i] < b.Data[i])
                {
                    return false; // If first mismatch is a < b, a < b
                }
                else if (a.Data[i] > b.Data[i])
                {
                    return true; // If first mismatch is a > b, a > b
                }
            }
            return false; // No mismatches
        }

        // We're a value, so we override all these
        public static bool operator ==(ID a, ID b)
        {
            // Handle null
            if (ValueType.ReferenceEquals(a, null))
            {
                ValueType.ReferenceEquals(b, null);
            }
            if (ValueType.ReferenceEquals(b, null))
            {
                return false;
            }

            // Actually check
            for (int i = 0; i < ID_LENGTH; i++)
            {
                if (a.Data[i] != b.Data[i])
                { // Find the first difference
                    return false;
                }
            }
            return true; // Must match
        }

        public static bool operator !=(ID a, ID b)
        {
            return !(a == b); // Already have that
        }

        public override int GetHashCode()
        {
            // Algorithm from http://stackoverflow.com/questions/16340/how-do-i-generate-a-hashcode-from-a-byte-array-in-c/425184#425184
            int hash = 0;
            for (int i = 0; i < ID_LENGTH; i++)
            {
                unchecked
                {
                    hash *= 31;
                }
                hash ^= Data[i];
            }
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is ID)
            {
                return this == (ID)obj;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines the least significant bit at which the given ID differs from this one, from 0 through 8 * ID_LENGTH - 1.
        /// PRECONDITION: IDs do not match.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int DifferingBit(ID other)
        {
            ID differingBits = this ^ other;
            int differAt = 0;


            int i = 0;
            while (i < ID_LENGTH && differingBits.Data[i] == 0)
            {
                differAt += 8;
                i++;
            }

            // Subtract 1 for every zero bit from the right
            int j = 7;
            // 1 << j = pow(2, j)
            while (j >= 0 && (differingBits.Data[i] & (1 << j)) == 0)
            {
                j--;
                differAt++;
            }

            return differAt;
        }

        /// <summary>
        /// Produce a random ID.
        /// TODO: Make into a constructor?
        /// </summary>
        /// <returns></returns>
        public static ID RandomID()
        {
            byte[] data = new byte[ID_LENGTH];
            rnd.NextBytes(data);
            return new ID(data);
        }

        /// <summary>
        /// Get an ID that will be the same between different calls on the 
        /// same machine by the same app run by the same user.
        /// If that ID is taken, returns a random ID.
        /// </summary>
        /// <returns></returns>
        public static ID HostID()
        {
            // If we already have a mutex handle, we're not the first.
            if (mutex != null)
            {
                Console.WriteLine("Using random ID");
                return RandomID();
            }

            // We might be the first
            string assembly = Assembly.GetEntryAssembly().GetName().Name;
            string libname = Assembly.GetExecutingAssembly().GetName().Name;
            string mutexName = libname + "-" + assembly + "-ID";
            try
            {
                mutex = Mutex.OpenExisting(mutexName);
                // If that worked, we're not the first
                Console.WriteLine("Using random ID");
                return RandomID();
            }
            catch (Exception ex)
            {
                // We're the first!
                mutex = new Mutex(true, mutexName);
                Console.WriteLine("Using host ID");
                // TODO: Close on assembly unload?
            }

            // Still the first! Calculate hashed ID.
            string app = System.Reflection.Assembly.GetEntryAssembly().GetName().FullName;
            string user = Environment.UserName;
            string machine = Environment.MachineName + " " + Environment.OSVersion.VersionString;

            // Get macs
            string macs = "";
            foreach (NetworkInterface i in NetworkInterface.GetAllNetworkInterfaces())
            {
                macs += i.GetPhysicalAddress().ToString() + "\n";
            }
            return ID.Hash(app + user + machine + macs);
        }

        /// <summary>
        /// Turn this ID into a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return BitConverter.ToString(Data);
        }

        /// <summary>
        /// Returns this ID represented as a path-safe string.
        /// </summary>
        /// <returns></returns>
        public string ToPathString()
        {
            return System.Web.HttpServerUtility.UrlTokenEncode(Data); // This is path safe.
        }

        /// <summary>
        /// Compare ourselves to an object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj is ID)
            {
                // Compare as ID.
                if (this < (ID)obj)
                {
                    return -1;
                }
                else if (this == (ID)obj)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return 1; // We're bigger than random crap
            }
        }
    }
}
