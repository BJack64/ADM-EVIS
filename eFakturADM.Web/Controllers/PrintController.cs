using System.Web.Mvc;

namespace eFakturADM.Web.Controllers
{
    public class PrintController : BaseController
    {
        public ActionResult PrintOrdnerList(string idprint)
        {
            Session["idprint"] = idprint;
            return View("PrintOrdnerList");
        }

    }
}
