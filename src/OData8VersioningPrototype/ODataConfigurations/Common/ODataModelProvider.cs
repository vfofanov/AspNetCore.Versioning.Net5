﻿// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace OData8VersioningPrototype.ODataConfigurations.Common
{
    public abstract class ODataModelProvider<TKey> : IODataModelProvider
    {
        private readonly Lazy<IEdmModel> _nameConventionModel;

        private readonly ConcurrentDictionary<TKey, IEdmModel> _cached = new();

        protected ODataModelProvider()
        {
            _nameConventionModel = new Lazy<IEdmModel>(CreateNameConventionEdmModel);
        }
        
        public IEdmModel GetNameConventionEdmModel()
        {
            return _nameConventionModel.Value;
        }
        
        public IEdmModel GetEdmModel(ApiVersion apiVersion, IServiceProvider serviceProvider)
        {
            var key = GetKey(apiVersion, serviceProvider);
            return _cached.GetOrAdd(key, CreateModel);
        }

        
        protected virtual IEdmModel CreateNameConventionEdmModel()
        {
            var builder =  CreateBulder();
            foreach (var key in GetAllKeys())
            {
                FillEdmModel(builder,key);
            }
            return builder.GetEdmModel();
        }

        protected AdvODataConventionModelBuilder CreateBulder(ODataConventionModelBuilder builder = null)
        {
            return new AdvODataConventionModelBuilder(builder ?? new ODataConventionModelBuilder());
        }

        protected abstract TKey GetKey(ApiVersion version, IServiceProvider provider);
        protected abstract IEnumerable<TKey> GetAllKeys();        
        protected abstract void FillEdmModel(AdvODataConventionModelBuilder builder, TKey key);
        
        private IEdmModel CreateModel(TKey key)
        {
            var builder = CreateBulder();
            
            builder.EnableLowerCamelCase();
            
            FillEdmModel(builder, key);
            return builder.GetEdmModel();
        }
    }
}
