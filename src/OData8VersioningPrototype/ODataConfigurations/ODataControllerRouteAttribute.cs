using System;
using Microsoft.AspNetCore.Mvc;

namespace OData8VersioningPrototype.ODataConfigurations
{
    /// <summary>
    /// Specifies an attribute route on a controller.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ODataControllerRouteAttribute : RouteAttribute
    {
        /// <inheritdoc />
        public ODataControllerRouteAttribute(string template) 
            : base(AddVersionPrefix(template))
        {
            ControllerTemplate = template;
        }

        public string ControllerTemplate { get; }

        private static string AddVersionPrefix(string template)
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                throw new ArgumentException(nameof(template));
            }
            return RouteODataConstants.VersionRoutePrefix + template.TrimStart('/');
        }
    }
}