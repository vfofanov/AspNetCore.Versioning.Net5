using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.OData.Routing.Template;
using Microsoft.OData.Edm;
using OData8VersioningPrototype.ODataConfigurations.Common;

namespace OData8VersioningPrototype.ApiConventions
{
    public class VersioningRoutingApplicationModelProvider : IApplicationModelProvider
    {
        private readonly string _prefix;
        private readonly List<(string Prefix, ApiVersionAnnotation Annotation)> _versionDescriptions;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="versions"></param>
        /// <param name="prefix"></param>
        public VersioningRoutingApplicationModelProvider(IEnumerable<ApiVersion> versions, string prefix = "v")
        {
            _versionDescriptions = versions.Select(v => (Prefix: prefix + v, Annotation: new ApiVersionAnnotation(v))).ToList();
            _prefix = prefix;
        }

        //After all providers
        /// <inheritdoc />
        public int Order => 2000;

        /// <inheritdoc />
        public void OnProvidersExecuted(ApplicationModelProviderContext context)
        {
            var apiControllers = context.Result.Controllers.Where(c => c.Attributes.OfType<ApiControllerAttribute>().Any()).ToList();

            foreach (var controller in apiControllers)
            {
                var versions = GetVersions(controller.Attributes);
                for (var i = 1; i < versions.Count; i++)
                {
                    var version = versions[i];
                    var current = new ControllerModel(controller);
                    ProcessController(current, version);
                    context.Result.Controllers.Add(current);
                }
                ProcessController(controller, versions[0]);
            }
        }

        private void ProcessController(ControllerModel controller, (string Prefix, ApiVersionAnnotation Annotation) versionDesc)
        {
            controller.SetProperty(versionDesc.Annotation);

            ApplyRoutePrefix(controller, versionDesc);

            for (var i = 0; i < controller.Actions.Count; i++)
            {
                var action = controller.Actions[i];
                if (IsApiVersionMatch(action.Attributes, versionDesc.Annotation.ApiVersion))
                {
                    action.SetProperty(versionDesc.Annotation);
                }
                else
                {
                    controller.Actions.RemoveAt(i);
                    i--;
                }
            }
        }

        private static void ApplyRoutePrefix(ControllerModel controller, (string Prefix, ApiVersionAnnotation Annotation) versionDesc)
        {
            foreach (var selector in controller.Selectors)
            {
                if (selector.AttributeRouteModel != null)
                {
                    var routeModel = selector.AttributeRouteModel;
                    if (routeModel.IsAbsoluteTemplate)
                    {
                        continue;
                    }
                    selector.AttributeRouteModel = new AttributeRouteModel { Template = versionDesc.Prefix + "/" + routeModel.Template.TrimStart('/') };
                }
                else
                {
                    selector.AttributeRouteModel = new AttributeRouteModel
                    {
                        Template = versionDesc.Prefix + "/[controller]"
                    };
                }
            }
        }


        public IReadOnlyList<(string Prefix, ApiVersionAnnotation Annotation)> GetVersions(IReadOnlyList<object> attributes)
        {
            if (IsApiVersionNeutral(attributes))
            {
                return _versionDescriptions.AsReadOnly();
            }

            var result = new List<(string Prefix, ApiVersionAnnotation Annotation)>(_versionDescriptions.Count);
            for (var i = 0; i < attributes.Count; i++)
            {
                var attribute = attributes[i];
                if (attribute is not IApiVersionProvider versionProvider)
                {
                    continue;
                }

                foreach (var version in versionProvider.Versions)
                {
                    if (result.FindIndex(x => x.Annotation.ApiVersion == version) >= 0)
                    {
                        continue;
                    }
                    result.Add(_versionDescriptions.Find(x => x.Annotation.ApiVersion == version));
                }
            }
            return result;
        }

        private static bool IsApiVersionNeutral(IReadOnlyList<object> attributes)
        {
            var result = true;
            for (var i = 0; i < attributes.Count; i++)
            {
                var attribute = attributes[i];
                if (attribute is IApiVersionNeutral)
                {
                    return true;
                }
                if (attribute is IApiVersionProvider)
                {
                    result = false;
                }
            }
            return result;
        }

        private static bool IsApiVersionMatch(IReadOnlyList<object> attributes, ApiVersion version)
        {
            var result = true;
            for (var i = 0; i < attributes.Count; i++)
            {
                switch (attributes[i])
                {
                    case IApiVersionNeutral:
                        return true;
                    case IApiVersionProvider versionProvider:
                    {
                        for (var v = 0; v < versionProvider.Versions.Count; v++)
                        {
                            if (versionProvider.Versions[v] == version)
                            {
                                return true;
                            }
                        }
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }

        /// <inheritdoc />
        public void OnProvidersExecuting(ApplicationModelProviderContext context)
        {
        }
    }
}
