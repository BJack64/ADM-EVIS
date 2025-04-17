using eFakturADM.Logic.Utilities;
using log4net;
using System.Web;
using System.Web.Mvc;

namespace eFakturADM.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}