using AspNetCore.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace TestSample
{
    /// <summary>
    /// Contains logic for control api versioning
    /// </summary>
    public static class ApiVersions
    {
        public static readonly ApiVersion V1 = new(1, 0);
        public static readonly ApiVersion V2 = new(2, 0);

        public static IApiVersionInfoProvider GetVersionsProvider(string pathPartFormat = "v{0}")
        {
            return new ApiVersionInfoProvider(V2,
                new ApiVersionInfo(V1, string.Format(pathPartFormat, V1)),
                new ApiVersionInfo(V2, string.Format(pathPartFormat, V2)));
        }
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