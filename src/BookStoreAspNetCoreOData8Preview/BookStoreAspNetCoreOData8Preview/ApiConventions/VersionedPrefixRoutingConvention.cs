using System;
using System.Text;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.OData.Routing.Controllers;


//TODO: Idea: {version:apiVersion}/ for all api controllers
//based on https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-5.0#use-application-model-to-customize-attribute-routes-1
namespace BookStoreAspNetCoreOData8Preview.ApiConventions
{
    public class VersionedPrefixRoutingConvention : Attribute, IControllerModelConvention
    {
        private readonly string _prefix;

        public VersionedPrefixRoutingConvention(string prefix = "v")
        {
            _prefix = (prefix ?? string.Empty) + "{version:apiVersion}";
        }

        public void Apply(ControllerModel controller)
        {
            var template = new StringBuilder();
            template.Append(_prefix + "/[controller]");

            foreach (var selector in controller.Selectors)
            {

                if (selector.AttributeRouteModel != null)
                {
                    var routeModel = selector.AttributeRouteModel;
                    if (routeModel.IsAbsoluteTemplate)
                    {
                        continue;
                    }
                    selector.AttributeRouteModel = new AttributeRouteModel { Template = _prefix + "/" + routeModel.Template.TrimStart('/') };
                }
                else
                {
                    selector.AttributeRouteModel = new AttributeRouteModel
                    {
                        Template = _prefix + "/[controller]"
                    };
                }
            }
        }
    }
}