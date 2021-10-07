﻿#nullable enable
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using OData8VersioningPrototype.ODataConfigurations.Common;

namespace OData8VersioningPrototype.ApiConventions
{
    public static class ApiVersionControllerExtensions
    {
        /// <summary>
        /// Gets current request <see cref="ApiVersion"/> version
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ApiVersion? GetApiVersion(this Controller controller)
        {
            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }
            var annotation = controller.ControllerContext.ActionDescriptor.GetProperty<ApiVersionAnnotation?>();
            return annotation?.ApiVersion;
        }
    }
}