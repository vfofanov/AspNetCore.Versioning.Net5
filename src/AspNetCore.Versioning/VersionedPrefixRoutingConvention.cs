using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

//based on https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-5.0#use-application-model-to-customize-attribute-routes-1
namespace AspNetCore.Versioning
{
    public class VersionedPrefixRoutingConvention : Attribute, IControllerModelConvention
    {
        private readonly string _prefix;
        private readonly List<(string Prefix, ApiVersionAnnotation Annotation)> _versionAnnotations;

        public VersionedPrefixRoutingConvention(IReadOnlyCollection<ApiVersion> versions, string prefix = "v")
        {
            _versionAnnotations = versions.Select(v => (Prefix: prefix + v, Annotation: new ApiVersionAnnotation(v))).ToList();
            _prefix = prefix;
        }

        public void Apply(ControllerModel controller)
        {
            foreach (var selector in controller.Selectors)
            {
//                controller.Selectors.Add(new SelectorModel(selector));

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