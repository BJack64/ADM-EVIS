using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml;
using System.Xml.Serialization;
using eFakturADM.DJPLib.Models;
using eFakturADM.DJPLib.Objects;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Logic.Utilities;
using eFakturADM.Shared.Utility;
using Newtonsoft.Json;
using RestSharp;

namespace eFakturADM.DJPLib
{
    public class ValidateFakturLib
    {
        private static readonly int maxConcurrentThreads = int.TryParse(
        GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.MaxConcurrentThreads).ConfigValue,
        out var result) ? result : 5;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(Environment.ProcessorCount * maxConcurrentThreads); // Batas maksimum 5 tugas berjalan bersamaan
        //private static readonly SemaphoreSlim _semaphoreSoftDeleteFPDetail = new SemaphoreSlim(Environment.ProcessorCount * maxConcurrentThreads); // Batas maksimum 5 tugas berjalan bersamaan


        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="timeOutSetting">Get from General Config setting(in millisecond)</param>
        /// <param name="msgError"></param>
        /// <param name="eStatus"></param>
        /// <param name="logKey"></param>
        /// <param name="proxyServer"></param>
        /// <param name="proxyPort"></param>
        /// <param name="useDefaultCredential"></param>
        /// <param name="isUseProxy"></param>
        /// <returns></returns>
        [Obsolete("Pake cara yg ini suka gagal get data djp nya")]
        public static ResValidateFakturPm GetValidateFakturObject(string url, int timeOutSetting, bool isUseProxy, string proxyServer, int? proxyPort, bool? useDefaultCredential, out string msgError, out WebExceptionStatus eStatus, out string logKey)
        {
            msgError = string.Empty;
            eStatus = WebExceptionStatus.Success;
            string msgToLog;
            Exception exToLog = null;
            logKey = string.Empty;
            ResValidateFakturPm xmlData = null;
            try
            {
                //Log.JustLog("Start Get ValidateFaktur from DJP for url : " + url, MethodBase.GetCurrentMethod());
                var request = WebRequest.Create(url);

                request.Timeout = timeOutSetting;
                request.Method = "GET";
                request.ContentType = "application/xml";

                if (isUseProxy)
                {
                    if (proxyPort.HasValue)
                    {
                        request.Proxy = new WebProxy(proxyServer, proxyPort.Value)
                        {
                            Credentials = CredentialCache.DefaultCredentials
                        };
                        string outLogKey;
                        Logger.WriteLog(out outLogKey, LogLevel.Info, "Use Proxy : " + proxyServer + ":" + proxyPort.Value, MethodBase.GetCurrentMethod());
                    }
                    else
                    {
                        request.Proxy = new WebProxy(proxyServer)
                        {
                            Credentials = CredentialCache.DefaultCredentials
                        };
                        string outLogKey;
                        Logger.WriteLog(out outLogKey, LogLevel.Info, "Use Proxy : " + proxyServer, MethodBase.GetCurrentMethod());
                    }

                    if (useDefaultCredential.HasValue)
                    {
                        request.UseDefaultCredentials = useDefaultCredential.Value;
                    }

                }

                using (var response = request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        if (responseStream == null)
                        {
                            msgError = "Empty response from url.";
                            //Log.WriteLog(msgError, MethodBase.GetCurrentMethod());
                            Logger.WriteLog(out logKey, LogLevel.Error, msgError, MethodBase.GetCurrentMethod());
                        }
                        else
                        {
                            var doc = new XmlDocument();
                            doc.Load(responseStream);
                            var strXml = doc.InnerXml;
                            var rdr = new StringReader(strXml);
                            var serializer = new XmlSerializer(typeof(ResValidateFakturPm));
                            xmlData = (ResValidateFakturPm)serializer.Deserialize(rdr);

                            responseStream.Close();
                            responseStream.Dispose();

                        }

                    }

                    response.Close();
                }

                if (xmlData == null)
                {
                    msgError = "Empty response from url.";
                    msgToLog = "Empty response from url : " + url;
                }
                else
                {
                    return xmlData;
                }

            }
            catch (WebException e)
            {
                eStatus = e.Status;
                msgError = EnumHelper.GetDescription(e.Status);
                msgToLog = "Error Get ValidateFaktur from DJP for Url : " + url + Environment.NewLine + "Error Info : " +
                           e.Message;
                exToLog = e;
            }
            catch (Exception exception)
            {
                eStatus = WebExceptionStatus.UnknownError;
                msgError = EnumHelper.GetDescription(WebExceptionStatus.UnknownError);
                msgToLog = "Error Get ValidateFaktur from DJP for Url : " + url + Environment.NewLine + "Error Info : " +
                           exception.StackTrace;
                exToLog = exception;
            }
            Logger.WriteLog(out logKey, LogLevel.Error, msgToLog, MethodBase.GetCurrentMethod(), exToLog);
            return xmlData;
        }


        //[Obsolete("Kena Security X509 di server ADM")]
        //public static ResValidateFakturPm GetValidateFakturObjectV2(string url, int timeOutSetting, bool isUseProxy, string proxyServer, int? proxyPort, bool? useDefaultCredential, out string msgError, out WebExceptionStatus eStatus, out string logKey)
        //{
        //    msgError = string.Empty;
        //    eStatus = WebExceptionStatus.Success;
        //    string msgToLog;
        //    Exception exToLog = null;
        //    logKey = string.Empty;
        //    ResValidateFakturPm jsonData = null;
        //    try
        //    {
        //        var client = new RestClient(url);

        //        client.Options.MaxTimeout = timeOutSetting;

        //        if (isUseProxy)
        //        {
        //            if (proxyPort.HasValue)
        //            {
        //                client.Options.Proxy = new WebProxy(proxyServer, proxyPort.Value)
        //                {
        //                    Credentials = CredentialCache.DefaultCredentials
        //                };
        //                string outLogKey;
        //                Logger.WriteLog(out outLogKey, LogLevel.Info, "Use Proxy : " + proxyServer + ":" + proxyPort.Value, MethodBase.GetCurrentMethod());
        //            }
        //            else
        //            {
        //                client.Options.Proxy = new WebProxy(proxyServer)
        //                {
        //                    Credentials = CredentialCache.DefaultCredentials
        //                };
        //                string outLogKey;
        //                Logger.WriteLog(out outLogKey, LogLevel.Info, "Use Proxy : " + proxyServer, MethodBase.GetCurrentMethod());
        //            }

        //            if (useDefaultCredential.HasValue)
        //            {
        //                client.Options.UseDefaultCredentials = useDefaultCredential.Value;
        //            }

        //        }



        //        var request = new RestRequest();
        //        var response = client.Execute(request, Method.Get);
        //        if (response.StatusCode == HttpStatusCode.OK)
        //        {
        //            jsonData = JsonConvert.DeserializeObject<ResValidateFakturPm>(response.Content);
        //        }
        //        else
        //        {
        //            eStatus = WebExceptionStatus.SendFailure;

        //        }

        //        if (jsonData == null)
        //        {
        //            msgError = "Empty response from url.";
        //            msgToLog = "Empty response from url : " + url;
        //        }
        //        else
        //        {
        //            return jsonData;
        //        }

        //    }
        //    catch (WebException e)
        //    {
        //        eStatus = e.Status;
        //        msgError = EnumHelper.GetDescription(e.Status);
        //        msgToLog = "Error Get ValidateFaktur from DJP for Url : " + url + Environment.NewLine + "Error Info : " +
        //                   e.Message;
        //        exToLog = e;
        //    }
        //    catch (Exception exception)
        //    {
        //        eStatus = WebExceptionStatus.UnknownError;
        //        msgError = EnumHelper.GetDescription(WebExceptionStatus.UnknownError);
        //        msgToLog = "Error Get ValidateFaktur from DJP for Url : " + url + Environment.NewLine + "Error Info : " +
        //                   exception.StackTrace;
        //        exToLog = exception;
        //    }
        //    Logger.WriteLog(out logKey, LogLevel.Error, msgToLog, MethodBase.GetCurrentMethod(), exToLog);
        //    return jsonData;
        //}

        public static ResValidateFakturPm GetValidateFakturObjectV3_2(string url, string tmp_file, out string logKey)
        {
            string msgError = string.Empty;
            string eStatus = "Success _:";
            string msgToLog;
            Exception exToLog = null;
            logKey = string.Empty;
            ResValidateFakturPm jsonData = null;

            WebClient client = new WebClient();
            if (File.Exists(tmp_file))
            {
                File.Delete(tmp_file);
            }

            try
            {
                client.DownloadFile(url, tmp_file);

                ResValidateFakturPm obj;
                using (StreamReader reader = new StreamReader(tmp_file))
                {
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(ResValidateFakturPm));
                    obj = (ResValidateFakturPm)serializer.Deserialize(reader);
                }
                jsonData = obj;

                if (jsonData == null)
                {
                    msgError = "Empty response from url.";
                    msgToLog = "Empty response from url : " + url;
                }
                else
                {
                    return jsonData;
                }

            }
            catch (Exception e)
            {
                eStatus = "Gagal _:";
                msgToLog = "Error Get ValidateFaktur from DJP for Url : " + url;
                exToLog = e;
            }
            Logger.WriteLog(out logKey, LogLevel.Error, msgToLog, MethodBase.GetCurrentMethod(), exToLog);
            return jsonData;
        }


        public static ResValidateFakturPm GetValidateFakturObjectV3(string url, int timeOutSetting, bool isUseProxy, string proxyServer, int? proxyPort, bool? useDefaultCredential, out string msgError, out WebExceptionStatus eStatus, out string logKey)
        {
            msgError = string.Empty;
            eStatus = WebExceptionStatus.Success;
            string msgToLog;
            Exception exToLog = null;
            logKey = string.Empty;
            ResValidateFakturPm jsonData = null;

            int maxRetriesInt = int.TryParse(
            GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.MaxRetries).ConfigValue,
            out var result) ? result : 5;

            int maxRetries = maxRetriesInt; // Maksimum jumlah percobaan jika null dari GeneralConfig maka defaultnya 3 kali percobaan
            int delayBetweenRetries = 2000; // Delay dalam milidetik sebelum mencoba lagi

            try
            {
                for (int attempt = 0; attempt < maxRetries; attempt++)
                {
                    HttpResponseMessage resp;
                    bool isResponseValid = false;

                    try
                    {
                        if (isUseProxy)
                        {
                            var port = proxyPort == null ? 80 : proxyPort;

                            var proxy = new WebProxy
                            {
                                Address = new Uri($"http://{proxyServer}:{port}"),
                                BypassProxyOnLocal = false,
                                UseDefaultCredentials = true
                            };

                            var httpClientHandler = new HttpClientHandler
                            {
                                Proxy = proxy,
                            };

                            httpClientHandler.UseProxy = true;

                            using (var client = new HttpClient(handler: httpClientHandler))
                            {
                                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                                client.BaseAddress = new Uri(url);
                                client.Timeout = TimeSpan.FromSeconds(timeOutSetting);

                                resp = client.GetAsync(url).Result; // Menggunakan .Result untuk panggilan sinkron
                            }
                        }
                        else
                        {
                            using (var client = new HttpClient())
                            {
                                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                                client.BaseAddress = new Uri(url);
                                client.Timeout = TimeSpan.FromSeconds(timeOutSetting);

                                resp = client.GetAsync(url).Result; // Menggunakan .Result untuk panggilan sinkron
                            }
                        }

                        // Cek status respons
                        if (resp.StatusCode == HttpStatusCode.OK)
                        {
                            var s = resp.Content.ReadAsStringAsync().Result;

                            // Deteksi apakah formatnya JSON atau XML
                            if (resp.Content.Headers.ContentType.MediaType == "application/json")
                            {
                                // Jika JSON, deserialisasi ke objek JSON
                                jsonData = JsonConvert.DeserializeObject<ResValidateFakturPm>(s);
                            }
                            else if (resp.Content.Headers.ContentType.MediaType == "application/xml")
                            {
                                // Jika XML, deserialisasi ke objek XML
                                var serializer = new XmlSerializer(typeof(ResValidateFakturPm));
                                using (var reader = new StringReader(s))
                                {
                                    jsonData = (ResValidateFakturPm)serializer.Deserialize(reader);
                                }
                            }
                            isResponseValid = true; // Menandakan respons berhasil
                        }
                        else
                        {
                            // Jika status bukan 200 OK, set status dan log error
                            eStatus = WebExceptionStatus.SendFailure;
                            msgError = $"Unexpected status code: {resp.StatusCode}.";
                            msgToLog = msgError + $" Response from URL: {url}.";
                        }
                    }
                    catch (WebException e)
                    {
                        eStatus = e.Status;
                        msgError = e.Response?.GetResponseStream().ToString() ?? "No response stream.";
                        msgToLog = "Error Get ValidateFaktur from DJP for Url : " + url + Environment.NewLine + "Error Info : " + e.Message;
                        exToLog = e;
                    }
                    catch (Exception exception)
                    {
                        eStatus = WebExceptionStatus.UnknownError;
                        msgError = exception.InnerException?.Message ?? exception.Message;
                        msgToLog = "Error Get ValidateFaktur from DJP for Url : " + url + Environment.NewLine + "Error Info : " + exception.StackTrace;
                        exToLog = exception;
                    }

                    // Jika respons tidak valid, coba lagi
                    if (!isResponseValid && attempt < maxRetries - 1)
                    {
                        // Delay sebelum mencoba ulang
                        Thread.Sleep(delayBetweenRetries); // Menggunakan Thread.Sleep untuk delay
                    }
                    else
                    {
                        break; // Keluar dari loop jika respons berhasil
                    }
                }

                if (jsonData == null)
                {
                    msgError = "Empty response from url.";
                    msgToLog = "Empty response from url : " + url;
                }
                else
                {
                    return jsonData;
                }
            }
            catch (AggregateException ag)
            {
                eStatus = WebExceptionStatus.UnknownError;

                if (ag.InnerExceptions.Count > 0)
                {
                    msgError = ag.InnerExceptions[0].InnerException?.Message ?? ag.InnerExceptions[0].Message;
                }

                msgToLog = "Error Get ValidateFaktur from DJP for Url : " + url + Environment.NewLine + "Error Info : " + ag.StackTrace;
                exToLog = ag;
            }
            catch (Exception exception)
            {
                eStatus = WebExceptionStatus.UnknownError;

                msgError = exception.InnerException?.Message ?? exception.Message;
                msgToLog = "Error Get ValidateFaktur from DJP for Url : " + url + Environment.NewLine + "Error Info : " + exception.StackTrace;
                exToLog = exception;
            }

            Logger.WriteLog(out logKey, LogLevel.Error, msgToLog, MethodBase.GetCurrentMethod(), exToLog);
            return jsonData;
        }

        public static FakturPajakResultModel ValidateFakturObjectAPI(string URL, string Source, string UserName, string FPdjpID, int timeOutSetting, bool isUseProxy, string inetProxy, int? inetProxyPort, bool? inetProxyUseCredential, out string msgError, out WebExceptionStatus eStatus, out string logKey, bool IsHitFromApi = true, bool UpdateExistingData = false, long LogPostingID = 0)
        {
            msgError = "";
            logKey = "";
            eStatus = WebExceptionStatus.Success;

            FakturPajakResultModel result = new FakturPajakResultModel();
            result.StatusValidasi = "false";
            ////result.NamaValidasi = "";
            result.KeteranganValidasi = "";
            result.DataFakturPajak = new List<DataFakturPajak>();
            result.FPdjpID = FPdjpID;

            URL = URL.Trim();
            //check if exists url on database
            var chkByUrl = FakturPajaks.GetByUrlScan(URL);

            if (!string.IsNullOrEmpty(inetProxy))
            {
                isUseProxy = true;
            }

            //isUseProxy = false;


            LogPostingTanggalLaporan logposting = new LogPostingTanggalLaporan();

            if (!string.IsNullOrEmpty(URL))
            {
                try
                {
                    var getTimeOutSetting = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.DJPRequestTimeOutSetting);

                    if (getTimeOutSetting == null)
                    {
                        msgError = "Config Data not found for [DJPRequestTimeOutSetting]";
                        Logger.WriteLog(out logKey, LogLevel.Error, msgError, MethodBase.GetCurrentMethod());
                        result.StatusValidasi = "true";
                        ////result.NamaValidasi = "";
                        result.KeteranganValidasi = msgError;
                    }

                    int timeOutSettingInt;

                    if (!int.TryParse(getTimeOutSetting.ConfigValue, out timeOutSettingInt))
                    {
                        msgError = "Invalid value Config Data [DJPRequestTimeOutSetting]";
                        Logger.WriteLog(out logKey, LogLevel.Error, msgError, MethodBase.GetCurrentMethod());
                        result.StatusValidasi = "true";
                        ////result.NamaValidasi = "";
                        result.KeteranganValidasi = msgError;
                    }
                    string logkey2;
                    var MsgErrorGetValidate = "";
                    var objXml = DJPLib.ValidateFakturLib.GetValidateFakturObjectV3(URL, timeOutSettingInt, isUseProxy, inetProxy, inetProxyPort, inetProxyUseCredential, out MsgErrorGetValidate, out eStatus, out logkey2);
                    //CR DZ moved from line 4 (start from method) to here
                    if (chkByUrl != null && chkByUrl.MasaPajak.HasValue && chkByUrl.TahunPajak.HasValue && eStatus == WebExceptionStatus.Success && !string.IsNullOrEmpty(objXml.NomorFaktur))
                    {
                        if (chkByUrl.Status == (int)ApplicationEnums.StatusFakturPajak.Success &&
                            chkByUrl.StatusFaktur != EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti))
                        {

                            if (!(chkByUrl.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturPajakNormal) &&
                                objXml.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti)
                                ))
                            {

                                msgError = "Sudah discan pada " +
                                ConvertHelper.DateTimeConverter.ToLongDateString(chkByUrl.Created) + " oleh " +
                                chkByUrl.CreatedBy +
                                ". No FP " + chkByUrl.FormatedNoFaktur + " sudah ada di Masa Pajak " + chkByUrl.MasaPajakName +
                                " " + chkByUrl.TahunPajak
                                + ", Nomor Filling Index " + chkByUrl.FillingIndex;

                                result.StatusValidasi = "true";
                                //result.NamaValidasi = "";
                                result.KeteranganValidasi = msgError;
                            }

                        }

                        if (chkByUrl.Status == (int)ApplicationEnums.StatusFakturPajak.ErrorRequest || chkByUrl.Status == (int)ApplicationEnums.StatusFakturPajak.ErrorValidation)// jika status 3/4 baru masuk sini
                        {
                            msgError = "Sudah discan pada "
                                + ConvertHelper.DateTimeConverter.ToLongDateString(chkByUrl.Created) + " oleh " +
                                chkByUrl.CreatedBy +
                                ". Di Masa Pajak " +
                                chkByUrl.MasaPajakName + " " + chkByUrl.TahunPajak
                                + ", Nomor Filling Index " + chkByUrl.FillingIndex + ". Tapi belum request ke DJP atau ada error validasi.";
                            result.StatusValidasi = "true";
                            //result.NamaValidasi = "";
                            result.KeteranganValidasi = msgError;

                        }
                    }
                  

                    if (eStatus == WebExceptionStatus.Success)
                    {
                        if (objXml != null && !string.IsNullOrEmpty(objXml.NomorFaktur))
                        {

                            var head = new Models.DataFakturPajak();
                            head.AlamatLawanTransaksi = objXml.AlamatLawanTransaksi;
                            head.AlamatPenjual = objXml.AlamatPenjual;
                            head.FgPengganti = objXml.FgPengganti;
                            head.JumlahDpp = objXml.JumlahDpp;
                            head.JumlahPpn = objXml.JumlahPpn;
                            head.JumlahPpnBm = objXml.JumlahPpnBm;
                            head.KdJenisTransaksi = objXml.KdJenisTransaksi;
                            head.NamaLawanTransaksi = objXml.NamaLawanTransaksi;
                            head.NamaPenjual = objXml.NamaPenjual;

                            head.NomorFakturPajak = objXml.NomorFaktur;
                            var formatedFp = FormatingDomains.GetFormatNoFaktur(ApplicationEnums.FPType.ScanNonIws, head.KdJenisTransaksi, head.FgPengganti, head.NomorFakturPajak);
                            if (formatedFp.IsValid)
                                head.NomorFakturPajakFormatted = formatedFp.FormattedField;
                            else head.NomorFakturPajakFormatted = objXml.NomorFaktur;

                            var lstnpwp = new List<string>();
                            lstnpwp.Add(objXml.NpwpLawanTransaksi);
                            var npwpformat = FormatingDomains.GetFormatNpwp(lstnpwp);
                            head.NpwpLawanTransaksi = objXml.NpwpLawanTransaksi;
                            head.NpwpLawanTransaksiFormatted = npwpformat.FirstOrDefault().FormattedField;

                            lstnpwp.Clear();
                            lstnpwp.Add(objXml.NpwpPenjual);
                            npwpformat = FormatingDomains.GetFormatNpwp(lstnpwp);
                            head.NpwpPenjual = objXml.NpwpPenjual;
                            head.NpwpPenjualFormatted = npwpformat.FirstOrDefault().FormattedField;

                            head.Referensi = objXml.Referensi;
                            head.StatusApproval = objXml.StatusApproval;
                            head.StatusFaktur = objXml.StatusFaktur;
                            head.TglFaktur = objXml.TanggalFaktur;

                            result.DataFakturPajak.Add(head);
                            result.DataFakturPajak[0].DetailTransaksi = new System.Collections.Generic.List<Models.DetailTransaksi>();
                            for (int i = 0; i < objXml.DetailTransaksi.Count; i++)
                            {
                                var detail = new Models.DetailTransaksi();
                                detail.Diskon = objXml.DetailTransaksi[i].Diskon;
                                detail.Dpp = objXml.DetailTransaksi[i].Dpp;
                                detail.HargaSatuan = objXml.DetailTransaksi[i].HargaSatuan;
                                detail.HargaTotal = objXml.DetailTransaksi[i].HargaTotal;
                                detail.JumlahBarang = objXml.DetailTransaksi[i].JumlahBarang;
                                detail.Nama = objXml.DetailTransaksi[i].Nama;
                                detail.Ppn = objXml.DetailTransaksi[i].Ppn;
                                detail.Ppnbm = objXml.DetailTransaksi[i].Ppnbm;
                                detail.TarifPpnbm = objXml.DetailTransaksi[i].TarifPpnbm;

                                result.DataFakturPajak[0].DetailTransaksi.Add(detail);
                            }

                            ResValidateFakturPm FakturPM = new ResValidateFakturPm();
                            FakturPajakBatchRequestSetting datarequest = new FakturPajakBatchRequestSetting();
                            datarequest.UrlScan = URL;

                            Exception ex = new Exception();
                            SaveDetailTransaksiToDatabase(objXml, URL, out ex);
                            //Task.Run(() => SaveToDbAsync(objXml, chkByUrl));

                            if (ex != null)
                                msgError = ex.Message;

                            #region validation
                            var msgValidation = new List<string>();

                            //var dt = DateTime.ParseExact(head.TglFaktur, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            //var strMasaPajak = dt.Month.ToString();
                            //var strTahunPajak = dt.Year.ToString();

                            var periode = OpenClosePeriods.GetOpenReguler().OrderBy(a => a.OpenClosePeriodId).FirstOrDefault();
                            var strMasaPajak = periode.MasaPajak.ToString();
                            var strTahunPajak = periode.TahunPajak.ToString();



                            if (!(string.IsNullOrEmpty(strMasaPajak) || strMasaPajak == "0") &&
                                !(string.IsNullOrEmpty(strTahunPajak) || strTahunPajak == "0"))
                            {
                                var getOpenClosePeriod = OpenClosePeriods.GetByMasaPajak(int.Parse(strMasaPajak),
                                    int.Parse(strTahunPajak));

                                if (getOpenClosePeriod != null)
                                {
                                    if (!getOpenClosePeriod.StatusRegular)
                                    {
                                        msgValidation.Add("Status Masa Pajak Close Reguler");
                                    }
                                    else
                                    {
                                        if (!getOpenClosePeriod.StatusSp2)
                                        {
                                            msgValidation.Add("Status Masa Pajak Close SP2");
                                        }
                                    }
                                }
                                else
                                {
                                    msgValidation.Add("Masa dan Tahun Pajak tidak tersedia di Data Open Close Periode");
                                }
                            }

                            int tahunPajak = 0;
                            int masaPajak = 0;
                            if (string.IsNullOrEmpty(strMasaPajak) || strMasaPajak == "0")
                            {
                                msgValidation.Add("Masa Pajak Mandatory");
                            }
                            else
                            {
                                if (!int.TryParse(strMasaPajak.Trim(), out masaPajak))
                                {
                                    msgValidation.Add("Invalid Tahun Pajak");
                                }
                            }
                            if (string.IsNullOrEmpty(strTahunPajak) || strTahunPajak == "0")
                            {
                                msgValidation.Add("Tahun Pajak Mandatory");
                            }
                            else
                            {
                                if (!int.TryParse(strTahunPajak.Trim(), out tahunPajak))
                                {
                                    msgValidation.Add("Invalid Tahun Pajak");
                                }
                            }

                            if (URL == null || URL.Trim().Length == 0)
                            {
                                msgValidation.Add("Scan Url Mandatory");
                            }
                            if (head.NomorFakturPajak == null || head.NomorFakturPajak.Trim().Length == 0)
                            {
                                msgValidation.Add("No Faktur Pajak Mandatory");
                            }

                            if (string.IsNullOrEmpty(head.TglFaktur))
                            {
                                msgValidation.Add("Tanggal Faktur Pajak Mandatory");
                            }
                            else
                            {
                                //getconfig
                                var configData = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.PelaporanTglFaktur);
                                if (configData == null)
                                {
                                    msgValidation.Add("GeneralConfig [PelaporanTglFaktur] not found.");
                                }
                                else
                                {
                                    var dats = configData.ConfigValue.Split(':').ToList();
                                    if (dats.Count != 2)
                                    {
                                        msgValidation.Add("GeneralConfig [PelaporanTglFaktur] not valid.");
                                    }
                                    else
                                    {
                                        if (masaPajak != 0 && tahunPajak != 0)
                                        {
                                            if (!string.IsNullOrEmpty(head.FgPengganti) && head.FgPengganti == "0")
                                            {
                                                //BPM No. ASMO3-201847620
                                                int min = int.Parse(dats[0].Replace("[", "").Replace("]", "")); // -3
                                                int max = int.Parse(dats[1].Replace("[", "").Replace("]", "")); // 0
                                                var dtMin = new DateTime(tahunPajak, masaPajak, 1).AddMonths(min); // ex: oktober -> 1 juli


                                                var maxperiode = OpenClosePeriods.GetOpenReguler().OrderByDescending(a => a.OpenClosePeriodId).FirstOrDefault();
                                                var maxMasaPajak = maxperiode.MasaPajak;
                                                var maxTahunPajak = maxperiode.TahunPajak;
                                                var dtMax = new DateTime(maxTahunPajak, maxMasaPajak, 1).AddMonths(max + 1).AddDays(-1); // ex: oktober -> tgl terakhir bulan oktober


                                                var dt = DateTime.ParseExact(head.TglFaktur, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                                                var dTglFaktur = Convert.ToDateTime(string.Concat(dt.Year, "-", dt.Month, "-", dt.Day));
                                                var dtMaxValidity = new DateTime(dtMax.Year, dtMax.Month, DateTime.DaysInMonth(dtMax.Year, dtMax.Month)); // ex: 30 oktober
                                                if (dTglFaktur < dtMin) // juli < april
                                                {
                                                    msgValidation.Add("Tanggal Faktur Pajak sudah kadaluarsa");
                                                }
                                                else
                                                {
                                                    if (dTglFaktur > dtMaxValidity)
                                                    {
                                                        msgValidation.Add("Tanggal Faktur Pajak tidak sesuai ketentuan");
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }
                            }

                            //Check NPWP Adm
                            if (string.IsNullOrEmpty(head.NpwpLawanTransaksi))
                            {
                                msgValidation.Add("NPWP Pembeli Mandatory");
                            }
                            else
                            {
                                var chkConfig = GeneralConfigs.GetConfigCheckNpwpAdm(head.NpwpLawanTransaksi);
                                if (chkConfig == null)
                                {
                                    var npwpAdm = GeneralConfigs.GetByKeyId(ApplicationEnums.GeneralConfig.NpwpAdm);
                                    msgValidation.Add("NPWP Pembeli bukan " + npwpAdm.ConfigValue);
                                }
                            }

                            //Check NPWP Adm
                            if (string.IsNullOrEmpty(head.NamaLawanTransaksi))
                            {
                                msgValidation.Add("Nama Pembeli Mandatory");
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(head.FgPengganti))
                                {
                                    if (head.FgPengganti != "2")
                                    {
                                        var d = GeneralConfigs.GetByKeyId(ApplicationEnums.GeneralConfig.NamaNpwpAdm);

                                        var dList = d.ConfigValue.Split(';').Where(c => !string.IsNullOrEmpty(c) && !string.IsNullOrEmpty(c.Trim())).ToList();
                                        var availableNamaPembeli = (string.Join(",", dList));

                                        var chk = dList.Where(dItem => head.NamaLawanTransaksi.Replace(" ", "").ToLower().Contains(dItem.Replace(" ", "").ToLower())).ToList();

                                        if (chk.Count <= 0)
                                        {
                                            msgValidation.Add("Nama Pembeli bukan yang diperbolehkan [" + availableNamaPembeli + "]");
                                        }
                                    }
                                }
                            }

                            if (string.IsNullOrEmpty(head.NpwpPenjual))
                            {
                                msgValidation.Add("NPWP Penjual Mandatory");
                            }
                            else
                            {
                                var v = Vendors.GetByNpwp(head.NpwpPenjual);
                                if (v != null && v.PkpDicabut)
                                {
                                    msgValidation.Add("PKP Dicabut atas NPWP Penjual");
                                }
                            }

                            var we = string.Join(Environment.NewLine, msgValidation);

                            if (string.IsNullOrEmpty(head.KdJenisTransaksi))
                            {
                                msgValidation.Add("KdJenisTransaksi Mandatory");
                            }

                            if (string.IsNullOrEmpty(head.FgPengganti))
                            {
                                msgValidation.Add("FgPengganti Mandatory");
                            }

                            if (string.IsNullOrEmpty(head.StatusFaktur))
                            {
                                msgValidation.Add("Status Faktur Mandatory");
                            }

                            var yyy = string.Join(Environment.NewLine, msgValidation);

                            if (msgValidation.Count <= 0)
                            {
                                /*
                                 * Disable berdasarkan CR Perubahan dari sisi e-Faktur V2.0
                                 * Terkait scan FP Pengganti (Kode yang digit ke -3 ‘1’ contoh: 011,021,031,041) maka tidak dibutuhkan scan FP awal (Kode yang digit ke -3 ‘0’ contoh: 010,020,030,040)"
                                 */
                                /*
                                 * Rollback berdasarkan CR terbaru, tetap ada validasi harus scan faktur pajak normal (Kode yang digit ke-3 nya '0'
                                 * BPM No. ASMO3-201847620 
                                 */
                                var ttt = string.Join(Environment.NewLine, msgValidation);
                                if (head.FgPengganti != "0")
                                {
                                    /* BPM No. ASMO3-201847620
                                     * (c) Saat user scan FP normal-pengganti, maka akan muncul request untuk scan FP diganti terlebih
                                     * dahulu agar dapat mengetahui kapan FP diganti tersebut kadaluarsa (jika FP diganti
                                     * expired/kadaluarsa maka ditolak oleh EVIS). - jika 010 belum di scan di EVIS 
                                     */
                                    var msgValidatePengganti = FakturPajaks.ValidateScanPengganti(head.NomorFakturPajak, head.FgPengganti,
                                        ApplicationEnums.FPType.External);
                                    if (!string.IsNullOrEmpty(msgValidatePengganti))
                                    {
                                        msgValidation.Add(msgValidatePengganti);
                                    }
                                    else
                                    {
                                        /* BPM No. ASMO3-201847620
                                         * (a) Jika FP diganti (kode awal 010) ada dan belum expired, maka FP normal-pengganti (kode awal 011) dapat diterima/discan. 
                                         * - langsung masuk (dan masih open), FP 010 & 011 harus discan dimasa yang sama. 
                                        */
                                        var fpNormal = FakturPajaks.GetFakturPajakNormal(head.NomorFakturPajak);
                                        if (fpNormal != null && fpNormal.FakturPajakId != 0)
                                        {
                                            var configData = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.PelaporanTglFaktur);
                                            if (configData == null)
                                            {
                                                msgValidation.Add("GeneralConfig [PelaporanTglFaktur] not found.");
                                            }
                                            else
                                            {
                                                //check apakah Faktur Normal sudah Expired
                                                var dats = configData.ConfigValue.Split(':').ToList();
                                                if (dats.Count != 2)
                                                {
                                                    msgValidation.Add("GeneralConfig [PelaporanTglFaktur] not valid.");
                                                }
                                                else
                                                {
                                                    /* BPM No. ASMO3-201847620
                                                     * (b) Jika FP diganti (kode awal 010) sudah expired, maka FP normal-pengganti (kode awal 011) akan
                                                     * ditolak oleh EVIS. 
                                                     * Tgl Faktur dibandingkan dengan MasaPajak-TahunPajak
                                                     */
                                                    if (tahunPajak != 0 && masaPajak != 0)
                                                    {
                                                        //noted tgl 2019-02-06
                                                        //apakah bisa dikatakan bahwa ketika scan fp normal pengganti tidak perlu cek expired tgl faktur terhadap masa pajak fp normal pengganti itu ?
                                                        //mas indra : Yup, bisa dikatakan begitu. Jadi FP normal pengganti selalu refer ke FP normal
                                                        //Jadi gini jika yg di scan fp normal pengganti:
                                                        //1. Dia akan cek apakah ada FP normalnya?
                                                        //1.a jika tidak maka notif perlu scan FP normalnya
                                                        //1.b jika ada maka cek masa pajak n tgl faktur
                                                        //2. Cek apakah masa pajak sama dg yg lg di scan?
                                                        //2.a jika tidak maka notif masa pajak harus sama
                                                        //2.b jika sama maka cek tgl faktur..
                                                        //berdasarkan pernyataannya maka point 2b tidak perlu dicek "Iya mas, jadi gak valid lg utk dicek. 2b jadi gak peru."

                                                        int min = int.Parse(dats[0].Replace("[", "").Replace("]", ""));
                                                        var dtMin = new DateTime(tahunPajak, masaPajak, 1).AddMonths(min);
                                                        if (fpNormal.TglFaktur < dtMin)
                                                        {
                                                            msgValidation.Add("Faktur Pajak Diganti sudah kadarluasa");
                                                        }
                                                        //else
                                                        //{
                                                        //    if (!(tahunPajak == fpNormal.TahunPajak && masaPajak == fpNormal.MasaPajak))
                                                        //    {
                                                        //        msgValidation.Add("Faktur Pajak Pengganti harus di Masa-Tahun Pajak yang sama dengan Faktur Pajak Normal");
                                                        //    }
                                                        //}

                                                        //if (!(tahunPajak == fpNormal.TahunPajak && masaPajak == fpNormal.MasaPajak))
                                                        //{
                                                        //    msgValidation.Add("Faktur Pajak Pengganti harus di Masa-Tahun Pajak yang sama dengan Faktur Pajak Normal");
                                                        //}

                                                        //CR DZ - Validasi Scan Pengganti (11) Jika FP Digantinya masih berstatus FP-Nornal



                                                        DateTime monthName = new DateTime(2020, fpNormal.MasaPajak.Value, 1);
                                                        if (fpNormal.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturPajakNormal))
                                                        {
                                                            msgValidation.Add("Faktur Pajak " + fpNormal.FormatedNoFaktur + " sudah dilaporkan di masa pajak " + monthName.ToString("MMMM") + " " + fpNormal.TahunPajak.ToString());
                                                            // msgValidation.Add("No FP Diganti [" + fpNormal.FormatedNoFaktur + "] belum discan");

                                                        }

                                                    }

                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    var fpNormal = FakturPajaks.GetFakturPajakDuplikasi(head.NomorFakturPajak, head.KdJenisTransaksi, head.FgPengganti);
                                    if (fpNormal.StatusFaktur != null)
                                    {
                                        msgValidation.Add(String.Format("Nomor faktur pajak {0} sudah ada pada aplikasi EVIS", head.NomorFakturPajak));
                                    }


                                    var fpNormalTerlapor = FakturPajaks.GetFakturPajakNormalTerlapor(head.NomorFakturPajak);
                                    if (fpNormalTerlapor.NamaPelaporan != null && fpNormalTerlapor.NamaPelaporan != "")
                                    {

                                        // -- mmd 05 04 2023
                                        //DateTime monthName = new DateTime(2020, fpNormalTerlapor.MasaPajak.Value, 1);
                                        DateTime monthName = new DateTime(fpNormalTerlapor.TahunPajak.Value, fpNormalTerlapor.MasaPajak.Value, 1);
                                        //--

                                        if (fpNormalTerlapor.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturPajakNormal))
                                        {
                                            msgValidation.Add("Faktur Pajak " + fpNormalTerlapor.FormatedNoFaktur + " sudah dilaporkan di masa pajak " + monthName.ToString("MMMM") + " " + fpNormalTerlapor.TahunPajak.ToString());

                                        }
                                    }
                                }
                                var www = string.Join(Environment.NewLine, msgValidation);
                                /*
                                 * Penambahan validasi berdasarkan CR Perubahan dari sisi e-Faktur V2.0
                                 * Ada validasi tambahan ketika semua Faktur Pajak (apapun kode FP nya) yang di scan adalah “Faktur Diganti” maka akan muncul notifikasi “Faktur Pajak sudah diganti”
                                 */
                                if (head.StatusFaktur.ToLower().Trim() == "faktur diganti")
                                {
                                    msgValidation.Add("Faktur Pajak sudah diganti");
                                }

                                /*
                                 * CR Penambahan validasi untuk faktur pajak yang sudah dibatalkan
                                 * BPM No. ASMO3-201847620 
                                 */
                                if (head.StatusFaktur.ToLower().Trim() == "faktur dibatalkan")
                                {
                                    msgValidation.Add("Faktur Pajak Dibatalkan");
                                }

                                /* BPM No. ASMO3-201847620
                                 * (d) Jika FP dengan kode 011 statusnya adalah FP diganti maka akan ditolak oleh EVIS - jika ada 2
                                 * FP pengganti
                                 */
                                if (head.StatusFaktur.ToLower().Trim() == "faktur diganti" && head.FgPengganti != "0")
                                {
                                    msgValidation.Add("Faktur Pajak Normal - Pengganti sudah diganti");
                                }
                                var sss = string.Join(Environment.NewLine, msgValidation);
                                //CR DZ -- Validasi Scan Diganti (10), jika FP Normal sudah dilaporkan, lalu dari DJP mengeluarkan FP Digantinya.
                                if (head.FgPengganti == "0" && head.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti))
                                {
                                    FakturPajak fpNormal = new FakturPajak();

                                    fpNormal = FakturPajaks.GetFakturPajakNormal(head.NomorFakturPajak);
                                    if (fpNormal != null)
                                    {
                                        if (fpNormal.FakturPajakId != 0)
                                        {
                                            if (fpNormal.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturPajakNormal) && fpNormal.MasaPajak.ToString() != masaPajak.ToString() && fpNormal.MasaPajak != null)
                                            {
                                                var getOpenClosePeriod = OpenClosePeriods.GetByMasaPajak(fpNormal.MasaPajak.Value, fpNormal.TahunPajak.Value);
                                                if (getOpenClosePeriod != null)
                                                {
                                                    if (!getOpenClosePeriod.StatusRegular || !getOpenClosePeriod.StatusSp2)
                                                    {
                                                        //                                    msgValidation.Add("Faktur Pajak-Nomal sudah dilaporkan");
                                                        //  DateTime monthName = new DateTime(2020, fpNormal.MasaPajak.Value, 1);
                                                        //  msgValidation.Add("Faktur Pajak " + fpNormal.FormatedNoFaktur + " sudah dilaporkan di masa pajak " + monthName.ToString("MMMM") + " " + fpNormal.TahunPajak.ToString());

                                                        msgValidation.Add("Sudah discan pada " +
                                                    ConvertHelper.DateTimeConverter.ToLongDateString(fpNormal.Created) + " oleh " +
                                                    fpNormal.CreatedBy +
                                                    ". No FP " + fpNormal.FormatedNoFaktur + " sudah ada di Masa Pajak " + fpNormal.MasaPajakName +
                                                    " " + fpNormal.TahunPajak
                                                    + ", Nomor Filling Index " + fpNormal.FillingIndex);

                                                    }

                                                }
                                            }
                                        }
                                    }
                                }

                            }
                            result.StatusValidasi = msgValidation.Count > 0 ? "true" : "false";
                            //result.NamaValidasi = "";5
                            result.KeteranganValidasi = string.Join(Environment.NewLine, msgValidation);

                            #endregion valdation


                            if (!IsHitFromApi)
                            {
                                logposting.CreatedBy = UserName;
                                logposting.Created = DateTime.Now;
                                logposting.Source = Source;
                                logposting.Payload = JsonConvert.SerializeObject(result);
                                logposting.FPdjpID = FPdjpID;
                                logposting.Action = EnumHelper.GetDescription(ApplicationEnums.EnumLogApiAction.SendFaktur);
                                LogPostingTanggalLaporans.Save(logposting);
                            }

                        }
                        else
                        {
                            result.StatusValidasi = "true";
                            //result.NamaValidasi = "";
                            //result.KeteranganValidasi = MsgErrorGetValidate;
                            result.KeteranganValidasi = "Error mapping object XML";
                            //logposting.Created = DateTime.Now;
                            //logposting.CreatedBy = UserName;
                            //logposting.Source = Source;
                            //logposting.Payload = "";
                            //logposting.FPdjpID = FPdjpID;
                            //logposting.Url = URL;
                            //logposting.Action = EnumHelper.GetDescription(ApplicationEnums.EnumLogApiAction.DJPError);
                            //LogPostingTanggalLaporans.Save(logposting);
                            msgError = result.KeteranganValidasi;
                        }

                    }
                    else
                    {
                        result.StatusValidasi = "true";
                        //result.NamaValidasi = "";
                        //result.KeteranganValidasi = MsgErrorGetValidate;
                        result.KeteranganValidasi = "Failed Get Data From DJP";
                        logposting.Created = DateTime.Now;
                        logposting.CreatedBy = UserName;
                        logposting.Source = Source;
                        logposting.Payload = "";
                        logposting.FPdjpID = FPdjpID;
                        logposting.Url = URL;
                        logposting.Message = MsgErrorGetValidate;
                        logposting.Action = EnumHelper.GetDescription(ApplicationEnums.EnumLogApiAction.DJPError);
                        logposting.Id = LogPostingID;
                        LogPostingTanggalLaporans.Save(logposting);

                        eStatus = WebExceptionStatus.SendFailure;

                    }
                }
                catch (Exception e)
                {
                    result.StatusValidasi = "true";
                    //result.NamaValidasi = "";
                    var ExceptionMsg = "";
                    if (e.InnerException == null)
                    {
                        ExceptionMsg = string.Concat(e.Message, Environment.NewLine, e.StackTrace);
                    }
                    else
                    {
                        ExceptionMsg = string.Concat(e.InnerException.Message, Environment.NewLine, e.StackTrace);
                    }
                    msgError = ExceptionMsg;
                    result.KeteranganValidasi = ExceptionMsg;
                    eStatus = WebExceptionStatus.SendFailure;
                }
            }
            else
            {

                result.StatusValidasi = "true";
                result.KeteranganValidasi = "URL Mandatory";

                eStatus = WebExceptionStatus.SendFailure;
            }
            return result;
        }

        public static FakturPajakResultModel ValidateFakturObjectAPIJob(string URL, string Source, string UserName, string FPdjpID, int timeOutSetting, bool isUseProxy, string inetProxy, int? inetProxyPort, bool? inetProxyUseCredential, out string msgError, out WebExceptionStatus eStatus, out string logKey, bool IsHitFromApi = true, bool UpdateExistingData = false, long LogPostingID = 0)
        {
            msgError = "";
            logKey = "";
            eStatus = WebExceptionStatus.Success;

            FakturPajakResultModel result = new FakturPajakResultModel();
            result.StatusValidasi = "false";
            ////result.NamaValidasi = "";
            result.KeteranganValidasi = "";
            result.DataFakturPajak = new List<DataFakturPajak>();
            result.FPdjpID = FPdjpID;

            URL = URL.Trim();
            //check if exists url on database
            var chkByUrl = FakturPajaks.GetByUrlScan(URL);

            if (!string.IsNullOrEmpty(inetProxy))
            {
                isUseProxy = true;
            }

            //isUseProxy = false;


            LogPostingTanggalLaporan logposting = new LogPostingTanggalLaporan();

            if (!string.IsNullOrEmpty(URL))
            {
                try
                {
                    var getTimeOutSetting = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.DJPRequestTimeOutSetting);

                    if (getTimeOutSetting == null)
                    {
                        msgError = "Config Data not found for [DJPRequestTimeOutSetting]";
                        Logger.WriteLog(out logKey, LogLevel.Error, msgError, MethodBase.GetCurrentMethod());
                        result.StatusValidasi = "true";
                        ////result.NamaValidasi = "";
                        result.KeteranganValidasi = msgError;
                    }

                    int timeOutSettingInt;

                    if (!int.TryParse(getTimeOutSetting.ConfigValue, out timeOutSettingInt))
                    {
                        msgError = "Invalid value Config Data [DJPRequestTimeOutSetting]";
                        Logger.WriteLog(out logKey, LogLevel.Error, msgError, MethodBase.GetCurrentMethod());
                        result.StatusValidasi = "true";
                        ////result.NamaValidasi = "";
                        result.KeteranganValidasi = msgError;
                    }
                    string logkey2;
                    var MsgErrorGetValidate = "";
                    var objXml = DJPLib.ValidateFakturLib.GetValidateFakturObjectV3(URL, timeOutSettingInt, isUseProxy, inetProxy, inetProxyPort, inetProxyUseCredential, out MsgErrorGetValidate, out eStatus, out logkey2);
                    //CR DZ moved from line 4 (start from method) to here
                    if (chkByUrl != null && chkByUrl.MasaPajak.HasValue && chkByUrl.TahunPajak.HasValue && eStatus == WebExceptionStatus.Success && !string.IsNullOrEmpty(objXml.NomorFaktur))
                    {
                        if (chkByUrl.Status == (int)ApplicationEnums.StatusFakturPajak.Success &&
                            chkByUrl.StatusFaktur != EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti))
                        {

                            if (!(chkByUrl.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturPajakNormal) &&
                                objXml.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti)
                                ))
                            {

                                msgError = "Sudah discan pada " +
                                ConvertHelper.DateTimeConverter.ToLongDateString(chkByUrl.Created) + " oleh " +
                                chkByUrl.CreatedBy +
                                ". No FP " + chkByUrl.FormatedNoFaktur + " sudah ada di Masa Pajak " + chkByUrl.MasaPajakName +
                                " " + chkByUrl.TahunPajak
                                + ", Nomor Filling Index " + chkByUrl.FillingIndex;

                                result.StatusValidasi = "true";
                                //result.NamaValidasi = "";
                                result.KeteranganValidasi = msgError;
                            }

                        }

                        if (chkByUrl.Status == (int)ApplicationEnums.StatusFakturPajak.ErrorRequest || chkByUrl.Status == (int)ApplicationEnums.StatusFakturPajak.ErrorValidation)// jika status 3/4 baru masuk sini
                        {
                            msgError = "Sudah discan pada "
                                + ConvertHelper.DateTimeConverter.ToLongDateString(chkByUrl.Created) + " oleh " +
                                chkByUrl.CreatedBy +
                                ". Di Masa Pajak " +
                                chkByUrl.MasaPajakName + " " + chkByUrl.TahunPajak
                                + ", Nomor Filling Index " + chkByUrl.FillingIndex + ". Tapi belum request ke DJP atau ada error validasi.";
                            result.StatusValidasi = "true";
                            //result.NamaValidasi = "";
                            result.KeteranganValidasi = msgError;

                        }
                    }


                    if (eStatus == WebExceptionStatus.Success)
                    {
                        if (objXml != null && !string.IsNullOrEmpty(objXml.NomorFaktur))
                        {

                            var head = new Models.DataFakturPajak();
                            head.AlamatLawanTransaksi = objXml.AlamatLawanTransaksi;
                            head.AlamatPenjual = objXml.AlamatPenjual;
                            head.FgPengganti = objXml.FgPengganti;
                            head.JumlahDpp = objXml.JumlahDpp;
                            head.JumlahPpn = objXml.JumlahPpn;
                            head.JumlahPpnBm = objXml.JumlahPpnBm;
                            head.KdJenisTransaksi = objXml.KdJenisTransaksi;
                            head.NamaLawanTransaksi = objXml.NamaLawanTransaksi;
                            head.NamaPenjual = objXml.NamaPenjual;

                            head.NomorFakturPajak = objXml.NomorFaktur;
                            var formatedFp = FormatingDomains.GetFormatNoFaktur(ApplicationEnums.FPType.ScanNonIws, head.KdJenisTransaksi, head.FgPengganti, head.NomorFakturPajak);
                            if (formatedFp.IsValid)
                                head.NomorFakturPajakFormatted = formatedFp.FormattedField;
                            else head.NomorFakturPajakFormatted = objXml.NomorFaktur;

                            var lstnpwp = new List<string>();
                            lstnpwp.Add(objXml.NpwpLawanTransaksi);
                            var npwpformat = FormatingDomains.GetFormatNpwp(lstnpwp);
                            head.NpwpLawanTransaksi = objXml.NpwpLawanTransaksi;
                            head.NpwpLawanTransaksiFormatted = npwpformat.FirstOrDefault().FormattedField;

                            lstnpwp.Clear();
                            lstnpwp.Add(objXml.NpwpPenjual);
                            npwpformat = FormatingDomains.GetFormatNpwp(lstnpwp);
                            head.NpwpPenjual = objXml.NpwpPenjual;
                            head.NpwpPenjualFormatted = npwpformat.FirstOrDefault().FormattedField;

                            head.Referensi = objXml.Referensi;
                            head.StatusApproval = objXml.StatusApproval;
                            head.StatusFaktur = objXml.StatusFaktur;
                            head.TglFaktur = objXml.TanggalFaktur;

                            result.DataFakturPajak.Add(head);
                            result.DataFakturPajak[0].DetailTransaksi = new System.Collections.Generic.List<Models.DetailTransaksi>();
                            for (int i = 0; i < objXml.DetailTransaksi.Count; i++)
                            {
                                var detail = new Models.DetailTransaksi();
                                detail.Diskon = objXml.DetailTransaksi[i].Diskon;
                                detail.Dpp = objXml.DetailTransaksi[i].Dpp;
                                detail.HargaSatuan = objXml.DetailTransaksi[i].HargaSatuan;
                                detail.HargaTotal = objXml.DetailTransaksi[i].HargaTotal;
                                detail.JumlahBarang = objXml.DetailTransaksi[i].JumlahBarang;
                                detail.Nama = objXml.DetailTransaksi[i].Nama;
                                detail.Ppn = objXml.DetailTransaksi[i].Ppn;
                                detail.Ppnbm = objXml.DetailTransaksi[i].Ppnbm;
                                detail.TarifPpnbm = objXml.DetailTransaksi[i].TarifPpnbm;

                                result.DataFakturPajak[0].DetailTransaksi.Add(detail);
                            }

                            ResValidateFakturPm FakturPM = new ResValidateFakturPm();
                            FakturPajakBatchRequestSetting datarequest = new FakturPajakBatchRequestSetting();
                            datarequest.UrlScan = URL;

                            //Exception ex = new Exception();
                            //SaveDetailTransaksiToDatabase(objXml, URL, out ex);
                            //Task.Run(() => SaveToDbAsync(objXml, chkByUrl));
                            string outLogKey;
                            Logger.WriteLog(out outLogKey, LogLevel.Info, $"[START] Save  FPDJPID : {FPdjpID}, No. Faktur : {objXml.NomorFaktur}", MethodBase.GetCurrentMethod());
                            Console.WriteLine($"[START] Save No. Faktur : {objXml.NomorFaktur}");
                            Task.Run(async () =>
                            {
                                try
                                {
                                    Logger.WriteLog(out outLogKey, LogLevel.Info, $"[START] SaveToDBAsync FPDJPID : {FPdjpID}, No. Faktur : {objXml.NomorFaktur}", MethodBase.GetCurrentMethod());
                                    Console.WriteLine($"[START] SaveToDBAsync No. Faktur : {objXml.NomorFaktur}");
                                    await SaveToDbAsync(objXml, chkByUrl);
                                    Logger.WriteLog(out outLogKey, LogLevel.Info, $"[FINISH] Success SaveToDBAsync FPDJPID : {FPdjpID}, No. Faktur : {objXml.NomorFaktur}", MethodBase.GetCurrentMethod());
                                    Console.WriteLine($"[FINISH] Success SaveToDBAsync No. Faktur : {objXml.NomorFaktur}");
                                }
                                catch (Exception ex)
                                {
                                    Logger.WriteLog(out outLogKey, LogLevel.Error, $"[ERROR] Terjadi error saat SaveToDBAsync FPDJPID : {FPdjpID}, Error Message: {ex.Message}", MethodBase.GetCurrentMethod());
                                    Console.WriteLine($"Terjadi error saat SaveToDBAsync: {ex.Message}");
                                    Console.WriteLine(outLogKey.ToString());
                                    throw;
                                }
                                finally
                                {
                                    Logger.WriteLog(out outLogKey, LogLevel.Info, $"[FINISH] Save  FPDJPID : {FPdjpID}, No. Faktur : {objXml.NomorFaktur}", MethodBase.GetCurrentMethod());
                                    Console.WriteLine($"[FINISH] Save No. Faktur : {objXml.NomorFaktur}");
                                }
                            });


                            //if (ex != null)
                            //    msgError = ex.Message;

                            #region validation
                            var msgValidation = new List<string>();

                            //var dt = DateTime.ParseExact(head.TglFaktur, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            //var strMasaPajak = dt.Month.ToString();
                            //var strTahunPajak = dt.Year.ToString();

                            var periode = OpenClosePeriods.GetOpenReguler().OrderBy(a => a.OpenClosePeriodId).FirstOrDefault();
                            var strMasaPajak = periode.MasaPajak.ToString();
                            var strTahunPajak = periode.TahunPajak.ToString();



                            if (!(string.IsNullOrEmpty(strMasaPajak) || strMasaPajak == "0") &&
                                !(string.IsNullOrEmpty(strTahunPajak) || strTahunPajak == "0"))
                            {
                                var getOpenClosePeriod = OpenClosePeriods.GetByMasaPajak(int.Parse(strMasaPajak),
                                    int.Parse(strTahunPajak));

                                if (getOpenClosePeriod != null)
                                {
                                    if (!getOpenClosePeriod.StatusRegular)
                                    {
                                        msgValidation.Add("Status Masa Pajak Close Reguler");
                                    }
                                    else
                                    {
                                        if (!getOpenClosePeriod.StatusSp2)
                                        {
                                            msgValidation.Add("Status Masa Pajak Close SP2");
                                        }
                                    }
                                }
                                else
                                {
                                    msgValidation.Add("Masa dan Tahun Pajak tidak tersedia di Data Open Close Periode");
                                }
                            }

                            int tahunPajak = 0;
                            int masaPajak = 0;
                            if (string.IsNullOrEmpty(strMasaPajak) || strMasaPajak == "0")
                            {
                                msgValidation.Add("Masa Pajak Mandatory");
                            }
                            else
                            {
                                if (!int.TryParse(strMasaPajak.Trim(), out masaPajak))
                                {
                                    msgValidation.Add("Invalid Tahun Pajak");
                                }
                            }
                            if (string.IsNullOrEmpty(strTahunPajak) || strTahunPajak == "0")
                            {
                                msgValidation.Add("Tahun Pajak Mandatory");
                            }
                            else
                            {
                                if (!int.TryParse(strTahunPajak.Trim(), out tahunPajak))
                                {
                                    msgValidation.Add("Invalid Tahun Pajak");
                                }
                            }

                            if (URL == null || URL.Trim().Length == 0)
                            {
                                msgValidation.Add("Scan Url Mandatory");
                            }
                            if (head.NomorFakturPajak == null || head.NomorFakturPajak.Trim().Length == 0)
                            {
                                msgValidation.Add("No Faktur Pajak Mandatory");
                            }

                            if (string.IsNullOrEmpty(head.TglFaktur))
                            {
                                msgValidation.Add("Tanggal Faktur Pajak Mandatory");
                            }
                            else
                            {
                                //getconfig
                                var configData = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.PelaporanTglFaktur);
                                if (configData == null)
                                {
                                    msgValidation.Add("GeneralConfig [PelaporanTglFaktur] not found.");
                                }
                                else
                                {
                                    var dats = configData.ConfigValue.Split(':').ToList();
                                    if (dats.Count != 2)
                                    {
                                        msgValidation.Add("GeneralConfig [PelaporanTglFaktur] not valid.");
                                    }
                                    else
                                    {
                                        if (masaPajak != 0 && tahunPajak != 0)
                                        {
                                            if (!string.IsNullOrEmpty(head.FgPengganti) && head.FgPengganti == "0")
                                            {
                                                //BPM No. ASMO3-201847620
                                                int min = int.Parse(dats[0].Replace("[", "").Replace("]", "")); // -3
                                                int max = int.Parse(dats[1].Replace("[", "").Replace("]", "")); // 0
                                                var dtMin = new DateTime(tahunPajak, masaPajak, 1).AddMonths(min); // ex: oktober -> 1 juli


                                                var maxperiode = OpenClosePeriods.GetOpenReguler().OrderByDescending(a => a.OpenClosePeriodId).FirstOrDefault();
                                                var maxMasaPajak = maxperiode.MasaPajak;
                                                var maxTahunPajak = maxperiode.TahunPajak;
                                                var dtMax = new DateTime(maxTahunPajak, maxMasaPajak, 1).AddMonths(max + 1).AddDays(-1); // ex: oktober -> tgl terakhir bulan oktober


                                                var dt = DateTime.ParseExact(head.TglFaktur, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                                                var dTglFaktur = Convert.ToDateTime(string.Concat(dt.Year, "-", dt.Month, "-", dt.Day));
                                                var dtMaxValidity = new DateTime(dtMax.Year, dtMax.Month, DateTime.DaysInMonth(dtMax.Year, dtMax.Month)); // ex: 30 oktober
                                                if (dTglFaktur < dtMin) // juli < april
                                                {
                                                    msgValidation.Add("Tanggal Faktur Pajak sudah kadaluarsa");
                                                }
                                                else
                                                {
                                                    if (dTglFaktur > dtMaxValidity)
                                                    {
                                                        msgValidation.Add("Tanggal Faktur Pajak tidak sesuai ketentuan");
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }
                            }

                            //Check NPWP Adm
                            if (string.IsNullOrEmpty(head.NpwpLawanTransaksi))
                            {
                                msgValidation.Add("NPWP Pembeli Mandatory");
                            }
                            else
                            {
                                var chkConfig = GeneralConfigs.GetConfigCheckNpwpAdm(head.NpwpLawanTransaksi);
                                if (chkConfig == null)
                                {
                                    var npwpAdm = GeneralConfigs.GetByKeyId(ApplicationEnums.GeneralConfig.NpwpAdm);
                                    msgValidation.Add("NPWP Pembeli bukan " + npwpAdm.ConfigValue);
                                }
                            }

                            //Check NPWP Adm
                            if (string.IsNullOrEmpty(head.NamaLawanTransaksi))
                            {
                                msgValidation.Add("Nama Pembeli Mandatory");
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(head.FgPengganti))
                                {
                                    if (head.FgPengganti != "2")
                                    {
                                        var d = GeneralConfigs.GetByKeyId(ApplicationEnums.GeneralConfig.NamaNpwpAdm);

                                        var dList = d.ConfigValue.Split(';').Where(c => !string.IsNullOrEmpty(c) && !string.IsNullOrEmpty(c.Trim())).ToList();
                                        var availableNamaPembeli = (string.Join(",", dList));

                                        var chk = dList.Where(dItem => head.NamaLawanTransaksi.Replace(" ", "").ToLower().Contains(dItem.Replace(" ", "").ToLower())).ToList();

                                        if (chk.Count <= 0)
                                        {
                                            msgValidation.Add("Nama Pembeli bukan yang diperbolehkan [" + availableNamaPembeli + "]");
                                        }
                                    }
                                }
                            }

                            if (string.IsNullOrEmpty(head.NpwpPenjual))
                            {
                                msgValidation.Add("NPWP Penjual Mandatory");
                            }
                            else
                            {
                                var v = Vendors.GetByNpwp(head.NpwpPenjual);
                                if (v != null && v.PkpDicabut)
                                {
                                    msgValidation.Add("PKP Dicabut atas NPWP Penjual");
                                }
                            }

                            var we = string.Join(Environment.NewLine, msgValidation);

                            if (string.IsNullOrEmpty(head.KdJenisTransaksi))
                            {
                                msgValidation.Add("KdJenisTransaksi Mandatory");
                            }

                            if (string.IsNullOrEmpty(head.FgPengganti))
                            {
                                msgValidation.Add("FgPengganti Mandatory");
                            }

                            if (string.IsNullOrEmpty(head.StatusFaktur))
                            {
                                msgValidation.Add("Status Faktur Mandatory");
                            }

                            var yyy = string.Join(Environment.NewLine, msgValidation);

                            if (msgValidation.Count <= 0)
                            {
                                /*
                                 * Disable berdasarkan CR Perubahan dari sisi e-Faktur V2.0
                                 * Terkait scan FP Pengganti (Kode yang digit ke -3 ‘1’ contoh: 011,021,031,041) maka tidak dibutuhkan scan FP awal (Kode yang digit ke -3 ‘0’ contoh: 010,020,030,040)"
                                 */
                                /*
                                 * Rollback berdasarkan CR terbaru, tetap ada validasi harus scan faktur pajak normal (Kode yang digit ke-3 nya '0'
                                 * BPM No. ASMO3-201847620 
                                 */
                                var ttt = string.Join(Environment.NewLine, msgValidation);
                                if (head.FgPengganti != "0")
                                {
                                    /* BPM No. ASMO3-201847620
                                     * (c) Saat user scan FP normal-pengganti, maka akan muncul request untuk scan FP diganti terlebih
                                     * dahulu agar dapat mengetahui kapan FP diganti tersebut kadaluarsa (jika FP diganti
                                     * expired/kadaluarsa maka ditolak oleh EVIS). - jika 010 belum di scan di EVIS 
                                     */
                                    var msgValidatePengganti = FakturPajaks.ValidateScanPengganti(head.NomorFakturPajak, head.FgPengganti,
                                        ApplicationEnums.FPType.External);
                                    if (!string.IsNullOrEmpty(msgValidatePengganti))
                                    {
                                        msgValidation.Add(msgValidatePengganti);
                                    }
                                    else
                                    {
                                        /* BPM No. ASMO3-201847620
                                         * (a) Jika FP diganti (kode awal 010) ada dan belum expired, maka FP normal-pengganti (kode awal 011) dapat diterima/discan. 
                                         * - langsung masuk (dan masih open), FP 010 & 011 harus discan dimasa yang sama. 
                                        */
                                        var fpNormal = FakturPajaks.GetFakturPajakNormal(head.NomorFakturPajak);
                                        if (fpNormal != null && fpNormal.FakturPajakId != 0)
                                        {
                                            var configData = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.PelaporanTglFaktur);
                                            if (configData == null)
                                            {
                                                msgValidation.Add("GeneralConfig [PelaporanTglFaktur] not found.");
                                            }
                                            else
                                            {
                                                //check apakah Faktur Normal sudah Expired
                                                var dats = configData.ConfigValue.Split(':').ToList();
                                                if (dats.Count != 2)
                                                {
                                                    msgValidation.Add("GeneralConfig [PelaporanTglFaktur] not valid.");
                                                }
                                                else
                                                {
                                                    /* BPM No. ASMO3-201847620
                                                     * (b) Jika FP diganti (kode awal 010) sudah expired, maka FP normal-pengganti (kode awal 011) akan
                                                     * ditolak oleh EVIS. 
                                                     * Tgl Faktur dibandingkan dengan MasaPajak-TahunPajak
                                                     */
                                                    if (tahunPajak != 0 && masaPajak != 0)
                                                    {
                                                        //noted tgl 2019-02-06
                                                        //apakah bisa dikatakan bahwa ketika scan fp normal pengganti tidak perlu cek expired tgl faktur terhadap masa pajak fp normal pengganti itu ?
                                                        //mas indra : Yup, bisa dikatakan begitu. Jadi FP normal pengganti selalu refer ke FP normal
                                                        //Jadi gini jika yg di scan fp normal pengganti:
                                                        //1. Dia akan cek apakah ada FP normalnya?
                                                        //1.a jika tidak maka notif perlu scan FP normalnya
                                                        //1.b jika ada maka cek masa pajak n tgl faktur
                                                        //2. Cek apakah masa pajak sama dg yg lg di scan?
                                                        //2.a jika tidak maka notif masa pajak harus sama
                                                        //2.b jika sama maka cek tgl faktur..
                                                        //berdasarkan pernyataannya maka point 2b tidak perlu dicek "Iya mas, jadi gak valid lg utk dicek. 2b jadi gak peru."

                                                        int min = int.Parse(dats[0].Replace("[", "").Replace("]", ""));
                                                        var dtMin = new DateTime(tahunPajak, masaPajak, 1).AddMonths(min);
                                                        if (fpNormal.TglFaktur < dtMin)
                                                        {
                                                            msgValidation.Add("Faktur Pajak Diganti sudah kadarluasa");
                                                        }
                                                        //else
                                                        //{
                                                        //    if (!(tahunPajak == fpNormal.TahunPajak && masaPajak == fpNormal.MasaPajak))
                                                        //    {
                                                        //        msgValidation.Add("Faktur Pajak Pengganti harus di Masa-Tahun Pajak yang sama dengan Faktur Pajak Normal");
                                                        //    }
                                                        //}

                                                        //if (!(tahunPajak == fpNormal.TahunPajak && masaPajak == fpNormal.MasaPajak))
                                                        //{
                                                        //    msgValidation.Add("Faktur Pajak Pengganti harus di Masa-Tahun Pajak yang sama dengan Faktur Pajak Normal");
                                                        //}

                                                        //CR DZ - Validasi Scan Pengganti (11) Jika FP Digantinya masih berstatus FP-Nornal



                                                        DateTime monthName = new DateTime(2020, fpNormal.MasaPajak.Value, 1);
                                                        if (fpNormal.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturPajakNormal))
                                                        {
                                                            msgValidation.Add("Faktur Pajak " + fpNormal.FormatedNoFaktur + " sudah dilaporkan di masa pajak " + monthName.ToString("MMMM") + " " + fpNormal.TahunPajak.ToString());
                                                            // msgValidation.Add("No FP Diganti [" + fpNormal.FormatedNoFaktur + "] belum discan");

                                                        }

                                                    }

                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    var fpNormal = FakturPajaks.GetFakturPajakDuplikasi(head.NomorFakturPajak, head.KdJenisTransaksi, head.FgPengganti);
                                    if (fpNormal.StatusFaktur != null)
                                    {
                                        msgValidation.Add(String.Format("Nomor faktur pajak {0} sudah ada pada aplikasi EVIS", head.NomorFakturPajak));
                                    }


                                    var fpNormalTerlapor = FakturPajaks.GetFakturPajakNormalTerlapor(head.NomorFakturPajak);
                                    if (fpNormalTerlapor.NamaPelaporan != null && fpNormalTerlapor.NamaPelaporan != "")
                                    {

                                        // -- mmd 05 04 2023
                                        //DateTime monthName = new DateTime(2020, fpNormalTerlapor.MasaPajak.Value, 1);
                                        DateTime monthName = new DateTime(fpNormalTerlapor.TahunPajak.Value, fpNormalTerlapor.MasaPajak.Value, 1);
                                        //--

                                        if (fpNormalTerlapor.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturPajakNormal))
                                        {
                                            msgValidation.Add("Faktur Pajak " + fpNormalTerlapor.FormatedNoFaktur + " sudah dilaporkan di masa pajak " + monthName.ToString("MMMM") + " " + fpNormalTerlapor.TahunPajak.ToString());

                                        }
                                    }
                                }
                                var www = string.Join(Environment.NewLine, msgValidation);
                                /*
                                 * Penambahan validasi berdasarkan CR Perubahan dari sisi e-Faktur V2.0
                                 * Ada validasi tambahan ketika semua Faktur Pajak (apapun kode FP nya) yang di scan adalah “Faktur Diganti” maka akan muncul notifikasi “Faktur Pajak sudah diganti”
                                 */
                                if (head.StatusFaktur.ToLower().Trim() == "faktur diganti")
                                {
                                    msgValidation.Add("Faktur Pajak sudah diganti");
                                }

                                /*
                                 * CR Penambahan validasi untuk faktur pajak yang sudah dibatalkan
                                 * BPM No. ASMO3-201847620 
                                 */
                                if (head.StatusFaktur.ToLower().Trim() == "faktur dibatalkan")
                                {
                                    msgValidation.Add("Faktur Pajak Dibatalkan");
                                }

                                /* BPM No. ASMO3-201847620
                                 * (d) Jika FP dengan kode 011 statusnya adalah FP diganti maka akan ditolak oleh EVIS - jika ada 2
                                 * FP pengganti
                                 */
                                if (head.StatusFaktur.ToLower().Trim() == "faktur diganti" && head.FgPengganti != "0")
                                {
                                    msgValidation.Add("Faktur Pajak Normal - Pengganti sudah diganti");
                                }
                                var sss = string.Join(Environment.NewLine, msgValidation);
                                //CR DZ -- Validasi Scan Diganti (10), jika FP Normal sudah dilaporkan, lalu dari DJP mengeluarkan FP Digantinya.
                                if (head.FgPengganti == "0" && head.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti))
                                {
                                    FakturPajak fpNormal = new FakturPajak();

                                    fpNormal = FakturPajaks.GetFakturPajakNormal(head.NomorFakturPajak);
                                    if (fpNormal != null)
                                    {
                                        if (fpNormal.FakturPajakId != 0)
                                        {
                                            if (fpNormal.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturPajakNormal) && fpNormal.MasaPajak.ToString() != masaPajak.ToString() && fpNormal.MasaPajak != null)
                                            {
                                                var getOpenClosePeriod = OpenClosePeriods.GetByMasaPajak(fpNormal.MasaPajak.Value, fpNormal.TahunPajak.Value);
                                                if (getOpenClosePeriod != null)
                                                {
                                                    if (!getOpenClosePeriod.StatusRegular || !getOpenClosePeriod.StatusSp2)
                                                    {
                                                        //                                    msgValidation.Add("Faktur Pajak-Nomal sudah dilaporkan");
                                                        //  DateTime monthName = new DateTime(2020, fpNormal.MasaPajak.Value, 1);
                                                        //  msgValidation.Add("Faktur Pajak " + fpNormal.FormatedNoFaktur + " sudah dilaporkan di masa pajak " + monthName.ToString("MMMM") + " " + fpNormal.TahunPajak.ToString());

                                                        msgValidation.Add("Sudah discan pada " +
                                                    ConvertHelper.DateTimeConverter.ToLongDateString(fpNormal.Created) + " oleh " +
                                                    fpNormal.CreatedBy +
                                                    ". No FP " + fpNormal.FormatedNoFaktur + " sudah ada di Masa Pajak " + fpNormal.MasaPajakName +
                                                    " " + fpNormal.TahunPajak
                                                    + ", Nomor Filling Index " + fpNormal.FillingIndex);

                                                    }

                                                }
                                            }
                                        }
                                    }
                                }

                            }
                            result.StatusValidasi = msgValidation.Count > 0 ? "true" : "false";
                            //result.NamaValidasi = "";5
                            result.KeteranganValidasi = string.Join(Environment.NewLine, msgValidation);

                            #endregion valdation


                            if (!IsHitFromApi)
                            {
                                logposting.CreatedBy = UserName;
                                logposting.Created = DateTime.Now;
                                logposting.Source = Source;
                                logposting.Payload = JsonConvert.SerializeObject(result);
                                logposting.FPdjpID = FPdjpID;
                                logposting.Action = EnumHelper.GetDescription(ApplicationEnums.EnumLogApiAction.SendFaktur);
                                LogPostingTanggalLaporans.Save(logposting);
                            }

                        }
                        else
                        {
                            result.StatusValidasi = "true";
                            //result.NamaValidasi = "";
                            //result.KeteranganValidasi = MsgErrorGetValidate;
                            result.KeteranganValidasi = "Error mapping object XML";
                            //logposting.Created = DateTime.Now;
                            //logposting.CreatedBy = UserName;
                            //logposting.Source = Source;
                            //logposting.Payload = "";
                            //logposting.FPdjpID = FPdjpID;
                            //logposting.Url = URL;
                            //logposting.Action = EnumHelper.GetDescription(ApplicationEnums.EnumLogApiAction.DJPError);
                            //LogPostingTanggalLaporans.Save(logposting);
                            msgError = result.KeteranganValidasi;
                        }

                    }
                    else
                    {
                        result.StatusValidasi = "true";
                        //result.NamaValidasi = "";
                        //result.KeteranganValidasi = MsgErrorGetValidate;
                        result.KeteranganValidasi = "Failed Get Data From DJP";
                        logposting.Created = DateTime.Now;
                        logposting.CreatedBy = UserName;
                        logposting.Source = Source;
                        logposting.Payload = "";
                        logposting.FPdjpID = FPdjpID;
                        logposting.Url = URL;
                        logposting.Message = MsgErrorGetValidate;
                        logposting.Action = EnumHelper.GetDescription(ApplicationEnums.EnumLogApiAction.DJPError);
                        logposting.Id = LogPostingID;
                        LogPostingTanggalLaporans.Save(logposting);

                        eStatus = WebExceptionStatus.SendFailure;

                    }
                }
                catch (Exception e)
                {
                    result.StatusValidasi = "true";
                    //result.NamaValidasi = "";
                    var ExceptionMsg = "";
                    if (e.InnerException == null)
                    {
                        ExceptionMsg = string.Concat(e.Message, Environment.NewLine, e.StackTrace);
                    }
                    else
                    {
                        ExceptionMsg = string.Concat(e.InnerException.Message, Environment.NewLine, e.StackTrace);
                    }
                    msgError = ExceptionMsg;
                    result.KeteranganValidasi = ExceptionMsg;
                    eStatus = WebExceptionStatus.SendFailure;
                }
            }
            else
            {

                result.StatusValidasi = "true";
                result.KeteranganValidasi = "URL Mandatory";

                eStatus = WebExceptionStatus.SendFailure;
            }
            return result;
        }


        public static FakturPajakResultModel ValidateFakturObjectAPI_2(string URL, string Source, string UserName, string FPdjpID, string tmp_file, out string logKey, out string eStatus, out string msgError)
        {
            logKey = "";
            msgError = "";
            FakturPajakResultModel result = new FakturPajakResultModel();
            result.StatusValidasi = "false";

            result.KeteranganValidasi = "";
            result.DataFakturPajak = new List<DataFakturPajak>();
            result.FPdjpID = FPdjpID;

            URL = URL.Trim();
            //check if exists url on database
            var chkByUrl = FakturPajaks.GetByUrlScan(URL);

            LogPostingTanggalLaporan logposting = new LogPostingTanggalLaporan();

            if (!string.IsNullOrEmpty(URL))
            {
                try
                {
                    string logkey2;
                    var MsgErrorGetValidate = "";
                    var objXml = DJPLib.ValidateFakturLib.GetValidateFakturObjectV3_2(URL, tmp_file, out logkey2);
                    if (chkByUrl != null && chkByUrl.MasaPajak.HasValue && chkByUrl.TahunPajak.HasValue)
                    {
                        if (chkByUrl.Status == (int)ApplicationEnums.StatusFakturPajak.Success &&
                            chkByUrl.StatusFaktur != EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti))
                        {

                            if (!(chkByUrl.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturPajakNormal) &&
                                objXml.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti)
                                ))
                            {

                                msgError = "Sudah discan pada " +
                                ConvertHelper.DateTimeConverter.ToLongDateString(chkByUrl.Created) + " oleh " +
                                chkByUrl.CreatedBy +
                                ". No FP " + chkByUrl.FormatedNoFaktur + " sudah ada di Masa Pajak " + chkByUrl.MasaPajakName +
                                " " + chkByUrl.TahunPajak
                                + ", Nomor Filling Index " + chkByUrl.FillingIndex;

                                result.StatusValidasi = "true";
                                result.KeteranganValidasi = msgError;
                            }

                        }

                        if (chkByUrl.Status == (int)ApplicationEnums.StatusFakturPajak.ErrorRequest || chkByUrl.Status == (int)ApplicationEnums.StatusFakturPajak.ErrorValidation)// jika status 3/4 baru masuk sini
                        {
                            msgError = "Sudah discan pada "
                                + ConvertHelper.DateTimeConverter.ToLongDateString(chkByUrl.Created) + " oleh " +
                                chkByUrl.CreatedBy +
                                ". Di Masa Pajak " +
                                chkByUrl.MasaPajakName + " " + chkByUrl.TahunPajak
                                + ", Nomor Filling Index " + chkByUrl.FillingIndex + ". Tapi belum request ke DJP atau ada error validasi.";
                            result.StatusValidasi = "true";
                            result.KeteranganValidasi = msgError;
                        }
                    }


                    if (objXml != null)
                    {

                        var head = new Models.DataFakturPajak();
                        head.AlamatLawanTransaksi = objXml.AlamatLawanTransaksi;
                        head.AlamatPenjual = objXml.AlamatPenjual;
                        head.FgPengganti = objXml.FgPengganti;
                        head.JumlahDpp = objXml.JumlahDpp;
                        head.JumlahPpn = objXml.JumlahPpn;
                        head.JumlahPpnBm = objXml.JumlahPpnBm;
                        head.KdJenisTransaksi = objXml.KdJenisTransaksi;
                        head.NamaLawanTransaksi = objXml.NamaLawanTransaksi;
                        head.NamaPenjual = objXml.NamaPenjual;

                        head.NomorFakturPajak = objXml.NomorFaktur;
                        var formatedFp = FormatingDomains.GetFormatNoFaktur(ApplicationEnums.FPType.ScanNonIws, head.KdJenisTransaksi, head.FgPengganti, head.NomorFakturPajak);
                        if (formatedFp.IsValid)
                            head.NomorFakturPajakFormatted = formatedFp.FormattedField;
                        else head.NomorFakturPajakFormatted = objXml.NomorFaktur;

                        var lstnpwp = new List<string>();
                        lstnpwp.Add(objXml.NpwpLawanTransaksi);
                        var npwpformat = FormatingDomains.GetFormatNpwp(lstnpwp);
                        head.NpwpLawanTransaksi = objXml.NpwpLawanTransaksi;
                        head.NpwpLawanTransaksiFormatted = npwpformat.FirstOrDefault().FormattedField;

                        lstnpwp.Clear();
                        lstnpwp.Add(objXml.NpwpPenjual);
                        npwpformat = FormatingDomains.GetFormatNpwp(lstnpwp);
                        head.NpwpPenjual = objXml.NpwpPenjual;
                        head.NpwpPenjualFormatted = npwpformat.FirstOrDefault().FormattedField;

                        head.Referensi = objXml.Referensi;
                        head.StatusApproval = objXml.StatusApproval;
                        head.StatusFaktur = objXml.StatusFaktur;
                        head.TglFaktur = objXml.TanggalFaktur;

                        result.DataFakturPajak.Add(head);
                        result.DataFakturPajak[0].DetailTransaksi = new System.Collections.Generic.List<Models.DetailTransaksi>();
                        for (int i = 0; i < objXml.DetailTransaksi.Count; i++)
                        {
                            var detail = new Models.DetailTransaksi();
                            detail.Diskon = objXml.DetailTransaksi[i].Diskon;
                            detail.Dpp = objXml.DetailTransaksi[i].Dpp;
                            detail.HargaSatuan = objXml.DetailTransaksi[i].HargaSatuan;
                            detail.HargaTotal = objXml.DetailTransaksi[i].HargaTotal;
                            detail.JumlahBarang = objXml.DetailTransaksi[i].JumlahBarang;
                            detail.Nama = objXml.DetailTransaksi[i].Nama;
                            detail.Ppn = objXml.DetailTransaksi[i].Ppn;
                            detail.Ppnbm = objXml.DetailTransaksi[i].Ppnbm;
                            detail.TarifPpnbm = objXml.DetailTransaksi[i].TarifPpnbm;

                            result.DataFakturPajak[0].DetailTransaksi.Add(detail);
                        }

                        ResValidateFakturPm FakturPM = new ResValidateFakturPm();
                        FakturPajakBatchRequestSetting datarequest = new FakturPajakBatchRequestSetting();
                        datarequest.UrlScan = URL;

                        Exception ex = new Exception();
                        SaveDetailTransaksiToDatabase(objXml, URL, out ex);
                        if (ex != null)
                            msgError = ex.Message;

                        var msgValidation = new List<string>();

                        var periode = OpenClosePeriods.GetOpenReguler().OrderBy(a => a.OpenClosePeriodId).FirstOrDefault();
                        var strMasaPajak = periode.MasaPajak.ToString();
                        var strTahunPajak = periode.TahunPajak.ToString();



                        if (!(string.IsNullOrEmpty(strMasaPajak) || strMasaPajak == "0") &&
                            !(string.IsNullOrEmpty(strTahunPajak) || strTahunPajak == "0"))
                        {
                            var getOpenClosePeriod = OpenClosePeriods.GetByMasaPajak(int.Parse(strMasaPajak),
                                int.Parse(strTahunPajak));

                            if (getOpenClosePeriod != null)
                            {
                                if (!getOpenClosePeriod.StatusRegular)
                                {
                                    msgValidation.Add("Status Masa Pajak Close Reguler");
                                }
                                else
                                {
                                    if (!getOpenClosePeriod.StatusSp2)
                                    {
                                        msgValidation.Add("Status Masa Pajak Close SP2");
                                    }
                                }
                            }
                            else
                            {
                                msgValidation.Add("Masa dan Tahun Pajak tidak tersedia di Data Open Close Periode");
                            }
                        }

                        int tahunPajak = 0;
                        int masaPajak = 0;
                        if (string.IsNullOrEmpty(strMasaPajak) || strMasaPajak == "0")
                        {
                            msgValidation.Add("Masa Pajak Mandatory");
                        }
                        else
                        {
                            if (!int.TryParse(strMasaPajak.Trim(), out masaPajak))
                            {
                                msgValidation.Add("Invalid Tahun Pajak");
                            }
                        }
                        if (string.IsNullOrEmpty(strTahunPajak) || strTahunPajak == "0")
                        {
                            msgValidation.Add("Tahun Pajak Mandatory");
                        }
                        else
                        {
                            if (!int.TryParse(strTahunPajak.Trim(), out tahunPajak))
                            {
                                msgValidation.Add("Invalid Tahun Pajak");
                            }
                        }

                        if (URL == null || URL.Trim().Length == 0)
                        {
                            msgValidation.Add("Scan Url Mandatory");
                        }
                        if (head.NomorFakturPajak == null || head.NomorFakturPajak.Trim().Length == 0)
                        {
                            msgValidation.Add("No Faktur Pajak Mandatory");
                        }

                        if (string.IsNullOrEmpty(head.TglFaktur))
                        {
                            msgValidation.Add("Tanggal Faktur Pajak Mandatory");
                        }
                        else
                        {
                            //getconfig
                            var configData = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.PelaporanTglFaktur);
                            if (configData == null)
                            {
                                msgValidation.Add("GeneralConfig [PelaporanTglFaktur] not found.");
                            }
                            else
                            {
                                var dats = configData.ConfigValue.Split(':').ToList();
                                if (dats.Count != 2)
                                {
                                    msgValidation.Add("GeneralConfig [PelaporanTglFaktur] not valid.");
                                }
                                else
                                {
                                    if (masaPajak != 0 && tahunPajak != 0)
                                    {
                                        if (!string.IsNullOrEmpty(head.FgPengganti) && head.FgPengganti == "0")
                                        {
                                            //BPM No. ASMO3-201847620
                                            int min = int.Parse(dats[0].Replace("[", "").Replace("]", "")); // -3
                                            int max = int.Parse(dats[1].Replace("[", "").Replace("]", "")); // 0
                                            var dtMin = new DateTime(tahunPajak, masaPajak, 1).AddMonths(min); // ex: oktober -> 1 juli


                                            var maxperiode = OpenClosePeriods.GetOpenReguler().OrderByDescending(a => a.OpenClosePeriodId).FirstOrDefault();
                                            var maxMasaPajak = maxperiode.MasaPajak;
                                            var maxTahunPajak = maxperiode.TahunPajak;
                                            var dtMax = new DateTime(maxTahunPajak, maxMasaPajak, 1).AddMonths(max + 1).AddDays(-1); // ex: oktober -> tgl terakhir bulan oktober


                                            var dt = DateTime.ParseExact(head.TglFaktur, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                                            var dTglFaktur = Convert.ToDateTime(string.Concat(dt.Year, "-", dt.Month, "-", dt.Day));
                                            var dtMaxValidity = new DateTime(dtMax.Year, dtMax.Month, DateTime.DaysInMonth(dtMax.Year, dtMax.Month)); // ex: 30 oktober
                                            if (dTglFaktur < dtMin) // juli < april
                                            {
                                                msgValidation.Add("Tanggal Faktur Pajak sudah kadaluarsa");
                                            }
                                            else
                                            {
                                                if (dTglFaktur > dtMaxValidity)
                                                {
                                                    msgValidation.Add("Tanggal Faktur Pajak tidak sesuai ketentuan");
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }

                        //Check NPWP Adm
                        if (string.IsNullOrEmpty(head.NpwpLawanTransaksi))
                        {
                            msgValidation.Add("NPWP Pembeli Mandatory");
                        }
                        else
                        {
                            var chkConfig = GeneralConfigs.GetConfigCheckNpwpAdm(head.NpwpLawanTransaksi);
                            if (chkConfig == null)
                            {
                                var npwpAdm = GeneralConfigs.GetByKeyId(ApplicationEnums.GeneralConfig.NpwpAdm);
                                msgValidation.Add("NPWP Pembeli bukan " + npwpAdm.ConfigValue);
                            }
                        }

                        //Check NPWP Adm
                        if (string.IsNullOrEmpty(head.NamaLawanTransaksi))
                        {
                            msgValidation.Add("Nama Pembeli Mandatory");
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(head.FgPengganti))
                            {
                                if (head.FgPengganti != "2")
                                {
                                    var d = GeneralConfigs.GetByKeyId(ApplicationEnums.GeneralConfig.NamaNpwpAdm);

                                    var dList = d.ConfigValue.Split(';').Where(c => !string.IsNullOrEmpty(c) && !string.IsNullOrEmpty(c.Trim())).ToList();
                                    var availableNamaPembeli = (string.Join(",", dList));

                                    var chk = dList.Where(dItem => head.NamaLawanTransaksi.Replace(" ", "").ToLower().Contains(dItem.Replace(" ", "").ToLower())).ToList();

                                    if (chk.Count <= 0)
                                    {
                                        msgValidation.Add("Nama Pembeli bukan yang diperbolehkan [" + availableNamaPembeli + "]");
                                    }
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(head.NpwpPenjual))
                        {
                            msgValidation.Add("NPWP Penjual Mandatory");
                        }
                        else
                        {
                            var v = Vendors.GetByNpwp(head.NpwpPenjual);
                            if (v != null && v.PkpDicabut)
                            {
                                msgValidation.Add("PKP Dicabut atas NPWP Penjual");
                            }
                        }

                        var we = string.Join(Environment.NewLine, msgValidation);

                        if (string.IsNullOrEmpty(head.KdJenisTransaksi))
                        {
                            msgValidation.Add("KdJenisTransaksi Mandatory");
                        }

                        if (string.IsNullOrEmpty(head.FgPengganti))
                        {
                            msgValidation.Add("FgPengganti Mandatory");
                        }

                        if (string.IsNullOrEmpty(head.StatusFaktur))
                        {
                            msgValidation.Add("Status Faktur Mandatory");
                        }

                        var yyy = string.Join(Environment.NewLine, msgValidation);

                        if (msgValidation.Count <= 0)
                        {
                            var ttt = string.Join(Environment.NewLine, msgValidation);
                            if (head.FgPengganti != "0")
                            {
                                var msgValidatePengganti = FakturPajaks.ValidateScanPengganti(head.NomorFakturPajak, head.FgPengganti,
                                    ApplicationEnums.FPType.External);
                                if (!string.IsNullOrEmpty(msgValidatePengganti))
                                {
                                    msgValidation.Add(msgValidatePengganti);
                                }
                                else
                                {
                                    var fpNormal = FakturPajaks.GetFakturPajakNormal(head.NomorFakturPajak);
                                    if (fpNormal != null && fpNormal.FakturPajakId != 0)
                                    {
                                        var configData = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.PelaporanTglFaktur);
                                        if (configData == null)
                                        {
                                            msgValidation.Add("GeneralConfig [PelaporanTglFaktur] not found.");
                                        }
                                        else
                                        {
                                            var dats = configData.ConfigValue.Split(':').ToList();
                                            if (dats.Count != 2)
                                            {
                                                msgValidation.Add("GeneralConfig [PelaporanTglFaktur] not valid.");
                                            }
                                            else
                                            {
                                                if (tahunPajak != 0 && masaPajak != 0)
                                                {
                                                    int min = int.Parse(dats[0].Replace("[", "").Replace("]", ""));
                                                    var dtMin = new DateTime(tahunPajak, masaPajak, 1).AddMonths(min);
                                                    if (fpNormal.TglFaktur < dtMin)
                                                    {
                                                        msgValidation.Add("Faktur Pajak Diganti sudah kadarluasa");
                                                    }

                                                    DateTime monthName = new DateTime(2020, fpNormal.MasaPajak.Value, 1);
                                                    if (fpNormal.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturPajakNormal))
                                                    {
                                                        msgValidation.Add("Faktur Pajak " + fpNormal.FormatedNoFaktur + " sudah dilaporkan di masa pajak " + monthName.ToString("MMMM") + " " + fpNormal.TahunPajak.ToString());
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var fpNormal = FakturPajaks.GetFakturPajakDuplikasi(head.NomorFakturPajak, head.KdJenisTransaksi, head.FgPengganti);
                                if (fpNormal.StatusFaktur != null)
                                {
                                    msgValidation.Add(String.Format("Nomor faktur pajak {0} sudah ada pada aplikasi EVIS", head.NomorFakturPajak));
                                }

                                var fpNormalTerlapor = FakturPajaks.GetFakturPajakNormalTerlapor(head.NomorFakturPajak);
                                if (fpNormalTerlapor.NamaPelaporan != null && fpNormalTerlapor.NamaPelaporan != "")
                                {
                                    DateTime monthName = new DateTime(fpNormalTerlapor.TahunPajak.Value, fpNormalTerlapor.MasaPajak.Value, 1);
                                    if (fpNormalTerlapor.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturPajakNormal))
                                    {
                                        msgValidation.Add("Faktur Pajak " + fpNormalTerlapor.FormatedNoFaktur + " sudah dilaporkan di masa pajak " + monthName.ToString("MMMM") + " " + fpNormalTerlapor.TahunPajak.ToString());
                                    }
                                }
                            }
                            var www = string.Join(Environment.NewLine, msgValidation);

                            if (head.StatusFaktur.ToLower().Trim() == "faktur diganti")
                            {
                                msgValidation.Add("Faktur Pajak sudah diganti");
                            }

                            if (head.StatusFaktur.ToLower().Trim() == "faktur dibatalkan")
                            {
                                msgValidation.Add("Faktur Pajak Dibatalkan");
                            }

                            if (head.StatusFaktur.ToLower().Trim() == "faktur diganti" && head.FgPengganti != "0")
                            {
                                msgValidation.Add("Faktur Pajak Normal - Pengganti sudah diganti");
                            }
                            var sss = string.Join(Environment.NewLine, msgValidation);

                            if (head.FgPengganti == "0" && head.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti))
                            {
                                FakturPajak fpNormal = new FakturPajak();

                                fpNormal = FakturPajaks.GetFakturPajakNormal(head.NomorFakturPajak);
                                if (fpNormal != null)
                                {
                                    if (fpNormal.FakturPajakId != 0)
                                    {
                                        if (fpNormal.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturPajakNormal) && fpNormal.MasaPajak.ToString() != masaPajak.ToString() && fpNormal.MasaPajak != null)
                                        {
                                            var getOpenClosePeriod = OpenClosePeriods.GetByMasaPajak(fpNormal.MasaPajak.Value, fpNormal.TahunPajak.Value);
                                            if (getOpenClosePeriod != null)
                                            {
                                                if (!getOpenClosePeriod.StatusRegular || !getOpenClosePeriod.StatusSp2)
                                                {
                                                    msgValidation.Add("Sudah discan pada " +
                                                    ConvertHelper.DateTimeConverter.ToLongDateString(fpNormal.Created) + " oleh " +
                                                    fpNormal.CreatedBy +
                                                    ". No FP " + fpNormal.FormatedNoFaktur + " sudah ada di Masa Pajak " + fpNormal.MasaPajakName +
                                                    " " + fpNormal.TahunPajak
                                                    + ", Nomor Filling Index " + fpNormal.FillingIndex);

                                                }

                                            }
                                        }
                                    }
                                }
                            }

                        }
                        result.StatusValidasi = msgValidation.Count > 0 ? "true" : "false";
                        result.KeteranganValidasi = string.Join(Environment.NewLine, msgValidation);

                    }
                    else
                    {
                        result.StatusValidasi = "true";
                        result.KeteranganValidasi = "Failed Get Data From DJP";
                        logposting.Created = DateTime.Now;
                        logposting.CreatedBy = UserName;
                        logposting.Source = Source;
                        logposting.Payload = "";
                        logposting.FPdjpID = FPdjpID;
                        logposting.Url = URL;
                        logposting.Message = MsgErrorGetValidate;
                        logposting.Action = EnumHelper.GetDescription(ApplicationEnums.EnumLogApiAction.DJPError);
                        logposting.Id = 990011;
                        LogPostingTanggalLaporans.Save(logposting);
                    }
                }
                catch (Exception e)
                {
                    result.StatusValidasi = "true";
                    var ExceptionMsg = "";
                    if (e.InnerException == null)
                    {
                        ExceptionMsg = string.Concat(e.Message, Environment.NewLine, e.StackTrace);
                    }
                    else
                    {
                        ExceptionMsg = string.Concat(e.InnerException.Message, Environment.NewLine, e.StackTrace);
                    }
                    msgError = ExceptionMsg;
                    result.KeteranganValidasi = ExceptionMsg;
                }
            }
            else
            {

                result.StatusValidasi = "true";
                result.KeteranganValidasi = "URL Mandatory";
            }

            eStatus = "OK";

            if (msgError != "")
            {
                eStatus = "NO";
            }

            return result;
        }
        public static string SaveDetailTransaksiToDatabase(DJPLib.Objects.ResValidateFakturPm objData, string URL, out Exception ex)
        {
            ex = null;
            var msgs = string.Empty;
            DJPLibConfiguration.LoadConfig();
            const string userName = DJPLibConfiguration.Actor;

            try
            {
                using (var eScope = new TransactionScope())
                {
                    var fp = FakturPajaks.GetByUrlScan(URL);
                    if (fp == null || fp.FakturPajakId == 0) return "Skip";
                    if (fp.StatusFaktur != objData.StatusFaktur)
                    {
                        if (objData.StatusFaktur.ToLower() == "faktur diganti" || objData.StatusFaktur.ToLower() == "faktur dibatalkan")
                        {
                            if (fp.FgPengganti == "0")
                            {

                                var fpoutcheck =
                                    FakturPajakDigantiOutstandings.GetByFormatedNoFaktur(fp.FormatedNoFaktur);
                                if (fpoutcheck == null || fpoutcheck.Id == 0)
                                {
                                    if (fp.TahunPajak.HasValue && fp.MasaPajak.HasValue &&
                                        fp.TglFaktur.HasValue)
                                    {
                                        //belum ada
                                        //Add to FP Diganti Outstanding
                                        var tahunpajaktosave = fp.TahunPajak.Value;
                                        var masapajaktosave = fp.MasaPajak.Value;
                                        var tahunpajaktocheck = fp.TglFaktur.Value.Year;
                                        var masapajaktocheck = fp.TglFaktur.Value.Month;

                                        var fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Outstanding;

                                        var dtmin =
                                            new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
                                                DJPLibConfiguration.MaxPelaporan);
                                        var dtmax =
                                            new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
                                                Math.Abs(DJPLibConfiguration.MinPelaporan));
                                        var availableperiods = OpenClosePeriods.GetByRange(dtmin, dtmax);
                                        if (availableperiods.Count > 0)
                                        {
                                            //jika semua nya tidak ada yang open maka langsung expired
                                            var chkopencloseperiod =
                                                availableperiods.Where(c => c.StatusRegular).ToList();
                                            if (chkopencloseperiod.Count <= 0)
                                            {
                                                fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Expired;
                                            }
                                        }
                                        var fpoutstanding = new FakturPajakDigantiOutstanding()
                                        {
                                            Id = 0,
                                            FormatedNoFaktur = fp.FormatedNoFaktur,
                                            TahunPajak = tahunpajaktosave,
                                            MasaPajak = masapajaktosave,
                                            StatusApproval = objData.StatusApproval,
                                            StatusFaktur = objData.StatusFaktur,
                                            Keterangan = null,
                                            KeteranganDjp = null,
                                            StatusOutstanding = (int)fpoustandingstatus,
                                            CreatedBy = userName
                                        };

                                        FakturPajakDigantiOutstandings.Save(fpoutstanding);

                                        fp.MasaPajak = null;
                                        fp.TahunPajak = null;
                                        fp.ModifiedBy = userName;
                                        fp.Modified = DateTime.Now;
                                        FakturPajaks.Save(fp);
                                    }
                                }

                            }
                            else
                            {
                                //FP Normal Pengganti yang berubah Status Faktur menjadi Faktur Diganti
                                //get fp normal
                                var getfpnormal = FakturPajaks.GetFakturPajakNormal(fp.NoFakturPajak);
                                if (getfpnormal != null && getfpnormal.TglFaktur.HasValue &&
                                    getfpnormal.TahunPajak.HasValue && getfpnormal.MasaPajak.HasValue)
                                {

                                    var fpoutchek =
                                        FakturPajakDigantiOutstandings.GetByFormatedNoFaktur(
                                            getfpnormal.FormatedNoFaktur);
                                    if (fpoutchek == null || fpoutchek.Id == 0)
                                    {
                                        var tahunpajaktocheck = getfpnormal.TglFaktur.Value.Year;
                                        var masapajaktocheck = getfpnormal.TglFaktur.Value.Month;
                                        var tahunpajaktosave = getfpnormal.TahunPajak.Value;
                                        var masapajaktosave = getfpnormal.MasaPajak.Value;

                                        var fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Outstanding;

                                        var dtmin =
                                            new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
                                                DJPLibConfiguration.MaxPelaporan);
                                        var dtmax =
                                            new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
                                                Math.Abs(DJPLibConfiguration.MinPelaporan));
                                        var availableperiods = OpenClosePeriods.GetByRange(dtmin, dtmax);
                                        if (availableperiods.Count > 0)
                                        {
                                            //jika semua nya tidak ada yang open maka langsung expired
                                            var chkopencloseperiod =
                                                availableperiods.Where(c => c.StatusRegular).ToList();
                                            if (chkopencloseperiod.Count <= 0)
                                            {
                                                fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Expired;
                                            }
                                        }
                                        var fpoutstanding = new FakturPajakDigantiOutstanding()
                                        {
                                            Id = 0,
                                            FormatedNoFaktur = getfpnormal.FormatedNoFaktur,
                                            TahunPajak = tahunpajaktosave,
                                            MasaPajak = masapajaktosave,
                                            StatusApproval = getfpnormal.StatusApproval,
                                            StatusFaktur = getfpnormal.StatusFaktur,
                                            Keterangan = null,
                                            KeteranganDjp = null,
                                            StatusOutstanding = (int)fpoustandingstatus,
                                            CreatedBy = userName
                                        };

                                        FakturPajakDigantiOutstandings.Save(fpoutstanding);

                                        getfpnormal.TahunPajak = null;
                                        getfpnormal.MasaPajak = null;
                                        getfpnormal.Modified = DateTime.Now;
                                        getfpnormal.ModifiedBy = userName;

                                        FakturPajaks.Save(getfpnormal);

                                        //update Status Faktur FP Normal Pengganti sesuai dari DJP
                                        fp.StatusFaktur = objData.StatusFaktur;
                                        fp.StatusApproval = objData.StatusApproval;
                                        fp.TahunPajak = null;
                                        fp.MasaPajak = null;
                                        fp.Modified = DateTime.Now;
                                        fp.ModifiedBy = userName;
                                        FakturPajaks.Save(fp);

                                    }
                                    else
                                    {
                                        //sudah ada
                                        //dibalikin jadi outstanding atau expired lagi
                                        var tahunpajaktocheck = getfpnormal.TglFaktur.Value.Year;
                                        var masapajaktocheck = getfpnormal.TglFaktur.Value.Month;

                                        var fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Outstanding;

                                        var dtmin =
                                            new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
                                                DJPLibConfiguration.MaxPelaporan);
                                        var dtmax =
                                            new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
                                                Math.Abs(DJPLibConfiguration.MinPelaporan));
                                        var availableperiods = OpenClosePeriods.GetByRange(dtmin, dtmax);
                                        if (availableperiods.Count > 0)
                                        {
                                            //jika semua nya tidak ada yang open maka langsung expired
                                            var chkopencloseperiod =
                                                availableperiods.Where(c => c.StatusRegular).ToList();
                                            if (chkopencloseperiod.Count <= 0)
                                            {
                                                fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Expired;
                                            }
                                        }

                                        fpoutchek.StatusOutstanding = (int)fpoustandingstatus;
                                        fpoutchek.StatusApproval = getfpnormal.StatusApproval;
                                        fpoutchek.StatusFaktur = getfpnormal.StatusFaktur;
                                        fpoutchek.TahunPajak = getfpnormal.TahunPajak;
                                        fpoutchek.MasaPajak = getfpnormal.MasaPajak;
                                        fpoutchek.Modified = DateTime.Now;
                                        fpoutchek.ModifiedBy = userName;

                                        FakturPajakDigantiOutstandings.Save(fpoutchek);

                                        //Reset Masa-Tahun Pajak FP Normal
                                        getfpnormal.TahunPajak = null;
                                        getfpnormal.MasaPajak = null;
                                        getfpnormal.Modified = DateTime.Now;
                                        getfpnormal.ModifiedBy = userName;

                                        FakturPajaks.Save(getfpnormal);

                                        //update Status Faktur FP Normal Pengganti sesuai dari DJP
                                        //Reset Masa-Tahun Pajak FP Normal Pengganti yang sudah berubah menjadi Faktur Diganti
                                        fp.StatusFaktur = objData.StatusFaktur;
                                        fp.StatusApproval = objData.StatusApproval;
                                        fp.TahunPajak = null;
                                        fp.MasaPajak = null;
                                        fp.Modified = DateTime.Now;
                                        fp.ModifiedBy = userName;
                                        FakturPajaks.Save(fp);

                                    }
                                }
                            }
                        }
                        else
                        {
                            fp.KdJenisTransaksi = objData.KdJenisTransaksi;
                            fp.FgPengganti = objData.FgPengganti;
                            fp.NoFakturPajak = objData.NomorFaktur;
                            fp.NPWPPenjual = objData.NpwpPenjual;
                            fp.NamaPenjual = objData.NamaPenjual;
                            fp.AlamatPenjual = objData.AlamatPenjual;
                            fp.NPWPLawanTransaksi = objData.NpwpLawanTransaksi;
                            fp.NamaLawanTransaksi = objData.NamaLawanTransaksi;
                            fp.AlamatLawanTransaksi = objData.AlamatLawanTransaksi;
                            fp.JumlahDPP = Convert.ToDecimal(objData.JumlahDpp);
                            fp.JumlahPPN = Convert.ToDecimal(objData.JumlahPpn);
                            fp.JumlahPPNBM = Convert.ToDecimal(objData.JumlahPpnBm);
                            fp.StatusApproval = objData.StatusApproval;
                            fp.StatusFaktur = objData.StatusFaktur;
                            fp.Pesan = objData.StatusApproval;
                            fp.TglFaktur = DateTime.ParseExact(objData.TanggalFaktur.Trim(), "dd/MM/yyyy",
                                CultureInfo.InvariantCulture);
                            fp.Status = (int)ApplicationEnums.StatusFakturPajak.Success;
                            fp.ModifiedBy = userName;
                            fp.Modified = DateTime.Now;

                            FakturPajaks.Save(fp);
                        }
                    }
                    else
                    {
                        fp.KdJenisTransaksi = objData.KdJenisTransaksi;
                        fp.FgPengganti = objData.FgPengganti;
                        fp.NoFakturPajak = objData.NomorFaktur;
                        fp.NPWPPenjual = objData.NpwpPenjual;
                        fp.NamaPenjual = objData.NamaPenjual;
                        fp.AlamatPenjual = objData.AlamatPenjual;
                        fp.NPWPLawanTransaksi = objData.NpwpLawanTransaksi;
                        fp.NamaLawanTransaksi = objData.NamaLawanTransaksi;
                        fp.AlamatLawanTransaksi = objData.AlamatLawanTransaksi;
                        fp.JumlahDPP = Convert.ToDecimal(objData.JumlahDpp);
                        fp.JumlahPPN = Convert.ToDecimal(objData.JumlahPpn);
                        fp.JumlahPPNBM = Convert.ToDecimal(objData.JumlahPpnBm);
                        fp.StatusApproval = objData.StatusApproval;
                        fp.StatusFaktur = objData.StatusFaktur;
                        fp.Pesan = objData.StatusApproval;
                        fp.TglFaktur = DateTime.ParseExact(objData.TanggalFaktur.Trim(), "dd/MM/yyyy",
                            CultureInfo.InvariantCulture);
                        fp.Status = (int)ApplicationEnums.StatusFakturPajak.Success;
                        fp.ModifiedBy = userName;
                        fp.Modified = DateTime.Now;

                        FakturPajaks.Save(fp);
                    }

                    //Delimite FakturPajakDetail by FakturPajakId
                    FakturPajakDetails.DeleteByFakturPajakId(fp.FakturPajakId, userName);

                    #region -------------- Save Detail Transaksi --------------
                    if (fp.FakturPajakId > 0 && objData.DetailTransaksi != null && objData.DetailTransaksi.Count > 0)
                    {
                        foreach (var dataitem in objData.DetailTransaksi)
                        {
                            var item = dataitem;
                            var fakturPajakDetail = new FakturPajakDetail()
                            {
                                Nama = item.Nama,
                                HargaSatuan = Convert.ToDecimal(item.HargaSatuan),
                                JumlahBarang = Convert.ToDecimal(item.JumlahBarang),
                                HargaTotal = Convert.ToDecimal(item.HargaTotal),
                                Diskon = Convert.ToDecimal(item.Diskon),
                                Dpp = Convert.ToDecimal(item.Dpp),
                                Ppn = Convert.ToDecimal(item.Ppn),
                                TarifPpnbm = Convert.ToDecimal(item.TarifPpnbm),
                                Ppnbm = Convert.ToDecimal(item.Ppnbm),
                                CreatedBy = userName,
                                FakturPajakId = fp.FakturPajakId
                            };

                            FakturPajakDetails.Insert(fakturPajakDetail);
                        }


                    }

                    #endregion

                    //insert ke log untuk di exclude ketika diproses
                    var logfp = new LogFPDigantiOutstanding()
                    {
                        FormatedNoFaktur = fp.FormatedNoFaktur
                    };

                    LogFPDigantiOutstandings.Add(logfp);


                    eScope.Complete();
                    eScope.Dispose();

                }
            }
            catch (Exception exception)
            {
                ex = exception;
                msgs = "Save to Database Failed for Url : " + URL;
            }

            return msgs;
        }

        public static async Task SaveToDbAsync(DJPLib.Objects.ResValidateFakturPm objData, FakturPajak chkByUrl)
        {

            //await _semaphore.WaitAsync(); // Menunggu sampai slot tersedia
            try
            {
                await _semaphore.WaitAsync(TimeSpan.FromSeconds(30));

                // Proses penyimpanan ke database
                DJPLibConfiguration.LoadConfig();
                const string userName = DJPLibConfiguration.Actor;
                var transactionOptions = new TransactionOptions
                {
                    IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted, // Sesuaikan level isolasi jika diperlukan
                    Timeout = TimeSpan.FromMinutes(5) // Atur timeout sesuai kebutuhan
                };

                using (var eScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
                {
                    //var fp = FakturPajaks.GetByUrlScan(URL);
                    var fp = chkByUrl;
                    if (fp != null)
                    {
                        if (fp.StatusFaktur != objData.StatusFaktur)
                        {
                            if (objData.StatusFaktur.ToLower() == "faktur diganti" || objData.StatusFaktur.ToLower() == "faktur dibatalkan")
                            {
                                if (fp.FgPengganti == "0")
                                {

                                    var fpoutcheck =
                                        FakturPajakDigantiOutstandings.GetByFormatedNoFaktur(fp.FormatedNoFaktur);
                                    if (fpoutcheck == null || fpoutcheck.Id == 0)
                                    {
                                        if (fp.TahunPajak.HasValue && fp.MasaPajak.HasValue &&
                                            fp.TglFaktur.HasValue)
                                        {
                                            //belum ada
                                            //Add to FP Diganti Outstanding
                                            var tahunpajaktosave = fp.TahunPajak.Value;
                                            var masapajaktosave = fp.MasaPajak.Value;
                                            var tahunpajaktocheck = fp.TglFaktur.Value.Year;
                                            var masapajaktocheck = fp.TglFaktur.Value.Month;

                                            var fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Outstanding;

                                            var dtmin =
                                                new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
                                                    DJPLibConfiguration.MaxPelaporan);
                                            var dtmax =
                                                new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
                                                    Math.Abs(DJPLibConfiguration.MinPelaporan));
                                            var availableperiods = OpenClosePeriods.GetByRange(dtmin, dtmax);
                                            if (availableperiods.Count > 0)
                                            {
                                                //jika semua nya tidak ada yang open maka langsung expired
                                                var chkopencloseperiod =
                                                    availableperiods.Where(c => c.StatusRegular).ToList();
                                                if (chkopencloseperiod.Count <= 0)
                                                {
                                                    fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Expired;
                                                }
                                            }
                                            var fpoutstanding = new FakturPajakDigantiOutstanding()
                                            {
                                                Id = 0,
                                                FormatedNoFaktur = fp.FormatedNoFaktur,
                                                TahunPajak = tahunpajaktosave,
                                                MasaPajak = masapajaktosave,
                                                StatusApproval = objData.StatusApproval,
                                                StatusFaktur = objData.StatusFaktur,
                                                Keterangan = null,
                                                KeteranganDjp = null,
                                                StatusOutstanding = (int)fpoustandingstatus,
                                                CreatedBy = userName
                                            };

                                            FakturPajakDigantiOutstandings.Save(fpoutstanding);

                                            fp.MasaPajak = null;
                                            fp.TahunPajak = null;
                                            fp.ModifiedBy = userName;
                                            fp.Modified = DateTime.Now;
                                            FakturPajaks.Save(fp);
                                        }
                                    }

                                }
                                else
                                {
                                    //FP Normal Pengganti yang berubah Status Faktur menjadi Faktur Diganti
                                    //get fp normal
                                    var getfpnormal = FakturPajaks.GetFakturPajakNormal(fp.NoFakturPajak);
                                    if (getfpnormal != null && getfpnormal.TglFaktur.HasValue &&
                                        getfpnormal.TahunPajak.HasValue && getfpnormal.MasaPajak.HasValue)
                                    {

                                        var fpoutchek =
                                            FakturPajakDigantiOutstandings.GetByFormatedNoFaktur(
                                                getfpnormal.FormatedNoFaktur);
                                        if (fpoutchek == null || fpoutchek.Id == 0)
                                        {
                                            var tahunpajaktocheck = getfpnormal.TglFaktur.Value.Year;
                                            var masapajaktocheck = getfpnormal.TglFaktur.Value.Month;
                                            var tahunpajaktosave = getfpnormal.TahunPajak.Value;
                                            var masapajaktosave = getfpnormal.MasaPajak.Value;

                                            var fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Outstanding;

                                            var dtmin =
                                                new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
                                                    DJPLibConfiguration.MaxPelaporan);
                                            var dtmax =
                                                new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
                                                    Math.Abs(DJPLibConfiguration.MinPelaporan));
                                            var availableperiods = OpenClosePeriods.GetByRange(dtmin, dtmax);
                                            if (availableperiods.Count > 0)
                                            {
                                                //jika semua nya tidak ada yang open maka langsung expired
                                                var chkopencloseperiod =
                                                    availableperiods.Where(c => c.StatusRegular).ToList();
                                                if (chkopencloseperiod.Count <= 0)
                                                {
                                                    fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Expired;
                                                }
                                            }
                                            var fpoutstanding = new FakturPajakDigantiOutstanding()
                                            {
                                                Id = 0,
                                                FormatedNoFaktur = getfpnormal.FormatedNoFaktur,
                                                TahunPajak = tahunpajaktosave,
                                                MasaPajak = masapajaktosave,
                                                StatusApproval = getfpnormal.StatusApproval,
                                                StatusFaktur = getfpnormal.StatusFaktur,
                                                Keterangan = null,
                                                KeteranganDjp = null,
                                                StatusOutstanding = (int)fpoustandingstatus,
                                                CreatedBy = userName
                                            };

                                            FakturPajakDigantiOutstandings.Save(fpoutstanding);

                                            getfpnormal.TahunPajak = null;
                                            getfpnormal.MasaPajak = null;
                                            getfpnormal.Modified = DateTime.Now;
                                            getfpnormal.ModifiedBy = userName;

                                            FakturPajaks.Save(getfpnormal);

                                            //update Status Faktur FP Normal Pengganti sesuai dari DJP
                                            fp.StatusFaktur = objData.StatusFaktur;
                                            fp.StatusApproval = objData.StatusApproval;
                                            fp.TahunPajak = null;
                                            fp.MasaPajak = null;
                                            fp.Modified = DateTime.Now;
                                            fp.ModifiedBy = userName;
                                            FakturPajaks.Save(fp);

                                        }
                                        else
                                        {
                                            //sudah ada
                                            //dibalikin jadi outstanding atau expired lagi
                                            var tahunpajaktocheck = getfpnormal.TglFaktur.Value.Year;
                                            var masapajaktocheck = getfpnormal.TglFaktur.Value.Month;

                                            var fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Outstanding;

                                            var dtmin =
                                                new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
                                                    DJPLibConfiguration.MaxPelaporan);
                                            var dtmax =
                                                new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
                                                    Math.Abs(DJPLibConfiguration.MinPelaporan));
                                            var availableperiods = OpenClosePeriods.GetByRange(dtmin, dtmax);
                                            if (availableperiods.Count > 0)
                                            {
                                                //jika semua nya tidak ada yang open maka langsung expired
                                                var chkopencloseperiod =
                                                    availableperiods.Where(c => c.StatusRegular).ToList();
                                                if (chkopencloseperiod.Count <= 0)
                                                {
                                                    fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Expired;
                                                }
                                            }

                                            fpoutchek.StatusOutstanding = (int)fpoustandingstatus;
                                            fpoutchek.StatusApproval = getfpnormal.StatusApproval;
                                            fpoutchek.StatusFaktur = getfpnormal.StatusFaktur;
                                            fpoutchek.TahunPajak = getfpnormal.TahunPajak;
                                            fpoutchek.MasaPajak = getfpnormal.MasaPajak;
                                            fpoutchek.Modified = DateTime.Now;
                                            fpoutchek.ModifiedBy = userName;

                                            FakturPajakDigantiOutstandings.Save(fpoutchek);

                                            //Reset Masa-Tahun Pajak FP Normal
                                            getfpnormal.TahunPajak = null;
                                            getfpnormal.MasaPajak = null;
                                            getfpnormal.Modified = DateTime.Now;
                                            getfpnormal.ModifiedBy = userName;

                                            FakturPajaks.Save(getfpnormal);

                                            //update Status Faktur FP Normal Pengganti sesuai dari DJP
                                            //Reset Masa-Tahun Pajak FP Normal Pengganti yang sudah berubah menjadi Faktur Diganti
                                            fp.StatusFaktur = objData.StatusFaktur;
                                            fp.StatusApproval = objData.StatusApproval;
                                            fp.TahunPajak = null;
                                            fp.MasaPajak = null;
                                            fp.Modified = DateTime.Now;
                                            fp.ModifiedBy = userName;
                                            FakturPajaks.Save(fp);

                                        }
                                    }
                                }
                            }
                            else
                            {
                                fp.KdJenisTransaksi = objData.KdJenisTransaksi;
                                fp.FgPengganti = objData.FgPengganti;
                                fp.NoFakturPajak = objData.NomorFaktur;
                                fp.NPWPPenjual = objData.NpwpPenjual;
                                fp.NamaPenjual = objData.NamaPenjual;
                                fp.AlamatPenjual = objData.AlamatPenjual;
                                fp.NPWPLawanTransaksi = objData.NpwpLawanTransaksi;
                                fp.NamaLawanTransaksi = objData.NamaLawanTransaksi;
                                fp.AlamatLawanTransaksi = objData.AlamatLawanTransaksi;
                                fp.JumlahDPP = Convert.ToDecimal(objData.JumlahDpp);
                                fp.JumlahPPN = Convert.ToDecimal(objData.JumlahPpn);
                                fp.JumlahPPNBM = Convert.ToDecimal(objData.JumlahPpnBm);
                                fp.StatusApproval = objData.StatusApproval;
                                fp.StatusFaktur = objData.StatusFaktur;
                                fp.Pesan = objData.StatusApproval;
                                fp.TglFaktur = DateTime.ParseExact(objData.TanggalFaktur.Trim(), "dd/MM/yyyy",
                                    CultureInfo.InvariantCulture);
                                fp.Status = (int)ApplicationEnums.StatusFakturPajak.Success;
                                fp.ModifiedBy = userName;
                                fp.Modified = DateTime.Now;

                                FakturPajaks.Save(fp);
                            }
                        }
                        else
                        {
                            fp.KdJenisTransaksi = objData.KdJenisTransaksi;
                            fp.FgPengganti = objData.FgPengganti;
                            fp.NoFakturPajak = objData.NomorFaktur;
                            fp.NPWPPenjual = objData.NpwpPenjual;
                            fp.NamaPenjual = objData.NamaPenjual;
                            fp.AlamatPenjual = objData.AlamatPenjual;
                            fp.NPWPLawanTransaksi = objData.NpwpLawanTransaksi;
                            fp.NamaLawanTransaksi = objData.NamaLawanTransaksi;
                            fp.AlamatLawanTransaksi = objData.AlamatLawanTransaksi;
                            fp.JumlahDPP = Convert.ToDecimal(objData.JumlahDpp);
                            fp.JumlahPPN = Convert.ToDecimal(objData.JumlahPpn);
                            fp.JumlahPPNBM = Convert.ToDecimal(objData.JumlahPpnBm);
                            fp.StatusApproval = objData.StatusApproval;
                            fp.StatusFaktur = objData.StatusFaktur;
                            fp.Pesan = objData.StatusApproval;
                            fp.TglFaktur = DateTime.ParseExact(objData.TanggalFaktur.Trim(), "dd/MM/yyyy",
                                CultureInfo.InvariantCulture);
                            fp.Status = (int)ApplicationEnums.StatusFakturPajak.Success;
                            fp.ModifiedBy = userName;
                            fp.Modified = DateTime.Now;

                            FakturPajaks.Save(fp);
                        }

                        //Delimite FakturPajakDetail by FakturPajakId
                        FakturPajakDetails.DeleteByFakturPajakId(fp.FakturPajakId, userName);


                        #region -------------- Save Detail Transaksi --------------
                        if (fp.FakturPajakId > 0 && objData.DetailTransaksi != null && objData.DetailTransaksi.Count > 0)
                        {
                            foreach (var dataitem in objData.DetailTransaksi)
                            {
                                var item = dataitem;
                                var fakturPajakDetail = new FakturPajakDetail()
                                {
                                    Nama = item.Nama,
                                    HargaSatuan = Convert.ToDecimal(item.HargaSatuan),
                                    JumlahBarang = Convert.ToDecimal(item.JumlahBarang),
                                    HargaTotal = Convert.ToDecimal(item.HargaTotal),
                                    Diskon = Convert.ToDecimal(item.Diskon),
                                    Dpp = Convert.ToDecimal(item.Dpp),
                                    Ppn = Convert.ToDecimal(item.Ppn),
                                    TarifPpnbm = Convert.ToDecimal(item.TarifPpnbm),
                                    Ppnbm = Convert.ToDecimal(item.Ppnbm),
                                    CreatedBy = userName,
                                    FakturPajakId = fp.FakturPajakId
                                };

                                FakturPajakDetails.Insert(fakturPajakDetail);
                            }


                        }

                        #endregion

                        //insert ke log untuk di exclude ketika diproses
                        var logfp = new LogFPDigantiOutstanding()
                        {
                            FormatedNoFaktur = fp.FormatedNoFaktur
                        };

                        LogFPDigantiOutstandings.Add(logfp);



                    }

                    eScope.Complete();
                    eScope.Dispose();

                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(out var outLogKey, LogLevel.Info, $"Terjadi error saat SaveToDBAsync: {ex.Message}", MethodBase.GetCurrentMethod());
                Console.WriteLine($"Terjadi error saat SaveToDBAsync: {ex.Message}");
                Console.WriteLine(outLogKey.ToString());
                throw ex;
            }
            finally
            {
                _semaphore.Release(); // Melepaskan slot
            }

        }

        public static void SaveToDb(DJPLib.Objects.ResValidateFakturPm objData, FakturPajak chkByUrl)
        {
            try
            {
                // Proses penyimpanan ke database
                DJPLibConfiguration.LoadConfig();
                const string userName = DJPLibConfiguration.Actor;

                using (var eScope = new TransactionScope())
                {
                    //var fp = FakturPajaks.GetByUrlScan(URL);
                    var fp = chkByUrl;
                    if (fp != null)
                    {
                        if (fp.StatusFaktur != objData.StatusFaktur)
                        {
                            if (objData.StatusFaktur.ToLower() == "faktur diganti" || objData.StatusFaktur.ToLower() == "faktur dibatalkan")
                            {
                                if (fp.FgPengganti == "0")
                                {

                                    var fpoutcheck =
                                        FakturPajakDigantiOutstandings.GetByFormatedNoFaktur(fp.FormatedNoFaktur);
                                    if (fpoutcheck == null || fpoutcheck.Id == 0)
                                    {
                                        if (fp.TahunPajak.HasValue && fp.MasaPajak.HasValue &&
                                            fp.TglFaktur.HasValue)
                                        {
                                            //belum ada
                                            //Add to FP Diganti Outstanding
                                            var tahunpajaktosave = fp.TahunPajak.Value;
                                            var masapajaktosave = fp.MasaPajak.Value;
                                            var tahunpajaktocheck = fp.TglFaktur.Value.Year;
                                            var masapajaktocheck = fp.TglFaktur.Value.Month;

                                            var fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Outstanding;

                                            var dtmin =
                                                new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
                                                    DJPLibConfiguration.MaxPelaporan);
                                            var dtmax =
                                                new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
                                                    Math.Abs(DJPLibConfiguration.MinPelaporan));
                                            var availableperiods = OpenClosePeriods.GetByRange(dtmin, dtmax);
                                            if (availableperiods.Count > 0)
                                            {
                                                //jika semua nya tidak ada yang open maka langsung expired
                                                var chkopencloseperiod =
                                                    availableperiods.Where(c => c.StatusRegular).ToList();
                                                if (chkopencloseperiod.Count <= 0)
                                                {
                                                    fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Expired;
                                                }
                                            }
                                            var fpoutstanding = new FakturPajakDigantiOutstanding()
                                            {
                                                Id = 0,
                                                FormatedNoFaktur = fp.FormatedNoFaktur,
                                                TahunPajak = tahunpajaktosave,
                                                MasaPajak = masapajaktosave,
                                                StatusApproval = objData.StatusApproval,
                                                StatusFaktur = objData.StatusFaktur,
                                                Keterangan = null,
                                                KeteranganDjp = null,
                                                StatusOutstanding = (int)fpoustandingstatus,
                                                CreatedBy = userName
                                            };

                                            FakturPajakDigantiOutstandings.Save(fpoutstanding);

                                            fp.MasaPajak = null;
                                            fp.TahunPajak = null;
                                            fp.ModifiedBy = userName;
                                            fp.Modified = DateTime.Now;
                                            FakturPajaks.Save(fp);
                                        }
                                    }

                                }
                                else
                                {
                                    //FP Normal Pengganti yang berubah Status Faktur menjadi Faktur Diganti
                                    //get fp normal
                                    var getfpnormal = FakturPajaks.GetFakturPajakNormal(fp.NoFakturPajak);
                                    if (getfpnormal != null && getfpnormal.TglFaktur.HasValue &&
                                        getfpnormal.TahunPajak.HasValue && getfpnormal.MasaPajak.HasValue)
                                    {

                                        var fpoutchek =
                                            FakturPajakDigantiOutstandings.GetByFormatedNoFaktur(
                                                getfpnormal.FormatedNoFaktur);
                                        if (fpoutchek == null || fpoutchek.Id == 0)
                                        {
                                            var tahunpajaktocheck = getfpnormal.TglFaktur.Value.Year;
                                            var masapajaktocheck = getfpnormal.TglFaktur.Value.Month;
                                            var tahunpajaktosave = getfpnormal.TahunPajak.Value;
                                            var masapajaktosave = getfpnormal.MasaPajak.Value;

                                            var fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Outstanding;

                                            var dtmin =
                                                new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
                                                    DJPLibConfiguration.MaxPelaporan);
                                            var dtmax =
                                                new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
                                                    Math.Abs(DJPLibConfiguration.MinPelaporan));
                                            var availableperiods = OpenClosePeriods.GetByRange(dtmin, dtmax);
                                            if (availableperiods.Count > 0)
                                            {
                                                //jika semua nya tidak ada yang open maka langsung expired
                                                var chkopencloseperiod =
                                                    availableperiods.Where(c => c.StatusRegular).ToList();
                                                if (chkopencloseperiod.Count <= 0)
                                                {
                                                    fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Expired;
                                                }
                                            }
                                            var fpoutstanding = new FakturPajakDigantiOutstanding()
                                            {
                                                Id = 0,
                                                FormatedNoFaktur = getfpnormal.FormatedNoFaktur,
                                                TahunPajak = tahunpajaktosave,
                                                MasaPajak = masapajaktosave,
                                                StatusApproval = getfpnormal.StatusApproval,
                                                StatusFaktur = getfpnormal.StatusFaktur,
                                                Keterangan = null,
                                                KeteranganDjp = null,
                                                StatusOutstanding = (int)fpoustandingstatus,
                                                CreatedBy = userName
                                            };

                                            FakturPajakDigantiOutstandings.Save(fpoutstanding);

                                            getfpnormal.TahunPajak = null;
                                            getfpnormal.MasaPajak = null;
                                            getfpnormal.Modified = DateTime.Now;
                                            getfpnormal.ModifiedBy = userName;

                                            FakturPajaks.Save(getfpnormal);

                                            //update Status Faktur FP Normal Pengganti sesuai dari DJP
                                            fp.StatusFaktur = objData.StatusFaktur;
                                            fp.StatusApproval = objData.StatusApproval;
                                            fp.TahunPajak = null;
                                            fp.MasaPajak = null;
                                            fp.Modified = DateTime.Now;
                                            fp.ModifiedBy = userName;
                                            FakturPajaks.Save(fp);

                                        }
                                        else
                                        {
                                            //sudah ada
                                            //dibalikin jadi outstanding atau expired lagi
                                            var tahunpajaktocheck = getfpnormal.TglFaktur.Value.Year;
                                            var masapajaktocheck = getfpnormal.TglFaktur.Value.Month;

                                            var fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Outstanding;

                                            var dtmin =
                                                new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
                                                    DJPLibConfiguration.MaxPelaporan);
                                            var dtmax =
                                                new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
                                                    Math.Abs(DJPLibConfiguration.MinPelaporan));
                                            var availableperiods = OpenClosePeriods.GetByRange(dtmin, dtmax);
                                            if (availableperiods.Count > 0)
                                            {
                                                //jika semua nya tidak ada yang open maka langsung expired
                                                var chkopencloseperiod =
                                                    availableperiods.Where(c => c.StatusRegular).ToList();
                                                if (chkopencloseperiod.Count <= 0)
                                                {
                                                    fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Expired;
                                                }
                                            }

                                            fpoutchek.StatusOutstanding = (int)fpoustandingstatus;
                                            fpoutchek.StatusApproval = getfpnormal.StatusApproval;
                                            fpoutchek.StatusFaktur = getfpnormal.StatusFaktur;
                                            fpoutchek.TahunPajak = getfpnormal.TahunPajak;
                                            fpoutchek.MasaPajak = getfpnormal.MasaPajak;
                                            fpoutchek.Modified = DateTime.Now;
                                            fpoutchek.ModifiedBy = userName;

                                            FakturPajakDigantiOutstandings.Save(fpoutchek);

                                            //Reset Masa-Tahun Pajak FP Normal
                                            getfpnormal.TahunPajak = null;
                                            getfpnormal.MasaPajak = null;
                                            getfpnormal.Modified = DateTime.Now;
                                            getfpnormal.ModifiedBy = userName;

                                            FakturPajaks.Save(getfpnormal);

                                            //update Status Faktur FP Normal Pengganti sesuai dari DJP
                                            //Reset Masa-Tahun Pajak FP Normal Pengganti yang sudah berubah menjadi Faktur Diganti
                                            fp.StatusFaktur = objData.StatusFaktur;
                                            fp.StatusApproval = objData.StatusApproval;
                                            fp.TahunPajak = null;
                                            fp.MasaPajak = null;
                                            fp.Modified = DateTime.Now;
                                            fp.ModifiedBy = userName;
                                            FakturPajaks.Save(fp);

                                        }
                                    }
                                }
                            }
                            else
                            {
                                fp.KdJenisTransaksi = objData.KdJenisTransaksi;
                                fp.FgPengganti = objData.FgPengganti;
                                fp.NoFakturPajak = objData.NomorFaktur;
                                fp.NPWPPenjual = objData.NpwpPenjual;
                                fp.NamaPenjual = objData.NamaPenjual;
                                fp.AlamatPenjual = objData.AlamatPenjual;
                                fp.NPWPLawanTransaksi = objData.NpwpLawanTransaksi;
                                fp.NamaLawanTransaksi = objData.NamaLawanTransaksi;
                                fp.AlamatLawanTransaksi = objData.AlamatLawanTransaksi;
                                fp.JumlahDPP = Convert.ToDecimal(objData.JumlahDpp);
                                fp.JumlahPPN = Convert.ToDecimal(objData.JumlahPpn);
                                fp.JumlahPPNBM = Convert.ToDecimal(objData.JumlahPpnBm);
                                fp.StatusApproval = objData.StatusApproval;
                                fp.StatusFaktur = objData.StatusFaktur;
                                fp.Pesan = objData.StatusApproval;
                                fp.TglFaktur = DateTime.ParseExact(objData.TanggalFaktur.Trim(), "dd/MM/yyyy",
                                    CultureInfo.InvariantCulture);
                                fp.Status = (int)ApplicationEnums.StatusFakturPajak.Success;
                                fp.ModifiedBy = userName;
                                fp.Modified = DateTime.Now;

                                FakturPajaks.Save(fp);
                            }
                        }
                        else
                        {
                            fp.KdJenisTransaksi = objData.KdJenisTransaksi;
                            fp.FgPengganti = objData.FgPengganti;
                            fp.NoFakturPajak = objData.NomorFaktur;
                            fp.NPWPPenjual = objData.NpwpPenjual;
                            fp.NamaPenjual = objData.NamaPenjual;
                            fp.AlamatPenjual = objData.AlamatPenjual;
                            fp.NPWPLawanTransaksi = objData.NpwpLawanTransaksi;
                            fp.NamaLawanTransaksi = objData.NamaLawanTransaksi;
                            fp.AlamatLawanTransaksi = objData.AlamatLawanTransaksi;
                            fp.JumlahDPP = Convert.ToDecimal(objData.JumlahDpp);
                            fp.JumlahPPN = Convert.ToDecimal(objData.JumlahPpn);
                            fp.JumlahPPNBM = Convert.ToDecimal(objData.JumlahPpnBm);
                            fp.StatusApproval = objData.StatusApproval;
                            fp.StatusFaktur = objData.StatusFaktur;
                            fp.Pesan = objData.StatusApproval;
                            fp.TglFaktur = DateTime.ParseExact(objData.TanggalFaktur.Trim(), "dd/MM/yyyy",
                                CultureInfo.InvariantCulture);
                            fp.Status = (int)ApplicationEnums.StatusFakturPajak.Success;
                            fp.ModifiedBy = userName;
                            fp.Modified = DateTime.Now;

                            FakturPajaks.Save(fp);
                        }

                        //Delimite FakturPajakDetail by FakturPajakId
                        FakturPajakDetails.DeleteByFakturPajakId(fp.FakturPajakId, userName);


                        #region -------------- Save Detail Transaksi --------------
                        if (fp.FakturPajakId > 0 && objData.DetailTransaksi != null && objData.DetailTransaksi.Count > 0)
                        {
                            foreach (var dataitem in objData.DetailTransaksi)
                            {
                                var item = dataitem;
                                var fakturPajakDetail = new FakturPajakDetail()
                                {
                                    Nama = item.Nama,
                                    HargaSatuan = Convert.ToDecimal(item.HargaSatuan),
                                    JumlahBarang = Convert.ToDecimal(item.JumlahBarang),
                                    HargaTotal = Convert.ToDecimal(item.HargaTotal),
                                    Diskon = Convert.ToDecimal(item.Diskon),
                                    Dpp = Convert.ToDecimal(item.Dpp),
                                    Ppn = Convert.ToDecimal(item.Ppn),
                                    TarifPpnbm = Convert.ToDecimal(item.TarifPpnbm),
                                    Ppnbm = Convert.ToDecimal(item.Ppnbm),
                                    CreatedBy = userName,
                                    FakturPajakId = fp.FakturPajakId
                                };

                                FakturPajakDetails.Insert(fakturPajakDetail);
                            }


                        }

                        #endregion

                        //insert ke log untuk di exclude ketika diproses
                        var logfp = new LogFPDigantiOutstanding()
                        {
                            FormatedNoFaktur = fp.FormatedNoFaktur
                        };

                        LogFPDigantiOutstandings.Add(logfp);



                    }

                    eScope.Complete();
                    eScope.Dispose();

                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(out var outLogKey, LogLevel.Info, $"Terjadi error saat SaveToDBAsync: {ex.Message}", MethodBase.GetCurrentMethod());
                Console.WriteLine($"Terjadi error saat SaveToDBAsync: {ex.Message}");
                Console.WriteLine(outLogKey.ToString());
                throw ex;
            }

        }
        #region Tidak Dipakai
        //public static async Task SoftDeleteFakturDetailByFakturPajakId(int fakturPajakId, string modifiedBy)
        //{
        //    await _semaphoreSoftDeleteFPDetail.WaitAsync();
        //    try
        //    {
        //        FakturPajakDetails.DeleteByFakturPajakId(fakturPajakId, modifiedBy);

        //    }
        //    finally
        //    {
        //        _semaphore.Release(); // Melepaskan slot
        //    }
        //}
        //public static async Task SaveToDbAsync(DJPLib.Objects.ResValidateFakturPm objData, FakturPajak chkByUrl)
        //{
        //    DJPLibConfiguration.LoadConfig();
        //    const string userName = DJPLibConfiguration.Actor;
        //    using (var eScope = new TransactionScope())
        //    {
        //        //var fp = FakturPajaks.GetByUrlScan(URL);
        //        var fp = chkByUrl;
        //        //if (fp == null || fp.FakturPajakId == 0) return "Skip";
        //        if (fp.StatusFaktur != objData.StatusFaktur)
        //        {
        //            if (objData.StatusFaktur.ToLower() == "faktur diganti" || objData.StatusFaktur.ToLower() == "faktur dibatalkan")
        //            {
        //                if (fp.FgPengganti == "0")
        //                {

        //                    var fpoutcheck =
        //                        FakturPajakDigantiOutstandings.GetByFormatedNoFaktur(fp.FormatedNoFaktur);
        //                    if (fpoutcheck == null || fpoutcheck.Id == 0)
        //                    {
        //                        if (fp.TahunPajak.HasValue && fp.MasaPajak.HasValue &&
        //                            fp.TglFaktur.HasValue)
        //                        {
        //                            //belum ada
        //                            //Add to FP Diganti Outstanding
        //                            var tahunpajaktosave = fp.TahunPajak.Value;
        //                            var masapajaktosave = fp.MasaPajak.Value;
        //                            var tahunpajaktocheck = fp.TglFaktur.Value.Year;
        //                            var masapajaktocheck = fp.TglFaktur.Value.Month;

        //                            var fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Outstanding;

        //                            var dtmin =
        //                                new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
        //                                    DJPLibConfiguration.MaxPelaporan);
        //                            var dtmax =
        //                                new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
        //                                    Math.Abs(DJPLibConfiguration.MinPelaporan));
        //                            var availableperiods = OpenClosePeriods.GetByRange(dtmin, dtmax);
        //                            if (availableperiods.Count > 0)
        //                            {
        //                                //jika semua nya tidak ada yang open maka langsung expired
        //                                var chkopencloseperiod =
        //                                    availableperiods.Where(c => c.StatusRegular).ToList();
        //                                if (chkopencloseperiod.Count <= 0)
        //                                {
        //                                    fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Expired;
        //                                }
        //                            }
        //                            var fpoutstanding = new FakturPajakDigantiOutstanding()
        //                            {
        //                                Id = 0,
        //                                FormatedNoFaktur = fp.FormatedNoFaktur,
        //                                TahunPajak = tahunpajaktosave,
        //                                MasaPajak = masapajaktosave,
        //                                StatusApproval = objData.StatusApproval,
        //                                StatusFaktur = objData.StatusFaktur,
        //                                Keterangan = null,
        //                                KeteranganDjp = null,
        //                                StatusOutstanding = (int)fpoustandingstatus,
        //                                CreatedBy = userName
        //                            };

        //                            FakturPajakDigantiOutstandings.Save(fpoutstanding);

        //                            fp.MasaPajak = null;
        //                            fp.TahunPajak = null;
        //                            fp.ModifiedBy = userName;
        //                            fp.Modified = DateTime.Now;
        //                            FakturPajaks.Save(fp);
        //                        }
        //                    }

        //                }
        //                else
        //                {
        //                    //FP Normal Pengganti yang berubah Status Faktur menjadi Faktur Diganti
        //                    //get fp normal
        //                    var getfpnormal = FakturPajaks.GetFakturPajakNormal(fp.NoFakturPajak);
        //                    if (getfpnormal != null && getfpnormal.TglFaktur.HasValue &&
        //                        getfpnormal.TahunPajak.HasValue && getfpnormal.MasaPajak.HasValue)
        //                    {

        //                        var fpoutchek =
        //                            FakturPajakDigantiOutstandings.GetByFormatedNoFaktur(
        //                                getfpnormal.FormatedNoFaktur);
        //                        if (fpoutchek == null || fpoutchek.Id == 0)
        //                        {
        //                            var tahunpajaktocheck = getfpnormal.TglFaktur.Value.Year;
        //                            var masapajaktocheck = getfpnormal.TglFaktur.Value.Month;
        //                            var tahunpajaktosave = getfpnormal.TahunPajak.Value;
        //                            var masapajaktosave = getfpnormal.MasaPajak.Value;

        //                            var fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Outstanding;

        //                            var dtmin =
        //                                new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
        //                                    DJPLibConfiguration.MaxPelaporan);
        //                            var dtmax =
        //                                new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
        //                                    Math.Abs(DJPLibConfiguration.MinPelaporan));
        //                            var availableperiods = OpenClosePeriods.GetByRange(dtmin, dtmax);
        //                            if (availableperiods.Count > 0)
        //                            {
        //                                //jika semua nya tidak ada yang open maka langsung expired
        //                                var chkopencloseperiod =
        //                                    availableperiods.Where(c => c.StatusRegular).ToList();
        //                                if (chkopencloseperiod.Count <= 0)
        //                                {
        //                                    fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Expired;
        //                                }
        //                            }
        //                            var fpoutstanding = new FakturPajakDigantiOutstanding()
        //                            {
        //                                Id = 0,
        //                                FormatedNoFaktur = getfpnormal.FormatedNoFaktur,
        //                                TahunPajak = tahunpajaktosave,
        //                                MasaPajak = masapajaktosave,
        //                                StatusApproval = getfpnormal.StatusApproval,
        //                                StatusFaktur = getfpnormal.StatusFaktur,
        //                                Keterangan = null,
        //                                KeteranganDjp = null,
        //                                StatusOutstanding = (int)fpoustandingstatus,
        //                                CreatedBy = userName
        //                            };

        //                            FakturPajakDigantiOutstandings.Save(fpoutstanding);

        //                            getfpnormal.TahunPajak = null;
        //                            getfpnormal.MasaPajak = null;
        //                            getfpnormal.Modified = DateTime.Now;
        //                            getfpnormal.ModifiedBy = userName;

        //                            FakturPajaks.Save(getfpnormal);

        //                            //update Status Faktur FP Normal Pengganti sesuai dari DJP
        //                            fp.StatusFaktur = objData.StatusFaktur;
        //                            fp.StatusApproval = objData.StatusApproval;
        //                            fp.TahunPajak = null;
        //                            fp.MasaPajak = null;
        //                            fp.Modified = DateTime.Now;
        //                            fp.ModifiedBy = userName;
        //                            FakturPajaks.Save(fp);

        //                        }
        //                        else
        //                        {
        //                            //sudah ada
        //                            //dibalikin jadi outstanding atau expired lagi
        //                            var tahunpajaktocheck = getfpnormal.TglFaktur.Value.Year;
        //                            var masapajaktocheck = getfpnormal.TglFaktur.Value.Month;

        //                            var fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Outstanding;

        //                            var dtmin =
        //                                new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
        //                                    DJPLibConfiguration.MaxPelaporan);
        //                            var dtmax =
        //                                new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
        //                                    Math.Abs(DJPLibConfiguration.MinPelaporan));
        //                            var availableperiods = OpenClosePeriods.GetByRange(dtmin, dtmax);
        //                            if (availableperiods.Count > 0)
        //                            {
        //                                //jika semua nya tidak ada yang open maka langsung expired
        //                                var chkopencloseperiod =
        //                                    availableperiods.Where(c => c.StatusRegular).ToList();
        //                                if (chkopencloseperiod.Count <= 0)
        //                                {
        //                                    fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Expired;
        //                                }
        //                            }

        //                            fpoutchek.StatusOutstanding = (int)fpoustandingstatus;
        //                            fpoutchek.StatusApproval = getfpnormal.StatusApproval;
        //                            fpoutchek.StatusFaktur = getfpnormal.StatusFaktur;
        //                            fpoutchek.TahunPajak = getfpnormal.TahunPajak;
        //                            fpoutchek.MasaPajak = getfpnormal.MasaPajak;
        //                            fpoutchek.Modified = DateTime.Now;
        //                            fpoutchek.ModifiedBy = userName;

        //                            FakturPajakDigantiOutstandings.Save(fpoutchek);

        //                            //Reset Masa-Tahun Pajak FP Normal
        //                            getfpnormal.TahunPajak = null;
        //                            getfpnormal.MasaPajak = null;
        //                            getfpnormal.Modified = DateTime.Now;
        //                            getfpnormal.ModifiedBy = userName;

        //                            FakturPajaks.Save(getfpnormal);

        //                            //update Status Faktur FP Normal Pengganti sesuai dari DJP
        //                            //Reset Masa-Tahun Pajak FP Normal Pengganti yang sudah berubah menjadi Faktur Diganti
        //                            fp.StatusFaktur = objData.StatusFaktur;
        //                            fp.StatusApproval = objData.StatusApproval;
        //                            fp.TahunPajak = null;
        //                            fp.MasaPajak = null;
        //                            fp.Modified = DateTime.Now;
        //                            fp.ModifiedBy = userName;
        //                            FakturPajaks.Save(fp);

        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                fp.KdJenisTransaksi = objData.KdJenisTransaksi;
        //                fp.FgPengganti = objData.FgPengganti;
        //                fp.NoFakturPajak = objData.NomorFaktur;
        //                fp.NPWPPenjual = objData.NpwpPenjual;
        //                fp.NamaPenjual = objData.NamaPenjual;
        //                fp.AlamatPenjual = objData.AlamatPenjual;
        //                fp.NPWPLawanTransaksi = objData.NpwpLawanTransaksi;
        //                fp.NamaLawanTransaksi = objData.NamaLawanTransaksi;
        //                fp.AlamatLawanTransaksi = objData.AlamatLawanTransaksi;
        //                fp.JumlahDPP = Convert.ToDecimal(objData.JumlahDpp);
        //                fp.JumlahPPN = Convert.ToDecimal(objData.JumlahPpn);
        //                fp.JumlahPPNBM = Convert.ToDecimal(objData.JumlahPpnBm);
        //                fp.StatusApproval = objData.StatusApproval;
        //                fp.StatusFaktur = objData.StatusFaktur;
        //                fp.Pesan = objData.StatusApproval;
        //                fp.TglFaktur = DateTime.ParseExact(objData.TanggalFaktur.Trim(), "dd/MM/yyyy",
        //                    CultureInfo.InvariantCulture);
        //                fp.Status = (int)ApplicationEnums.StatusFakturPajak.Success;
        //                fp.ModifiedBy = userName;
        //                fp.Modified = DateTime.Now;

        //                FakturPajaks.Save(fp);
        //            }
        //        }
        //        else
        //        {
        //            fp.KdJenisTransaksi = objData.KdJenisTransaksi;
        //            fp.FgPengganti = objData.FgPengganti;
        //            fp.NoFakturPajak = objData.NomorFaktur;
        //            fp.NPWPPenjual = objData.NpwpPenjual;
        //            fp.NamaPenjual = objData.NamaPenjual;
        //            fp.AlamatPenjual = objData.AlamatPenjual;
        //            fp.NPWPLawanTransaksi = objData.NpwpLawanTransaksi;
        //            fp.NamaLawanTransaksi = objData.NamaLawanTransaksi;
        //            fp.AlamatLawanTransaksi = objData.AlamatLawanTransaksi;
        //            fp.JumlahDPP = Convert.ToDecimal(objData.JumlahDpp);
        //            fp.JumlahPPN = Convert.ToDecimal(objData.JumlahPpn);
        //            fp.JumlahPPNBM = Convert.ToDecimal(objData.JumlahPpnBm);
        //            fp.StatusApproval = objData.StatusApproval;
        //            fp.StatusFaktur = objData.StatusFaktur;
        //            fp.Pesan = objData.StatusApproval;
        //            fp.TglFaktur = DateTime.ParseExact(objData.TanggalFaktur.Trim(), "dd/MM/yyyy",
        //                CultureInfo.InvariantCulture);
        //            fp.Status = (int)ApplicationEnums.StatusFakturPajak.Success;
        //            fp.ModifiedBy = userName;
        //            fp.Modified = DateTime.Now;

        //            FakturPajaks.Save(fp);
        //        }

        //        //Delimite FakturPajakDetail by FakturPajakId
        //        FakturPajakDetails.DeleteByFakturPajakId(fp.FakturPajakId, userName);

        //        #region -------------- Save Detail Transaksi --------------
        //        if (fp.FakturPajakId > 0 && objData.DetailTransaksi != null && objData.DetailTransaksi.Count > 0)
        //        {
        //            foreach (var dataitem in objData.DetailTransaksi)
        //            {
        //                var item = dataitem;
        //                var fakturPajakDetail = new FakturPajakDetail()
        //                {
        //                    Nama = item.Nama,
        //                    HargaSatuan = Convert.ToDecimal(item.HargaSatuan),
        //                    JumlahBarang = Convert.ToDecimal(item.JumlahBarang),
        //                    HargaTotal = Convert.ToDecimal(item.HargaTotal),
        //                    Diskon = Convert.ToDecimal(item.Diskon),
        //                    Dpp = Convert.ToDecimal(item.Dpp),
        //                    Ppn = Convert.ToDecimal(item.Ppn),
        //                    TarifPpnbm = Convert.ToDecimal(item.TarifPpnbm),
        //                    Ppnbm = Convert.ToDecimal(item.Ppnbm),
        //                    CreatedBy = userName,
        //                    FakturPajakId = fp.FakturPajakId
        //                };

        //                FakturPajakDetails.Insert(fakturPajakDetail);
        //            }


        //        }

        //        #endregion

        //        //insert ke log untuk di exclude ketika diproses
        //        var logfp = new LogFPDigantiOutstanding()
        //        {
        //            FormatedNoFaktur = fp.FormatedNoFaktur
        //        };

        //        LogFPDigantiOutstandings.Add(logfp);


        //        eScope.Complete();
        //        eScope.Dispose();

        //    }

        //}
        #endregion
    }
}