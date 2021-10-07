using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace AspNetCore.Versioning
{
    /// <summary>
    /// Versioning routing provider for controllers with attribute <see cref="ApiControllerAttribute"/>
    /// </summary>
    public sealed class ApiVersioningRoutingApplicationModelProvider : VersioningRoutingApplicationModelProvider
    {
        /// <inheritdoc />
        public ApiVersioningRoutingApplicationModelProvider(IApiVersionInfoProvider versionInfoProvider, string prefixFormat = "{0}")
            : base(versionInfoProvider, prefixFormat)
        {
        }

        /// <inheritdoc />
        public override IEnumerable<ControllerModel> GetApiControllers(ApplicationModelProviderContext context)
        {
            return context.Result.Controllers.Where(c => c.Attributes.OfType<ApiControllerAttribute>().Any());
        }
    }
}