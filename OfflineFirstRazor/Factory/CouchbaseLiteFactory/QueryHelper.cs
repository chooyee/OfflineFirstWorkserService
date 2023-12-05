using Factory.DB;
using System.Text;
using System.Linq.Expressions;

namespace Factory.CouchbaseLiteFactory
{

    public enum SqlOperator
    {
        and,
        or
    }

    public enum SqlSortOrder
    {
        desc,
        asc
    }

    public class QueryHelper
    {
        public static string BuildQuery<T>(string collectionName, ConditionBuilder<T> condition, SortBuilder<T>? sortBuilder = null, int limit = 0, int offset = 0)
        {
            var queryStr = new StringBuilder();
            queryStr.Append($"select * from {collectionName} where {condition.GetSqlString()}");

            if (sortBuilder != null)
            {
                queryStr.Append(sortBuilder.GetSqlString());
            }
            if (offset > 0)
            {
                queryStr.Append($" offset {offset}");
            }

            if (limit > 0)
            {
                queryStr.Append($" limit {limit}");
            }

            return queryStr.ToString();
        }
    }

    public class ConditionBuilder<T> 
    {

        private StringBuilder _sqlConditions = new StringBuilder();
        private DynamicSqlParameter _sqlParams = new DynamicSqlParameter();

        public DynamicSqlParameter SqlParams { get { return _sqlParams; } }

        public SqlOperator sqlOperator = SqlOperator.and;

        public void Add(Expression<Func<T, object>> propertySelector, string op, string paramValue)
        {            
            if (propertySelector.Body is MemberExpression memberExpression)
            {
                string[] splitResult = memberExpression.ToString().Split('.');

                // Removing the first part
                string propertyName = string.Join(".", splitResult, 1, splitResult.Length - 1);
                string paramName = splitResult[splitResult.Length - 1];


                _sqlConditions.AppendLine($"{propertyName} {op} ${paramName}");
                _sqlParams.Add(paramName, paramValue);
            }
            else
            {
                throw new ArgumentException("Invalid property selector expression.");
            }
        }

        public string GetSqlString()
        {
            return _sqlConditions.ToString();

        }

    }

    public class SortBuilder<T>
    {

        private List<string> _sqlStr = new List<string>();

        public void Add(Expression<Func<T, object>> propertySelector, SqlSortOrder sortOrder = SqlSortOrder.asc)
        {
            if (propertySelector.Body is MemberExpression memberExpression)
            {
                string[] splitResult = memberExpression.ToString().Split('.');

                // Removing the first part
                string propertyName = string.Join(".", splitResult, 1, splitResult.Length - 1);

                var sort = sortOrder == SqlSortOrder.desc ? "DESC" : "ASC";
                _sqlStr.Add($"{propertyName} {sort}");
                
            }
            else
            {
                throw new ArgumentException("Invalid property selector expression.");
            }
        }

        public string GetSqlString()
        {
            return $" order by {string.Join(",", _sqlStr)}";

        }

    }

   
}
