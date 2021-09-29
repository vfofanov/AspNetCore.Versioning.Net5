// Copyright saxu@microsoft.com.  All rights reserved.
// Licensed under the MIT License.

using System.Linq;
using BookStoreAspNetCoreOData8Preview.Models.v1;
using BookStoreAspNetCoreOData8Preview.ODataConfigurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace BookStoreAspNetCoreOData8Preview.Controllers.v1
{
    [ApiVersion("1.0")]
    public class CustomersController : ODataController
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
