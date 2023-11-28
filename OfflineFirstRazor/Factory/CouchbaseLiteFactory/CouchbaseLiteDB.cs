using Couchbase.Lite;


namespace Factory.CouchbaseLiteFactory
{
    public class CouchbaseLiteDB
    {
        public static Database GetDatabase(string localDBName)
        {
            return new Database(localDBName);


        }
    }
}
