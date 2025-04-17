using eFakturADM.Logic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace eFakturADM.Web.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class EncryptedParameterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            Dictionary<string, object> decryptedParameters = new Dictionary<string, object>();
            if (HttpContext.Current.Request.QueryString.Get("q") != null)
            {
                string encryptedQueryString = HttpContext.Current.Request.QueryString.Get("q");
                encryptedQueryString = encryptedQueryString.Replace(" ", "+");
                try
                {
                    string decrptedString = CryptographyHelperExtension.DecryptWithHash(encryptedQueryString.ToString());
                    string[] paramsArrs = decrptedString.Split('?');

                    for (int i = 0; i < paramsArrs.Length; i++)
                    {
                        string[] paramArr = paramsArrs[i].Split('=');
                        //decryptedParameters.Add(paramArr[0], Convert.ToInt32(paramArr[1]));
                        decryptedParameters.Add(paramArr[0], paramArr[1]);
                    }

                    for (int i = 0; i < decryptedParameters.Count; i++)
                    {
                        filterContext.ActionParameters[decryptedParameters.Keys.ElementAt(i)] = decryptedParameters.Values.ElementAt(i);
                    }
                }
                catch
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Errors", action = "Http404" }));
                }

            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new
                RouteValueDictionary(new { controller = "Errors", action = "Http404" }));
            }
            base.OnActionExecuting(filterContext);
        }

    }
}