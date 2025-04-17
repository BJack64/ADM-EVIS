using eFakturADM.ExternalLib.Objects;
using eFakturADM.Logic.Core;
using eFakturADM.Shared.Utility;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Web.Hosting;
using System.Web.Script.Serialization;

namespace eFakturADM.ExternalLib
{
    public class EcmLib
    {
        private string PathDownload(long objectID) => string.Format("/otcs/cs.exe/api/v2/nodes/{0}/content", objectID);
        private string PathAuthentication() => string.Format("/otcs/cs.exe/api/v1/auth");

        private string _username;
        private string _password;
        private string _baseUrl;
        public EcmLib(string baseUrl, string username, string password)
        {
            _username = username;
            _password = password;
            _baseUrl = baseUrl;
        }

        [Obsolete("Ganti dengan RestClient")]
        public ResEcmDownload GetEcmDownloadFakturPajak(long objectID, string folder, out string msgError, out WebExceptionStatus eStatus, out string logKey)
        {
            msgError = string.Empty;
            eStatus = WebExceptionStatus.Success;
            string msgToLog = null;
            Exception exToLog = null;
            logKey = string.Empty;
            ResEcmDownload result = new ResEcmDownload();
            try
            {
                var auth = AuthenticationV3(out string msgErrorAuth, out WebExceptionStatus eStatusAuth, out string logKeyAuth);
                if (eStatusAuth != WebExceptionStatus.Success)
                {
                    Logger.WriteLog(out logKey, LogLevel.Error, msgErrorAuth, MethodBase.GetCurrentMethod(), exToLog);
                    return result;
                }

                var request = WebRequest.Create(_baseUrl + PathDownload(objectID));
                request.Headers.Add("otcsticket", auth.Ticket);
                request.Method = "GET";
                request.ContentType = "application/json";

                using (var response = request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        if (responseStream == null)
                        {
                            msgError = "Empty response from url.";
                            result.Error = "Empty response from url.";
                            Logger.WriteLog(out logKey, LogLevel.Error, msgError, MethodBase.GetCurrentMethod());
                        }
                        else
                        {
                            string fileName = $"{objectID}.pdf";
                            string parentFolder = Path.Combine(Directory.GetParent(HostingEnvironment.MapPath("~/")).FullName, folder);
                            bool exists = System.IO.Directory.Exists(parentFolder);
                            if (!exists)
                                System.IO.Directory.CreateDirectory(parentFolder);

                            var filePath = Path.Combine(parentFolder, fileName);
                            using (var fileStream = File.Create(filePath))
                            {
                                responseStream.CopyTo(fileStream);
                            }
                            result.FilePath = filePath;
                            responseStream.Close();
                            responseStream.Dispose();
                        }

                    }

                    response.Close();
                }

            }
            catch (WebException e)
            {
                eStatus = e.Status;
                msgError = EnumHelper.GetDescription(e.Status);
                msgToLog = "Error Get Document from ECM for Url : " + _baseUrl + Environment.NewLine + "Error Info : " +
                           e.Message + Environment.NewLine;
                result.Error = "Error Get Document from ECM for Url : " + _baseUrl + Environment.NewLine + "Error Info : " +
                           e.Message;
                exToLog = e;
            }
            catch (Exception exception)
            {
                eStatus = WebExceptionStatus.UnknownError;
                msgError = EnumHelper.GetDescription(WebExceptionStatus.UnknownError);
                msgToLog = "Error Get Document from ECM for Url : " + _baseUrl + Environment.NewLine + "Error Info : " +
                           exception.StackTrace;
                result.Error = "Error Get Document from ECM for Url : " + _baseUrl + Environment.NewLine + "Error Info : " +
                           exception.StackTrace;
                exToLog = exception;
            }
            Logger.WriteLog(out logKey, LogLevel.Error, msgToLog, MethodBase.GetCurrentMethod(), exToLog);
            return result;
        }

