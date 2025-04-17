using System;
using System.Collections.Generic;
using System.Linq;
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

namespace eFakturADM.Web.Controllers
{
    public class OpenClosePeriodController : BaseController
    {
        [AuthActivity("31")]
        public ActionResult ListOpenClosePeriod()
        {
            return View("ListOpenClosePeriod");
        }

        public JsonResult GetListOpenClosePeriodDataTable(string sEcho, int iDisplayStart, int iDisplayLength, string MasaPajakStart, string MasaPajakEnd, string TahunPajakStart, string TahunPajakEnd)
        {
            return GetListOpenClosePeriod(sEcho, iDisplayStart, iDisplayLength, MasaPajakStart, MasaPajakEnd, TahunPajakStart, TahunPajakEnd);
        }

        [AuthActivity("32")]
        public JsonResult GetOpenPeriodDialog()
        {
            return Json(new
            {
                Html = this.RenderPartialView(@"GetOpenPeriodDialog", null),

            }, JsonRequestBehavior.AllowGet);
        }

        [AuthActivity("33")]
        public JsonResult GetCloseSP2Dialog(string openClosePeriodId)
        {
            return Json(new
            {
                Html = this.RenderPartialView(@"GetCloseSP2Dialog", openClosePeriodId),

            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidationOnCloseSp2(string openClosePeriodId)
        {
            var model = PrivValidationOnCloseSp2(openClosePeriodId);
            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ValidationAndPushCloseSP2(string OpenClosePeriodId, string FilePath)
        {
            var userName = Session["UserName"] as string;

            var model = PrivValidationOnCloseSp2(OpenClosePeriodId);
            if (model.InfoType != RequestResultInfoType.Success)
            {
                return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
            }

            if (!string.IsNullOrEmpty(OpenClosePeriodId))
            {
                OpenClosePeriod dataOpenClosePeriod = OpenClosePeriods.GetById(Convert.ToInt32(OpenClosePeriodId));
                if (dataOpenClosePeriod.OpenClosePeriodId > 0)
                {
                    try
                    {
                        using (var eScope = new TransactionScope())
                        {
                            //close SP2
                            OpenClosePeriods.UpdateStatusSP2(Convert.ToInt32(OpenClosePeriodId), false, FilePath, userName);
                            eScope.Complete();
                            eScope.Dispose();
                        }
                        model.InfoType = RequestResultInfoType.Success;
                        model.Message = "Close SP2 has been submitted.";
                    }
                    catch (Exception ex)
                    {
                        string logKey;
                        Logger.WriteLog(out logKey, LogLevel.Fatal, ex.Message, MethodBase.GetCurrentMethod(), ex);
                        model.Message = "Close SP2 failed";
                        model.InfoType = RequestResultInfoType.ErrorOrDanger;
                    }
                }
            }

            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult SubmitOpenPeriod(string MasaPajak, string Tahun)
        {
            var model = ValidationSubmitOpenPeriod(MasaPajak, Tahun);

            if (model.InfoType != RequestResultInfoType.Success)
            {
                return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var dataToSave = new OpenClosePeriod()
                {
                    MasaPajak = Convert.ToInt32(MasaPajak),
                    TahunPajak = Convert.ToInt32(Tahun),
                };

                using (var eScope = new TransactionScope())
                {
                    SaveOpenPeriod(dataToSave);
                    eScope.Complete();
                    eScope.Dispose();
                }
                model.Message = "Open Period has been created.";
            }
            catch (Exception ex)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Fatal, ex.Message, MethodBase.GetCurrentMethod(), ex);
                model.Message = "Submit data failed";
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
            }

            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }

        [AuthActivity("34,35")]
        public JsonResult SubmitOpenCloseRegular(List<int> OpenClosePeriodIds, string IsOpen)
        {
            return SaveOpenCloseRegular(OpenClosePeriodIds, IsOpen);
        }

        #region --------------- Private Methods --------------

        private JsonResult GetListOpenClosePeriod(string sEcho, int iDisplayStart, int iDisplayLength, string MasaPajakStart, string MasaPajakEnd, string TahunPajakStart, string TahunPajakEnd)
        {
            var filter = new Logic.Utilities.Filter
            {
                CurrentPage = (iDisplayStart / iDisplayLength) + 1,
                ItemsPerPage = iDisplayLength,
                SortOrderAsc = Request["sSortDir_0"] == "desc",
                SortColumn = Convert.ToInt32(Request["iSortCol_0"]),
                Search = HttpUtility.UrlDecode(Request["sSearch"]),
                SortColumnName = "MasaPajak"
            };

            switch (Convert.ToInt32(Request["iSortCol_0"]))
            {
                case 1: filter.SortColumnName = "MasaPajak"; break;
                case 2: filter.SortColumnName = "TahunPajak"; break;
                case 3: filter.SortColumnName = "StatusRegularText"; break;
                case 4: filter.SortColumnName = "StatusSp2Text"; break;
            }
            int totalItems;

            int? intNull = null;
            int? iMasaPajakStart = !string.IsNullOrEmpty(MasaPajakStart) ? MasaPajakStart != "0" ? Convert.ToInt32(MasaPajakStart) : intNull : intNull;
            int? iMasaPajakEnd = !string.IsNullOrEmpty(MasaPajakEnd) ? MasaPajakEnd != "0" ? Convert.ToInt32(MasaPajakEnd) : intNull : intNull;
            int? iTahunPajakStart = !string.IsNullOrEmpty(TahunPajakStart) ? Convert.ToInt32(TahunPajakStart) : intNull;
            int? iTahunPajakEnd = !string.IsNullOrEmpty(TahunPajakEnd) ? Convert.ToInt32(TahunPajakEnd) : intNull;

            List<OpenClosePeriod> listOpenClosePeriod = OpenClosePeriods.GetListOpenClosePeriod(filter, out totalItems, iMasaPajakStart, iMasaPajakEnd, iTahunPajakStart, iTahunPajakEnd);

            return Json(new
            {
                sEcho = sEcho,
                iTotalRecords = totalItems,
                iTotalDisplayRecords = totalItems,
                aaData = listOpenClosePeriod
            },
            JsonRequestBehavior.AllowGet);
        }

        private RequestResultModel PrivValidationOnCloseSp2(string openClosePeriodId)
        {
            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = ""
            };

            var msgs = new List<string>();

            if (string.IsNullOrEmpty(openClosePeriodId))
            {
                msgs.Add("Open Close Period Kosong");
            }
            else
            {
                int id;
                if (int.TryParse(openClosePeriodId, out id))
                {
                    var dbCheck = OpenClosePeriods.GetById(id);
                    if (dbCheck == null)
                    {
                        msgs.Add("Open Close Period Tidak Ditemukan");
                    }
                    else
                    {
                        if (dbCheck.StatusRegular)
                        {
                            msgs.Add("Status Regular harus diclose periode dahulu sebelum close Status SP2");
                        }
                        else
                        {
                            if (!dbCheck.StatusSp2)
                            {
                                msgs.Add("Sudah Close Status SP2");
                            }
                        }
                    }
                }
                else
                {
                    msgs.Add("Open Close Period Salah");
                }
            }

            if (msgs.Count > 0)
            {
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = string.Join("<br />", msgs);
            }

            return model;
        }

        private RequestResultModel ValidationSubmitOpenPeriod(string masaPajak, string tahunPajak)
        {
            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = ""
            };

            var msgs = new List<string>();

            if (string.IsNullOrEmpty(masaPajak))
            {
                msgs.Add("Masa Pajak Mandatory");
            }
            if (string.IsNullOrEmpty(tahunPajak))
            {
                msgs.Add("Tahun Pajak Mandatory");
            }
            if (msgs.Count > 0)
            {
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = string.Join("<br />", msgs);

                return model;
            }

            //check is data already exists
            OpenClosePeriod dataToValidation = OpenClosePeriods.GetByMasaPajak(Convert.ToInt32(masaPajak), Convert.ToInt32(tahunPajak));

            if (dataToValidation != null)
            {
                model.Message = "Masa & Tahun Pajak is already exsists";
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
            }
            return model;
        }

        private void SaveOpenPeriod(OpenClosePeriod info)
        {
            var userName = Session["UserName"] as string;

            var dataToSave = new OpenClosePeriod
            {
                OpenClosePeriodId = 0,
                MasaPajak = info.MasaPajak,
                TahunPajak = info.TahunPajak,
                StatusSp2 = true, //default data
                StatusRegular = true,
                CreatedBy = userName, //default data
            };

            OpenClosePeriods.Save(dataToSave);
        }

        private JsonResult SaveOpenCloseRegular(List<int> OpenClosePeriodIds, string IsOpen)
        {
            var userName = Session["UserName"] as string;
            var openRegStats = Convert.ToBoolean(IsOpen);
            var model = ValidationSaveOpenCloseRegular(OpenClosePeriodIds, openRegStats);

            if (model.InfoType != RequestResultInfoType.Success)
            {
                return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
            }

            int minpelaporan = 1;
            int maxpelaporan = 1;

            var configData = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.PelaporanTglFaktur);
            if (configData == null)
            {
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = "GeneralConfig [PelaporanTglFaktur] not found.";
                return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
            }

            var dats = configData.ConfigValue.Split(':').ToList();
            if (dats.Count != 2)
            {
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = "GeneralConfig [PelaporanTglFaktur] not valid.";
                return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
            }

            minpelaporan = int.Parse(dats[0].Replace("[", "").Replace("]", ""));
            maxpelaporan = int.Parse(dats[1].Replace("[", "").Replace("]", ""));

            var msgs = new List<string>();

            try
            {
                //CR DZ 325-328
                TransactionOptions options = new TransactionOptions();
                options.Timeout = new TimeSpan(0, 15, 0);
                using (var eScope = new TransactionScope(TransactionScopeOption.Required, options))
                //using (var eScope = new TransactionScope())

                {
                    //open close regular
                    foreach (var t in OpenClosePeriodIds)
                    {
                        var openCloseData = OpenClosePeriods.GetById(t);

                        OpenClosePeriods.UpdateStatusRegular(t, openRegStats, userName);
                        var dtmin = new DateTime(openCloseData.TahunPajak, openCloseData.MasaPajak, 1).AddMonths(minpelaporan);
                        var dtmax =
                            new DateTime(openCloseData.TahunPajak, openCloseData.MasaPajak, 1).AddMonths(maxpelaporan + 1).AddDays(-1);

                        //ada logic untuk Update Status FP Diganti Outstanding set ke Expired jika close, Outstanding jika open
                        var fpdigantiouststandingdat = FakturPajakDigantiOutstandings.GetByDateRange(dtmin, dtmax);
                        if (fpdigantiouststandingdat.Count > 0)
                        {
                            if (openRegStats)
                            {
                                //open period, set Expired to Outstanding
                                var dattoupdate =
                                    fpdigantiouststandingdat.Where(
                                        c =>
                                            c.StatusOutstanding ==
                                            (int) ApplicationEnums.StatusDigantiOutstanding.Expired).ToList();

                                if (dattoupdate.Count > 0)
                                {
                                    foreach (var x in dattoupdate)
                                    {
                                        //update from expired to outstanding
                                        FakturPajakDigantiOutstandings.UpdateStatusByFormatedNoFaktur(
                                            x.FormatedNoFaktur, ApplicationEnums.StatusDigantiOutstanding.Outstanding,
                                            x.Keterangan, userName);
                                    }
                                }
                            }
                            else
                            {
                                var dattoupdate =
                                    fpdigantiouststandingdat.Where(
                                        c =>
                                            c.StatusOutstanding ==
                                            (int)ApplicationEnums.StatusDigantiOutstanding.Outstanding).ToList();
                                if (dattoupdate.Count > 0)
                                {
                                    foreach (var x in dattoupdate)
                                    {
                                        var periodyear = x.TglFaktur.Year;
                                        var periodmonth = x.TglFaktur.Month;
                                        var dtminx = new DateTime(periodyear, periodmonth, 1).AddMonths(maxpelaporan);
                                        var dtmaxx =
                                            new DateTime(periodyear, periodmonth, 1).AddMonths(Math.Abs(minpelaporan));
                                        var availableperiods = OpenClosePeriods.GetByRange(dtminx, dtmaxx);
                                        if (availableperiods.Count > 0)
                                        {
                                            //jika semua nya open maka langsung expired
                                            var chkopencloseperiod =
                                                availableperiods.Where(c => c.StatusRegular).ToList();
                                            if (chkopencloseperiod.Count <= 0)
                                            {
                                                //update to expired
                                                FakturPajakDigantiOutstandings.UpdateStatusByFormatedNoFaktur(
                                                    x.FormatedNoFaktur,
                                                    ApplicationEnums.StatusDigantiOutstanding.Expired, x.Keterangan,
                                                    userName);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        msgs.Add("Period " + openCloseData.MonthName + " - " + openCloseData.TahunPajak + " berhasil " + (openRegStats ? "Open" : "Close") + " Regular.");
                    }

                    eScope.Complete();
                    eScope.Dispose();
                }
            }
            catch (Exception ex)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Fatal, ex.Message, MethodBase.GetCurrentMethod(), ex);
                model.Message = "Submit data failed";
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
            }

            if (msgs.Count > 0)
            {
                model.Message = string.Join("<br />", msgs);
            }

            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }

        private RequestResultModel ValidationSaveOpenCloseRegular(List<int> OpenClosePeriodIds, bool IsOpen)
        {
            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = string.Empty
            };

            var msgs = new List<string>();

            if (OpenClosePeriodIds.Count <= 0)
            {
                msgs.Add("No Data");
            }
            else
            {
                if (IsOpen)
                {
                    //validation for open reguler
                    foreach (var item in OpenClosePeriodIds)
                    {
                        var dbCheck = OpenClosePeriods.GetById(item);
                        if (dbCheck.StatusRegular)
                        {
                            msgs.Add("Period " + dbCheck.MonthName + "-" + dbCheck.TahunPajak + " sedang Open Period");
                        }
                        else
                        {
                            if (!dbCheck.StatusSp2)
                            {
                                msgs.Add("Period " + dbCheck.MonthName + "-" + dbCheck.TahunPajak + " sudah Close SP2");
                            }
                            else
                            {
                                //check jika sudah create SPM
                                var chkSpm = ReportSuratPemberitahuanMasas.GetLastDataByMasaPajak(dbCheck.MasaPajak,
                                    dbCheck.TahunPajak);
                                if (chkSpm != null)
                                {
                                    msgs.Add("Period " + dbCheck.MonthName + "-" + dbCheck.TahunPajak + " sudah Create SPM");
                                }
                            }
                        }
                    }
                }
                else
                {
                    //validation for close reguler
                    foreach (var item in OpenClosePeriodIds)
                    {
                        var dbCheck = OpenClosePeriods.GetById(item);
                        if (!dbCheck.StatusRegular)
                        {
                            msgs.Add("Period " + dbCheck.MonthName + "-" + dbCheck.TahunPajak + " sedang Close Period");
                        }
                    }
                }
                
            }

            if (msgs.Count > 0)
            {
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = string.Join("<br />", msgs);
            }

            return model;
        }

        #endregion
    }
}
