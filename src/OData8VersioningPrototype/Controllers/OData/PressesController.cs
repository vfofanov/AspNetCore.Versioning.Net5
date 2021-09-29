using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using OData8VersioningPrototype.Models.OData;
using OData8VersioningPrototype.ODataConfigurations;

namespace OData8VersioningPrototype.Controllers.OData
{
    [ApiVersionV2]
    [ODataControllerRoute(EntitySets.Presses)]
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

        [HttpGet]
        [EnableQuery]
        public IQueryable<Press> Get()
        {
            return _db.Presses;
        }

        /// <summary>
        /// Returns suppliers that have deals with current user's buyer company SuppliersThatHaveDealsWithCurrentBuyer
        /// </summary>
        /// <returns></returns>
        [HttpGet(EntityOperations.EBooks)]
        [EnableQuery(PageSize = 20, AllowedQueryOptions = AllowedQueryOptions.All)]
        public Task<IQueryable<Press>> EBooks()
        {
            return Task.FromResult(_db.Presses.Where(p => p.Category == Category.EBook).AsQueryable());
        }
    }
}
