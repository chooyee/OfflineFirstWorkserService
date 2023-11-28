using Couchbase.Lite;

namespace Factory.CouchbaseLiteFactory
{
    public class CouchbaseDatabase
    {
        protected Database _database;
       

        public static Database GetDatabase(string localDBName)
        {
            return new Database(localDBName);
        }
    }
}
