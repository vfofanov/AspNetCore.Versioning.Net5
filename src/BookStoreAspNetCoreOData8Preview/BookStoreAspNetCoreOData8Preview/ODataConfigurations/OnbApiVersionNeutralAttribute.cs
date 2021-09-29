using System;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace BookStoreAspNetCoreOData8Preview.ODataConfigurations
{
    /// <summary>
    /// Represents the metadata to indicate a service is API version neutral.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class OnbApiVersionNeutralAttribute : Attribute, IApiVersionNeutral
    {
    }
}