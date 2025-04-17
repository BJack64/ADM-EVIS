using System;
using System.Collections.Generic;
using System.Reflection;
using System.Transactions;
using System.Web.Mvc;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Shared.Utility;
using eFakturADM.Web.Helpers;
using eFakturADM.Web.Models;
using System.Web;
using System.Linq;
using Microsoft.Reporting.WebForms;

namespace eFakturADM.Web.Controllers
{
    public class OrdnerController : BaseController
    {
        //
        // GET: /RequestFakturPajak/

        [AuthActivity("28")]
        public ActionResult Index()
        {
            return View("Index");
        }

        [AuthActivity("28")]
        public ActionResult ListOrdner()
        {
            return View("ListOrdner");
        }


        public JsonResult ViewOrdnerFakturPajak(string fakturPajakId)
        {
            var model = new ViewOrdnerFakturPajakModel()
            {
                FakturPajakId = Convert.ToInt32(fakturPajakId)
            };
            return Json(new
            {
                Html = this.RenderPartialView(@"ViewOrdnerFakturPajak", model),

            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetListLogPrintOrdner(long fakturPajakId)
        {
            var dats = LogPrintFakturPajaks.GetByFakturPajakIdViewReason(fakturPajakId);
            return Json(new
            {
                aaData = dats
            },
            JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetListOrdnerPrintDataTable(string firstLoad, string sEcho, int iDisplayStart, int iDisplayLength, 
            string NoFakturStart, string NoFakturEnd, string TglFakturStart, string TglFakturEnd, string BulanPajak, string TahunPajak, string PIC)
        {
            return GetListOrdner(firstLoad, sEcho, iDisplayStart, iDisplayLength, NoFakturStart, NoFakturEnd, TglFakturStart, TglFakturEnd, BulanPajak, TahunPajak, PIC);
        }

        [AuthActivity("29,30")]
        public JsonResult ValidationOnPrint(List<long> fakturPajakIds, bool isReprint)
        {

            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = string.Empty
            };

            if (fakturPajakIds.Count <= 0) return Json(new
            {
                Html = new RequestResultModel()
                {
                    InfoType = RequestResultInfoType.Warning,
                    Message = "No Data Selected"
                }
            }, JsonRequestBehavior.AllowGet);

            var getConfigDefaultPrintOrdner =
                GeneralConfigs.GetById((int) ApplicationEnums.GeneralConfig.DefaultPrintOrdner);

            if (getConfigDefaultPrintOrdner == null)
            {
                return Json(new
                {
                    Html = new RequestResultModel()
                    {
                        InfoType = RequestResultInfoType.Warning,
                        Message = "General Config Data [DefaultPrintOrdner] not found"
                    }
                }, JsonRequestBehavior.AllowGet);
            }

            var fpIds = string.Join(",", fakturPajakIds);

            var chkData = FakturPajakPrintCounts.GetByFaktuPajakIds(fpIds, ApplicationEnums.LogPrintType.Ordner);

            if (isReprint)
            {
                //Re-Print
                var chkIsFirstPrint = chkData.Where(c => c.PrintCount < Convert.ToInt32(getConfigDefaultPrintOrdner.ConfigValue)).ToList();
                if (chkIsFirstPrint.Count > 0)
                {
                    model.InfoType = RequestResultInfoType.Warning;
                    model.Message = "Default Print untuk No Faktur Pajak : " + string.Join(",", chkIsFirstPrint.Select(d => d.NoFakturPajak)) + " adalah " + getConfigDefaultPrintOrdner.ConfigValue + " kali. <br />Silahkan pilih tombol Print untuk No Faktur Pajak tersebut.";
                }
            }
            else
            {
                //Print
                var chkIsRePrint = chkData.Where(c => c.PrintCount >= Convert.ToInt32(getConfigDefaultPrintOrdner.ConfigValue)).ToList();
                if (chkIsRePrint.Count <= 0) return Json(new {Html = model}, JsonRequestBehavior.AllowGet);
                model.InfoType = RequestResultInfoType.Warning;
                model.Message = "Default Print untuk No Faktur Pajak : " + string.Join(",", chkIsRePrint.Select(d => d.NoFakturPajak)) + " adalah " + getConfigDefaultPrintOrdner.ConfigValue + " kali. <br />Silahkan pilih tombol Re-Print untuk No Faktur Pajak tersebut.";
            }

            return Json(new {Html = model}, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateOrdner(List<FakturPajakInfoModel> ListFakturPajakId)
        {
            var model = new RequestResultModel
            {
                InfoType = RequestResultInfoType.Success,
                Message = ""
            };

            string listFP = string.Join(",", ListFakturPajakId.Select(d => d.FakturPajakId));

            if (listFP != "")
            {
                try
                {
                    var userNameLogin = Session["UserName"].ToString();
                    using (var eScope = new TransactionScope())
                    {
                        string errMessageFromProcess;
                        var processResult = FillingIndexs.CreateFillingIndex(listFP, userNameLogin, out errMessageFromProcess);
                        if (!processResult)
                        {
                            model.InfoType = RequestResultInfoType.Warning;
                            model.Message = errMessageFromProcess;
                        }
                        else
                        {
                            var msg = new List<string>() { "Create Ordner Succeed" };
                            model.InfoType = RequestResultInfoType.Success;
                            model.Message = string.Join("<br />", msg.Select(d => d));
                            eScope.Complete();
                        }
                        
                        eScope.Dispose();
                    }
                }
                catch
                {
                    model.InfoType = RequestResultInfoType.Warning;
                    model.Message = "Create Ordner Failed";
                }
            }

            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReasonDialog(List<long> fakturPajakIds, List<string> fillingIndexs)
        {
            var fpIds = string.Join(",", fakturPajakIds);
            var lstIds = string.Join(",", fillingIndexs);
            return Json(new
            {
                Html = this.RenderPartialView(@"GetReasonDialog", new OrdnerGetReasonDialogModel (){ Ids = fpIds, FillingIndexs = lstIds }),

            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RePrintOrdner(string fakturPajakIds, string fillingIndexs, string reason)
        {

            var lstIds = fillingIndexs.Split(',').ToList();

            if (lstIds.Count <= 0) return Json(new
            {
                Html = new RequestResultModel()
                {
                    InfoType = RequestResultInfoType.Warning,
                    Message = "No Data Selected"
                }
            }, JsonRequestBehavior.AllowGet);

            var lstCleanIds = lstIds.Where(c => !string.IsNullOrEmpty(c)).ToList();

            if (lstCleanIds.Count <= 0) return Json(new
            {
                Html = new RequestResultModel()
                {
                    InfoType = RequestResultInfoType.Warning,
                    Message = "No Data Selected"
                }
            }, JsonRequestBehavior.AllowGet);

            return DoPrintOrdner(fakturPajakIds, lstIds, reason);
        }

        public JsonResult PrintOrdner(List<long> fakturPajakIds, List<string> fillingIndexs)
        {
            var fIds = string.Join(",", fakturPajakIds);
            return DoPrintOrdner(fIds, fillingIndexs, "");
        }

        #region --------------- Private Methods --------------

        private JsonResult GetListOrdner(string firstLoad, string sEcho, int iDisplayStart, int iDisplayLength, string NoFakturStart, 
            string NoFakturEnd, string TglFakturStart, 
            string TglFakturEnd, string BulanPajak, string TahunPajak, string picEntry)
        {
            if (Convert.ToBoolean(firstLoad))
            {
                return Json(new
                {
                    sEcho = sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = new List<Ordner>()
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
                case 11: filter.SortColumnName = "StatusAPproval"; break;
            }
            int totalItems;

            DateTime? TanggalFakturStart = string.IsNullOrEmpty(TglFakturStart) || TglFakturStart == "undefine"
                ? (DateTime?) null
                : Convert.ToDateTime(TglFakturStart);
            DateTime? TanggalFakturEnd = string.IsNullOrEmpty(TglFakturEnd) || TglFakturEnd == "undefine" 
                ? (DateTime?) null : Convert.ToDateTime(TglFakturEnd);

            int? iBulanPajak = string.IsNullOrEmpty(BulanPajak) || BulanPajak == "undefine" || BulanPajak == "0"
                ? (int?) null
                : Convert.ToInt32(BulanPajak);
            int? iTahunPajak = string.IsNullOrEmpty(TahunPajak) || TahunPajak == "undefine" || TahunPajak == "0"
                ? (int?) null
                : Convert.ToInt32(TahunPajak);

            List<Ordner> listFakturPajak = Ordners.GetList(filter, out totalItems, NoFakturStart, NoFakturEnd,
                TanggalFakturStart, TanggalFakturEnd, iBulanPajak, iTahunPajak, picEntry);

            return Json(new
            {
                sEcho = sEcho,
                iTotalRecords = totalItems,
                iTotalDisplayRecords = totalItems,
                aaData = listFakturPajak
            },
            JsonRequestBehavior.AllowGet);
        }

        private JsonResult DoPrintOrdner(string fakturPajakIds, List<string> fillingIndexs, string reason)
        {
            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = "Print on Progress"
            };

            var lstIds = fakturPajakIds.Split(',').ToList();

            if (lstIds.Count <= 0) return Json(new
            {
                Html = new RequestResultModel()
                {
                    InfoType = RequestResultInfoType.Warning,
                    Message = "No Data Selected"
                }
            }, JsonRequestBehavior.AllowGet);

            var lstCleanIds = lstIds.Where(c => !string.IsNullOrEmpty(c)).ToList();

            if (lstCleanIds.Count <= 0) return Json(new
            {
                Html = new RequestResultModel()
                {
                    InfoType = RequestResultInfoType.Warning,
                    Message = "No Data Selected"
                }
            }, JsonRequestBehavior.AllowGet);

            var userNameLogin = Session["UserName"].ToString();
            var listFp = string.Join(",", lstCleanIds);
            var idSession = "";
            try
            {

                LogPrintFakturPajaks.BulkInsert(listFp, EnumHelper.GetDescription(ApplicationEnums.LogPrintType.Ordner), userNameLogin, reason, userNameLogin);
                idSession = Guid.NewGuid().ToString();
                Session[idSession] = fillingIndexs;
            }
            catch (Exception ex)
            {
                //Log.WriteLog(ex.Message, MethodBase.GetCurrentMethod());
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, ex.Message, MethodBase.GetCurrentMethod());
                model.InfoType = RequestResultInfoType.Warning;
                model.Message = "Print Ordner Failed. See Log with Key " + logKey + " for details.";
            }

            return Json(new { Html = model, Id = idSession }, JsonRequestBehavior.AllowGet);
        }
        
        #endregion

        public ActionResult GetPrintOut(string idprint)
        {
            if (string.IsNullOrEmpty(idprint))
            {
                return RedirectToAction("Http404", "Errors");
            }
            try
            {
                var ordnerList = Session[idprint] as List<string>;
                if (ordnerList == null)
                {
                    return RedirectToAction("Http404", "Errors");
                }
                var ds = new List<PrintOrdner>();
                int id = 0;
                foreach (var it in ordnerList)
                {
                    ds.Add(new PrintOrdner()
                    {
                        FakturPajakId = id,
                        FillingIndex = it
                    });

                    id++;

                }
                var localReport = new LocalReport();
                localReport.ReportPath = Server.MapPath("~/bin/Reports/PrintOrdner.rdlc");
                var reportDataSource = new ReportDataSource("dsPrintPrintOrdners", ds);
                
                localReport.DataSources.Add(reportDataSource);

                Session[idprint] = null;

                string reportType = "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;

                //The DeviceInfo settings should be changed based on the reportType
                //http://msdn.microsoft.com/en-us/library/ms155397.aspx
                string deviceInfo =
                "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>8.27in</PageWidth>" +
                "  <PageHeight>11.69in</PageHeight>" +
                "  <MarginTop>0.3in</MarginTop>" +
                "  <MarginLeft>0in</MarginLeft>" +
                "  <MarginRight>0in</MarginRight>" +
                "  <MarginBottom>0in</MarginBottom>" +
                "</DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;

                //Render the report
                renderedBytes = localReport.Render(
                    reportType,
                    deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);
                //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension);
                return File(renderedBytes, mimeType);

            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                throw;
            }
            

        }

    }
}
