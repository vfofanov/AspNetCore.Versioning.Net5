// Licensed under the MIT License.

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Edm;

namespace OData8VersioningPrototype.ODataConfigurations.Common
{
    public interface IODataModelProvider
    {
        IEdmModel GetEdmModel(ApiVersion version, IServiceProvider provider);
        
        /// <summary>
        /// Model for run standard entites' name conventions for odata controllers.
        /// </summary>
        /// <returns></returns>
        IEdmModel GetNameConventionEdmModel();
    }
}
