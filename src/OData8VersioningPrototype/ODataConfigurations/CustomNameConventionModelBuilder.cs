using System;
using OData8VersioningPrototype.ODataConfigurations.Common;

namespace OData8VersioningPrototype.ODataConfigurations
{
    public sealed class CustomNameConventionModelBuilder : NameConventionModelBuilder
    {
        /// <inheritdoc />
        protected override Type GetNameConventionType(Type entityType)
        {
            return entityType;
            //return typeof(NameConventionDummyModel);
        }
    }
    
    public class NameConventionDummyModel
    {
        public int Id { get; set; }
    }
}