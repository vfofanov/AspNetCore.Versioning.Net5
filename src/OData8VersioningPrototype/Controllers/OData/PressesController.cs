using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using OData8VersioningPrototype.Models.OData;

namespace OData8VersioningPrototype.Controllers.OData
{
    public class PressesController : ODataController
    {
        private readonly BookStoreContext _db;

        public PressesController(BookStoreContext context)
        {
            _db = context;
            if (context.Books.Any())
            {
                return;
            }
            
            foreach (var b in DataSource.GetBooks())
            {
                context.Books.Add(b);
                context.Presses.Add(b.Press);
            }
            context.SaveChanges();
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_db.Presses);
        }
    }
}
