using Factory.DB.Model;
using Serilog;
using System.Reflection.Metadata.Ecma335;

namespace Factory.DB.Init
{
    public class InitDB
    {
        public static int Init()
        {
            try
            {
                //Create database - required for prod
                using (var dbContext = new DBContext())
                {
                    _ = dbContext.CreateDatabaseAsync("Olif").Result;
                }
            }
            catch (Exception ex)
            {
                //do nothng    
            }

            try
            {
               

                using (var dbContext = new DBContext())
                {

                    var result = new List<Task>();
                    var query = dbContext.QueryFactory.CreateTable(typeof(ModTableSSOUser));
                    result.Add(dbContext.ExecuteNonQueryAsync(query));

                    query = dbContext.QueryFactory.CreateTable(typeof(ModTableAuditLog));
                    result.Add(dbContext.ExecuteNonQueryAsync(query));

                    query = dbContext.QueryFactory.CreateTable(typeof(ModTableMachineLog));
                    result.Add(dbContext.ExecuteNonQueryAsync(query));

                    query = dbContext.QueryFactory.CreateTable(typeof(UserLoginLog));
                    result.Add(dbContext.ExecuteNonQueryAsync(query));

                    query = dbContext.QueryFactory.CreateTable(typeof(TrustedClient));
                    result.Add(dbContext.ExecuteNonQueryAsync(query));


                    var trustedClient = new TrustedClient()
                    {
                        ClientId = "projectowl",
                        ClientName = "Olif",
                    };

                    result.Add(trustedClient.Save());
                    

                    trustedClient = new TrustedClient()
                    {
                        ClientId = "projectwms",
                        ClientName = "WMS",
                    };

                    result.Add(trustedClient.Save());
                   
                    Task.WaitAll(result.ToArray());

                    Log.Debug("Init DB done");
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Init " + ex.Message);
                return 0;
            }
        }
    }
}
