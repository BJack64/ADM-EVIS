using eFakturADM.Logic.Utilities;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace eFakturADM.Web.Helpers
{
    public static class UrlHelperExtensions
    {
        public static string EncryptedAction(this UrlHelper url, string action, string controller, object routeValues)
        {
            string queryString = string.Empty;

            if (routeValues != null)
            {
                RouteValueDictionary d = new RouteValueDictionary(routeValues);
                for (int i = 0; i < d.Keys.Count; i++)
                {
                    if (i > 0)
                    {
                        queryString += "?";
                    }
                    queryString += d.Keys.ElementAt(i) + "=" + d.Values.ElementAt(i);
                }
            }

            Uri requestUrl = url.RequestContext.HttpContext.Request.Url;
            string absoluteAction = string.Format("{0}{1}?q={2}",
               requestUrl.GetLeftPart(UriPartial.Authority),
            url.Action(action, controller, null), CryptographyHelperExtension.EncryptAndHash(queryString));

            return absoluteAction;
        }
    }
}