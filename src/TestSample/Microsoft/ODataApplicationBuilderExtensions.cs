//-----------------------------------------------------------------------------
// <copyright file="ODataApplicationBuilderExtensions.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System;
using Microsoft.AspNetCore.Builder;

namespace TestSample.Microsoft
{
    /// <summary>
    /// Provides extension methods for <see cref="IApplicationBuilder"/> to add OData routes.
    /// </summary>
    public static class ODataApplicationBuilderVersioningExtensions
    {
        private const string DefaultODataRouteDebugMiddlewareRoutePattern = "$odata";

        /// <summary>
        /// Use OData route debug middleware. You can send request "~/$odata" after enabling this middleware.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder "/> to use.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseODataRouteVersioningDebug(this IApplicationBuilder app)
        {
            return app.UseODataRouteVersioningDebug(DefaultODataRouteDebugMiddlewareRoutePattern);
        }

        /// <summary>
        /// Use OData route debug middleware using the given route pattern.
        /// For example, if the given route pattern is "myrouteinfo", then you can send request "~/myrouteinfo" after enabling this middleware.
        /// Please use basic (literal) route pattern.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder "/> to use.</param>
        /// <param name="routePattern">The given route pattern.</param>
        /// <returns>The <see cref="IApplicationBuilder "/>.</returns>
        public static IApplicationBuilder UseODataRouteVersioningDebug(this IApplicationBuilder app, string routePattern)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (routePattern == null)
            {
                throw new ArgumentNullException(nameof(routePattern));
            }

            return app.UseMiddleware<ODataRouteDebugVersionedMiddleware>(routePattern);
        }
    }
}
