using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.IO.Compression;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Objects;
using eFakturADM.Web.Models;

namespace eFakturADM.Web.Controllers
{
    public class BaseController : Controller
    {
        public bool Authenticate(ref String Email, String Password)
        {
            return false;
        }

        public static string ToJson(object input)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(input);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string _userAgent = Request.UserAgent.ToString();
            LoginModel logindata = new LoginModel();
            string reqUrl = Request.Url.LocalPath;
            if (reqUrl == "/User/Login" || reqUrl == "/Home/Index" || reqUrl == "/")
            {

            }
            else
            {
                if (Session["Login"] != null)
                {
                    var dataLogin = (LoginResult)Session["Login"];
                    string token = dataLogin.Token;
                    UserAuthentication userAuthentication = UserAuthentications.GetToken(token);
                    if (userAuthentication == null)
                    {
                        Session["Login"] = null;
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                        {
                            controller = "User",
                            action = "Login"
                        }));
                    }
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "User",
                        action = "Login"
                    }));
                }
            }
        }
    }
    public static class Extensions
    {
        /// <summary>
        /// Takes a relative or absolute url and returns the fully-qualified url path.
        /// </summary>
        /// <param name="text">The url to make fully-qualified. Ex: Home/About</param>
        /// <returns>The absolute url plus protocol, server, & port. Ex: http://localhost:1234/Home/About</returns>
        public static string FullyQualified(this UrlHelper url, string text)
        {

            //### the VirtualPathUtility doesn"t handle querystrings, so we break the original url up
            var oldUrl = text;
            var oldUrlArray = (oldUrl.Contains("?") ? oldUrl.Split('?') : new[] { oldUrl, "" });

            //### we"ll use the Request.Url object to recreate the current server request"s base url
            //### requestUri.AbsoluteUri = "http://domain.com:1234/Home/Index?page=123"
            //### requestUri.LocalPath = "/Home/Index"
            //### requestUri.Query = "?page=123"
            //### subtract LocalPath and Query from AbsoluteUri and you get "http://domain.com:1234", which is urlBase
            var requestUri = GetRequestUri();
            var localPathAndQuery = requestUri.LocalPath + requestUri.Query;
            var urlBase = requestUri.AbsoluteUri.Substring(0, requestUri.AbsoluteUri.Length - localPathAndQuery.Length);

            //### convert the request url into an absolute path, then reappend the querystring, if one was specified
            var newUrl = VirtualPathUtility.ToAbsolute(oldUrlArray[0]);
            if (!string.IsNullOrEmpty(oldUrlArray[1]))
                newUrl += "?" + oldUrlArray[1];

            //### combine the old url base (protocol + server + port) with the new local path
            return urlBase + newUrl;
        }

        static Uri GetRequestUri()
        {
            return HttpContext.Current.Request.Url;
        }

        static Dictionary<string, string> codes = new Dictionary<string, string>(){ 
			{@"\*","<b>$2</b>"}, 
			{"_","<i>$2</i>"}, 
			{"-","<s>$2</s>"} 
		};
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuthActivityAttribute : AuthorizeAttribute
    {
        private readonly string[] allowedActivity;
        public AuthActivityAttribute(params string[] activity)
        {
            this.allowedActivity = activity;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = true;

            LoginResult _loginResult = new LoginResult();

            if (httpContext.Session["Login"] != null)
            {
                _loginResult = (LoginResult)httpContext.Session["Login"];
                foreach (var activity in allowedActivity)
                {
                    var roleActivities_count = RoleActivities.GetByActivityId_RoleId_Auth(activity.ToString(), _loginResult.RoleId[0]);
                    var check = roleActivities_count.Count != 0 ? true : false;
                    if (activity.Equals("View Home") || check) // if check is true
                    {
                        authorize = true;
                    }
                    else
                    {
                        authorize = false;
                    }
                }

            }
            else
            {
                new RedirectToRouteResult(new
                 RouteValueDictionary(new { controller = "User", action = "Login" }));
            }

            return authorize;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new
                RouteValueDictionary(new { controller = "Errors", action = "AccessDenied" }));
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuditEventExecutionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }
    }

    public class CacheFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Gets or sets the cache duration in seconds. The default is 10 seconds.
        /// </summary>
        /// <value>The cache duration in seconds.</value>
        public int Duration
        {
            get;
            set;
        }

        public CacheFilterAttribute()
        {
            Duration = 10;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Duration <= 0) return;

            HttpCachePolicyBase cache = filterContext.HttpContext.Response.Cache;
            TimeSpan cacheDuration = TimeSpan.FromSeconds(Duration);

            cache.SetCacheability(HttpCacheability.Public);
            cache.SetExpires(DateTime.Now.Add(cacheDuration));
            cache.SetMaxAge(cacheDuration);
            cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
        }
    }

    public class CompressFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpRequestBase request = filterContext.HttpContext.Request;

            string acceptEncoding = request.Headers["Accept-Encoding"];

            if (string.IsNullOrEmpty(acceptEncoding)) return;

            acceptEncoding = acceptEncoding.ToUpperInvariant();

            HttpResponseBase response = filterContext.HttpContext.Response;

            if (acceptEncoding.Contains("GZIP"))
            {
                response.AppendHeader("Content-encoding", "gzip");
                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
            }
            else if (acceptEncoding.Contains("DEFLATE"))
            {
                response.AppendHeader("Content-encoding", "deflate");
                response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
            }
        }
    }

    public class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var cache = filterContext.HttpContext.Response.Cache;
            cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            cache.SetValidUntilExpires(false);
            cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            cache.SetCacheability(HttpCacheability.NoCache);
            cache.SetNoStore();
            base.OnResultExecuting(filterContext);
        }
    }
}

