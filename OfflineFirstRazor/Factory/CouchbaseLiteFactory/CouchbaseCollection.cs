using Couchbase.Lite;
using Serilog;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Factory.CouchbaseLiteFactory
{
    public class CouchbaseCollection: CouchbaseDatabase
    {
        protected Collection _collection;

        /// <summary>
        /// GetCollection
        /// </summary>
        /// <param name="collectionName">collectionName</param>
        /// <param name="scopeName"> string scopeName = CouchbaseDefault.Scope</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Collection GetCollection(string collectionName, string scopeName = CouchbaseDefault.Scope)
        {
            try
            {
                var nameValidationResult = IsValidCollectionName(collectionName);
                if (!nameValidationResult.Item1)
                    throw nameValidationResult.Item2;

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

        /// <summary>
        /// NewCollection
        /// </summary>
        /// <param name="collectionName">collectionName</param>
        /// <param name="scopeName">string scopeName = CouchbaseDefault.Scope</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Collection NewCollection(string collectionName, string scopeName = CouchbaseDefault.Scope)
        {
            try
            {
                var nameValidationResult = IsValidCollectionName(collectionName);
                if (!nameValidationResult.Item1)
                {
                    throw nameValidationResult.Item2;                  
                }

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

        /// <summary>
        /// Get all collection names
        /// </summary>
        /// <param name="scopeName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IEnumerable<string> GetCollectionNames(string scopeName = CouchbaseDefault.Scope)
        {
            try
            {
                var collections = _database.GetCollections(scopeName);
                return collections.Select(x => x.Name);
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);

                Log.Error("{funcName}: Connection to SQLITE Failed: {error}", funcName, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Ensure collection name is according to Couchbase lite requirement
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Tuple<bool, Exception?> IsValidCollectionName(string name)
        {
            if (name.Equals(CouchbaseDefault.Collection, StringComparison.OrdinalIgnoreCase)) return Tuple.Create(true, default(Exception));
            try
            {
                if (name.Length > 251)
                {
                    throw new ArgumentOutOfRangeException("Collection name length cannot more than 251 characters!");
                }

                // Define the regular expression pattern
                string pattern = @"^[A-Za-z0-9][A-Za-z0-9_%-]*$";

                // Use Regex.IsMatch to check if the input matches the pattern
                if (!Regex.IsMatch(name, pattern))
                {
                    throw new ArgumentException("Collection name can only contain the characters A-Z, a-z, 0-9, and the symbols _, -, and %");
                }

                return Tuple.Create(true, default(Exception));
            }
            catch(Exception ex) {
                return Tuple.Create(false, ex);
            }
        }
    }
}
