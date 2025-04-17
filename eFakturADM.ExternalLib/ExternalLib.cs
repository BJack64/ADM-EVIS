using eFakturADM.ExternalLib.Objects;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;
using eFakturADM.Shared.Utility;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace eFakturADM.ExternalLib
{
    public class ExternalLib
    {
        [Obsolete("Ganti dengan RestClient")]
        public ExternalResponse Request(string url, string method, string payload, out string msgError, out WebExceptionStatus eStatus, out string logKey)
        {
            msgError = string.Empty;
            eStatus = WebExceptionStatus.Success;
            string msgToLog = null;
            Exception exToLog = null;
            logKey = string.Empty;
            ExternalResponse result = new ExternalResponse();
            try
            {
                var auth = new EcmLib(GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.EcmApiUrl)?.ConfigValue,
                    GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.EcmApiUsername)?.ConfigValue,
                    GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.EcmApiPassword)?.ConfigValue)
                    .AuthenticationV3(out string msgErrorAuth, out WebExceptionStatus eStatusAuth, out string logKeyAuth);

                if (eStatusAuth != WebExceptionStatus.Success)
                {
                    Logger.WriteLog(out logKey, LogLevel.Error, msgErrorAuth, MethodBase.GetCurrentMethod(), exToLog);
                    return result;
                }

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Headers.Add("otcsticket", auth.Ticket);
                request.Method = method;
                request.ContentType = "application/json";

                var data = Encoding.ASCII.GetBytes(payload);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                result.IsSuccess = response.StatusCode == HttpStatusCode.OK;
            }
            catch (WebException e)
            {
                eStatus = e.Status;
                msgError = EnumHelper.GetDescription(e.Status);
                msgToLog = "Error " + method + " from Url : " + url + Environment.NewLine + "Error Info : " +
                           e.Message;
                result.Error = "Error " + method + " from Url : " + url + Environment.NewLine + "Error Info : " +
                           e.Message;
                exToLog = e;
            }
            catch (Exception exception)
            {
                eStatus = WebExceptionStatus.UnknownError;
                msgError = EnumHelper.GetDescription(WebExceptionStatus.UnknownError);
                msgToLog = "Error " + method + " from Url : " + url + Environment.NewLine + "Error Info : " +
                           exception.Message;
                result.Error = "Error " + method + " from Url : " + url + Environment.NewLine + "Error Info : " +
                           exception.Message;
                exToLog = exception;
            }
            Logger.WriteLog(out logKey, LogLevel.Error, msgToLog, MethodBase.GetCurrentMethod(), exToLog);
            return result;
        }

        [Obsolete("Kena Security X509 di server ADM")]
        public ExternalResponse RequestV2(string url, string method, string payload, out string msgError, out WebExceptionStatus eStatus, out string logKey, bool IsUsingAuthentication = true)
        {
            msgError = string.Empty;
            eStatus = WebExceptionStatus.Success;
            string msgToLog = null;
            Exception exToLog = null;
            logKey = string.Empty;
            ExternalResponse result = new ExternalResponse();
            ResEcmAuth auth = new ResEcmAuth();
            try
            {
                if (IsUsingAuthentication)
                {
                    auth = new EcmLib(GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.EcmApiUrl)?.ConfigValue,
                        GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.EcmApiUsername)?.ConfigValue,
                        GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.EcmApiPassword)?.ConfigValue)
                        .AuthenticationV3(out string msgErrorAuth, out WebExceptionStatus eStatusAuth, out string logKeyAuth);

                    if (eStatusAuth != WebExceptionStatus.Success)
                    {
                        Logger.WriteLog(out logKey, LogLevel.Error, msgErrorAuth, MethodBase.GetCurrentMethod(), exToLog);
                        return result;
                    }
                }

                var client = new RestClient(url);
                client.Options.MaxTimeout = -1;
                var request = new RestRequest();
                if (IsUsingAuthentication)
                    request.AddHeader("otcsticket", auth.Ticket);

                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

                var payloadSplit = payload.Split(',');
                if (payloadSplit.Length > 0)
                {
                    for (int i = 0; i < payloadSplit.Length; i++)
                    {
                        var itemSplit = payloadSplit[i].Split('=');
                        for (int j = 0; j < itemSplit.Length; j++)
                        {
                            request.AddParameter(itemSplit[0], itemSplit[1]);
                        }
                    }

                    RestResponse response = client.Execute(request, method.ToLower() == "put" ? Method.Put : Method.Post);

                    result.IsSuccess = response.StatusCode == HttpStatusCode.OK;
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        msgError = response.ErrorMessage;
                    }

                }
                else
                {
                    msgError = "payloadSplit.Length = 0";
                }



            }
            catch (WebException e)
            {
                eStatus = e.Status;
                if (e.InnerException != null)
                    msgError = e.InnerException.Message + Environment.NewLine + e.StackTrace;
                else msgError = e.Message + Environment.NewLine + e.StackTrace;

                msgToLog = "Error " + method + " from Url : " + url + Environment.NewLine + "Error Info : " +
                           e.Message;
                result.Error = "Error " + method + " from Url : " + url + Environment.NewLine + "Error Info : " +
                           e.Message;
                exToLog = e;
            }
            catch (Exception exception)
            {
                eStatus = WebExceptionStatus.UnknownError;

                if (exception.InnerException != null)
                    msgError = exception.InnerException.Message + Environment.NewLine + exception.StackTrace;
                else msgError = exception.Message + Environment.NewLine + exception.StackTrace; ;

                msgToLog = "Error " + method + " from Url : " + url + Environment.NewLine + "Error Info : " +
                           exception.Message;
                result.Error = "Error " + method + " from Url : " + url + Environment.NewLine + "Error Info : " +
                           exception.Message;
                exToLog = exception;
            }
            Logger.WriteLog(out logKey, LogLevel.Error, msgToLog, MethodBase.GetCurrentMethod(), exToLog);
            return result;
        }


        public ExternalResponse RequestV3(ApplicationEnums.EnumLogApiAction Action , string url, string method, string payload, out string msgError, out WebExceptionStatus eStatus, out string logKey, bool IsUsingAuthentication = true)
        {
            msgError = string.Empty;
            eStatus = WebExceptionStatus.Success;
            string msgToLog = null;
            Exception exToLog = null;
            logKey = string.Empty;
            ExternalResponse result = new ExternalResponse();
            ResEcmAuth auth = new ResEcmAuth();
            try
            {
                if (IsUsingAuthentication)
                {
                    auth = new EcmLib(GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.EcmApiUrl)?.ConfigValue,
                        GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.EcmApiUsername)?.ConfigValue,
                        GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.EcmApiPassword)?.ConfigValue)
                        .AuthenticationV3(out string msgErrorAuth, out WebExceptionStatus eStatusAuth, out string logKeyAuth);

                    if (eStatusAuth != WebExceptionStatus.Success)
                    {
                        Logger.WriteLog(out logKey, LogLevel.Error, msgErrorAuth, MethodBase.GetCurrentMethod(), exToLog);
                        msgError = msgErrorAuth;
                        return result;
                    }

                }


                using (var client = new HttpClient())
                {
                    if (IsUsingAuthentication) client.DefaultRequestHeaders.Add("otcsticket", auth.Ticket);

                    client.BaseAddress = new Uri(url);

                    if (Action == ApplicationEnums.EnumLogApiAction.RepostingDate)
                    {
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                        var payloadSplit = payload.Split(',');
                        if (payloadSplit.Length > 0)
                        {

                            List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>();
                            for (int i = 0; i < payloadSplit.Length; i++)
                            {
                                var itemSplit = payloadSplit[i].Split('=');
                                for (int j = 0; j < itemSplit.Length; j++)
                                {
                                    data.Add(new KeyValuePair<string, string>(itemSplit[0], itemSplit[1]));
                                }
                            }

                            var formContent = new FormUrlEncodedContent(data);

                            Task<HttpResponseMessage> resp;
                            if (method.ToLower() == "post")
                            {
                                resp = client.PostAsync(url, formContent);
                            }
                            else //put
                                resp = client.PutAsync(url, formContent);

                            var response = resp.Result;
                            result.IsSuccess = response.StatusCode == HttpStatusCode.OK;
                            if (response.StatusCode != HttpStatusCode.OK)
                            {
                                msgError = response.Content.ReadAsStringAsync().Result;
                            }

                        }
                        else
                        {
                            msgError = "payloadSplit.Length = 0";
                        }
                    }
                    else
                    {
                        //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        if (!String.IsNullOrEmpty(payload))
                        {
                            var formContent = new StringContent(payload, Encoding.UTF8, "application/json");

                            Task<HttpResponseMessage> resp;
                            if (method.ToLower() == "post")
                            {
                                resp = client.PostAsync(url, formContent);
                            }
                            else //put
                                resp = client.PutAsync(url, formContent);

                            var response = resp.Result;
                            result.IsSuccess = response.StatusCode == HttpStatusCode.OK;
                            if (response.StatusCode != HttpStatusCode.OK)
                            {
                                msgError = response.Content.ReadAsStringAsync().Result;
                            }
                        }
                        else
                        {
                            msgError = "Payload is empty";
                        }

                    }
                }
            }
            catch (WebException e)
            {
                eStatus = e.Status;
                if (e.InnerException != null)
                    msgError = e.InnerException.Message + Environment.NewLine + e.StackTrace;
                else msgError = e.Message + Environment.NewLine + e.StackTrace;

                msgToLog = "Error " + method + " from Url : " + url + Environment.NewLine + "Error Info : " +
                           e.Message;
                result.Error = "Error " + method + " from Url : " + url + Environment.NewLine + "Error Info : " +
                           e.Message;
                exToLog = e;
            }
            catch (AggregateException ag)
            {
                eStatus = WebExceptionStatus.UnknownError;

                if (ag.InnerExceptions.Count > 0)
                {
                    if (ag.InnerExceptions[0].InnerException != null)
                        msgError = ag.InnerExceptions[0].InnerException.Message;
                    else msgError = ag.InnerExceptions[0].Message;
                }
                else msgError = ag.Message;
                
                msgToLog = "Error " + method + " from Url : " + url + Environment.NewLine + "Error Info : " +
                           ag.Message;
                result.Error = "Error " + method + " from Url : " + url + Environment.NewLine + "Error Info : " +
                           ag.Message;
                exToLog = ag;
            }
            catch (Exception exception)
            {
                eStatus = WebExceptionStatus.UnknownError;

                if (exception.InnerException != null)
                    msgError = exception.InnerException.Message + Environment.NewLine + exception.StackTrace;
                else msgError = exception.Message + Environment.NewLine + exception.StackTrace; ;

                msgToLog = "Error " + method + " from Url : " + url + Environment.NewLine + "Error Info : " +
                           exception.Message;
                result.Error = "Error " + method + " from Url : " + url + Environment.NewLine + "Error Info : " +
                           exception.Message;
                exToLog = exception;
            }

            Logger.WriteLog(out logKey, LogLevel.Error, msgToLog, MethodBase.GetCurrentMethod(), exToLog);
            return result;
        }

        [Obsolete("Ganti dengan RestClient")]
        public ExternalResponse RequestNoAuthentication(string url, string method, string payload, out string msgError, out WebExceptionStatus eStatus, out string logKey)
        {
            msgError = string.Empty;
            eStatus = WebExceptionStatus.Success;
            string msgToLog = null;
            Exception exToLog = null;
            logKey = string.Empty;
            ExternalResponse result = new ExternalResponse();
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = method;
                request.ContentType = "application/json";

                var data = Encoding.ASCII.GetBytes(payload);
                //request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                result.IsSuccess = response.StatusCode == HttpStatusCode.OK;
            }
            catch (WebException e)
            {
                eStatus = e.Status;
                msgError = EnumHelper.GetDescription(e.Status);
                msgToLog = "Error " + method + " from Url : " + url + Environment.NewLine + "Error Info : " +
                           e.Message;
                result.Error = "Error " + method + " from Url : " + url + Environment.NewLine + "Error Info : " +
                           e.Message;
                exToLog = e;
            }
            catch (Exception exception)
            {
                eStatus = WebExceptionStatus.UnknownError;
                msgError = EnumHelper.GetDescription(WebExceptionStatus.UnknownError);
                msgToLog = "Error " + method + " from Url : " + url + Environment.NewLine + "Error Info : " +
                           exception.Message;
                result.Error = "Error " + method + " from Url : " + url + Environment.NewLine + "Error Info : " +
                           exception.Message;
                exToLog = exception;
            }
            Logger.WriteLog(out logKey, LogLevel.Error, msgToLog, MethodBase.GetCurrentMethod(), exToLog);
            return result;
        }
    }
}
