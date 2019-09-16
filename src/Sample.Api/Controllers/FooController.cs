using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Sample.Api.Controllers
{
    class output
    {
        public string[] values { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class FooController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(output), 200)]
        [SwaggerOperation(OperationId = "Foos_GetAll")]
        public ActionResult<IEnumerable<string>> GetAll()
        {
            var result = new output() { values = new string[] { "foo", "bar" } };
            return new JsonResult(result);
        }
    }
}
