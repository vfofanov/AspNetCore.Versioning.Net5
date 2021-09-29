// Copyright saxu@microsoft.com.  All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using BookStoreAspNetCoreOData8Preview.Models;
using BookStoreAspNetCoreOData8Preview.Models.v1;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace BookStoreAspNetCoreOData8Preview.ODataConfigurations
{
    public class OnbODataModelProvider : IODataModelProvider
    {
        private readonly IDictionary<string, IEdmModel> _cached = new Dictionary<string, IEdmModel>();

        public IEdmModel GetEdmModel(string apiVersion)
        {
            if (_cached.TryGetValue(apiVersion, out var model))
            {
                return model;
            }

            model = BuildEdmModel(apiVersion);
            _cached[apiVersion] = model;
            return model;
        }

        public static IEdmModel GetFullEdmModel()
        {
            return BuildEdmModel("full");
        }

        public static IEdmModel BuildEdmModel(string version)
        {
            var builder = new ODataConventionModelBuilder();
            switch (version)
            {
                case "1.0":
                    builder.EntitySet<Book>("Books");
                    builder.EntitySet<Customer>("Customers");
                    break;
                case "2.0":
                    builder.EntitySet<Press>("Presses");
                    builder.EntitySet<Models.v2.Customer>("Customers");
                    break;
                case "full":
                    //NOTE: We need this for properly work standard entitites' conventions.
                    builder.EntitySet<DummyModel>("Books");
                    builder.EntitySet<DummyModel>("Presses");
                    builder.EntitySet<DummyModel>("Addresses");
                    builder.EntitySet<DummyModel>("Customers");
                    break;
                default:
                    throw new NotSupportedException($"The input version '{version}' is not supported!");
            }
            return builder.GetEdmModel();
        }
    }
}
