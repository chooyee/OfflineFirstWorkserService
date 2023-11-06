using Factory;
using Factory.DB;
using Model;

namespace Factory.DB.Init
{
    public class InitDB
    {
        public static int Init()
        {
            using (var dbContext = new DBContext())
            {
                var result = new List<Task>();
                var query = dbContext.QueryFactory.CreateTable(typeof(TableStruct));
                result.Add(dbContext.ExecuteNonQueryAsync(query));

                query = dbContext.QueryFactory.CreateTable(typeof(TableSource));
                result.Add(dbContext.ExecuteNonQueryAsync(query));

                Task.WaitAll(result.ToArray());
                return 1;
            }
        }
    }
}
