using System;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using eFakturADM.Logic;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Objects;
using eFakturADM.Shared.Utility;
using eFakturADM.WebApi.Helper;

namespace eFakturADM.WebApi.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class CustomAuthorize : AuthorizeAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            string outlogkey;
            var authheader = actionContext.Request.Headers.Authorization;
            var msgtolog = "";

            Models.TokenResultStatus tokenstatresult = Models.TokenResultStatus.Invalid;

            if (authheader != null)
            {
                if (authheader.Scheme.ToLower() == "bearer" && !string.IsNullOrEmpty(authheader.Parameter))
                {
                    UserAuthentication userAuthentication = UserAuthentications.GetToken(authheader.Parameter);
                    if (userAuthentication != null)
                    {
                        tokenstatresult = Models.TokenResultStatus.OK;
                    }
                    else
                    {
                        var config = GeneralConfigs.GetConfigStaticToken(authheader.Parameter);
                        if (config != null)
                        {
                            tokenstatresult = Models.TokenResultStatus.OK;
                        }
                        else {
                            msgtolog = "UnAuthorize Access. Invalid Authorization Header.";
                        }
                    }
                }
                else
                {
                    msgtolog = "UnAuthorize Access. Invalid Authorization Header.";
                }
            }
            else
            {
                msgtolog = "UnAuthorize Access. Authorization Header NULL.";
            }

            Logger.WriteLog(out outlogkey, LogLevel.Debug, msgtolog, MethodBase.GetCurrentMethod());
         
            if(tokenstatresult != Models.TokenResultStatus.OK)
            {
                actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.Unauthorized, new { Status = false, ErrorCode = (int)tokenstatresult, Message = msgtolog });
            }

        }

    }
}