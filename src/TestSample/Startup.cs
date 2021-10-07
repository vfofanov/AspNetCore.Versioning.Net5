using System.Linq;
using AspNetCore.OData.Versioning;
using AspNetCore.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Conventions;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using TestSample.Controllers.OData;
using TestSample.Models.OData;
using TestSample.Swagger;

namespace TestSample
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

            const string apiVersionPrefix = "api/{0}";
            const string odataVersionPrefix = "api/{0}/odata";
            
            var apiVersionsProvider = ApiVersions.GetVersionsProvider();
            services.AddSingleton(apiVersionsProvider);
            
            var modelProvider = new ODataModelProvider();
            services.AddSingleton<IODataModelProvider>(modelProvider);

            //NOTE: Only for debug
            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IApplicationModelProvider, DebugCheckApplicationModelProvider>());
            
            //TODO: Put prefix to Options and resolve providers by DI
            //NOTE: Copies controller's models by versions and set versioned routing 
            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IApplicationModelProvider, ApiVersioningRoutingApplicationModelProvider>(
                    _ => new ApiVersioningRoutingApplicationModelProvider(apiVersionsProvider, apiVersionPrefix)));
            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IApplicationModelProvider, ODataVersioningRoutingApplicationModelProvider>(
                    _ => new ODataVersioningRoutingApplicationModelProvider(apiVersionsProvider, odataVersionPrefix)));
            
            
           
            services.AddControllers(options =>
                {
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
                    options.Conventions.Add(new VersionedMetadataRoutingConvention<MetadataController>());
                    
                    foreach (var version in apiVersionsProvider.Versions)
                    {
                        var prefix = string.Format(odataVersionPrefix, version.PathPartName);
                        options.AddRouteComponents(prefix, modelProvider.GetNameConventionEdmModel(version.Version));
                    }
                    
                    options.EnableQueryFeatures();
                });
            
            services.TryAddEnumerable(ServiceDescriptor.Singleton<MatcherPolicy, VersionedODataRoutingMatcherPolicy>());
            
            //Swagger
            services.AddMvcCore().AddApiExplorer();
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseODataRouteDebug(); //OData debugging endpoints page 
            }
            
            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                var versionProvider = app.ApplicationServices.GetRequiredService<IApiVersionInfoProvider>();
                foreach (var apiVersion in versionProvider.Versions)
                {
                    var name = apiVersion.PathPartName;
                    options.SwaggerEndpoint($"/swagger/{name}/swagger.json", name);
                }
                options.RoutePrefix = string.Empty;
                options.DocExpansion(DocExpansion.None);
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}