        [Obsolete("Kena Security X509 di server ADM")]
        public ResEcmDownload GetEcmDownloadFakturPajakV2(long objectID, string folder, out string msgError, out WebExceptionStatus eStatus, out string logKey)
        {
            msgError = string.Empty;
            eStatus = WebExceptionStatus.Success;
            string msgToLog = null;
            Exception exToLog = null;
            logKey = string.Empty;
            ResEcmDownload result = new ResEcmDownload();
            try
            {
                var auth = AuthenticationV3(out string msgErrorAuth, out WebExceptionStatus eStatusAuth, out string logKeyAuth);
                if (eStatusAuth != WebExceptionStatus.Success)
                {
                    Logger.WriteLog(out logKey, LogLevel.Error, msgErrorAuth, MethodBase.GetCurrentMethod(), exToLog);
                    return result;
                }


                var client = new RestClient(_baseUrl + PathDownload(objectID));
                client.Options.MaxTimeout = -1;
                var request = new RestRequest();
                request.AddHeader("otcsticket", auth.Ticket);

                RestResponse response = client.Execute(request, Method.Get);

                var stream = client.DownloadStream(request);


                using (Stream responseStream = stream)
                {
                    if (responseStream == null)
                    {
                        msgError = "Empty response from url.";
                        result.Error = "Empty response from url.";
                        Logger.WriteLog(out logKey, LogLevel.Error, msgError, MethodBase.GetCurrentMethod());
                    }
                    else
                    {
                        string fileName = $"{objectID}.pdf";
                        string parentFolder = Path.Combine(Directory.GetParent(HostingEnvironment.MapPath("~/")).FullName, folder);
                        bool exists = System.IO.Directory.Exists(parentFolder);
                        if (!exists)
                            System.IO.Directory.CreateDirectory(parentFolder);

                        var filePath = Path.Combine(parentFolder, fileName);
                        using (var fileStream = File.Create(filePath))
                        {
                            responseStream.CopyTo(fileStream);
                        }
                        result.FilePath = filePath;
                        responseStream.Close();
                        responseStream.Dispose();
                    }

                }


            }
            catch (WebException e)
            {
                eStatus = e.Status;
                msgError = EnumHelper.GetDescription(e.Status);
                msgToLog = "Error Get Document from ECM for Url : " + _baseUrl + Environment.NewLine + "Error Info : " +
                           e.Message + Environment.NewLine;
                result.Error = "Error Get Document from ECM for Url : " + _baseUrl + Environment.NewLine + "Error Info : " +
                           e.Message;
                exToLog = e;
            }
            catch (Exception exception)
            {
                eStatus = WebExceptionStatus.UnknownError;
                msgError = EnumHelper.GetDescription(WebExceptionStatus.UnknownError);
                msgToLog = "Error Get Document from ECM for Url : " + _baseUrl + Environment.NewLine + "Error Info : " +
                           exception.StackTrace;
                result.Error = "Error Get Document from ECM for Url : " + _baseUrl + Environment.NewLine + "Error Info : " +
                           exception.StackTrace;
                exToLog = exception;
            }
            Logger.WriteLog(out logKey, LogLevel.Error, msgToLog, MethodBase.GetCurrentMethod(), exToLog);
            return result;
        }

