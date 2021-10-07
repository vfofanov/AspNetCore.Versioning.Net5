using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.OData.Routing.Template;
using Microsoft.OData.Edm;

namespace OData8VersioningPrototype.ApiConventions
{
    public class VersioningRoutingApplicationModelProvider : IApplicationModelProvider
    {
        //After all
        public int Order => 2000;
        
        public void OnProvidersExecuted(ApplicationModelProviderContext context)
        {
            foreach (var controller in context.Result.Controllers)
            {
                if (controller.Attributes.OfType<ODataAttributeRoutingAttribute>().Any())
                {
                    continue;
                }

                foreach (var action in controller.Actions)
                {
                    
                }
                
            }
        }

        public void OnProvidersExecuting(ApplicationModelProviderContext context)
        {
        }
    }
}
