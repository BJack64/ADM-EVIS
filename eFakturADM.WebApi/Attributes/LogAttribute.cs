using eFakturADM.Shared.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace eFakturADM.WebApi.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    public class LogAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            string outlogkey;
            var reqcontent = actionContext.ActionArguments;
            var reqcontentstring = JsonConvert.SerializeObject(reqcontent);
            var test = MethodBase.GetCurrentMethod();
            Logger.WriteLog(out outlogkey, LogLevel.Info, string.Format("Action Method {0} executing at {1}", actionContext.ActionDescriptor.ActionName, DateTime.Now.ToShortDateString()), MethodBase.GetCurrentMethod());
            Logger.WriteLog(out outlogkey, LogLevel.Info, string.Format("ActionArguments {0}", reqcontentstring), MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            string outlogkey;
            Logger.WriteLog(out outlogkey, LogLevel.Info, string.Format("Action Method {0} executed at {1}", actionExecutedContext.ActionContext.ActionDescriptor.ActionName, DateTime.Now.ToShortDateString()), MethodBase.GetCurrentMethod());
        }
    }
}