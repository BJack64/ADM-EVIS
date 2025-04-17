using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using eFakturADM.FileManager;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Logic.Collections;
using eFakturADM.Web.Models;
using eFakturADM.Web.Helpers;

namespace eFakturADM.Web.Controllers
{
    public class LogMonitoringController : BaseController
    {
        [AuthActivity("36")]
        public ActionResult LogSap()
        {
            //try
            //{
            //    var objR = GetOutputUploadPPNCredit("8e07a959-f8ea-428d-af46-2a2c1781920b");
            //}
            //catch (Exception ex)
            //{

            //}

            return View("LogSAP");
        }

        public JsonResult GetListLogSAPDataTable(string firstLoad, string sEcho, int iDisplayStart, int iDisplayLength, string Status, string TanggalRekam)
        {
            if (Convert.ToBoolean(firstLoad))
            {
                return Json(new
                {
                    sEcho = sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = new List<LogSap>()
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
                SortColumnName = "Created"
            };

            switch (Convert.ToInt32(Request["iSortCol_0"]))
            {
                case 2: filter.SortColumnName = "FileName"; break;
                case 3: filter.SortColumnName = "Created"; break;
                case 5: filter.SortColumnName = "Note"; break;
            }

            int totalItems;

            DateTime? tglRekam = null;
            if (!string.IsNullOrEmpty(TanggalRekam))
            {
                tglRekam = Convert.ToDateTime(TanggalRekam);
            }

            int? statusValue = null;
            if (!string.IsNullOrEmpty(Status))
            {
                statusValue = Convert.ToInt32(Status);
            }

            List<LogSap> listSAP = LogSaps.GetList(filter, out totalItems, statusValue, tglRekam).ToList();
            return Json(new
            {
                sEcho = sEcho,
                iTotalRecords = totalItems,
                iTotalDisplayRecords = totalItems,
                aaData = listSAP
            },
            JsonRequestBehavior.AllowGet);
        }

        [AuthActivity("37")]
        public JsonResult ValidationRetryFileTransfer(List<long> logSapId)
        {

            var validationResult = PrivValidationRetryFileTransfer(logSapId);
            if (validationResult.InfoType != RequestResultInfoType.Success)
                return Json(new { Html = validationResult }, JsonRequestBehavior.AllowGet);

            return Json(new { Html = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = string.Empty
            } }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RetryFileTransfer(List<long> logSapId)
        {

            var validationResult = PrivValidationRetryFileTransfer(logSapId);
            if (validationResult.InfoType != RequestResultInfoType.Success)
                return Json(new { Html = validationResult }, JsonRequestBehavior.AllowGet);

            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = "Retry File Transfer sukses"
            };

            var userName = Session["UserName"] as string;

            var tProc = ExportFileManager.XmlRetryFileTransferToSap(logSapId, userName);

            if (tProc.InfoType != CommonOutputType.Success)
            {
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = tProc.MessageInfo;
            }
            else
            {
                if (!string.IsNullOrEmpty(tProc.MessageInfo))
                {
                    model.Message = tProc.MessageInfo;
                }
            }

            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }

        private RequestResultModel PrivValidationRetryFileTransfer(List<long> logSapIds)
        {
            if (logSapIds.Count <= 0)
            {
                return new RequestResultModel()
                {
                    InfoType = RequestResultInfoType.ErrorOrDanger,
                    Message = "No Data"
                };
            }

            var msgs = (from id in logSapIds select LogSaps.GetById(id) into chkDb where chkDb != null where chkDb.Status == (int)ApplicationEnums.SapStatusLog.Success select "File " + chkDb.FileName + " already success file transfer.").ToList();

            if (msgs.Count > 0)
            {
                return new RequestResultModel()
                {
                    InfoType = RequestResultInfoType.ErrorOrDanger,
                    Message = string.Join("<br />", msgs)
                };
            }
            return new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = ""
            };

        }


        public JsonResult ViewLogSAPDetail(string IdNo)
        {
            var model = new ViewLogSAPInfoModel()
            {
                IdNo = IdNo
            };
            return Json(new
            {
                Html = this.RenderPartialView(@"ViewLogSAP", model),

            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetOutputUploadPPNCredit(string sEcho, string idNo, string filterstatus)
        {
            var logProcSap = LogProcessSaps.GetLastOutputPpnCreditByIdNo(idNo);
            var filepath = "";
            var ispendingoutput = true;
            if (logProcSap == null || string.IsNullOrEmpty(logProcSap.FilePath))
            {
                var getdatoriginal = LogSaps.GetByIdNo(idNo);
                filepath = getdatoriginal.LocalPath;
            }
            else
            {
                filepath = logProcSap.FilePath;
                ispendingoutput = false;
            }

            var ifilterstatus = 0;
            if (!string.IsNullOrEmpty(filterstatus))
            {
                try
                {
                    ifilterstatus = int.Parse(filterstatus);
                }
                catch (Exception)
                {                    
                }
            }

            var objRet = new ObjOutUploadPPNCredit();
            var sourceFile = new FileInfo(filepath);
            XmlDocument xml = new XmlDocument();
            xml.Load(sourceFile.FullName);

            //Set XML Namespace
            NameTable nt = new NameTable();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nt);
            nsmgr.AddNamespace("ns1", "http://admevis");

            //Get Data 

            var headerNode = xml.SelectSingleNode("/ns1:MT_UploadPPNCredit_Resp", nsmgr);
            XmlNodeList itemNode;
            if (headerNode == null)
            {
                nsmgr = new XmlNamespaceManager(nt);
                nsmgr.AddNamespace("ns0", "http://admevis");
                headerNode = xml.SelectSingleNode("/ns0:MT_UploadPPNCredit_Req", nsmgr);
                itemNode = xml.SelectNodes("/ns0:MT_UploadPPNCredit_Req/Input/Item", nsmgr);
            }
            else
            {
                itemNode = xml.SelectNodes("/ns1:MT_UploadPPNCredit_Resp/Output/Item", nsmgr);
            }            

            var listItems = new List<ObjOutUploadPPNCreditItem>();
            
            idNo = headerNode["IDNo"].InnerText.Trim();
            if (!ispendingoutput)
            {
                var confirm = headerNode["Confirm"].InnerText.Trim();
                var message = headerNode["Message"].InnerText.Trim();
                objRet.Confirm = confirm;
                objRet.Message = message;
                var fiscalYear = headerNode["FiscalYear"].InnerText.Trim();
                var accDocNo = headerNode["AccountingDocNo"].InnerText.Trim();
                objRet.FiscalYear = fiscalYear;
                objRet.AccountingDocNo = accDocNo;
            }
            else
            {
                objRet.Confirm = "";
                objRet.Message = "";
                objRet.FiscalYear = "";
                objRet.AccountingDocNo = "";
            }

            objRet.IDNo = idNo;

            foreach (XmlNode item in itemNode)
            {
                var sFp = item["FP"].InnerText;
                var sNpwp = item["NPWP"].InnerText;
                var sPembetulanKe = item["PembetulanKe"].InnerText;
                var sMasaPajakBulan = item["MasaPajakBulan"].InnerText;
                var sMasaPajakTahun = item["MasaPajakTahun"].InnerText;
                var sAccountingDocDebet = item["AccountingDocDebet"].InnerText;
                var sFiscalYearDebet = item["FiscalYearDebet"].InnerText;
                var sLineItem = item["LineItem"].InnerText;
                var sGLAccount = item["GLAccount"].InnerText;
                var sAmountPPN = item["AmountPPN"].InnerText;                
                var sMessage = "";
                var sConfirm = "";
                var status = 3;
                if (!ispendingoutput)
                {
                    sMessage = item["Message"].InnerText;
                    sConfirm = item["Confirm"].InnerText;
                    status = string.IsNullOrEmpty(sConfirm) ? 2 : 1;
                }
                listItems.Add(new ObjOutUploadPPNCreditItem()
                {
                    FP = string.IsNullOrEmpty(sFp) ? string.Empty : sFp.Trim(),
                    NPWP = string.IsNullOrEmpty(sNpwp) ? string.Empty : sNpwp.Trim(),
                    PembetulanKe = string.IsNullOrEmpty(sPembetulanKe) ? string.Empty : sPembetulanKe.Trim(),
                    MasaPajakBulan = string.IsNullOrEmpty(sMasaPajakBulan) ? string.Empty : sMasaPajakBulan,
                    MasaPajakTahun = string.IsNullOrEmpty(sMasaPajakTahun) ? string.Empty : sMasaPajakTahun,
                    AccountingDocDebet = string.IsNullOrEmpty(sAccountingDocDebet) ? string.Empty : sAccountingDocDebet,
                    AccountingDocNoCredit = string.IsNullOrEmpty(objRet.AccountingDocNo) ? string.Empty : objRet.AccountingDocNo,
                    FiscalYearDebet = string.IsNullOrEmpty(sFiscalYearDebet) ? string.Empty : sFiscalYearDebet,
                    LineItem = string.IsNullOrEmpty(sLineItem) ? string.Empty : sLineItem,
                    GLAccount = string.IsNullOrEmpty(sGLAccount) ? string.Empty : sGLAccount,
                    AmountPPN = string.IsNullOrEmpty(sAmountPPN) ? string.Empty : sAmountPPN,
                    Message = string.IsNullOrEmpty(sMessage) ? string.Empty : sMessage,                    
                    Status = status,
                    Confirm = string.IsNullOrEmpty(sConfirm) ? string.Empty : sConfirm
                });
            }
            objRet.Items = new List<ObjOutUploadPPNCreditItem>();
            objRet.Items.AddRange(listItems.Where(c => (filterstatus == "0" || (filterstatus != "0" && c.Status == ifilterstatus))));
            int totalItems = listItems.Where(c => (filterstatus == "0" || (filterstatus != "0" && c.Status == ifilterstatus))).Count();
            //objRet.Items.AddRange(listItems);
            //int totalItems = listItems.Count();
            return Json(new
            {
                sEcho = sEcho,
                iTotalRecords = totalItems,
                iTotalDisplayRecords = totalItems,
                aaData = objRet.Items
            },
            JsonRequestBehavior.AllowGet);
            //return objRet;
        }

    }
}