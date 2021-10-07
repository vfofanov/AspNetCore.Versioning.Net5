using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OData8VersioningPrototype.ApiConventions;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OData8VersioningPrototype
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionInfoProvider _versionInfoProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
        /// </summary>
        public ConfigureSwaggerOptions(IApiVersionInfoProvider versionInfoProvider)
        {
            if (versionInfoProvider == null)
            {
                throw new ArgumentNullException(nameof(versionInfoProvider));
            }
            _versionInfoProvider = versionInfoProvider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            // add a swagger document for each discovered API version
            // note: you might choose to skip or document deprecated API versions differently
            foreach (var info in _versionInfoProvider.Versions)
            {
                var desc = info.Description;
                options.SwaggerDoc(info.PathPartName, CreateInfoForApiVersion(desc));
            }
            
            // add a custom operation filter which sets default values
            options.OperationFilter<SwaggerDefaultValues>();
            
            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        }

        private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo
            {
                Title = "Test API",
                Version = description.ApiVersion.ToString()
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }
    }
}
