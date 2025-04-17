using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Shared.Utility;
using eFakturADM.Web.Helpers;
using eFakturADM.Web.Models;
using System.Web;
using System.Linq;
using eFakturADM.FileManager;

namespace eFakturADM.Web.Controllers
{
    public class FpDigantiOutstandingController : BaseController
    {
        //
        // GET: /FpDigantiOutstanding/
        [AuthActivity("63")]
        public ActionResult Index()
        {
            return View();
        }

        [AuthActivity("64")]
        public JsonResult Upload(string accessForm)
        {
            return Json(new
            {
                Html = this.RenderPartialView(@"Upload", accessForm)
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthActivity("64")]
        public JsonResult ProcessUpload(string sEcho)
        {
            HttpPostedFileBase file = Request.Files["file-fpdigantioutstanding-upload"];
            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = "Upload FP Diganti Outstanding Success"
            };

            var getTimeOutSetting = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.DJPRequestTimeOutSetting);

            if (getTimeOutSetting == null)
            {
                const string msgError = "Config Data not found for [DJPRequestTimeOutSetting]";
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, msgError, MethodBase.GetCurrentMethod());
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = msgError;
                return Json(model, JsonRequestBehavior.AllowGet);
            }

            int timeOutSettingInt;

            if (!int.TryParse(getTimeOutSetting.ConfigValue, out timeOutSettingInt))
            {
                const string msgError = "Invalid value Config Data [DJPRequestTimeOutSetting]";
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, msgError, MethodBase.GetCurrentMethod());
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = msgError;
                return Json(model, JsonRequestBehavior.AllowGet);
            }

            var userNameLogin = Session["UserName"] as string;

            var isUseProxy = false;
            var inetProxy = WebConfiguration.InternetProxy;
            var inetProxyPort = WebConfiguration.InternetProxyPort;
            var inetProxyUseCredential = WebConfiguration.UseDefaultCredential;
            if (!string.IsNullOrEmpty(inetProxy) || inetProxyPort.HasValue || inetProxyUseCredential.HasValue)
            {
                isUseProxy = true;
            }

            var uploadResult = UploadFileManager.UploadFpDigantiOutstanding(file, userNameLogin, isUseProxy, inetProxy,
                inetProxyPort, inetProxyUseCredential, timeOutSettingInt);

            if (uploadResult.InfoType == CommonOutputType.ErrorWithFileDownload)
            {
                model.InfoType = RequestResultInfoType.ErrorWithFileDownload;
                model.Message = "<a href='" +
                                        Url.Action("DownloadFile", "ExportDownload",
                                            new
                                            {
                                                type = EnumHelper.GetDescription(ApplicationEnums.DownloadFolderType.Upload),
                                                fileFolder = EnumHelper.GetDescription(ApplicationEnums.DownloadFileType.FpDigantiOutstanding),
                                                fileName = uploadResult.FilePath
                                            }) +
                                        "' target='_blank'>Error Validasi</a>";
            }
            else if (uploadResult.InfoType == CommonOutputType.Error)
            {
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = uploadResult.MessageInfo;
            }
            
