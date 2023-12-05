namespace Factory.CouchbaseLiteFactory
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
    public class DocumentIdAttribute:Attribute
    {
        public DocumentIdAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
    public class CollectionAttribute : Attribute
    {
        public string CollectionName { get; private set; }
        public CollectionAttribute(string collectionName) {
            CollectionName = collectionName;
}
    }
}
