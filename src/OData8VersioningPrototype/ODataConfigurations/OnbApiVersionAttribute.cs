using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreAspNetCoreOData8Preview.ODataConfigurations
{
    [DebuggerDisplay("{_version}")]
    public class OnbApiVersionAttribute : ApiVersionAttribute
    {
        private readonly ApiVersion _version;

        /// <inheritdoc />
        protected OnbApiVersionAttribute(ApiVersion version) 
            : base(version)
        {
            _version = version;
        }

        /// <inheritdoc />
        public OnbApiVersionAttribute(string version)
            : this(ApiVersion.Parse(version))
        {
        }
    }
}