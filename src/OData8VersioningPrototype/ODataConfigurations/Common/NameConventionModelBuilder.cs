using System;
using System.Linq;
using Microsoft.OData.ModelBuilder;

namespace OData8VersioningPrototype.ODataConfigurations.Common
{
    
    public abstract class NameConventionModelBuilder : ODataConventionModelBuilder
    {
        /// <summary>
        /// Select specific dummy for name convention type
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        protected abstract Type GetNameConventionType(Type entityType);

        /// <inheritdoc />
        public override EntityTypeConfiguration AddEntityType(Type type)
        {
            return base.AddEntityType(GetNameConventionType(type));
        }

        /// <inheritdoc />
        public override EntitySetConfiguration AddEntitySet(string name, EntityTypeConfiguration entityType)
        {
            if (EntitySets.Any(s => s.Name == name))
            {
                return new EntitySetConfiguration(this, entityType, name);
            }
            return base.AddEntitySet(name, entityType);
        }
    }
}