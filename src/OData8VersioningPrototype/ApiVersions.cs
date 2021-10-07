using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace OData8VersioningPrototype
{
    public static class ApiVersions
    {
        public static readonly ApiVersion V1 = new(1, 0);
        public static readonly ApiVersion V2 = new(2, 0);
        public static readonly ApiVersion[] List = { V1, V2 };

        public static readonly ApiVersionDescription[] Descriptions =
        {
            new(V1, V1.ToString(), false),
            new(V2, V2.ToString(), false)
        };
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