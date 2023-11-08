using Factory.DB;
using Factory.RHSSOService.Model;
using Model;
using Serilog;
using System.Diagnostics;

namespace Service
{
    public class AuditLogService
    {
        public static async Task<int> AuditLog(string username, string action, string actionDesc)
        {
            var newAuditLog = new ModTableAuditLog(username, action, actionDesc); 
            try
            {
                using (var dbContext = new DBContext())
                {                   
                    var query = dbContext.QueryFactory.Insert(newAuditLog);
                    return await dbContext.ExecuteNonQueryAsync(query.Item1, query.Item2);
                }
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
