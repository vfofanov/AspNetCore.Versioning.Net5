// Licensed under the MIT License.

using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using OData8VersioningPrototype.Models.OData.v2;
using OData8VersioningPrototype.ODataConfigurations.Common;

namespace OData8VersioningPrototype.Controllers.OData.v2
{
    [ApiVersionV2]
    public class CustomersController : ODataController<Customer>
    {
        private readonly Customer[] _customers = {
            new()
            {
                Id = 11,
                ApiVersion = "v2.0",
                FirstName = "YXS",
                LastName = "WU",
                Email = "yxswu@abc.com"
            },
            new()
            {
                Id = 12,
                ApiVersion = "v2.0",
                FirstName = "KIO",
                LastName = "XU",
                Email = "kioxu@efg.com"
            }
        };

        [EnableQuery]
        public IQueryable<Customer> Get()
        {
            return _customers.AsQueryable();
        }

        [EnableQuery]
        public IActionResult Get(int key)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == key);
            if (customer == null)
            {
                return NotFound($"Cannot find customer with Id={key}.");
            }

            return Ok(customer);
        }
    }
}
