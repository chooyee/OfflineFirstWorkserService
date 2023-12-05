using Couchbase.Lite.Query;
using Couchbase.Lite;
using Factory.DB;
using Newtonsoft.Json;
using System.Diagnostics;
using Serilog;
using System.Reflection;
namespace Factory.CouchbaseLiteFactory
{
    public class CouchbaseDocument:CouchbaseCollection
    {
        public static string GetPrimaryKey<T>(T thisObj)
        {
            var documentID = "";
            var primary = ReflectionFactory.GetMappableProperties(thisObj.GetType()).Where(x => x.GetCustomAttribute<SqlPrimaryKey>() != null);
            if (!primary.Any()) primary = ReflectionFactory.GetMappableProperties(thisObj.GetType()).Where(x => x.GetCustomAttribute<DocumentIdAttribute>() != null);

            if (primary.Any())
            {
                var primaryProp = primary.First();
                documentID = primaryProp.GetValue(thisObj).ToString();
            }
            else
            {
                documentID = new CouchbaseObjectId().ToString();
            }
            return documentID;
        }

        /// <summary>
        /// SaveDocument - Add or Update. If the object ID (documentid in couchbase) is empty it will save as new record
        /// return original object with ID property
        /// Use the T.ID property to retrieve back the document
        /// </summary>
        /// <typeparam name="T">Object Model</typeparam>
        /// <param name="obj">Object</param>
        /// <returns>Return saved object</returns>
        public T SaveDocument<T>(T obj)
        {
            var modelAttribute = ReflectionFactory.GetModelAttribute(typeof(T), typeof(CollectionAttribute));

            if (modelAttribute == null) throw new ArgumentException(nameof(T) + " is not a valid couchbase model!");

            var documentID = GetPrimaryKey<T>(obj);

            //Using reflection to get object property with [documentid]
           
            var json = JsonConvert.SerializeObject(obj);

            try
            {
                MutableDocument doc = new MutableDocument(documentID, json);
                //doc.SetJSON(json);
                if (!doc.Contains(CouchbaseObjectId.CouchbaseObjectIdColumnName))
                {
                    doc.SetString(CouchbaseObjectId.CouchbaseObjectIdColumnName, documentID);
                }
                else if (string.IsNullOrEmpty(doc.GetString(CouchbaseObjectId.CouchbaseObjectIdColumnName)))
                {
                    doc.SetString(CouchbaseObjectId.CouchbaseObjectIdColumnName, documentID);
                }

                _collection.Save(doc);

                var jsonFinal = JsonConvert.SerializeObject(doc);
                return JsonConvert.DeserializeObject<T>(jsonFinal);

            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}:{ex}", funcName, ex.Message);
                Log.Error("{json}", json);
                throw;
            }
        }


