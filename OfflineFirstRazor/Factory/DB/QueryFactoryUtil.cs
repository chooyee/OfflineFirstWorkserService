namespace Factory.DB
{
    internal class QueryFactoryUtil
    {
    }

    public struct QueryParam
    {
        public string ParamName { get; set; }
        public Type ParamType { get; set; }
        public QueryOperator QueryOperator { get; set; }
        public string ParamValue { get; set; }

    }

    public struct QueryOperator
    {
        public static string Equal => "=";
        public static string Greater => ">";
        public static string GreaterOrEqual => ">=";
        public static string Less => "<";
        public static string LessOrEqual => "<=";
        public static string Like => "like";
    }
}
