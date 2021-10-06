// Licensed under the MIT License.

using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using OData8VersioningPrototype.Models.OData.v1;
using OData8VersioningPrototype.ODataConfigurations.Common;

namespace OData8VersioningPrototype.Controllers.OData.v1
{
    [ApiVersionV1]
    public class CustomersController : ODataController<Customer>
    {
        private readonly Customer[] _customers = {
            new()
            {
                Id = 1,
                ApiVersion = "v1.0",
                Name = "Sam",
                PhoneNumber = "111-222-3333"
            },
            new()
            {
                Id = 2,
                ApiVersion = "v1.0",
                Name = "Peter",
                PhoneNumber = "456-ABC-8888"
            }
        };

        [HttpGet]
        [EnableQuery]
        public IQueryable<Customer> Get()
        {
            return _customers.AsQueryable();
        }

        [HttpGet]
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