        /// <summary>
        /// SaveDocument
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="documentID"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public T SaveDocument<T>(string documentID, T obj)
        {
            var json = JsonConvert.SerializeObject(obj);

            try
            {
                MutableDocument doc = new MutableDocument(documentID, json);

                doc.SetString(CouchbaseObjectId.CouchbaseObjectIdColumnName, documentID);

                _collection.Save(doc);
                var jsonFinal = doc.ToJSON();
                return JsonConvert.DeserializeObject<T>(jsonFinal);

            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}:{documentID}:{ex}", funcName, documentID, ex.Message);
                Log.Error("{json}", json);
                throw;
            }
        }


        /// <summary>
        /// GetDocuments - simple query helper
        /// Query helper will select all and return full documents
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">ConditionBuilder</param>
        /// <param name="sortBuilder">SortBuilder</param>
        /// <param name="limit">Limit</param>
        /// <param name="offset">Offset</param>
        /// <returns>List<Couchbase.Lite.Document></returns>
        public List<Document> GetDocuments<T>(ConditionBuilder<T> condition, SortBuilder<T>? sortBuilder = null, int limit = 0, int offset = 0)
        {            
            var results = new List<Document>();

            var queryStr = QueryHelper.BuildQuery<T>(_collection.Name, condition, sortBuilder, limit, offset);

            var sqlParam = condition.SqlParams;
            try
            {
                using var query = _database.CreateQuery(queryStr);
                if (sqlParam != null) query.Parameters = sqlParam.CouchbaseParameters;

                foreach (var result in query.Execute())
                {
                    var dictObj = result.GetDictionary(_collection.Name);
                    var docid = dictObj.GetString(CouchbaseObjectId.CouchbaseObjectIdColumnName);
                    results.Add(_collection.GetDocument(docid.ToString()));
                }
                return results;
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}:{ex}", funcName, ex.Message);
                Log.Error("Query: {queryStr}", queryStr);
                if (sqlParam!=null)
                    Log.Error("Param: {param}", sqlParam.GetAsString());
                throw;
            }
        }

        /// <summary>
        /// Get document by document id
        /// </summary>
        /// <param name="documentID"></param>
        /// <returns>Couchbase Document</returns>
        public Document GetDocumentById(string documentID)
        {
            try
            {
                return _collection.GetDocument(documentID);
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}:{documentID}:{ex}", funcName, documentID, ex.Message);
                throw;
            }
        }


        /// <summary>
        /// QueryCollection - Select documents from the collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryStr">string</param>
        /// <param name="sqlParam">DynamicSqlParameter</param>
        /// <returns>IEnumerable<T></returns>
        public IEnumerable<T> QueryCollection<T>(string queryStr, DynamicSqlParameter? sqlParam = null)
        {
            var returnResult = new List<T>();
            try
            {
                using var query = _database.CreateQuery(queryStr);
                if (sqlParam != null) query.Parameters = sqlParam.CouchbaseParameters;

                foreach (var result in query.Execute())
                {
                    //get as readonly dictionary object
                    dynamic dicObj = result.GetDictionary(_collection.Name);
                    if (dicObj == null) 
                        dicObj = result;

                    //Export the dict object to json and deserialize to model
                    //store into return result
                    returnResult.Add(JsonConvert.DeserializeObject<T>(dicObj.ToJSON()));
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}:{ex}", funcName, ex.Message);
                Log.Error("Query: {queryStr}", queryStr);
                if (sqlParam != null)
                    Log.Error("Param: {param}", sqlParam.GetAsString());
                throw;
            }
        }

        /// <summary>
        /// QueryCollection
        /// </summary>
        /// <param name="queryStr">string</param>
        /// <param name="sqlParam">DynamicSqlParameter</param>
        /// <param name="returnType">To indicate return as Dictionary or Json string</param>
        /// <returns>List<Dictionary<string, object>> | json string</returns>
        public dynamic QueryCollection(string queryStr, DynamicSqlParameter? sqlParam = null, QueryResultReturnType returnType = QueryResultReturnType.Dictionary)
        {
            var queryResult = new List<Dictionary<string, object>>();
            try
            {
                using var query = _database.CreateQuery(queryStr);
                if (sqlParam != null) query.Parameters = sqlParam.CouchbaseParameters;

                foreach (var result in query.Execute())
                {
                    //get as readonly dictionary object
                    dynamic dicObj = result.GetDictionary(_collection.Name);
                    if (dicObj == null)
                        dicObj = result;

                    //Export to dictionary and store into return result
                    queryResult.Add(dicObj.ToDictionary());
                }
                if (returnType == QueryResultReturnType.Dictionary)
                    return queryResult;
                else
                    return JsonConvert.SerializeObject(queryResult);
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}:{ex}", funcName, ex.Message);
                Log.Error("Query: {queryStr}", queryStr);
                if (sqlParam != null)
                    Log.Error("Param: {param}", sqlParam.GetAsString());
                throw;
            }
        }

        public void CreateIndex(string indexName, string[] indexProperties)
        {
            var config = new ValueIndexConfiguration(indexProperties);
            _collection.CreateIndex(indexName, config);
        }

        /// <summary>
        /// DeleteDocument - to delete document, first you need to get couchbase document. Then pass the document id to this function
        /// </summary>
        /// <param name="documentID">document id</param>
        public bool DeleteDocument(string documentID)
        {
            try
            {
                var document = _collection.GetDocument(documentID);
                _collection.Delete(document);
                return true;
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}:{ex}", funcName, ex.Message);
                Log.Error("documentID: {documentID}", documentID);
               
                throw;
            }
        }

    }
}