        public ResEcmDownload GetEcmDownloadFakturPajakV3(long objectID, string folder, out string msgError, out WebExceptionStatus eStatus, out string logKey)
        {
            msgError = string.Empty;
            eStatus = WebExceptionStatus.Success;
            string msgToLog = null;
            Exception exToLog = null;
            logKey = string.Empty;
            ResEcmDownload result = new ResEcmDownload();
            try
            {
                var auth = AuthenticationV3(out string msgErrorAuth, out WebExceptionStatus eStatusAuth, out string logKeyAuth);
                if (eStatusAuth != WebExceptionStatus.Success)
                {
                    Logger.WriteLog(out logKey, LogLevel.Error, msgErrorAuth, MethodBase.GetCurrentMethod(), exToLog);
                    return result;
                }

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("otcsticket", auth.Ticket);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(_baseUrl + PathDownload(objectID));
                    var resp = client.GetAsync(_baseUrl + PathDownload(objectID));
                    var response = resp.Result;
                    var stream = response.Content.ReadAsStreamAsync();
                    using (Stream responseStream = stream.Result)
                    {
                        if (responseStream == null)
                        {
                            msgError = "Empty response from url.";
                            result.Error = "Empty response from url.";
                            Logger.WriteLog(out logKey, LogLevel.Error, msgError, MethodBase.GetCurrentMethod());
                        }
                        else
                        {
                            string fileName = $"{objectID}.pdf";
                            string parentFolder = Path.Combine(Directory.GetParent(HostingEnvironment.MapPath("~/")).FullName, folder);
                            bool exists = System.IO.Directory.Exists(parentFolder);
                            if (!exists)
                                System.IO.Directory.CreateDirectory(parentFolder);

                            var filePath = Path.Combine(parentFolder, fileName);
                            using (var fileStream = File.Create(filePath))
                            {
                                responseStream.CopyTo(fileStream);
                            }
                            result.FilePath = filePath;
                            responseStream.Close();
                            responseStream.Dispose();
                        }

                    }
                }

            }
            catch (WebException e)
            {
                eStatus = e.Status;
                msgError = EnumHelper.GetDescription(e.Status);
                msgToLog = "Error Get Document from ECM for Url : " + _baseUrl + Environment.NewLine + "Error Info : " +
                           e.Message + Environment.NewLine;
                result.Error = "Error Get Document from ECM for Url : " + _baseUrl + Environment.NewLine + "Error Info : " +
                           e.Message;
                exToLog = e;
            }
            catch (Exception exception)
            {
                eStatus = WebExceptionStatus.UnknownError;
                msgError = EnumHelper.GetDescription(WebExceptionStatus.UnknownError);
                msgToLog = "Error Get Document from ECM for Url : " + _baseUrl + Environment.NewLine + "Error Info : " +
                           exception.StackTrace;
                result.Error = "Error Get Document from ECM for Url : " + _baseUrl + Environment.NewLine + "Error Info : " +
                           exception.StackTrace;
                exToLog = exception;
            }
            Logger.WriteLog(out logKey, LogLevel.Error, msgToLog, MethodBase.GetCurrentMethod(), exToLog);
            return result;
        }

        [Obsolete("Ganti dengan RestClient")]
        public ResEcmAuth Authentication(out string msgError, out WebExceptionStatus eStatus, out string logKey)
        {
            msgError = string.Empty;
            eStatus = WebExceptionStatus.Success;
            string msgToLog = null;
            Exception exToLog = null;
            logKey = string.Empty;
            ResEcmAuth result = new ResEcmAuth();
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(_baseUrl + PathAuthentication());
                request.Method = "POST";
                request.ContentType = "application/json";
                var postData = "Username=" + Uri.EscapeDataString(_username);
                postData += "&Password=" + Uri.EscapeDataString(_password);
                var data = Encoding.ASCII.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                JavaScriptSerializer objJS = new JavaScriptSerializer();
                result = objJS.Deserialize<ResEcmAuth>(responseString);

            }
            catch (WebException e)
            {
                eStatus = e.Status;
                msgError = EnumHelper.GetDescription(e.Status);
                msgToLog = "Error Auth from ECM for Url : " + _baseUrl + Environment.NewLine + "Error Info : " +
                           e.Message;
                result.Error = "Error Auth from ECM for Url : " + _baseUrl + Environment.NewLine + "Error Info : " +
                           e.Message;
                exToLog = e;
            }
            catch (Exception exception)
            {
                eStatus = WebExceptionStatus.UnknownError;
                msgError = EnumHelper.GetDescription(WebExceptionStatus.UnknownError);
                msgToLog = "Error Auth from ECM for Url : " + _baseUrl + Environment.NewLine + "Error Info : " +
                           exception.StackTrace;
                result.Error = "Error Auth from ECM for Url : " + _baseUrl + Environment.NewLine + "Error Info : " +
                           exception.StackTrace;
                exToLog = exception;
            }
            Logger.WriteLog(out logKey, LogLevel.Error, msgToLog, MethodBase.GetCurrentMethod(), exToLog);
            return result;
        }

        [Obsolete("Kena Security X509 di server ADM")]
        public ResEcmAuth AuthenticationV2(out string msgError, out WebExceptionStatus eStatus, out string logKey)
        {
            msgError = string.Empty;
            eStatus = WebExceptionStatus.Success;
            string msgToLog = null;
            Exception exToLog = null;
            logKey = string.Empty;
            ResEcmAuth result = new ResEcmAuth();
            try
            {

                var client = new RestClient(_baseUrl + PathAuthentication());
                client.Options.MaxTimeout = -1;
                var request = new RestRequest();
                request.AlwaysMultipartFormData = true;
                request.AddParameter("Username", _username);
                request.AddParameter("Password", _password);
                RestResponse response = client.Execute(request, Method.Post);

                result = JsonConvert.DeserializeObject<ResEcmAuth>(response.Content);

            }
            catch (WebException e)
            {
                eStatus = e.Status;
                msgError = EnumHelper.GetDescription(e.Status);
                msgToLog = "Error Auth from ECM for Url : " + _baseUrl + Environment.NewLine + "Error Info : " +
                           e.Message;
                result.Error = "Error Auth from ECM for Url : " + _baseUrl + Environment.NewLine + "Error Info : " +
                           e.Message;
                exToLog = e;
            }
            catch (Exception exception)
            {
                eStatus = WebExceptionStatus.UnknownError;
                msgError = EnumHelper.GetDescription(WebExceptionStatus.UnknownError);
                msgToLog = "Error Auth from ECM for Url : " + _baseUrl + Environment.NewLine + "Error Info : " +
                           exception.StackTrace;
                result.Error = "Error Auth from ECM for Url : " + _baseUrl + Environment.NewLine + "Error Info : " +
                           exception.StackTrace;
                exToLog = exception;
            }
            Logger.WriteLog(out logKey, LogLevel.Error, msgToLog, MethodBase.GetCurrentMethod(), exToLog);
            return result;
        }

        public ResEcmAuth AuthenticationV3(out string msgError, out WebExceptionStatus eStatus, out string logKey)
        {
            msgError = string.Empty;
            eStatus = WebExceptionStatus.Success;
            string msgToLog = null;
            Exception exToLog = null;
            logKey = string.Empty;
            ResEcmAuth result = new ResEcmAuth();
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(_baseUrl + PathAuthentication());
                    var formContent = new MultipartFormDataContent()
                    {
                        // Send form text values here
                        {new StringContent(_username),"Username"},
                        { new StringContent(_password),"Password"}
                    };
                    var resp = client.PostAsync(_baseUrl + PathAuthentication(), formContent);
                    var response = resp.Result;
                    if (resp.Result.StatusCode == HttpStatusCode.OK)
                    {
                        var s = response.Content.ReadAsStringAsync().Result;
                        result = JsonConvert.DeserializeObject<ResEcmAuth>(s);
                    }
                    else
                    {
                        eStatus = WebExceptionStatus.SendFailure;
                    }
                }
            }
            catch (WebException e)
            {
                eStatus = e.Status;
                msgError = EnumHelper.GetDescription(e.Status);
                msgToLog = "Error Auth from ECM for Url : " + _baseUrl + Environment.NewLine + "Error Info : " +
                           e.Message;
                result.Error = "Error Auth from ECM for Url : " + _baseUrl + Environment.NewLine + "Error Info : " +
                           e.Message;
                exToLog = e;
            }
            catch (Exception exception)
            {
                eStatus = WebExceptionStatus.UnknownError;
                msgError = EnumHelper.GetDescription(WebExceptionStatus.UnknownError);
                msgToLog = "Error Auth from ECM for Url : " + _baseUrl + Environment.NewLine + "Error Info : " +
                           exception.StackTrace;
                result.Error = "Error Auth from ECM for Url : " + _baseUrl + Environment.NewLine + "Error Info : " +
                           exception.StackTrace;
                exToLog = exception;
            }
            Logger.WriteLog(out logKey, LogLevel.Error, msgToLog, MethodBase.GetCurrentMethod(), exToLog);
            return result;
        }
    }
}
