using Microsoft.AspNetCore.Mvc;

namespace OData8VersioningPrototype
{
    public static class ApiVersions
    {
        public static readonly ApiVersion V1 = new(1, 0);
        public static readonly ApiVersion V2 = new(1, 0);
        public static readonly ApiVersion[] List = { V1, V2 };
    }

    public class ApiVersionV1Attribute : ApiVersionAttribute
    {
        /// <inheritdoc />
        public ApiVersionV1Attribute() 
            : base(ApiVersions.V1)
        {
        }
    }
    
    public class ApiVersionV2Attribute : ApiVersionAttribute
    {
        /// <inheritdoc />
        public ApiVersionV2Attribute() 
            : base(ApiVersions.V2)
        {
        }
    }
}