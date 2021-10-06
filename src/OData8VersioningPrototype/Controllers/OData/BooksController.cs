using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using OData8VersioningPrototype.Models.OData;
using OData8VersioningPrototype.ODataConfigurations.Common;

namespace OData8VersioningPrototype.Controllers.OData
{
    /// <summary>
    /// Books' endpoint
    /// </summary>
    public class BooksController : ODataController<Book>
    {
        private readonly BookStoreContext _db;

        public BooksController(BookStoreContext context)
        {
            _db = context;
            _db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            if (!context.Books.Any())
            {
                foreach (var b in DataSource.GetBooks())
                {
                    context.Books.Add(b);
                    context.Presses.Add(b.Press);
                }
                context.SaveChanges();
            }
        }

        [ApiVersionV2]
        [EnableQuery]
        public IQueryable<Book> Get()
        {
            return _db.Books;
        }

        [EnableQuery]
        public IActionResult Get(int key)
        {
            return Ok(_db.Books.FirstOrDefault(c => c.Id == key));
        }

        [ApiVersionV2]
        [EnableQuery]
        public IActionResult Post([FromBody] Book book)
        {
            _db.Books.Add(book);
            _db.SaveChanges();
            return Created(book);
        }
        
        [ApiVersionV2]
        [EnableQuery]
        public IActionResult Delete(int key)
        {
            var b = _db.Books.FirstOrDefault(c => c.Id == key);
            if (b == null)
            {
                return NotFound();
            }

            _db.Books.Remove(b);
            _db.SaveChanges();
            return Ok();
        }
    }
}