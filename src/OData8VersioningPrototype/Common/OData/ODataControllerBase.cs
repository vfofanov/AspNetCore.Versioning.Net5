using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace OData8VersioningPrototype.ODataConfigurations.Common
{
    public abstract class ODataController<TEntity> : ODataController, IODataController<TEntity>
    {
    }
}