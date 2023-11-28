using Couchbase.Lite.Query;
using Couchbase.Lite;
using Factory.DB;
using Newtonsoft.Json;
using System.Diagnostics;
using Serilog;

namespace Factory.CouchbaseLiteFactory
{
    public class CouchbaseDocument:CouchbaseCollection
    {
        /// <summary>
        /// SaveDocument
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>DocumentID</returns>
        public T SaveDocument<T>(T obj)
        {
            var documentID = new CouchbaseObjectId().ToString();
            var json = JsonConvert.SerializeObject(obj);

            try
            {
                MutableDocument doc = new MutableDocument(documentID);
                doc.SetJSON(json);
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

        private List<Result> ExecuteQueryCollection(string queryStr, DynamicSqlParameter? sqlParam = null)
        {
            try
            {
                using var query = _database.CreateQuery(queryStr);
                if (sqlParam != null) query.Parameters = sqlParam.CouchbaseParameters;

                return query.Execute().AllResults();
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}:{ex}", funcName, ex.Message);
                Log.Error("Query: {queryStr}", queryStr);
                Log.Error("Param: {param}", sqlParam.GetAsString());
                throw;
            }
        }

        private DictionaryObject? QueryDocument(string queryStr, DynamicSqlParameter? sqlParam = null)
        {
            try
            {
                var results = ExecuteQueryCollection(queryStr, sqlParam);
                if (results != null && results.Any())
                {
                    return results.First().GetDictionary(_collection.Name);
                }
                else
                {
                    return default;
                }

            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}:{ex}", funcName, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// GetDocument - return as couchbase document object
        /// </summary>
        /// <param name="queryStr"></param>
        /// <param name="sqlParam"></param>
        /// <returns>Couchbase Document</returns>
        public Document? GetDocument(string queryStr, DynamicSqlParameter? sqlParam = null)
        {
            try
            {
                var results = ExecuteQueryCollection(queryStr, sqlParam);
                if (results != null && results.Any())
                {
                    var firstDoc = results.FirstOrDefault();
                    var docId = firstDoc.GetString(CouchbaseObjectId.CouchbaseObjectIdColumnName);
                    return _collection.GetDocument(docId.ToString());
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}:{ex}", funcName, ex.Message);
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
        /// GetDocument - return document as single object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryStr"></param>
        /// <param name="sqlParam"></param>
        /// <returns></returns>
        public T? GetDocument<T>(string queryStr, DynamicSqlParameter? sqlParam = null)
        {
           
            try
            {
                var doc = QueryDocument(queryStr, sqlParam);

                if (doc != null)
                {
                    return JsonConvert.DeserializeObject<T>(doc.ToJSON());
                }
                else {
                    return default;
                }

            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}:{ex}", funcName, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// GetDocument - Search collection, return as dictionary
        /// </summary>
        /// <param name="queryStr"></param>
        /// <param name="sqlParam"></param>
        /// <returns>Dictionary<string, object></returns>
        public Dictionary<string, object>? GetDocumentAsDict(string queryStr, DynamicSqlParameter? sqlParam = null)
        {
            try
            {
                var doc = QueryDocument(queryStr, sqlParam);

                if (doc != null)
                {
                    return doc.ToDictionary();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}:{ex}", funcName, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// GetDocument - Search collection, return as Json
        /// </summary>
        /// <param name="queryStr"></param>
        /// <param name="sqlParam"></param>
        /// <returns>string (Json)</returns>
        public string? GetDocumentAsJson(string queryStr, DynamicSqlParameter? sqlParam = null)
        {
            try
            {
                var doc = QueryDocument(queryStr, sqlParam);

                if (doc != null)
                {
                    return doc.ToJSON();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}:{ex}", funcName, ex.Message);
                throw;
            }
        }


        /// <summary>
        /// QueryCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryStr"></param>
        /// <param name="sqlParam"></param>
        /// <returns>IEnumerable<T></returns>
        public IEnumerable<T> QueryCollection<T>(string queryStr, DynamicSqlParameter? sqlParam = null)
        {
            var returnResult = new List<T>();
            try
            {
                var allResults = ExecuteQueryCollection(queryStr, sqlParam);

                foreach (var result in allResults)
                {
                    //get as readonly dictionary object
                    var dicObj = result.GetDictionary(_collection.Name);
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
                throw;
            }
        }

        /// <summary>
        /// QueryCollection
        /// </summary>
        /// <param name="queryStr"></param>
        /// <param name="sqlParam"></param>
        /// <returns>List<Dictionary<string, object>></returns>
        public List<Dictionary<string, object>> QueryCollection(string queryStr, DynamicSqlParameter? sqlParam = null)
        {
            var returnResult = new List<Dictionary<string, object>>();
            try
            {
                var allResults = ExecuteQueryCollection(queryStr, sqlParam);

                foreach (var result in allResults)
                {
                    //get as readonly dictionary object
                    var dicObj = result.GetDictionary(_collection.Name);
                    //Export to dictionary and store into return result
                    returnResult.Add(dicObj.ToDictionary());
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}:{ex}", funcName, ex.Message);
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
        public void DeleteDocument(string documentID)
        {
            var document = _collection.GetDocument(documentID);
            _collection.Delete(document);
        }

    }
}
