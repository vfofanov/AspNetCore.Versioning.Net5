using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing;
using Microsoft.AspNetCore.OData.Routing.Conventions;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Converters;
using OData8VersioningPrototype.ApiConventions;
using OData8VersioningPrototype.Controllers.OData;
using OData8VersioningPrototype.Models.OData;
using OData8VersioningPrototype.ODataConfigurations;
using OData8VersioningPrototype.ODataConfigurations.Common;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace OData8VersioningPrototype
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<BookStoreContext>(opt => opt.UseInMemoryDatabase("BookLists"));

            var modelProvider = new CustomODataModelProvider();
            services.AddSingleton<IODataModelProvider>(modelProvider);
            
            //NOTE: Hide OData controllers from Api Versioning 
            //services.AddSingleton<IApiControllerFilter, IgnoreODataControllersForVersioningApiControllerFilter>();
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
            });

            services.AddControllers(options =>
                {
                    options.Conventions.Add(new VersionedPrefixRoutingConvention());
                    options.Conventions.Add(new SkipStandardODataMetadataControllerRoutingConvention());
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                })
                .AddOData(options =>
                {
                    //NOTE:Replace metadata convension
                    options.Conventions.Remove(options.Conventions.OfType<MetadataRoutingConvention>().First());
                    options.Conventions.Add(new VersionedMetadataRoutingConvention<CustomMetadataController>());
                    
                    options.AddRouteComponents(RouteODataConstants.VersionRouteComponentPrefix, modelProvider.GetNameConventionEdmModel());
                    
                    options.EnableQueryFeatures();
                });
            
            services.TryAddEnumerable(ServiceDescriptor.Singleton<MatcherPolicy, VersionedODataRoutingMatcherPolicy>());
            
            //Swagger
            services.AddVersionedApiExplorer(
                options =>
                {
                    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                    // version format: https://github.com/microsoft/aspnet-api-versioning/wiki/Version-Format#custom-api-version-format-strings
                    options.GroupNameFormat = @"'v'VV";
                });

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseODataBatching();

            app.UseODataRouteDebug(); // Remove it if not needed
            
            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                foreach (var apiVersion in ApiVersions.List)
                {
                    c.SwaggerEndpoint($"/swagger/v{apiVersion}/swagger.json", $"version {apiVersion}");
                }
                c.RoutePrefix = string.Empty;
                c.DocExpansion(DocExpansion.None);
            });
            
            // Test middelware
            app.Use(next => context =>
            {
                var endpoint = context.GetEndpoint();
                if (endpoint == null)
                {
                    return next(context);
                }

                IEnumerable<string> templates;
                var metadata = endpoint.Metadata.GetMetadata<IODataRoutingMetadata>();
                if (metadata != null)
                {
                    templates = metadata.Template.GetTemplates();
                }

                return next(context);
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
       
    }
}
