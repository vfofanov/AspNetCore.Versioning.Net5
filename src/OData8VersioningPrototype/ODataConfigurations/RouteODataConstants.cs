namespace OData8VersioningPrototype.ODataConfigurations
{
    public static class RouteODataConstants
    {
        public const string VersionParameterName = "version";
        public const string VersionRouteComponentPrefix = "odata/v{version}";
        public const string VersionRoutePrefix = "~/" + VersionRouteComponentPrefix + "/";
    }
}