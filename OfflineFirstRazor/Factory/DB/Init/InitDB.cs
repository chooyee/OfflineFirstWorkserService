using Factory.CouchbaseLiteFactory.Model;
using Factory.DB.Model;
using Serilog;
using Service;

namespace Factory.DB.Init
{
    public class InitDB
    {
        public static int Init()
        {

            try
            {
                using (var db = new CouchbaseService())
                {
                    Log.Debug("InitDB : Init: Couchbase initiation");
                }

                var trustedClient = new TrustedClient()
                {
                    ClientId = "projectowl",
                    ClientName = "Olif",
                };
                _ = trustedClient.Save().Result;

                trustedClient = new TrustedClient()
                {
                    ClientId = "projectwms",
                    ClientName = "WMS",
                };
                _ = trustedClient.Save().Result;

                Log.Debug("Init DB done");
                return 1;
              
            }
            catch (Exception ex)
            {
                Log.Error("Init " + ex.Message);
                return 0;
            }
        }
    }
}
