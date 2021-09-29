using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.ModelBuilder;
using OData8VersioningPrototype.Controllers.OData;
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
                    builder.EntitySet<Book>(EntitySets.Books);
                    builder.EntitySet<Customer>(EntitySets.Customers);
                    break;
                case "2.0":
                    builder.EntitySet<Book>(EntitySets.Books);
                    var presses = builder.EntitySet<Press>(EntitySets.Presses).EntityType;
                    presses.Collection
                        .Action(EntityOperations.EBooks)
                        .ReturnsCollectionFromEntitySet<Press>(EntitySets.Presses);
                    
                    builder.EntitySet<Models.OData.v2.Customer>(EntitySets.Customers);
                    break;
                default:
                    throw new NotSupportedException($"The input version '{key}' is not supported!");
            }
        }
    }
}