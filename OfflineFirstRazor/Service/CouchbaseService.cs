using Factory;
using Factory.CouchbaseLiteFactory;
using Factory.CouchbaseLiteFactory.Model;
using Factory.DB;
using Newtonsoft.Json;
using Serilog;
using System.Collections;

namespace Service
{
    public struct Address
    {
        public string AddressType { get; set; }
        public string Address1 { get; set; }
    }

    public class TestModel: CouchbaseObjectModelBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Age { get; set; }
        public Address Addr { get; set; }

        public TestModel()
        { }
    }

    internal class CouchbaseService:IDisposable
    {
        private readonly CouchBaseFactory _factory;
        private bool disposedValue;

        /// <summary>
        /// Initiate with default scope and collection
        /// </summary>
        internal CouchbaseService()
        {
            _factory = new CouchBaseFactory(Global.GlobalConfig.Instance.CouchbaseConfig.DBName, CouchbaseDefault.Scope, CouchbaseDefault.Collection);
        }

        /// <summary>
        /// Initiate with default scope and collection
        /// </summary>
        internal CouchbaseService(string collection)
        {
            var nameValidationResult = CouchbaseCollection.IsValidCollectionName(collection);
            if (!nameValidationResult.Item1)
                throw nameValidationResult.Item2;

            _factory = new CouchBaseFactory(Global.GlobalConfig.Instance.CouchbaseConfig.DBName, CouchbaseDefault.Scope, collection);
        }


        /// <summary>
        /// Save - New or update
        /// The class need to inherit from CouchbaseModelBase or declare the class as Couchbasemodel
        /// Ex:
        /// [CouchbaseModel]
        ///  public class Yourclass
        ///  {
        ///    [DocumentId]
        ///    public string ID { get; set; }
        ///  }
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="document"></param>
        /// <returns></returns>
        public string Save<T>(T document)
        {
            //Console.WriteLine(document.GetType());
            var modelAttribute = ReflectionFactory.GetModelAttribute(document.GetType(), typeof(CollectionAttribute));

            if (modelAttribute == null) throw new ArgumentException(nameof(T) + " is not a valid couchbase model!");

            var primaryKey = CouchBaseFactory.GetPrimaryKey<T>(document);

            if (primaryKey == null) throw new ArgumentException(document.GetType().Name + " is not a valid couchbase model! No primary key/document id exist.");

            try
            {
                return _factory.SaveDocument(document);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string Save(string jsonDoc)
        {
            try
            {
                return _factory.SaveDocument(jsonDoc);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// LoadAsJson - load document as json
        /// </summary>
        /// <param name="id">Document ID</param>
        /// <returns>Json string</returns>
        /// <exception cref="Exception"></exception>
        public string LoadAsJson(string id)
        {            
            var document = _factory.GetDocumentById(id);
            if (document == null)
                throw new Exception($"No record found for document id {id}");
            return document.ToJSON();
        }

        /// <summary>
        /// Load - Load document as class model
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="id">Document ID</param>
        /// <returns>T</returns>
        /// <exception cref="Exception"></exception>
        public T Load<T>(string id)
        {
            var modelAttribute = ReflectionFactory.GetModelAttribute(typeof(T), typeof(CollectionAttribute));

            if (modelAttribute == null) throw new ArgumentException(nameof(T) + " is not a valid couchbase model!");

            var document = _factory.GetDocumentById(id);
            if (document == null)
                throw new Exception($"No record found for document id {id}");
            return JsonConvert.DeserializeObject<T>(document.ToJSON());
        }

        public string LoadDocumentAsJson(string collectionName, string id)
        {
            var nameValidationResult = CouchbaseCollection.IsValidCollectionName(collectionName);
            if (!nameValidationResult.Item1)
                throw nameValidationResult.Item2;
            
            var sqlParam = new DynamicSqlParameter();
            sqlParam.Add("id", id);
            var document = _factory.QueryCollection($"select * from {collectionName} where ID = $id", sqlParam);
            if (document == null)
                throw new Exception($"No record found for document id {id}");
            return document;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="sqlPaam"></param>
        /// <returns></returns>
        public IEnumerable<T> Search<T>(string query, DynamicSqlParameter sqlParam = null)
        {
            var modelAttribute = ReflectionFactory.GetModelAttribute(typeof(T), typeof(CollectionAttribute));

            if (modelAttribute == null) throw new ArgumentException(nameof(T) + " is not a valid couchbase model!");

            return _factory.QueryCollection<T>(query, sqlParam);
        }

        public string Search(string query, DynamicSqlParameter sqlParam = null)
        {   
            return _factory.QueryCollection(query, sqlParam, QueryResultReturnType.JSON);
        }

        public IEnumerable<string> GetAllCollections()
        {
            return _factory.GetCollectionNames();
        }

        public bool Delete(string id)
        {
            return _factory.DeleteDocument(id);
        }

        internal string Test_SaveDoc(TestModel test)
        {
            //var test = new TestModel()
            //{
            //    Name = "test",
            //    Description = "desc",
            //    Age = 25,
            //    Addr = new Address() { 
            //        Address1= "address1",
            //        AddressType = "home"
            //    }
            //};
            try
            {
                var savedObject = _factory.SaveDocument<TestModel>(test);

                return "ok";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Log.Error(ex.Message);
                return ex.Message;
            }
        }

        internal TestModel Test_GetDoc(string query)
        {         
            try
            {
                var savedObject = _factory.QueryCollection<TestModel>(query).FirstOrDefault();

                return savedObject;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Log.Error(ex.Message);
                return default;
            }
        }

        internal string Test_GetJson(string query)
        {          
            try
            {
                var savedJsonObject = _factory.QueryCollection(query, null, QueryResultReturnType.JSON);

                return savedJsonObject;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Log.Error(ex.Message);
                return default;
            }
        }

        internal Dictionary<string,object> Test_GetDict(string query)
        {
            try
            {
                var savedJsonObject = _factory.QueryCollection(query, null, QueryResultReturnType.Dictionary);

                return savedJsonObject;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Log.Error(ex.Message);
                return default;
            }
        }

        internal IEnumerable<TestModel> Test_GetCollection()
        {
            try
            {
                var savedJsonObject = _factory.QueryCollection<TestModel>("select * from oliftest");

                return savedJsonObject;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Log.Error(ex.Message);
                return default;
            }
        }


        internal string Test_GeQueryDoc()
        {
            var test = new ConditionBuilder<TestModel>();
            test.Add(x => x.Name, Factory.DB.QueryOperator.Equal, "test");
            //test.Add(x => x.Name, Factory.DB.Model.QueryOperator.Equal, "home");
            try
            {
                var savedJsonObject = _factory.GetDocuments(test, null, 1).FirstOrDefault();

                return savedJsonObject.ToJSON();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Log.Error(ex.Message);
                return default;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _factory.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~CouchbaseService()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
