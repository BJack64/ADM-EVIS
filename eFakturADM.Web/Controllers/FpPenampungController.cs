using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Objects;
using eFakturADM.Shared.Utility;
using eFakturADM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace eFakturADM.Web.Controllers
{
    public class FpPenampungController : BaseController
    {
        //
        // GET: /FpPenampung/
        //#if TOTAL_DEV

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetListFakturPajakPenampungDataTable(string firstLoad, string sEcho, int iDisplayStart, int iDisplayLength, string NoFaktur1, string NoFaktur2, string NPWP,
            string Nama, string TglFakturStart, string TglFakturEnd, string Status, string scanDateAwal, string scanDateAkhir, string source, string remark, string sSearch_1,
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
                    aaData = new List<FakturPajakPenampung>()
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
                case 0: filter.SortColumnName = "NPWPLawanTransaksi"; break;
                case 1: filter.SortColumnName = "NamaLawanTransaksi"; break;
                case 2: filter.SortColumnName = "NoFakturPajak"; break;
                case 3: filter.SortColumnName = "TglFaktur"; break;
                case 4: filter.SortColumnName = "JumlahDPP"; break;
                case 5: filter.SortColumnName = "JumlahPPN"; break;
                case 6: filter.SortColumnName = "JumlahPPNBM"; break;
            }

            string _FormatedNpwpPenjual = sSearch_1;
            string _NamaPenjual = sSearch_2;
            string _FormatedNoFaktur = sSearch_3;
            string _TglFakturString = sSearch_4;
            string _DPPString = sSearch_5;
            string _PPNString = sSearch_6;
            string _PPNBMString = sSearch_7;
            string _StatusFaktur = sSearch_8;
            string _SUserName = sSearch_9;
            string _SSource = sSearch_10;
            string _SStatusPayment = sSearch_11;
            string _Remark = sSearch_12;

            int totalItems;

            DateTime? tglFakturStart = !string.IsNullOrEmpty(TglFakturStart) ? Convert.ToDateTime(TglFakturStart) : (DateTime?)null;
            DateTime? tglFakturEnd = !string.IsNullOrEmpty(TglFakturEnd) ? Convert.ToDateTime(TglFakturEnd) : (DateTime?)null;
            DateTime? dscanDateAwal = null;
            DateTime? dscanDateAkhir = null;
            

            if (!string.IsNullOrEmpty(scanDateAwal) && scanDateAwal != "undefined")
            {
                dscanDateAwal = Convert.ToDateTime(scanDateAwal);
            }

            if (!string.IsNullOrEmpty(scanDateAkhir) && scanDateAkhir != "undefined")
            {
                dscanDateAkhir = Convert.ToDateTime(scanDateAkhir);
            }

            string logKey;
            Logger.WriteLog(out logKey, LogLevel.Error, "Start Get FP Penampung", MethodBase.GetCurrentMethod());

            List<FakturPajakPenampung> listFakturPajak = FakturPajakPenampungs.GetList(filter, out totalItems, NoFaktur1, NoFaktur2, NPWP, Nama, tglFakturStart, tglFakturEnd, Status, dscanDateAwal, dscanDateAkhir, source, remark,
                _FormatedNpwpPenjual, _NamaPenjual, _FormatedNoFaktur, _TglFakturString, _DPPString, _PPNString, _PPNBMString, _StatusFaktur, _SUserName, _SSource, _SStatusPayment, _Remark).ToList();

            Logger.WriteLog(out logKey, LogLevel.Error, "End Get FP Penampung: " + totalItems + " records", MethodBase.GetCurrentMethod());
            return Json(new
            {
                sEcho = sEcho,
                iTotalRecords = totalItems,
                iTotalDisplayRecords = totalItems,
                aaData = listFakturPajak
            },
            JsonRequestBehavior.AllowGet);
        }

        public JsonResult RestoreFakturPajakPenampung(List<long> fakturPajakPenampungIds, bool selectAll, string noFaktur1, string noFaktur2, string npwp,
            string nama, string tglStart, string tglEnd,
            string status, string scanDateAwal, string scanDateAkhir, string source, string remark,
            string fNpwpPenjual, string fNamaPenjual,
            string fNoFaktur, string fTglFaktur, string fDppString, string fPpnString,
            string fPpnBmString, string fStatusFaktur, string fUserName, string fSource, string fStatusPayment, string fRemarks, string fpIds)
        {
            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = "Faktur Pajak has been resored."
            };

            if (fakturPajakPenampungIds.Count <= 0)
            {
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = "No Data Selected";
                return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
            }
            //delete data
            var msgs = new List<string>();
            //do delete
            try
            {
                var userName = Session["UserName"] as string;
                FakturPajakPenampungs.Restore(!selectAll ? string.Join(",", fakturPajakPenampungIds.Select(x => x.ToString())) : null, 
                userName ?? "SYSTEM", noFaktur1, noFaktur2, npwp,
                nama, tglStart, tglEnd,
                status, scanDateAwal, scanDateAkhir, source, remark,
                fNpwpPenjual, fNamaPenjual,
                fNoFaktur, fTglFaktur, fDppString, fPpnString,
                fPpnBmString, fStatusFaktur, fUserName, fSource, fStatusPayment, fRemarks);
            }
            catch (Exception ex)
            {
                msgs.Add(ex.Message);
            }

            if (msgs.Count > 0)
            {
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = string.Join("<br />", msgs);
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, model.Message, MethodBase.GetCurrentMethod());
            }
            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }

    }
}