            return Json(new
            {
                Html = model
            }, "text/html");
        }

        [HttpGet]
        [AuthActivity("64")]
        public JsonResult GetListFakturPajakDigantiOutstandingDataTable(string firstLoad, string sEcho, int iDisplayStart, int iDisplayLength, string NoFaktur1, string NoFaktur2, string NPWP,
            string Nama, string TglFakturStart, string TglFakturEnd, string Status,
            string dataType, string scanDateAwal, string scanDateAkhir, string fillingIndex, string sSearch_1,
            string sSearch_2, string sSearch_3, string sSearch_4, string sSearch_5, string sSearch_6, string sSearch_7, string sSearch_8, string sSearch_9, string sSearch_10
            , string sSearch_11, string sSearch_12)
        {
            if (Convert.ToBoolean(firstLoad))
            {
                return Json(new
                {
                    sEcho = sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = new List<FakturPajakDigantiOutstanding>()
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
                SortColumnName = "NPWPLawanTransaksi"
            };

            switch (Convert.ToInt32(Request["iSortCol_0"]))
            {
                case 1: filter.SortColumnName = "FormatedNpwpPenjual"; break;
                case 2: filter.SortColumnName = "NamaPenjual"; break;
                case 3: filter.SortColumnName = "FormatedNoFaktur"; break;
                case 4: filter.SortColumnName = "TglFaktur"; break;
                case 5: filter.SortColumnName = "MasaPajak"; break;
                case 6: filter.SortColumnName = "TahunPajak"; break;
                case 7: filter.SortColumnName = "JumlahDPP"; break;
                case 8: filter.SortColumnName = "JumlahPPN"; break;
                case 9: filter.SortColumnName = "JumlahPPNBM"; break;
                case 10: filter.SortColumnName = "FillingIndex"; break;
                case 11: filter.SortColumnName = "StatusOutstanding"; break;
                case 12: filter.SortColumnName = "CreatedBy"; break;
                case 13: filter.SortColumnName = "Keterangan"; break;
            }

            string _FormatedNpwpPenjual = sSearch_1;
            string _NamaPenjual = sSearch_2;
            string _FormatedNoFaktur = sSearch_3;
            string _TglFakturString = sSearch_4;
            string _MasaPajakName = sSearch_5;
            string _TahunPajak = sSearch_6;
            string _DPPString = sSearch_7;
            string _PPNString = sSearch_8;
            string _PPNBMString = sSearch_9;
            string _SFillingIndex = sSearch_10;
            string _StatusFaktur = sSearch_11;
            string _SUserName = sSearch_12;

            int totalItems;

            DateTime? tglFakturStart = !string.IsNullOrEmpty(TglFakturStart) ? Convert.ToDateTime(TglFakturStart) : (DateTime?)null;
            DateTime? tglFakturEnd = !string.IsNullOrEmpty(TglFakturEnd) ? Convert.ToDateTime(TglFakturEnd) : (DateTime?)null;

            int? idataType = null;
            DateTime? dscanDateAwal = null;
            DateTime? dscanDateAkhir = null;
            string ifillingIndex = null;
            int? iStatus = null;
            if (!string.IsNullOrEmpty(dataType) && dataType != "undefined")
            {
                idataType = int.Parse(dataType);
            }

            if (!string.IsNullOrEmpty(scanDateAwal) && scanDateAwal != "undefined")
            {
                dscanDateAwal = Convert.ToDateTime(scanDateAwal);
            }

            if (!string.IsNullOrEmpty(scanDateAkhir) && scanDateAkhir != "undefined")
            {
                dscanDateAkhir = Convert.ToDateTime(scanDateAkhir);
            }

            if (!string.IsNullOrEmpty(fillingIndex) && fillingIndex != "undefinded")
            {
                ifillingIndex = fillingIndex;
            }
            if (Status != "")
            {
                iStatus = Int32.Parse(Status);
            }
            //int? masaPajak = string.IsNullOrEmpty(MasaPajak) || MasaPajak == "0" ? (int?)null : int.Parse(MasaPajak);
            //int? tahunPajak = string.IsNullOrEmpty(TahunPajak) || TahunPajak == "0" ? (int?)null : int.Parse(TahunPajak);
            string logKey;
            Logger.WriteLog(out logKey, LogLevel.Error, "Start Get FP", MethodBase.GetCurrentMethod());
            List<FakturPajakDigantiOutstanding> listFakturPajak = FakturPajakDigantiOutstandings.GetList(filter, out totalItems, NoFaktur1, NoFaktur2, tglFakturStart, tglFakturEnd, NPWP,
                Nama, iStatus, _FormatedNpwpPenjual, _NamaPenjual, _FormatedNoFaktur, _TglFakturString, _MasaPajakName, _TahunPajak, _DPPString, _PPNString,
                _PPNBMString, _StatusFaktur, idataType, dscanDateAwal, dscanDateAkhir, ifillingIndex, _SFillingIndex, _SUserName).ToList();
            Logger.WriteLog(out logKey, LogLevel.Error, "End Get FP : " + totalItems + " records", MethodBase.GetCurrentMethod());
            return Json(new
            {
                sEcho = sEcho,
                iTotalRecords = totalItems,
                iTotalDisplayRecords = totalItems,
                aaData = listFakturPajak
            },
            JsonRequestBehavior.AllowGet);
        }
        [AuthActivity("64")]
        public JsonResult SetRemark(long id)
        {
            return Json(new
            {
                Html = this.RenderPartialView(@"Remark", new FpDigantiOutstandingModel()
                {
                    ID = id
                }),

            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FPDSSaveRemark(long ID, string remarks)
        {
            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = "Save Remarks Successfully"
            };

            if (string.IsNullOrEmpty(remarks))
            {
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = "Input Remarks is required";
                return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var userNameLogin = Session["UserName"].ToString();
                FakturPajakDigantiOutstandings.UpdateKeterangan(ID, remarks, userNameLogin);
            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = "Save Remarks failed. See Log with key " + logKey + " for details.";
            }
            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }


        [AuthActivity("64")]
        public JsonResult SetMultipleRemarks(List<long> fakturPajakDigantiOutstandingIds)
        {
            string FakturPajakDigantiOutstandingIDs = null;
            foreach (var item in fakturPajakDigantiOutstandingIds)
            {
                FakturPajakDigantiOutstandingIDs += item.ToString() + ',';
            }
            return Json(new
            {
                Html = this.RenderPartialView(@"MultipleRemarks", new FpDigantiOutstandingModel()
                {
                    OutstandingfakturPajakIDs = FakturPajakDigantiOutstandingIDs
                }),

            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FPDSSaveRemarks(string IDs, string remarks)
        {
            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = "Save Remarks Successfully"
            };

            if (string.IsNullOrEmpty(remarks))
            {
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = "Input Remarks is required";
                return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var userNameLogin = Session["UserName"].ToString();
                FakturPajakDigantiOutstandings.UpdateKeteranganByIds(IDs, remarks, userNameLogin);
            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = "Save Remarks failed. See Log with key " + logKey + " for details.";
            }
            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }
    }
}
