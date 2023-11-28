using Couchbase.Lite;
using System.Diagnostics;
using System.Reflection;

namespace Factory.CouchbaseLiteFactory
{
    public class CouchbaseCollection: CouchbaseDatabase
    {
        protected Collection _collection;

        public Collection GetCollection(string collectionName, string scopeName = CouchbaseDefault.Scope)
        {
            try
            {
                _collection = _database.GetCollection(collectionName, scopeName);
                if (_collection == null)
                    _collection = _database.CreateCollection(collectionName, scopeName);
                return _collection;
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);

                Log.Error("{funcName}: Connection to SQLITE Failed: {error}", funcName, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public Collection NewCollection(string collectionName, string scopeName = CouchbaseDefault.Scope)
        {
            try
            {
                _collection = _database.CreateCollection(collectionName, scopeName);
                return _collection;
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);

                Log.Error("{funcName}: Connection to SQLITE Failed: {error}", funcName, ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
