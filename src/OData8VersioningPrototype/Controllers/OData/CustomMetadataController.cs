using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.Extensions.Primitives;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;

namespace OData8VersioningPrototype.Controllers.OData
{
     /// <summary>
    /// Represents a controller for generating OData service and metadata ($metadata) documents.
    /// </summary>
    public class CustomMetadataController : ControllerBase
    {
        private static readonly Version DefaultEdmxVersion = new(4, 0);

        /// <summary>
        /// Generates the OData $metadata document.
        /// </summary>
        /// <returns>The <see cref="IEdmModel"/> representing $metadata.</returns>
        [HttpGet]
        public IEdmModel GetMetadata()
        {
            return GetModel();
        }

        /// <summary>
        /// Generates the OData service document.
        /// </summary>
        /// <returns>The service document for the service.</returns>
        [HttpGet]
        public ODataServiceDocument GetServiceDocument()
        {
            var model = GetModel();
            return model.GenerateServiceDocument();
        }
  
        [HttpOptions]
        public IActionResult GetOptions()
        {
            var headers = Response.Headers;

            headers.Add("Allow", new StringValues(new[] { "GET", "OPTIONS" }));
            headers.Add(ODataConstants.ODataVersionHeader, ODataUtils.ODataVersionToString(ODataVersion.V4));

            return Ok();
        }
        
        private IEdmModel GetModel()
        {
            var model = Request.GetModel();
            if (model == null)
            {
                throw new InvalidOperationException();
            }

            model.SetEdmxVersion(DefaultEdmxVersion);
            return model;
        }
    }
}