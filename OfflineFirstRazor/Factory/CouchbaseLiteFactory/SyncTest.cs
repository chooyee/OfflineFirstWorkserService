using Couchbase.Lite;
using Couchbase.Lite.Sync;
using Global;
using static Factory.CouchbaseLiteFactory.CouchbaseDocument;

namespace Factory.CouchbaseLiteFactory
{
    public class SyncTest
    {
        public static string Sync()
        {
            try
            {
                using var db = new CouchBaseFactory(GlobalConfig.Instance.CouchbaseConfig.DBName);
                var collection = db.GetCollection("collection1", "scope1");
                MutableDocument doc = new MutableDocument("1234", "{\"id\":\"abc\"}");
                doc.SetString(CouchbaseObjectId.CouchbaseObjectIdColumnName, "1234");

                collection.Save(doc);
                var targetEndpoint = new URLEndpoint(new Uri("ws://139.144.116.211:4984/ofstestdb"));
                var replConfig = new ReplicatorConfiguration(targetEndpoint);
                replConfig.AddCollection(collection);
                
                // Add authentication
                replConfig.Authenticator = new BasicAuthenticator("sgwuser3", "sgwuser3123");
                replConfig.ReplicatorType = ReplicatorType.Pull;

                // Create replicator (make sure to add an instance or static variable
                // named _Replicator)
                var replicator = new Replicator(replConfig);
                var pendingDocIDs = new HashSet<string>();
                
                replicator.AddChangeListener((sender, change) =>
                {
                    Console.WriteLine(change.Status.Activity.ToString());
                    Console.WriteLine($"Replicator activity level is " + change.Status.Activity.ToString());
                    //if (change.Status.Activity.ToString() == "Stopped")
                    //{
                    //    Environment.Exit(0);
                    //}
                    foreach (var docID in pendingDocIDs)
                    {
                        if (!replicator.IsDocumentPending(docID, collection))
                        {
                            Console.WriteLine($"Doc ID {docID} now pushed");
                        }
                    }
                });

                replicator.Start();
                while (true)
                {
                //    pendingDocIDs = new HashSet<string>(replicator.GetPendingDocumentIDs(collection));
                //    if (pendingDocIDs.Count == 0)
                //    {
                //        Console.WriteLine("no pending");
                //        break;
                //    }
                    if (replicator.Status.Activity ==ReplicatorActivityLevel.Stopped)
                    { break; }
                }
                return "ok";
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
