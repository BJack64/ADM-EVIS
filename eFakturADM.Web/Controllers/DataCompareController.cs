using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using eFakturADM.FileManager;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Shared.Utility;
using eFakturADM.Web.Helpers;
using eFakturADM.Web.Models;
using System.Transactions;
using System.Linq;

namespace eFakturADM.Web.Controllers
{
    public class DataCompareController : BaseController
    {

        public class TestGridInfo
        {
            public string Nama { get; set; }
            public string Alamat { get; set; }
            public string Notes { get; set; }
        }

        [AuthActivity("21")]
        public ActionResult EvisVsIws()
        {
            return View("EvisVsIws");
        }

        [AuthActivity("24")]
        public ActionResult EvisVsSap()
        {
            return View("EvisVsSap");
        }

        #region ------------ Compare Evis VS Iws --------

        public JsonResult GetDataEvisVsIwsDataTable(string sEcho, int iDisplayStart, int iDisplayLength, string receivingDateStart, string receivingDateEnd, string statusId, string scanUserName, string scanDateString)
        {
            if (string.IsNullOrEmpty(receivingDateStart) || string.IsNullOrEmpty(receivingDateEnd))
            {
                return Json(new
                {
                    sEcho = sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = new List<CompEvisIws>()
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
                SortColumnName = "VendorCode"
            };

            switch (Convert.ToInt32(Request["iSortCol_0"]))
            {
                case 0: filter.SortColumnName = "ReceivedDate"; break;
                case 1: filter.SortColumnName = "VendorCode"; break;
                case 2: filter.SortColumnName = "VendorName"; break;
                case 3: filter.SortColumnName = "ScanDate"; break;
                case 4: filter.SortColumnName = "TaxInvoiceNumberEVIS"; break;
                case 5: filter.SortColumnName = "TaxInvoiceNumberIWS"; break;
                case 6: filter.SortColumnName = "InvoiceNumber"; break;
                case 7: filter.SortColumnName = "VATAmountScanned"; break;
                case 8: filter.SortColumnName = "VATAmountIWS"; break;
                case 9: filter.SortColumnName = "VATAmountDiff"; break;
                case 10: filter.SortColumnName = "StatusDJP"; break;
                case 11: filter.SortColumnName = "StatusCompare"; break;
                case 12: filter.SortColumnName = "Notes"; break;
                case 13: filter.SortColumnName = "ScanUserName"; break;
            }

            int totalItems;
            int? statusIntId = null;
            if (!(string.IsNullOrEmpty(statusId) || statusId == "0"))
            {
                statusIntId = Convert.ToInt32(statusId);
            }

            DateTime? dScanDate = string.IsNullOrEmpty(scanDateString)
                ? (DateTime?)null
                : Convert.ToDateTime(scanDateString);

            var dtStartSearch = Convert.ToDateTime(receivingDateStart);
            var dtEndSearch = Convert.ToDateTime(receivingDateEnd);
            var dats = CompEvisIwss.GetByReceivingDate(filter, out totalItems, dtStartSearch, dtEndSearch, statusIntId, scanUserName, dScanDate);
            return Json(new
            {
                sEcho = sEcho,
                iTotalRecords = totalItems,
                iTotalDisplayRecords = totalItems,
                aaData = dats
            },
            JsonRequestBehavior.AllowGet);
        }

        [AuthActivity("22")]
        public JsonResult GenerateCompareEvisVsIws(string receivingStartDate, string ReceivingEndDate, string scanUserName, string scanDateString)
        {
            if (string.IsNullOrEmpty(receivingStartDate) || string.IsNullOrEmpty(ReceivingEndDate))
            {
                return Json(new
                {
                    Html = new RequestResultModel()
                    {
                        InfoType = RequestResultInfoType.Warning,
                        Message = "Please Specify Receiving Date"
                    }
                }, JsonRequestBehavior.AllowGet);
            }

            DateTime dtReceivingDateStart;
            DateTime dtReceivingDateEnd;
            if (!DateTime.TryParse(receivingStartDate, out dtReceivingDateStart))
            {
                return Json(new
                {
                    Html = new RequestResultModel()
                    {
                        InfoType = RequestResultInfoType.Warning,
                        Message = "Invalid input for Receiving Start Date (" + receivingStartDate + ")"
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            else if (!DateTime.TryParse(ReceivingEndDate, out dtReceivingDateEnd))
            {
                return Json(new
                {
                    Html = new RequestResultModel()
                    {
                        InfoType = RequestResultInfoType.Warning,
                        Message = "Invalid input for Receiving End Date (" + ReceivingEndDate + ")"
                    }
                }, JsonRequestBehavior.AllowGet);
            }

            DateTime? dScanDate = null;
            if (!string.IsNullOrEmpty(scanDateString))
            {
                DateTime dScanDateD;
                if (!DateTime.TryParse(scanDateString, out dScanDateD))
                {
                    return Json(new
                    {
                        Html = new RequestResultModel()
                        {
                            InfoType = RequestResultInfoType.Warning,
                            Message = "Invalid input for Receiving Date (" + receivingStartDate + ")"
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                dScanDate = dScanDateD;
            }

            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = string.Empty
            };

            try
            {
                var userNameLogin = Session["UserName"].ToString();
                CompEvisIwss.GenerateCompareByReceivingDate(dtReceivingDateStart, dtReceivingDateEnd, userNameLogin, scanUserName, dScanDate);
                model.Message = "Submit Data Compare EVIS VS IWS Success";
            }
            catch (Exception exception)
            {

                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = "Submit Data Compare EVIS VS IWS failed. See Log with Key " + logKey + " for details.";
            }

            return Json(new
            {
                Html = model
            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult EvisVsIwsInputNotesDialog(long id)
        {
            var dbData = CompEvisIwss.GetById(id);
            return Json(new
            {
                Html = this.RenderPartialView(@"EvisVsIwsInputNotesDialog", new DataCompareEvisVsIwsInputNotesDialogModel()
                {
                    Id = id,
                    Notes = dbData.Notes
                }),

            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EvisVsIwsSaveNotes(long id, string notes)
        {
            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = "Save Notes Successfully"
            };

            if (string.IsNullOrEmpty(notes))
            {
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = "Input Notes is required";
                return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var userNameLogin = Session["UserName"].ToString();
                CompEvisIwss.UpdateNotesById(id, notes, userNameLogin);
            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = "Save Notes failed. See Log with key " + logKey + " for details.";
            }
            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region ---------- Compare Evis vs Sap ------------------
        public JsonResult GetDataEvisVsSapDataTable(string sEcho, int iDisplayStart, int iDisplayLength, string firstLoad, string tglPostingStart, string tglPostingEnd, string tglFakturStart, string tglFakturEnd, string noFakturStart,
            string noFakturEnd, string scanDate, string masaPajak, string username, string tahunPajak, string statusId, string statusPosting)
        {
            if (Convert.ToBoolean(firstLoad))
            {
                return Json(new
                {
                    sEcho = sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = new List<CompEvisIws>()
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
                SortColumnName = "PostingDate"
            };

            switch (Convert.ToInt32(Request["iSortCol_0"]))
            {
                case 1: filter.SortColumnName = "PostingDate"; break;
                case 2: filter.SortColumnName = "AccountingDocNo"; break;
                case 3: filter.SortColumnName = "ItemNo"; break;
                case 4: filter.SortColumnName = "TglFaktur"; break;
                case 5: filter.SortColumnName = "NamaVendor"; break;
                case 6: filter.SortColumnName = "ScanDate"; break;
                case 7: filter.SortColumnName = "TaxInvoiceNumberEvis"; break;
                case 8: filter.SortColumnName = "TaxInvoiceNumberSap"; break;
                case 9: filter.SortColumnName = "DocumentHeaderText"; break;
                case 10: filter.SortColumnName = "Npwp"; break;
                case 11: filter.SortColumnName = "AmountEvis"; break;
                case 12: filter.SortColumnName = "AmountSap"; break;
                case 13: filter.SortColumnName = "AmountDiff"; break;
                case 14: filter.SortColumnName = "StatusCompare"; break;
                case 16: filter.SortColumnName = "UserNameCreator"; break;
            }

            int totalItems;
            int? statusIntId = null;
            if (!(string.IsNullOrEmpty(statusId) || statusId == "0"))
            {
                statusIntId = Convert.ToInt32(statusId);
            }

            int? statusIntPosting = 0;
            if (!(string.IsNullOrEmpty(statusPosting) || statusPosting == "0"))
            {
                statusIntPosting = Convert.ToInt32(statusPosting);
            }

            int? intNull = null;
            DateTime? dateNull = null;

            DateTime? dttglPostingStart = !string.IsNullOrEmpty(tglPostingStart) ? Convert.ToDateTime(tglPostingStart) : dateNull;
            DateTime? dttglPostingEnd = !string.IsNullOrEmpty(tglPostingEnd) ? Convert.ToDateTime(tglPostingEnd) : dateNull;
            DateTime? dttglFakturStart = !string.IsNullOrEmpty(tglFakturStart) ? Convert.ToDateTime(tglFakturStart) : dateNull;
            DateTime? dttglFakturEnd = !string.IsNullOrEmpty(tglFakturEnd) ? Convert.ToDateTime(tglFakturEnd) : dateNull;
            DateTime? dtScanDate = !string.IsNullOrEmpty(scanDate) ? Convert.ToDateTime(scanDate) : dateNull;
            int? iMasaPajak = !string.IsNullOrEmpty(masaPajak) && masaPajak != "0" ? Convert.ToInt32(masaPajak) : intNull;
            int? iTahunPajak = !string.IsNullOrEmpty(tahunPajak) && tahunPajak != "0" ? Convert.ToInt32(tahunPajak) : intNull;

            var dats = CompEvisSaps.GetCompareList(filter, out totalItems, dttglPostingStart, dttglPostingEnd, dttglFakturStart, dttglFakturEnd, noFakturStart, noFakturEnd, dtScanDate, username, iMasaPajak, iTahunPajak, statusIntId, statusIntPosting);

            var jsonresult = Json(new
            {
                sEcho = sEcho,
                iTotalRecords = totalItems,
                iTotalDisplayRecords = totalItems,
                aaData = dats
            },
            JsonRequestBehavior.AllowGet);
            jsonresult.MaxJsonLength = int.MaxValue;
            return jsonresult;
        }

        [AuthActivity("25,26")]
        public JsonResult ValidationSubmitCompareEvisSap(DataCompareEvisVsSapInfoModel info)
        {
            var model = new RequestResultModel
            {
                InfoType = RequestResultInfoType.Success,
                Message = string.Empty
            };

            if (info.ListCompare != null)
            {
                var isForceSubmit = info.SubmitType == ApplicationEnums.SubmitType.ForceSubmit.ToString();

                var glAccount = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.GLAccountForceSubmitSAP);
                if (glAccount == null)
                {
                    model.InfoType = RequestResultInfoType.ErrorOrDanger;
                    model.Message = "GL Account default not found on General Config";
                    return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
                }

                if (isForceSubmit)
                {
                    //check is any OK Data Compare Status
                    var chkok = info.ListCompare.Where(c => c.StatusCompare.ToLower() == EnumHelper.GetDescription(ApplicationEnums.StatusCompareEvisVsSap.Ok)).ToList();
                    if (chkok.Count > 0)
                    {
                        model.InfoType = RequestResultInfoType.ErrorOrDanger;
                        model.Message = "Can't Force Submit because Status Compare is OK";
                    }
                }
                else
                {
                    //check is any Not OK Data Compare Status
                    var chknotok = info.ListCompare.Where(c => c.StatusCompare.ToLower() == EnumHelper.GetDescription(ApplicationEnums.StatusCompareEvisVsSap.NotOk)).ToList();
                    if (chknotok.Count > 0)
                    {
                        model.InfoType = RequestResultInfoType.ErrorOrDanger;
                        model.Message = "Can't Submit because Status Compare is Not OK";
                    }
                }

                if (model.InfoType == RequestResultInfoType.Success)
                {
                    //validasi faktur manual untuk vendor
                    var fpkhusus = info.ListCompare.Where(c => c.FPType.HasValue && c.FPType.Value == (int)ApplicationEnums.FPType.ScanManual).ToList();
                    if (fpkhusus.Count > 0)
                    {
                        var datvendors = Vendors.Get();
                        var chkvendor = (from x in fpkhusus
                                         join y in datvendors on x.NPWPPenjual equals y.NPWP into xy
                                         from subxy in xy.DefaultIfEmpty()
                                         where subxy == null
                                         select x).ToList();
                        if (chkvendor.Count > 0)
                        {
                            model.InfoType = RequestResultInfoType.ErrorOrDanger;
                            model.Message = "[FP Khusus]NPWP Penjual tidak terdaftar di Master Vendor.";
                        }
                    }
                }

            }
            else
            {
                model.InfoType = RequestResultInfoType.Warning;
                model.Message = "No Data";
            }

            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PushSubmitCompareEvisSap(DataCompareEvisVsSapInfoModel info)
        {
            var model = new RequestResultModel
            {
                InfoType = RequestResultInfoType.Success,
                Message = string.Empty
            };
            try
            {
                var resultSave = SaveSubmitCompareEvisSap(info);
                model.InfoType = resultSave.InfoType;
                model.Message = resultSave.Message;
            }
            catch (Exception ex)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, ex.Message, MethodBase.GetCurrentMethod(), ex);
                model.Message = "Submit data failed. See Log with Key " + logKey + " for details.";
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
            }

            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PushSubmitCompareEvisSapAll(EvisVsSapSubmitAllModel info)
        {
            var model = new RequestResultModel
            {
                InfoType = RequestResultInfoType.Success,
                Message = string.Empty
            };
            try
            {
                var resultSave = SaveSubmitCompareEvisSapAll(info);
                model.InfoType = resultSave.InfoType;
                model.Message = resultSave.Message;
            }
            catch (Exception ex)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, ex.Message, MethodBase.GetCurrentMethod(), ex);
                model.Message = "Submit data failed. See Log with Key " + logKey + " for details.";
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
            }

            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }

        private RequestResultModel SaveSubmitCompareEvisSap(DataCompareEvisVsSapInfoModel info)
        {
            var objRet = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = "Compare EVIS vs SAP has been created."
            };

            var userName = Session["UserName"] as string;

            DateTime? dateNull = null;
            decimal? decNull = null;
            int? intNull = null;

            var isForceSubmit = info.SubmitType == ApplicationEnums.SubmitType.ForceSubmit.ToString();
            var glAccount = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.GLAccountForceSubmitSAP);
            if (glAccount == null)
            {
                objRet.InfoType = RequestResultInfoType.ErrorOrDanger;
                objRet.Message = "GL Account default not found on General Config";
                return objRet;
            }

            List<CompEvisSap> listToSave = new List<CompEvisSap>();
            for (int i = 0; i < info.ListCompare.Count; i++)
            {
                var data = new CompEvisSap();

                data.PostingDate = !string.IsNullOrEmpty(info.ListCompare[i].PostingDate)
                    ? Convert.ToDateTime(info.ListCompare[i].PostingDate)
                    : dateNull;
                data.AccountingDocNo = info.ListCompare[i].AccountingDocNo;
                data.ItemNo = info.ListCompare[i].ItemNo;
                data.GLAccount = isForceSubmit ? glAccount.ConfigValue : info.ListCompare[i].GLAccount;

                data.TglFaktur = !string.IsNullOrEmpty(info.ListCompare[i].TglFaktur)
                    ? Convert.ToDateTime(info.ListCompare[i].TglFaktur)
                    : dateNull;
                data.TaxInvoiceNumberEvis = info.ListCompare[i].TaxInvoiceNumberEVIS;
                data.TaxInvoiceNumberSap = info.ListCompare[i].TaxInvoiceNumberSAP;
                data.DocumentHeaderText = info.ListCompare[i].DocumentHeaderText;
                data.Npwp = info.ListCompare[i].NPWP;
                data.AmountEvis = !string.IsNullOrEmpty(info.ListCompare[i].AmountEVIS)
                    ? Convert.ToDecimal(info.ListCompare[i].AmountEVIS.Replace(",", ""))
                    : decNull;
                data.AmountSap = !string.IsNullOrEmpty(info.ListCompare[i].AmountSAP)
                    ? Convert.ToDecimal(info.ListCompare[i].AmountSAP.Replace(",", ""))
                    : decNull;
                data.AmountDiff = !string.IsNullOrEmpty(info.ListCompare[i].AmountDiff)
                    ? Convert.ToDecimal(info.ListCompare[i].AmountDiff.Replace(",", ""))
                    : decNull;
                data.StatusCompare = info.ListCompare[i].StatusCompare;
                data.Notes = info.ListCompare[i].Notes;
                data.Pembetulan = Convert.ToInt32(info.ListCompare[i].Pembetulan);
                data.MasaPajak = !string.IsNullOrEmpty(info.ListCompare[i].MasaPajak)
                    ? info.ListCompare[i].MasaPajak == "null" ? intNull : Convert.ToInt32(info.ListCompare[i].MasaPajak)
                    : intNull;
                data.TahunPajak = !string.IsNullOrEmpty(info.ListCompare[i].TahunPajak)
                    ? info.ListCompare[i].TahunPajak == "null"
                        ? intNull
                        : Convert.ToInt32(info.ListCompare[i].TahunPajak)
                    : intNull;
                data.ItemText = info.ListCompare[i].ItemText;
                data.FiscalYearDebet = !string.IsNullOrEmpty(info.ListCompare[i].FiscalYearDebet)
                    ? info.ListCompare[i].FiscalYearDebet.ToLower() == "null" ? intNull
                    : Convert.ToInt32(info.ListCompare[i].FiscalYearDebet)
                    : intNull;
                data.NPWPPenjual = info.ListCompare[i].NPWPPenjual;
                data.StatusFaktur = info.ListCompare[i].StatusFaktur;

                listToSave.Add(data);
            }

            string RepoUser = FileManagerConfiguration.RepoUser;
            string RepoPassword = FileManagerConfiguration.RepoPassword;
            string RepoRootPath = FileManagerConfiguration.RepoRootPath;
            string DataFolderWatcherInboxService = FileManagerSapConfiguration.DataFolderWatcherInboxService;
            string PrefixUploadPpnCredit = FileManagerSapConfiguration.PrefixUploadPpnCredit;

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = IsolationLevel.ReadCommitted;
            options.Timeout = new TimeSpan(0, 15, 0);
            //https://stackoverflow.com/questions/193154/the-operation-is-not-valid-for-the-state-of-the-transaction-error-and-transact

            using (var eScope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    string idNo;
                    
                    CompEvisSaps.SaveSubmitBulk(listToSave, userName, out idNo);
                        
                    //create xml sap
                    var createFileResult = isForceSubmit ?
                        ExportFileManager.CompEvisVsSapCreateForceSubmitXml(idNo, RepoUser, RepoPassword, RepoRootPath, DataFolderWatcherInboxService, PrefixUploadPpnCredit)
                        : ExportFileManager.CompEvisVsSapCreateSubmitXml(idNo, RepoUser, RepoPassword, RepoRootPath, DataFolderWatcherInboxService, PrefixUploadPpnCredit);

                    if (createFileResult.InfoType == CommonOutputType.Success)
                    {
                        //save LogSap
                        var logToSave = new LogSap()
                        {
                            IdNo = idNo,
                            TransactionType =
                                EnumHelper.GetDescription(ApplicationEnums.LogSapTransactionType.ReconcileWithSap),
                            CreatedBy = userName,
                            LogSapId = 0,
                            Status = (int)ApplicationEnums.SapStatusLog.Submitted,
                            FileName = System.IO.Path.GetFileName(createFileResult.FilePath),
                            LocalPath = createFileResult.FilePath
                        };

                        LogSaps.Save(logToSave);

                        eScope.Complete();
                    }
                    else
                    {
                        string logKey;
                        Logger.WriteLog(out logKey, LogLevel.Error, createFileResult.MessageInfo, MethodBase.GetCurrentMethod());
                        objRet.InfoType = RequestResultInfoType.ErrorOrDanger;
                        objRet.Message = "Submit data failed. See Log with Key " + logKey + " for details.";
                    }
                    
                }
                catch (Exception exception)
                {
                    string logKey;
                    Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                    objRet.InfoType = RequestResultInfoType.ErrorOrDanger;
                    objRet.Message = exception.StackTrace + exception.InnerException;
                    //objRet.Message = "Submit data failed. See Log with Key " + logKey + " for details.";
                }
                eScope.Dispose();
            }
            return objRet;

        }

        private RequestResultModel SaveSubmitCompareEvisSapAll(EvisVsSapSubmitAllModel info)
        {
            var objRet = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = "Compare EVIS vs SAP has been created."
            };

            int? statusIntId = null;
            if (!(string.IsNullOrEmpty(info.StatusId) || info.StatusId == "0"))
            {
                statusIntId = Convert.ToInt32(info.StatusId);
            }

            int? statusIntPosting = null;
            if (!(string.IsNullOrEmpty(info.StatusPosting) || info.StatusPosting == "0"))
            {
                statusIntPosting = Convert.ToInt32(info.StatusPosting);
            }


            DateTime? dateNull = null;
            int? intNull = null;

            int? iMasaPajak = !string.IsNullOrEmpty(info.MasaPajak) && info.MasaPajak != "0" ? Convert.ToInt32(info.MasaPajak) : intNull;
            int? iTahunPajak = !string.IsNullOrEmpty(info.TahunPajak) && info.TahunPajak != "0" ? Convert.ToInt32(info.TahunPajak) : intNull;
            DateTime? dttglPostingStart = !string.IsNullOrEmpty(info.TglPostingStart) ? Convert.ToDateTime(info.TglPostingStart) : dateNull;
            DateTime? dttglPostingEnd = !string.IsNullOrEmpty(info.TglPostingEnd) ? Convert.ToDateTime(info.TglPostingEnd) : dateNull;
            DateTime? dttglFakturStart = !string.IsNullOrEmpty(info.TglFakturStart) ? Convert.ToDateTime(info.TglFakturStart) : dateNull;
            DateTime? dttglFakturEnd = !string.IsNullOrEmpty(info.TglFakturEnd) ? Convert.ToDateTime(info.TglFakturEnd) : dateNull;
            DateTime? dtScanDate = !string.IsNullOrEmpty(info.ScanDate) ? Convert.ToDateTime(info.ScanDate) : dateNull;

            var dats2 = CompEvisSaps.GetCompareListToDownload(dttglPostingStart, dttglPostingEnd, dttglFakturStart,
                dttglFakturEnd, info.NoFakturStart, info.NoFakturEnd, dtScanDate, info.UserName, iMasaPajak, iTahunPajak, statusIntId, statusIntPosting);

            if (dats2.Count <= 0)
            {
                objRet.InfoType = RequestResultInfoType.ErrorOrDanger;
                objRet.Message = "No Data";
            }
            else
            {
                //var dats = CompEvisSaps.GetCompareListToDownload(dttglPostingStart, dttglPostingEnd, dttglFakturStart,
                //dttglFakturEnd, info.NoFakturStart, info.NoFakturEnd, dtScanDate, info.UserName, iMasaPajak, iTahunPajak, statusIntId).OrderBy(o => o.VSequence).ToList();
                var dats = dats2.OrderBy(o => o.VSequence).ToList();
                //validate
                var isForceSubmit = info.SubmitType == ApplicationEnums.SubmitType.ForceSubmit.ToString();
                var glAccount = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.GLAccountForceSubmitSAP);
                if (glAccount == null)
                {
                    objRet.InfoType = RequestResultInfoType.ErrorOrDanger;
                    objRet.Message = "GL Account default not found on General Config";
                    return objRet;
                }

                if (isForceSubmit)
                {
                    //check is any OK Data Compare Status
                    var chkok = dats.Where(c => c.StatusCompare.ToLower() == EnumHelper.GetDescription(ApplicationEnums.StatusCompareEvisVsSap.Ok)).ToList();
                    if (chkok.Count > 0)
                    {
                        objRet.InfoType = RequestResultInfoType.ErrorOrDanger;
                        objRet.Message = "Can't Force Submit because Status Compare is OK";
                    }
                    else
                    {
                        for (int i = 0; i < dats.Count; i++)
                        {
                            dats[i].GLAccount = glAccount.ConfigValue;
                        }
                    }
                }
                else
                {
                    //check is any Not OK Data Compare Status
                    var chknotok = dats.Where(c => c.StatusCompare.ToLower() == EnumHelper.GetDescription(ApplicationEnums.StatusCompareEvisVsSap.NotOk)).ToList();
                    if (chknotok.Count > 0)
                    {
                        objRet.InfoType = RequestResultInfoType.ErrorOrDanger;
                        objRet.Message = "Can't Submit because Status Compare is Not OK";
                    }
                }
                if (objRet.InfoType == RequestResultInfoType.Success)
                {
                    //validasi faktur manual untuk vendor
                    var fpkhusus = dats.Where(c => c.FPType.HasValue && c.FPType.Value == (int)ApplicationEnums.FPType.ScanManual).ToList();
                    if (fpkhusus.Count > 0)
                    {
                        var datvendors = Vendors.Get();
                        var chkvendor = (from x in fpkhusus
                                         join y in datvendors on x.NPWPPenjual equals y.NPWP into xy
                                         from subxy in xy.DefaultIfEmpty()
                                         where subxy == null
                                         select x).ToList();
                        if (chkvendor.Count > 0)
                        {
                            objRet.InfoType = RequestResultInfoType.ErrorOrDanger;
                            objRet.Message = "[FP Khusus]NPWP Penjual tidak terdaftar di Master Vendor.";
                        }
                    }
                }
                if (objRet.InfoType == RequestResultInfoType.Success)
                {
                    //proses save dan generate xml                    
                    //max item per batch
                    var cfgmaxperxml = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.MaxItemPerXmlCompareEvisVsSap);
                    var maxperxml = 900;
                    if (cfgmaxperxml != null)
                    {
                        try
                        {
                            maxperxml = Convert.ToInt32(cfgmaxperxml.ConfigValue);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    int startidx = 1;
                    int batchorder = 1;
                    int endidx = batchorder * maxperxml;
                    int maxidx = dats.Count;
                    var resultlist = new List<RequestResultModel>();
                    var actiontext = isForceSubmit ? "Force Submit" : "Submit";
                    var userName = Session["UserName"] as string;

                    string RepoUser = FileManagerConfiguration.RepoUser;
                    string RepoPassword = FileManagerConfiguration.RepoPassword;
                    string RepoRootPath = FileManagerConfiguration.RepoRootPath;
                    string DataFolderWatcherInboxService = FileManagerSapConfiguration.DataFolderWatcherInboxService;
                    string PrefixUploadPpnCredit = FileManagerSapConfiguration.PrefixUploadPpnCredit;

                    while (startidx <= maxidx)
                    {

                        try
                        {
                            TransactionOptions options = new TransactionOptions();
                            options.IsolationLevel = IsolationLevel.ReadCommitted;
                            options.Timeout = new TimeSpan(0, 15, 0);
                            //https://stackoverflow.com/questions/193154/the-operation-is-not-valid-for-the-state-of-the-transaction-error-and-transact
                            using (var eScope = new TransactionScope(TransactionScopeOption.Required, options))
                            {
                                string idNo;
                                var listToSave = dats.Where(c => c.VSequence >= startidx && c.VSequence <= endidx).ToList();
                                //submit
                                //var resultSave = CompEvisSaps.Submit(listToSave, userName, out idNo);
                                CompEvisSaps.SaveSubmitBulk(listToSave, userName, out idNo);
                                //create xml sap
                                var createFileResult = isForceSubmit ? 
                                    ExportFileManager.CompEvisVsSapCreateForceSubmitXml(idNo, RepoUser, RepoPassword, RepoRootPath, DataFolderWatcherInboxService, PrefixUploadPpnCredit) 
                                    : ExportFileManager.CompEvisVsSapCreateSubmitXml(idNo, RepoUser, RepoPassword, RepoRootPath, DataFolderWatcherInboxService, PrefixUploadPpnCredit);

                                if (createFileResult.InfoType == CommonOutputType.Success)
                                {
                                    //save LogSap
                                    var logToSave = new LogSap()
                                    {
                                        IdNo = idNo,
                                        TransactionType =
                                            EnumHelper.GetDescription(ApplicationEnums.LogSapTransactionType.ReconcileWithSap),
                                        CreatedBy = userName,
                                        LogSapId = 0,
                                        Status = (int)ApplicationEnums.SapStatusLog.Submitted,
                                        FileName = System.IO.Path.GetFileName(createFileResult.FilePath),
                                        LocalPath = createFileResult.FilePath
                                    };

                                    LogSaps.Save(logToSave);

                                    eScope.Complete();
                                }
                                else
                                {
                                    string logKey;
                                    Logger.WriteLog(out logKey, LogLevel.Error, createFileResult.MessageInfo, MethodBase.GetCurrentMethod());
                                    resultlist.Add(new RequestResultModel()
                                    {
                                        Title = logKey,
                                        InfoType = RequestResultInfoType.ErrorOrDanger,
                                        Message = actiontext + " failed Data Batch : " + batchorder.ToString()
                                    });
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            string outlogkey;
                            Logger.WriteLog(out outlogkey, LogLevel.Error, ex.Message, MethodBase.GetCurrentMethod(), ex);
                            resultlist.Add(new RequestResultModel()
                            {
                                Title = outlogkey,
                                InfoType = RequestResultInfoType.ErrorOrDanger,
                                Message = actiontext + " failed Data Batch : " + batchorder.ToString()
                            });
                        }


                        batchorder = batchorder + 1;
                        startidx = endidx + 1;
                        endidx = batchorder * maxperxml;
                    }
                    if (resultlist.Count > 0)
                    {
                        var chks = resultlist.Where(c => c.InfoType != RequestResultInfoType.Success).ToList();
                        if (chks.Count > 0)
                        {
                            objRet.InfoType = RequestResultInfoType.ErrorOrDanger;
                            objRet.Message = string.Join("<br />", chks.Select(d => d.Message));
                        }
                    }
                }
            }

            return objRet;
        }

        #endregion

        public ActionResult GenerateOuputUploadPpnCredit()
        {
            return View("GenerateOuputUploadPpnCredit");
        }
        public JsonResult PushGenerateOuputUploadPpnCredit(string idNo, int isSuccess, string message, string accountingDocNoKredit, string fiscalYearKredit)
        {
            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = "Generate Output UploadPPNCredit successfully"
            };
            try
            {
                var createFileResult = ExportFileManager.GenerateResponUploadPppnCredit(idNo, isSuccess == 1, message,
                    accountingDocNoKredit, string.IsNullOrEmpty(fiscalYearKredit) ? 0 : Convert.ToInt32(fiscalYearKredit));
                if (createFileResult.InfoType != CommonOutputType.Success)
                {
                    string logKey;
                    Logger.WriteLog(out logKey, LogLevel.Error, createFileResult.MessageInfo, MethodBase.GetCurrentMethod());
                    model.InfoType = RequestResultInfoType.ErrorOrDanger;
                    model.Message = "Error Log : " + logKey;
                }

            }
            catch (Exception ex)
            {
                string outLogKey;
                Logger.WriteLog(out outLogKey, LogLevel.Error, ex.Message, MethodBase.GetCurrentMethod(), ex);

                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = "Error Log : " + outLogKey;

            }
            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }

    }

}
