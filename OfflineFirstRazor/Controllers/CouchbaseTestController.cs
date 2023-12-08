using Microsoft.AspNetCore.Mvc;
using Service;

namespace WebApi.Controllers
{
    [Route("test/[controller]")]
    [ApiController]
    public class CouchbaseTestController : ControllerBase
    {
        [HttpPost, Route("/set/{collectionName}")]
        public ActionResult<string> SetDocument(string collectionName, string jsonDoc)
        {
            var db = new CouchbaseService(collectionName);

            return db.Save(jsonDoc);

        }

        [HttpPost, Route("/get")]
        public ActionResult<string> GetDocument(string query)
        {
            var db = new CouchbaseService();
            if (string.IsNullOrEmpty(query))
            {
                query = "select * from oliftest where Name='test'";
            }
            return new JsonResult(db.Test_GetDoc(query));

        }

        [HttpGet, Route("/collection/{collectionName}/id/{id}")]
        public ActionResult<string> GetDocumentById(string collectionName, string id)
        {
            using var db = new CouchbaseService();
            var test  = db.LoadDocumentAsJson(collectionName, id);
            return test;
        }

        [HttpGet, Route("/collection/name/all")]
        public ActionResult<string> GetAllCollections()
        {
            using var db = new CouchbaseService();
            var result = db.GetAllCollections();
            return new JsonResult(result);

        }


        [HttpGet, Route("/collection/{name}")]
        public ActionResult<string> GetCollection(string name)
        {
            using var db = new CouchbaseService();
            var result = db.Search($"select * from {name}");
            return result;

        }



    }
}
