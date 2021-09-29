// Copyright saxu@microsoft.com.  All rights reserved.
// Licensed under the MIT License.

using System.Linq;
using BookStoreAspNetCoreOData8Preview.Models.v2;
using BookStoreAspNetCoreOData8Preview.ODataConfigurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace BookStoreAspNetCoreOData8Preview.Controllers.v2
{
    [ApiVersion("2.0")]
    [ApiVersion("2.0")]
    public class CustomersController : ODataController
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
