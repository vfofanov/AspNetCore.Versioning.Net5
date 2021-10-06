using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using OData8VersioningPrototype.Controllers.OData;
using OData8VersioningPrototype.Controllers.OData.v1;
using OData8VersioningPrototype.Models.OData;
using OData8VersioningPrototype.Models.OData.v1;
using OData8VersioningPrototype.ODataConfigurations.Common;

namespace OData8VersioningPrototype.ODataConfigurations
{
    public class CustomODataModelProvider : ODataModelProvider<ApiVersion>
    {
        /// <inheritdoc />
        protected override ApiVersion GetNameConventionKey(ApiVersion version)
        {
            return version;
        }

        /// <inheritdoc />
        protected override ApiVersion GetKey(ApiVersion version, IServiceProvider provider)
        {
            return version;
        }

        

        /// <inheritdoc />
        protected override void FillEdmModel(AdvODataConventionModelBuilder builder, ApiVersion key)
        {
            switch (key)
            {
                case { MajorVersion: 1, MinorVersion: 0 }:
                    builder.Add<Book, BooksController>();
                    builder.EntitySet<Customer, CustomersController>();
                    break;
                case { MajorVersion: 2, MinorVersion: 0 }:
                    builder.EntitySet<Book, BooksController>();
                    builder.Add<Press, PressesController>(type =>
                    {
                        type.Collection
                            .Function(nameof(PressesController.EBooks))
                            .ReturnsCollectionFromEntitySet<Press, PressesController>();
                    });

                    builder.EntitySet<Models.OData.v2.Customer, Controllers.OData.v2.CustomersController>();
                    break;
                default:
                    throw new NotSupportedException($"The input version '{key}' is not supported!");
            }

            builder.EnableLowerCamelCase();
        }
    }
}