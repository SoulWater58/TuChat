using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace TuChatWebService2.Controllers
{
    public class ExampleController : ApiController
    {
        [AcceptVerbs("GET")]
        [Route("Ping/{id}")]
        public async Task<IHttpActionResult> Ping(int id)
        {
            return Ok(new { date = DateTime.Now, data = id });
        }
    }
}
