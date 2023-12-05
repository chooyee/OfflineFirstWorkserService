using Factory.CouchbaseLiteFactory;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace WebApi.Controllers
{
    [Route("test/[controller]")]
    [ApiController]
    public class CouchbaseTestController : ControllerBase
    {
        [HttpPost, Route("/set")]
        public ActionResult<string> SetDocument(TestModel test)
        {
            var db = new CouchbaseService();

            return db.Test_SaveDoc(test);

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

        [HttpGet, Route("/get/id/{id}")]
        public ActionResult<string> GetDocumentById(string id)
        {
            using var db = new CouchbaseService();
            var test  = db.Load<TestModel>(id);
            return new JsonResult(test);
        }

        [HttpGet, Route("/get/json")]
        public ActionResult<string> GetDocumentAsJson()
        {
            var db = new CouchbaseService();

            return new JsonResult(db.Test_GeQueryDoc());

        }

        [HttpGet, Route("/get/collection")]
        public ActionResult<string> GetCollection()
        {
            var db = new CouchbaseService();

            return new JsonResult(db.Test_GetCollection());

        }



    }
}
