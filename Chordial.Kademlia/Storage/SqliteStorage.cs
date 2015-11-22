//using System;
//using System.Collections.Generic;
//using System.Data.SQLite;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using Dapper;
//namespace Chordial.Kademlia.Storage
//{
//    public class SqliteStorage : IStorage, IDisposable
//    {
//        private SQLiteConnection con;

//        ~SqliteStorage()
//        {
//            disposed = true;
//        }

//        public SqliteStorage(string connectionString)
//        {
//            con = new SQLiteConnection(connectionString);
//            con.Open();

//            using (var tran = con.BeginTransaction())
//            {

//                con.Execute(DatabaseSQL.CreateTable);
//                tran.Commit();
//            }

//            //Thread foo = new Thread(x => ExpireOldItems());
//            // foo.IsBackground = true;
//            //foo.Start();
//        }

//        public bool Contains(byte[] key, byte[] dataHash)
//        {
//            var count = con.Query<long>
//                ("select count(*) from StorageItems where Key = @Key and Hash = @Hash", new { Key = key, Hash = dataHash })
//                .Single();

//            return count != 0;
//        }


//        public bool ContainsKey(byte[] key)
//        {
//            var count = con.Query<long>
//                ("select count(*) from StorageItems where Key = @key", new { Key = key })
//                .Single();

//            return count != 0;
//        }

//        public IEnumerable<StorageItem> GetItems(byte[] key)
//        {
//            return con.Query<StorageItem>
//                ("select * from StorageItems where Key = @Key", new { Key = key });

//        }

//        public StorageItem GetItem(byte[] key, byte[] dataHash)
//        {
//            return con.Query<StorageItem>
//                ("select * from StorageItems where Key = @Key and Hash = @Hash", new { Key = key, Hash = dataHash }).
//                SingleOrDefault();
//        }

//        public void PutItem(StorageItem item)
//        {
//            using (var tran = con.BeginTransaction())
//            {
//                con.Execute(@"insert or replace into StorageItems(Key,Hash,Value,PublicationDate,Expires) 
//                                values (@Key,@Hash,@Value,@PublicationDate,@Expires)", item);
//                tran.Commit();
//            }
//        }


//        private void ExpireOldItems()
//        {

//            while (!disposed)
//            {
//                using (var tran = con.BeginTransaction())
//                {
//                    con.Execute(@"delete from StorageItems where Expires < @Expires", new { Expires = DateTime.UtcNow });
//                    tran.Commit();
//                }

//                System.Threading.Thread.Sleep(5000);
//            }
//        }

//        private volatile bool disposed = false;

//        public void Dispose()
//        {
//            disposed = true;
//        }
//    }

//    public static class DatabaseSQL
//    {
//        public static readonly string CreateTable = @"

//            create table if not exists StorageItems(
//                Key blob not null,
//                Hash blob not null,
//                Value text not null,
//                PublicationDate datetime,
//                Expires         datetime not null,
//                PRIMARY KEY (Key,hash)
//              );

//            create index if not exists myIndex on StorageItems (Expires);            
//                                                       ";
//    }

//}
