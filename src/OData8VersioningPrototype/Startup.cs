using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
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
using static Microsoft.AspNetCore.Mvc.Versioning.ApiVersionReader;

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

            var apiVersionPrefix = "api/{0}";
            const string odataVersionPrefix = "api/{0}/odata";
            
            var apiVersionsProvider = ApiVersions.GetVersionsProvider();
            services.AddSingleton(apiVersionsProvider);
            
            var modelProvider = new MyODataModelProvider();
            services.AddSingleton<IODataModelProvider>(modelProvider);

            //TODO: apiVersionPrefix to Options
            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IApplicationModelProvider, ApiVersioningRoutingApplicationModelProvider>(
                    _ => new ApiVersioningRoutingApplicationModelProvider(apiVersionsProvider, apiVersionPrefix)));
            
            //NOTE: Hide OData controllers from Api Versioning 
            //services.AddSingleton<IApiControllerFilter, IgnoreODataControllersForVersioningApiControllerFilter>();
            // services.AddApiVersioning(options =>
            // {
            //     options.ReportApiVersions = true;
            // });

            services.AddControllers(options =>
                {
                    options.Conventions.Add(new SkipStandardODataMetadataControllerRoutingConvention());
                    //TODO: Add Route to odata controllers and add it to ApiExplorer
                    //options.Conventions.Add(new ApiVisibilityConvention());
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
                    
                    foreach (var version in apiVersionsProvider.Versions)
                    {
                        var prefix = string.Format(odataVersionPrefix, version.PathPartName);
                        options.AddRouteComponents(prefix, modelProvider.GetNameConventionEdmModel(version.Version));
                    }
                    //
                    
                    options.EnableQueryFeatures();
                });
            
            services.TryAddEnumerable(ServiceDescriptor.Singleton<MatcherPolicy, VersionedODataRoutingMatcherPolicy>());
            
            //Swagger
            // services.AddVersionedApiExplorer(
            //     options =>
            //     {
            //         // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
            //         // version format: https://github.com/microsoft/aspnet-api-versioning/wiki/Version-Format#custom-api-version-format-strings
            //         options.GroupNameFormat = @"'v'VV";
            //     });

            services.AddMvcCore().AddApiExplorer();
            services.TryAddSingleton<IOptionsFactory<ApiExplorerOptions>, ApiExplorerOptionsFactory<ApiExplorerOptions>>();
            services.TryAddSingleton<IApiVersionDescriptionProvider, DefaultApiVersionDescriptionProvider>();
            services.TryAddEnumerable(ServiceDescriptor.Transient<IApiDescriptionProvider, VersionedApiDescriptionProvider>() );
            
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

            app.UseODataRouteVersioningDebug(); // Page with versioning endpoints
            
            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                var versionProvider = app.ApplicationServices.GetRequiredService<IApiVersionInfoProvider>();
                foreach (var apiVersion in versionProvider.Versions)
                {
                    var name = apiVersion.PathPartName;
                    c.SwaggerEndpoint($"/swagger/{name}/swagger.json", name);
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
