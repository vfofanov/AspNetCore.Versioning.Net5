using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Attributes;

namespace AspNetCore.Versioning
{
    [ODataIgnored]
    [ApiController]
    [Route("[controller]")]
    public abstract class ApiController : Controller
    {
    }
}