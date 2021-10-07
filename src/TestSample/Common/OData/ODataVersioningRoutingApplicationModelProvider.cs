using System.Collections.Generic;
using System.Linq;
using AspNetCore.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using OData8VersioningPrototype.ODataConfigurations.Common;

namespace OData8VersioningPrototype.ApiConventions
{
    /// <summary>
    /// Versioning routing provider for odata controllers with attribute <see cref="ODataAttributeRoutingAttribute"/>
    /// </summary>
    public sealed class ODataVersioningRoutingApplicationModelProvider : VersioningRoutingApplicationModelProvider
    {
        /// <inheritdoc />
        public ODataVersioningRoutingApplicationModelProvider(IApiVersionInfoProvider versionInfoProvider, string prefixFormat = "{0}/odata")
            : base(versionInfoProvider, prefixFormat)
        {
        }

        /// <inheritdoc />
        public override IEnumerable<ControllerModel> GetApiControllers(ApplicationModelProviderContext context)
        {
            return context.Result.Controllers.Where(c => c.ControllerType.IsAssignableTo(typeof(MetadataControllerBase)) ||
                                                         c.Attributes.OfType<ODataAttributeRoutingAttribute>().Any());
        }

        /// <inheritdoc />
        public override void OnProvidersExecuted(ApplicationModelProviderContext context)
        {
            var standardMetadataController =
                context.Result.Controllers.FirstOrDefault(c => c.ControllerType == typeof(Microsoft.AspNetCore.OData.Routing.Controllers.MetadataController));
            
            
            base.OnProvidersExecuted(context);
        }

        /// <inheritdoc />
        protected override void CleanUpActionSelectors((string Prefix, ApiVersionInfo Info) versionDesc, IList<SelectorModel> selectors)
        {
            //NOTE: After OData model provider need clean up selectors by version 
            CleanUpSelectors(versionDesc, selectors);
        }
    }
}