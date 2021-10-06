using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.AspNetCore.OData.Routing.Conventions;
using Microsoft.AspNetCore.OData.Routing.Template;
using Microsoft.OData.Edm;

namespace OData8VersioningPrototype.ODataConfigurations.Common
{
    /// <summary>
    /// The convention for $metadata.
    /// </summary>
    public sealed class VersionedFilterRoutingConvention : IODataControllerActionConvention
    {
        /// <summary>
        /// Gets the order value for determining the order of execution of conventions.
        /// Filter must be after all other conventions and has 2000 order.
        /// </summary>
        public int Order => 2000;

        /// <inheritdoc />
        public bool AppliesToController(ODataControllerActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException();
            }

            var version = GetApiVersion(context);

            if (!IsApiVersionMatch(context.Controller, version))
            {
                return false;
            }

            for (var i = 0; i < context.Controller.Actions.Count; i++)
            {
                var action = context.Controller.Actions[i];
                if (!IsApiVersionMatch(action, version))
                {
                    //context.Controller.Actions.RemoveAt(i);
                    //i--;
                }
            }
            return true;
        }

        /// <inheritdoc />
        public bool AppliesToAction(ODataControllerActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException();
            }
            return IsApiVersionMatch(context.Action, context);
        }

        private static bool IsApiVersionMatch(ICommonModel model, ODataControllerActionContext context)
        {
            var version = GetApiVersion(context);
            return IsApiVersionMatch(model, version);
        }

        private static ApiVersion GetApiVersion(ODataControllerActionContext context)
        {
            return context.Model.GetAnnotationValue<ApiVersionAnnotation>(context.Model)?.ApiVersion;
        }

        private static bool IsApiVersionMatch(ICommonModel model, ApiVersion version)
        {
            var apiVersionsNeutral = model.Attributes.OfType<IApiVersionNeutral>().ToList();
            if (apiVersionsNeutral.Count != 0)
            {
                return true;
            }

            var apiVersions = model.Attributes.OfType<IApiVersionProvider>().SelectMany(p => p.Versions).Distinct().ToList();
            if (apiVersions.Count == 0)
            {
                // If no [ApiVersion] on the controller,
                // Let's simply return true, it means it can work the input version or any version.
                return true;
            }
            return apiVersions.Any(v => v == version);
        }
    }
}