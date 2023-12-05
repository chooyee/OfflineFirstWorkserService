namespace Factory.CouchbaseLiteFactory
{
    public struct CouchbaseDefault
    {
        public const string Scope = "_default";
        public const string Collection = "_default";
    }

    public enum QueryResultReturnType { 
        Dictionary,
        JSON
    }
}
