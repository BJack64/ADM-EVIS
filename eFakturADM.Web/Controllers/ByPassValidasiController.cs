using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Web.Mvc;
using eFakturADM.DJPLib.Objects;
using eFakturADM.FileManager;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Logic.Utilities;
using eFakturADM.Shared.Utility;
using eFakturADM.Web.Helpers;
using eFakturADM.Web.Models;
using System.Transactions;
using System.Web;
using System.Linq;

namespace eFakturADM.Web.Controllers
{
    public class ByPassValidasiController : BaseController
    {
        [AuthActivity("71")]
        public ActionResult Index()
        {
            return View();
        }


        public JsonResult GetListByPassValidasiDataTable(string firstLoad, string sEcho, int iDisplayStart, int iDisplayLength,
            string NoFaktur1, string NoFaktur2,
            string TglFakturStart, string TglFakturEnd,
            string ReceivedDateAwal, string ReceivedDateAkhir,
            string Source, string CheckingStatus,
            string NPWP, string Nama,  string Status,  
            string sSearch_1, string sSearch_2, string sSearch_3, string sSearch_4,
            string sSearch_5, string sSearch_6, string sSearch_7,string sSearch_8, 
            string sSearch_9, string sSearch_10, string sSearch_11, string sSearch_12, string sSearch_13)
        {
            if (Convert.ToBoolean(firstLoad))
            {
                return Json(new
                {
                    sEcho = sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = new List<ByPassValidasi>()
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
                SortColumnName = "FormatedNpwpPenjual"
            };

            switch (Convert.ToInt32(Request["iSortCol_0"]))
            {
                case 0: filter.SortColumnName = "NPWPVendor"; break;
                case 1: filter.SortColumnName = "VendorName"; break;
                case 2: filter.SortColumnName = "FPdjpNumber"; break;
                case 3: filter.SortColumnName = "FPdjpDate"; break;
                case 4: filter.SortColumnName = "DPP"; break;
                case 5: filter.SortColumnName = "PPN"; break;
                case 6: filter.SortColumnName = "Sources"; break;
                case 7: filter.SortColumnName = "Status"; break;
                case 8: filter.SortColumnName = "CheckingStatus"; break;
                case 9: filter.SortColumnName = "CheckingCount"; break;
                case 10: filter.SortColumnName = "CheckingStart"; break;
                case 11: filter.SortColumnName = "CheckingLast"; break;
                case 12: filter.SortColumnName = "IsByPass"; break;
            }

            string _FormatedNpwpPenjual = sSearch_1;
            string _NamaPenjual = sSearch_2;
            string _FormatedNoFaktur = sSearch_3;
            string _TglFakturString = sSearch_4;
            string _DPPString = sSearch_5;
            string _PPNString = sSearch_6;
            string _Source = sSearch_7;
            string _Status = sSearch_8;
            string _CheckingStatus = sSearch_9;
            string _CheckingCount = sSearch_10;
            string _CheckingStart = sSearch_11;
            string _CheckingLast = sSearch_12;
            string _IsByPass = sSearch_13;

            int totalItems;

            DateTime? tglFakturStart = !string.IsNullOrEmpty(TglFakturStart) ? Convert.ToDateTime(TglFakturStart) : (DateTime?)null;
            DateTime? tglFakturEnd = !string.IsNullOrEmpty(TglFakturEnd) ? Convert.ToDateTime(TglFakturEnd) : (DateTime?)null;

            DateTime? receivedDtAwal = !string.IsNullOrEmpty(ReceivedDateAwal) ? Convert.ToDateTime(ReceivedDateAwal) : (DateTime?)null;
            DateTime? receivedDtAkhir = !string.IsNullOrEmpty(ReceivedDateAkhir) ? Convert.ToDateTime(ReceivedDateAkhir) : (DateTime?)null;


            string logKey;
            Logger.WriteLog(out logKey, LogLevel.Error, "Start Get ByPassValidasi", MethodBase.GetCurrentMethod());

            List<ByPassValidasi> listFakturPajak = ByPassValidasiCollection.GetList(filter, out totalItems,
                NoFaktur1, NoFaktur2,
                tglFakturStart, tglFakturEnd,
                receivedDtAwal, receivedDtAkhir,
                Source, CheckingStatus,
                NPWP, Nama, Status,
                sSearch_1, sSearch_2, sSearch_3, sSearch_4,
                sSearch_5, sSearch_6, sSearch_7, sSearch_8,
                sSearch_9, sSearch_10, sSearch_11, sSearch_12, sSearch_13
            ).ToList();

            Logger.WriteLog(out logKey, LogLevel.Error, "End Get ByPassValidasi : " + totalItems + " records", MethodBase.GetCurrentMethod());
            return Json(new
            {
                sEcho = sEcho,
                iTotalRecords = totalItems,
                iTotalDisplayRecords = totalItems,
                aaData = listFakturPajak
            },
            JsonRequestBehavior.AllowGet);
        }

        [AuthActivity("72")]
        public JsonResult ByPassValidasiByIds(List<string> FPdjpIDs)
        {
            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = "By Pass Validasi has done."
            };

            if (FPdjpIDs.Count <= 0)
            {
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = "No Data Selected";
                return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
            }
            string msgs = "";
     
            try
            {
                using (var escope = new TransactionScope())
                {
                    var GetValidasi = ByPassValidasiCollection.CheckingStatusPendingValidation(string.Join(",", FPdjpIDs));
                    GetValidasi = "";
                    if (string.IsNullOrEmpty(GetValidasi))
                    {
                        var userName = Session["UserName"] as string;
                        ByPassValidasiCollection.ByPassValidasiDelvi(string.Join(",", FPdjpIDs), userName);
                    }
                    else {
                        model.Message = string.Concat("No faktur ",GetValidasi," tidak bisa di bypass, silahkan cek ulang");
                        model.InfoType = RequestResultInfoType.Warning;
                    }
                    escope.Complete();

                }
            }
            catch (Exception ex)
            {
                msgs= ex.Message;
            }

            if (!string.IsNullOrEmpty(msgs))
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
