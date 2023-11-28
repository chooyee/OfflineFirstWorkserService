
using Couchbase.Lite.Query;
using System.Data.SQLite;


namespace Factory.DB
{
    public class DynamicSqlParameter
    {

        private List<Tuple<string, object>> sqlParams;
        public DynamicSqlParameter()
        {
            sqlParams = new List<Tuple<string, object>>();
        }

        public SQLiteParameter[] SQLiteParameters
        {
            get
            {
                var sqlParameter = new List<SQLiteParameter>();
                foreach (var param in sqlParams)
                {
                    sqlParameter.Add(new SQLiteParameter(param.Item1, param.Item2));
                }
                return sqlParameter.ToArray();

            }
        }

        public Parameters CouchbaseParameters
        {
            get
            {
                var sqlParameter = new Parameters();
                foreach (var param in sqlParams)
                {
                    sqlParameter.Add(param.Item1, param.Item2);
                }
                return sqlParameter;

            }
        }

        public void Add(string name, object value)
        {
            sqlParams.Add(Tuple.Create(name, value));

        }


        public string GetAsString()
        {
            return string.Join(',', sqlParams.Select(tuple => $"{tuple.Item1}: {tuple.Item2}"));
        }
    }

    internal static class ParameterExtension
    {
      
        internal static void Add<T>(this Parameters param, string paramName, T paramValue)
        {
            Type type = typeof(T);

            switch (type.Name)
            {
                case nameof(Couchbase.Lite.Blob):
                    param.SetBlob(paramName, (Couchbase.Lite.Blob)Convert.ChangeType(paramValue, typeof(Couchbase.Lite.Blob)));
                    break;

                case nameof(Boolean):
                    param.SetBoolean(paramName, (bool)Convert.ChangeType(paramValue, typeof(bool)));
                    break;

                case nameof(DateTime):
                    param.SetDate(paramName, (DateTime)Convert.ChangeType(paramValue, typeof(DateTime)));
                    break;

                case nameof(Double):
                    param.SetDouble(paramName, (double)Convert.ChangeType(paramValue, typeof(double)));
                    break;

                case nameof(Decimal):
                case nameof(Single):
                    param.SetFloat(paramName, (float)Convert.ChangeType(paramValue, typeof(float)));
                    break;

                case nameof(Int32):
                    param.SetInt(paramName, (int)Convert.ChangeType(paramValue, typeof(int)));
                    break;

                case nameof(Int64):
                    param.SetLong(paramName, (long)Convert.ChangeType(paramValue, typeof(long)));
                    break;

                case nameof(String):
                    param.SetString(paramName, (string)Convert.ChangeType(paramValue, typeof(string)));
                    break;

                case nameof(Object):
                    param.SetValue(paramName, (object)Convert.ChangeType(paramValue, typeof(object)));
                    break;
            }
        }
    }
}

