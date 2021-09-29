using System;
using System.Diagnostics;
using System.Reflection;
using BookStoreAspNetCoreOData8Preview.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.AspNetCore.OData.Routing.Conventions;
using Microsoft.AspNetCore.OData.Routing.Template;

namespace BookStoreAspNetCoreOData8Preview.ODataConfigurations
{
   /// <summary>
    /// The convention for $metadata.
    /// </summary>
    public sealed class OnbMetadataRoutingConvention : IODataControllerActionConvention
    {
        private static readonly TypeInfo metadataTypeInfo = typeof(OnbMetadataController).GetTypeInfo();

        /// <summary>
        /// Gets the order value for determining the order of execution of conventions.
        /// Metadata routing convention has 0 order.
        /// </summary>
        public int Order => 0;

        /// <inheritdoc />
        public bool AppliesToController(ODataControllerActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException();
            }

            // This convention only applies to "MetadataController".
            return context.Controller.ControllerType == metadataTypeInfo;
        }

        /// <inheritdoc />
        public bool AppliesToAction(ODataControllerActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException();
            }

            Debug.Assert(context.Controller != null);
            Debug.Assert(context.Action != null);
            
            var action = context.Action;
            var actionName = action.ActionName;

            switch (actionName)
            {
                case nameof(OnbMetadataController.GetMetadata):
                {
                    // for ~$metadata
                    var template = new ODataPathTemplate(MetadataSegmentTemplate.Instance);
                    action.AddSelector(HttpMethods.Get, context.Prefix, context.Model, template, context.Options?.RouteOptions);
                    return true;
                }
                case nameof(OnbMetadataController.GetServiceDocument):
                {
                    // GET for ~/
                    var template = new ODataPathTemplate();
                    action.AddSelector(HttpMethods.Get, context.Prefix, context.Model, template, context.Options?.RouteOptions);
                    return true;
                }
                case nameof(OnbMetadataController.GetOptions):
                {
                    //OPTIONS for ~/
                    var template = new ODataPathTemplate();
                    action.AddSelector(HttpMethods.Options, context.Prefix, context.Model, template, context.Options?.RouteOptions);
                    return true;
                }
                default:
                    return false;
            }
        }
    }
}