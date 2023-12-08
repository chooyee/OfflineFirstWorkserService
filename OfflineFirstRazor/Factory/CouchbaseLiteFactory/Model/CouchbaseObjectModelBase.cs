using Newtonsoft.Json;
using Service;
using System.Collections;
using System.Security.Cryptography.X509Certificates;

namespace Factory.CouchbaseLiteFactory.Model
{
    public interface ICouchbaseObjectBase
    {
        string ID { get; set; }
    }

 
    public class CouchbaseObjectModelBase : ICouchbaseObjectBase
    {
        [DocumentId]
        public string ID { get; set; }

        public static async Task<IEnumerable<T>> LoadAll<T>()
        {        
            var modelAttribute = (CollectionAttribute)ReflectionFactory.GetModelAttribute(typeof(T), typeof(CollectionAttribute));

            if (modelAttribute == null) throw new ArgumentException(nameof(T) + " is not a valid couchbase model!");

            var collectionName = modelAttribute.CollectionName;

            return await Task.Run(() => {
                using var db = new CouchbaseService(collectionName);
                return db.Search<T>($"select * from {collectionName}");
            });
        }

        public static string GetCollectionName<T>()
        {
            var modelAttribute = (CollectionAttribute)ReflectionFactory.GetModelAttribute(typeof(T), typeof(CollectionAttribute));

            var nameValidationResult = CouchbaseCollection.IsValidCollectionName(modelAttribute.CollectionName);
            if (!nameValidationResult.Item1)
                throw nameValidationResult.Item2;

            return modelAttribute.CollectionName;
        }
    }

    public static class CouchbaseModelExt
    {
        

        public static async Task<bool> Load<T>(this T thisObj, string documentID)
        {
            Type type = thisObj.GetType();
            var modelAttribute = (CollectionAttribute)ReflectionFactory.GetModelAttribute(type, typeof(CollectionAttribute));

            if (modelAttribute == null) throw new ArgumentException(nameof(type) + " is not a valid couchbase model!");

            var collectionName = modelAttribute.CollectionName;
            return await Task.Run(() => {
                using var db = new CouchbaseService(collectionName);
                var json = db.LoadAsJson(documentID);
                thisObj = JsonConvert.DeserializeObject<T>(json);
                return true;
            });
        }


        public static async Task<dynamic> Save(this ICouchbaseObjectBase thisObj)
        {
            Type type = thisObj.GetType();
            var modelAttribute = (CollectionAttribute)ReflectionFactory.GetModelAttribute(type, typeof(CollectionAttribute));

            if (modelAttribute == null) throw new ArgumentException(nameof(type) + " is not a valid couchbase model!");

            var collectionName = modelAttribute.CollectionName;

            try
            {
                return await Task.Run(() =>
                {
                    using var db = new CouchbaseService(collectionName);
                  
                    //assign the id back to the object
                    thisObj.ID = db.Save(thisObj);

                    return thisObj;
                });
                
            }
            catch (Exception ex) { throw;}
        }


        public static async Task<bool> Delete(this ICouchbaseObjectBase thisObj)
        {
            Type type = thisObj.GetType();
            var modelAttribute = (CollectionAttribute)ReflectionFactory.GetModelAttribute(type, typeof(CollectionAttribute));

            if (modelAttribute == null) throw new ArgumentException(nameof(type) + " is not a valid couchbase model!");

            var collectionName = modelAttribute.CollectionName;

            try
            {
                return await Task.Run(() =>
                {
                    using var db = new CouchbaseService(collectionName);
                    return db.Delete(thisObj.ID);
                });
            }
            catch (Exception ex) { throw; }
        }
    }
}
