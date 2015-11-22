//using System;
//using System.Diagnostics;
//using Chordial.Kademlia.Storage;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.Linq;

//namespace Chordial.Kademlia.Tests
//{
//    [TestClass]
//    public class StorageTests
//    {
//        [TestMethod]
//        public void TestCreate()
//        {
//            var store = new SqliteStorage("Data Source=:memory:;Version=3;");
//        }


//        [TestMethod]
//        public void TestGetItems()
//        {
//            var store = new SqliteStorage("Data Source=:memory:;Version=3;");

//            StorageItem item = new StorageItem()
//            {
//                Hash = Guid.NewGuid().ToByteArray(),
//                Key = Guid.NewGuid().ToByteArray(),
//                Value = "Hello",
//                PublicationDate = DateTime.Now,
//                Expires = DateTime.Now.AddSeconds(5)
//            };

//            Stopwatch watch = new Stopwatch();

//            watch.Start();
//            store.PutItem(item);

//            item.Hash = Guid.NewGuid().ToByteArray();
//            store.PutItem(item);

//            item.Hash = Guid.NewGuid().ToByteArray();
//            store.PutItem(item);

//            var items = store.GetItems(item.Key).ToList();
//            watch.Stop();

//            Assert.AreEqual(3,items.Count);
//        }


//        [TestMethod]
//        public void TestPutAndGet()
//        {

//            var store = new SqliteStorage("Data Source=:memory:;Version=3;");

//            StorageItem item = new StorageItem()
//                        {
//                            Hash = Guid.NewGuid().ToByteArray(),
//                            Key = Guid.NewGuid().ToByteArray(),
//                            Value = "Hello",
//                            PublicationDate = DateTime.Now,
//                            Expires = DateTime.Now.AddSeconds(5)
//                        };

//            store.PutItem(item);


//            Assert.IsTrue(store.ContainsKey(item.Key));

//            Assert.IsTrue(store.Contains(item.Key, item.Hash));

//            var readBack = store.GetItem(item.Key, item.Hash);

//            Assert.IsTrue(item.Key.SequenceEqual(readBack.Key));

//            Assert.IsTrue(item.Hash.SequenceEqual(readBack.Hash));

//            Assert.AreEqual(item.Value, readBack.Value);

//            Assert.AreEqual(item.PublicationDate, readBack.PublicationDate);

//            Assert.AreEqual(item.Expires, readBack.Expires);

//            //Wait 5 seconds

//            System.Threading.Thread.Sleep(6000);

//            readBack = store.GetItem(item.Key, item.Hash);

//            Assert.IsNull(readBack);
//        }

//    }
//}
