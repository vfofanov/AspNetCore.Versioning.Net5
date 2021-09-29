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
using OData8VersioningPrototype.ApiConventions;
using OData8VersioningPrototype.Models.OData;
using OData8VersioningPrototype.ODataConfigurations;
using OData8VersioningPrototype.ODataConfigurations.Common;

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
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
            });

            services.AddControllers(options =>
                {
                    options.Conventions.Add(new VersionedPrefixRoutingConvention());
                    options.Conventions.Add(new SkipStandardODataMetadataControllerRoutingConvention());
                })
                .AddOData(options =>
                {
                    //NOTE:Replace metadata convension
                    options.Conventions.Remove(options.Conventions.OfType<MetadataRoutingConvention>().First());
                    options.Conventions.Add(new OnbMetadataRoutingConvention());
                    
                    options.AddRouteComponents(OnbODataConstants.VersionRoutePrefix, modelProvider.GetNameConventionEdmModel());
                    
                    options.EnableQueryFeatures();
                });
            
            services.TryAddEnumerable(ServiceDescriptor.Singleton<MatcherPolicy, CustomODataRoutingMatcherPolicy>());
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
