using Hangfire;
using System.Web.Http;

namespace eFakturADM.WebApi.Controllers
{
    public class HangfireController : ApiController
    {
        [HttpGet]
        [Route("hangfire")]
        public IHttpActionResult Dashboard()
        {
            return Ok("Hangfire Dashboard is accessible."); // Anda dapat mengarahkan ke dashboard Hangfire jika perlu
        }
    }
}
