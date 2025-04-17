using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Transactions;
using System.Web.Mvc;
using eFakturADM.Logic.Collections;
using System.Web;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Shared.Utility;
using eFakturADM.Web.Helpers;
using eFakturADM.Web.Models;
using System.Globalization;

namespace eFakturADM.Web.Controllers
{
    public class RequestFakturPajakController : BaseController
    {
        //
        // GET: /RequestFakturPajak/

        [AuthActivity("13")]
        public ActionResult Index()
        {
            return View("ListRequestFakturPajak");
        }

        public JsonResult GetListRequestFakturPajakDataTable(string firstLoad, string sEcho, int iDisplayStart, int iDisplayLength, string NoFaktur1, string NoFaktur2,
            string TglFakturStart, string TglFakturEnd, string MasaPajak, string TahunPajak)
        {
            if (Convert.ToBoolean(firstLoad))
            {
                return Json(new
                {
                    sEcho = sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = new List<FakturPajak>()
                },
            JsonRequestBehavior.AllowGet);
            }
            var filter = new Logic.Utilities.Filter
            {
                CurrentPage = (iDisplayStart / iDisplayLength) + 1,
                ItemsPerPage = iDisplayLength,
                SortOrderAsc = Request["sSortDir_0"] == "desc",
                SortColumn = Convert.ToInt32(Request["iSortCol_0"]),
                Search = HttpUtility.UrlDecode(Request["sSearch"]),
                SortColumnName = "NoFakturPajak",
            };

            switch (Convert.ToInt32(Request["iSortCol_0"]))
            {
                case 0: filter.SortColumnName = "NoFakturPajak"; break;
                case 1: filter.SortColumnName = "UrlScan"; break;
                case 2: filter.SortColumnName = "NoFakturPajak"; break;
                case 3: filter.SortColumnName = "TglFakturString"; break;
                case 4: filter.SortColumnName = "NPWPLawanTransaksi"; break;
                case 5: filter.SortColumnName = "NamaLawanTransaksi"; break;
                case 6: filter.SortColumnName = "StatusFaktur"; break;
            }
            int totalItems;
            DateTime? dtTanggalFakturStart = null;
            if (!string.IsNullOrEmpty(TglFakturStart))
            {
                dtTanggalFakturStart = Convert.ToDateTime(TglFakturStart);
            }
            DateTime? dtTanggalFakturEnd = null;
            if (!string.IsNullOrEmpty(TglFakturEnd))
            {
                dtTanggalFakturEnd = Convert.ToDateTime(TglFakturEnd);
            }
            int? iMasaPajak = null;
            if (!string.IsNullOrEmpty(MasaPajak) && MasaPajak != "0")
            {
                iMasaPajak = Convert.ToInt32(MasaPajak);
            }
            int? iTahunPajak = null;
            if (!string.IsNullOrEmpty(TahunPajak))
            {
                iTahunPajak = Convert.ToInt32(TahunPajak);
            }
            var lstFp = FakturPajaks.GetListRequestFakturPajak(filter, out totalItems, NoFaktur1, NoFaktur2, dtTanggalFakturStart, dtTanggalFakturEnd, string.Empty, string.Empty,
                iMasaPajak, iTahunPajak, string.Empty);

            return Json(new
            {
                sEcho = sEcho,
                iTotalRecords = totalItems,
                iTotalDisplayRecords = totalItems,
                aaData = lstFp
            },
            JsonRequestBehavior.AllowGet);
        }

        #region ------------------- Process Send Request Detail Transaksi Faktur Pajak ----------------------

        [AuthActivity("14,15")]
        public JsonResult SendRequestFakturPajak(List<FakturPajakInfoModel> fakturPajakIds, bool isAllDetail)
        {
            if (fakturPajakIds.Count <= 0)
            {
                return Json(new
                {
                    Html = new RequestResultModel()
                    {
                        InfoType = RequestResultInfoType.Warning,
                        Message = "No Data Selected"
                    }
                }, JsonRequestBehavior.DenyGet);
            }

            var timeOutSetting =
                    GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.DJPRequestTimeOutSetting);

            int iTimeOutSetting;

            if (timeOutSetting == null)
            {
                return Json(new
                {
                    Html = new RequestResultModel()
                    {
                        InfoType = RequestResultInfoType.Warning,
                        Message = "Config Data not found for [DJPRequestTimeOutSetting]"
                    }
                }, JsonRequestBehavior.DenyGet);
            }

            if (!int.TryParse(timeOutSetting.ConfigValue, out iTimeOutSetting))
            {
                return Json(new
                {
                    Html = new RequestResultModel()
                    {
                        InfoType = RequestResultInfoType.Warning,
                        Message = "Invalid Value Config Data for [DJPRequestTimeOutSetting]"
                    }
                }, JsonRequestBehavior.DenyGet);
            }

            var ids = string.Join(",", fakturPajakIds.Select(d => d.FakturPajakId));
            //var ids = string.Join(",", fakturPajakIds);
            var fakturPajakToProcess = FakturPajaks.GetByIds(ids);

            var processResult = ProcessGetDetailTransaksiFakturPajak(fakturPajakToProcess, iTimeOutSetting, isAllDetail);

