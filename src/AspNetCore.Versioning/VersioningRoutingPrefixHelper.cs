namespace AspNetCore.Versioning
{
    public static class VersioningRoutingPrefixHelper
    {
        public static string GeneratePrefix(string prefixFormat, ApiVersionInfo v)
        {
            return string.Format("/" + prefixFormat.TrimStart('/'), v.PathPartName).TrimEnd('/');
        }
    }
}