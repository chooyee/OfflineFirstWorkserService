namespace Model.SSO
{
    interface ISSOConfig
    {
        string Http { get; set; }
        string AbsUrl { get; set; }
        string Auth { get; set; }
        string Introspect { get; set; }
        string HealthCheck { get; set; }
        string Realm { get; set; }
    }

    
}
