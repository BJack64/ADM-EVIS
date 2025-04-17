using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http.Filters;
using eFakturADM.WebApi.Models;
using eFakturADM.Shared.Utility;

namespace eFakturADM.WebApi.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomExceptionFilter : ExceptionFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            HttpStatusCode status = HttpStatusCode.InternalServerError;
            var exc = actionExecutedContext.Exception;
            var message = string.Empty;
            string outlogkey = "";
            Logger.WriteLog(out outlogkey, LogLevel.Error, exc.Message, MethodBase.GetCurrentMethod(), exc);
            var exceptionType = exc.GetType();
            if (exceptionType == typeof(UnauthorizedAccessException))
            {
                message = "Access to the Web API is not authorized. Log Key : " + outlogkey;
                status = HttpStatusCode.Unauthorized;
            }
            else
            {
                message = "Unhandle Exception. Log Key : " + outlogkey;
                status = HttpStatusCode.InternalServerError;
            }
            var contentresult = new RequestResultModel()
            {
                Status = false,
                Message = message
            };
            
            actionExecutedContext.Response = new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(contentresult), System.Text.Encoding.UTF8, "text/json"),
                StatusCode = status
            };
            base.OnException(actionExecutedContext);
        }
    }
}