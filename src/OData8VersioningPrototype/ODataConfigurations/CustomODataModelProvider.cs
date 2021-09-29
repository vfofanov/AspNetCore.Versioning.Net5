using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.ModelBuilder;
using OData8VersioningPrototype.Models.OData;
using OData8VersioningPrototype.Models.OData.v1;
using OData8VersioningPrototype.ODataConfigurations.Common;

namespace OData8VersioningPrototype.ODataConfigurations
{
    public class CustomODataModelProvider : ODataModelProvider<ApiVersion, CustomNameConventionModelBuilder>
    {
        /// <inheritdoc />
        protected override ApiVersion GetKey(ApiVersion version, IServiceProvider provider)
        {
            return version;
        }

        /// <inheritdoc />
        protected override IEnumerable<ApiVersion> GetAllKeys()
        {
            return ApiVersions.List;
        }

        /// <inheritdoc />
        protected override void FillEdmModel(ODataConventionModelBuilder builder, ApiVersion key)
        {
            switch (key.ToString())
            {
                case "1.0":
                    builder.EntitySet<Book>("Books");
                    builder.EntitySet<Customer>("Customers");
                    break;
                case "2.0":
                    builder.EntitySet<Book>("Books");
                    builder.EntitySet<Press>("Presses");
                    builder.EntitySet<Models.OData.v2.Customer>("Customers");
                    break;
                default:
                    throw new NotSupportedException($"The input version '{key}' is not supported!");
            }
        }
    }
}