            if (processResult.Count > 0)
            {
                //Write Log for Details
                string logKey;
                var msgToLog = string.Join(Environment.NewLine,
                    processResult.Select(
                        d => d.Message + Environment.NewLine + "Exception : " + d.ExceptionDetails.Message + 
                            Environment.NewLine + "StackTrace : " + d.ExceptionDetails.StackTrace));
                Logger.WriteLog(out logKey, LogLevel.Error, msgToLog, MethodBase.GetCurrentMethod());
                return Json(new
                {
                    Html = new RequestResultModel()
                    {
                        InfoType = RequestResultInfoType.Warning,
                        Message = string.Join("<br />", processResult.Select(d => d.Message))
                    }
                }, JsonRequestBehavior.DenyGet);
            }

            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = "Send Request Detail Transaksi Faktur Pajak Succeed"
            };

            return Json(new { Html = model }, JsonRequestBehavior.DenyGet);

        }

        private List<RequestFakturPajakResultModel> ProcessGetDetailTransaksiFakturPajak(IEnumerable<FakturPajak> fakturPajakToProcess, int iTimeOutSetting, bool isAllDetail)
        {
            var msgs = new List<RequestFakturPajakResultModel>();
            var isAnyTimeoutError = false;
            string urlScan = string.Empty;
            string logKeyError = string.Empty;

            var isUseProxy = false;
            var inetProxy = WebConfiguration.InternetProxy;
            var inetProxyPort = WebConfiguration.InternetProxyPort;
            var inetProxyUseCredential = WebConfiguration.UseDefaultCredential;
            
            if (Environment.MachineName != "LAPTOP-FG63PUP7")
            {
                isUseProxy = true;
            }

            //if (!string.IsNullOrEmpty(inetProxy) || inetProxyPort.HasValue || inetProxyUseCredential.HasValue)
            //{
            //    isUseProxy = true;
            //}
            
            foreach (var fakturPajak in fakturPajakToProcess)
            {
                try
                {
                    WebExceptionStatus eStatus;
                    string msgError;
                    string logKey;
                    var objXml = DJPLib.ValidateFakturLib.GetValidateFakturObjectV3(fakturPajak.UrlScan,
                        iTimeOutSetting, isUseProxy, inetProxy, inetProxyPort, inetProxyUseCredential, out msgError, out eStatus, out logKey);

                    if (eStatus != WebExceptionStatus.Success)
                    {
                        if (eStatus == WebExceptionStatus.Timeout)
                        {
                            isAnyTimeoutError = true;
                            urlScan = fakturPajak.UrlScan;
                            logKeyError = logKey;
                        }
                        msgs.Add(new RequestFakturPajakResultModel() { Message = msgError, ExceptionDetails = null });
                    }
                    else
                    {
                        Exception ex;
                        var rest = SaveDetailTransaksiToDatabase(objXml, fakturPajak, isAllDetail, out ex);
                        if (!string.IsNullOrEmpty(rest))
                        {
                            msgs.Add(new RequestFakturPajakResultModel() { Message = rest, ExceptionDetails = ex });
                        }
                    }
                }
                catch (Exception exception)
                {
                    msgs.Add(new RequestFakturPajakResultModel() { 
                        Message = "Error Getting Request Detail Transaksi Faktur Pajak for Url : " + fakturPajak.UrlScan, 
                        ExceptionDetails = exception });
                }
            }
            if (isAnyTimeoutError)
            {
                //Send email notification
                var mh = new MailHelper();
                bool isErrorSendMail;
                mh.DjpRequestErrorSendMail(out isErrorSendMail, urlScan, logKeyError);
                if (isErrorSendMail)
                {
                    msgs.Add(new RequestFakturPajakResultModel()
                    {
                        Message = "Error Send Email Notification",
                        ExceptionDetails = null
                    });
                }
            }
            return msgs;
        }

        private string SaveDetailTransaksiToDatabase(DJPLib.Objects.ResValidateFakturPm objData, FakturPajak fp, bool isAllDetail, out Exception ex)
        {
            ex = null;
            var msgs = string.Empty;
            var userName = Session["UserName"] as string;
            try
            {
                using (var eScope = new TransactionScope())
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
                    fp.Status = (int) ApplicationEnums.StatusFakturPajak.Success;
                    fp.ModifiedBy = userName;

                    var masterResult = FakturPajaks.Save(fp);

                    //Delimite FakturPajakDetail by FakturPajakId
                    FakturPajakDetails.DeleteByFakturPajakId(masterResult.FakturPajakId, userName);

                    #region -------------- Save Detail Transaksi --------------
                    if (masterResult.FakturPajakId > 0 && objData.DetailTransaksi != null && objData.DetailTransaksi.Count > 0)
                    {
                        if (isAllDetail)
                        {
                            foreach (var item in objData.DetailTransaksi)
                            {
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
                                    FakturPajakId = masterResult.FakturPajakId
                                };

                                FakturPajakDetails.Insert(fakturPajakDetail);
                            }
                        }
                        else
                        {
                            var item = objData.DetailTransaksi.First();
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
                                FakturPajakId = masterResult.FakturPajakId
                            };

                            FakturPajakDetails.Insert(fakturPajakDetail);
                        }

                    }

                    #endregion

                    eScope.Complete();
                    eScope.Dispose();

                }
            }
            catch (Exception exception)
            {
                ex = exception;
                msgs = "Save to Database Failed for Url : " + fp.UrlScan;
            }

            return msgs;
        }

        #endregion

    }

}

