using System.Web.Mvc;

namespace eFakturADM.Web.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Home/
        //#if TOTAL_DEV

        public ActionResult Index()
        {
            return View();
        }
    }
}
