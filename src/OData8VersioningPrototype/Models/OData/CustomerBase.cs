// Licensed under the MIT License.

namespace OData8VersioningPrototype.Models.OData
{
    public abstract class CustomerBase
    {
        public int Id { get; set; }

        public string ApiVersion { get; set; }
    }
}
