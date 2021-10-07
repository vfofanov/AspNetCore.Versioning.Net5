using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace OData8VersioningPrototype.ApiConventions
{
    public interface IApiVersionInfoProvider
    {
        ApiVersion Default { get; }
        IReadOnlyList<ApiVersionInfo> Versions { get; }
    }
}