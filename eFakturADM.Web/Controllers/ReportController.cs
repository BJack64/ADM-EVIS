using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Objects;

namespace eFakturADM.Web.Controllers
{
    public class ReportController : BaseController
    {
        [AuthActivity("52")]
        public ActionResult SuratPemberitahuanMasa()
        {
            return View();
        }
        [AuthActivity("54")]
        public ActionResult DetailFakturPajak()
        {
            return View();
        }

        [AuthActivity("56")]
        public ActionResult FakturPajakMasukan()
        {
            return View();
        }

        [AuthActivity("58")]
        public ActionResult FakturPajakOutstanding()
        {
            return View();
        }

        [AuthActivity("60")]
        public ActionResult FakturPajakBelumDiJurnal()
        {
            return View();
        }

        public JsonResult GetListDetailFakturPajak(string firstLoad, string sEcho, int iDisplayStart, int iDisplayLength, string noFaktur1, string noFaktur2, string NPWP,
            string Nama, string TglFakturStart, string TglFakturEnd, string MasaPajak, string TahunPajak, string scanDateAwal, string scanDateAkhir)
        {
            if (Convert.ToBoolean(firstLoad))
            {
                return Json(new
                {
                    sEcho = sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = new List<ReportDetailFakturPajak>()
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
                SortColumnName = "FormatedNoFaktur"
            };

            switch (Convert.ToInt32(Request["iSortCol_0"]))
            {
                case 1: filter.SortColumnName = "FormatedNoFaktur"; break;
                case 2: filter.SortColumnName = "Nama"; break;
                case 3: filter.SortColumnName = "HargaSatuan"; break;
                case 4: filter.SortColumnName = "Diskon"; break;
                case 5: filter.SortColumnName = "JumlahDPP"; break;
                case 6: filter.SortColumnName = "JumlahPPN"; break;
                case 7: filter.SortColumnName = "JumlahPPNBM"; break;
                case 8: filter.SortColumnName = "Dpp"; break;
                case 9: filter.SortColumnName = "Ppn"; break;
                case 10: filter.SortColumnName = "Ppnbm"; break;
                case 11: filter.SortColumnName = "TarifPpnbm"; break;
                case 12: filter.SortColumnName = "FormatedNpwpPenjual"; break;
                case 13: filter.SortColumnName = "NamaPenjual"; break;
                case 14: filter.SortColumnName = "FillingIndex"; break;
            }
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
            int? masaPajak = string.IsNullOrEmpty(MasaPajak) || MasaPajak == "0" ? (int?)null : int.Parse(MasaPajak);
            int? tahunPajak = string.IsNullOrEmpty(TahunPajak) || TahunPajak == "0" ? (int?)null : int.Parse(TahunPajak);


            var datList = ReportDetailFakturPajaks.GetList(filter, out totalItems, noFaktur1, noFaktur2, tglFakturStart, tglFakturEnd, NPWP,
                Nama, masaPajak, tahunPajak, dscanDateAwal, dscanDateAkhir);

            return Json(new
            {
                sEcho = sEcho,
                iTotalRecords = totalItems,
                iTotalDisplayRecords = totalItems,
                aaData = datList
            },
            JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetListFakturPajakMasukan(string firstLoad, string sEcho, int iDisplayStart, int iDisplayLength, string sTglFakturStart, string sTglFakturEnd, string picEntry, string fillingIndexStart, string fillingIndexEnd,
            string masaPajak, string tahunPajak)
        {
            if (Convert.ToBoolean(firstLoad))
            {
                return Json(new
                {
                    sEcho = sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = new List<ReportFakturPajakMasukan>()
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
                SortColumnName = "NamaPenjual"
            };

            switch (Convert.ToInt32(Request["iSortCol_0"]))
            {
                case 1: filter.SortColumnName = "NamaPenjual"; break;
                case 2: filter.SortColumnName = "FormatedNpwpPenjual"; break;
                case 3: filter.SortColumnName = "FormatedNoFaktur"; break;
                case 4: filter.SortColumnName = "TglFaktur"; break;
                case 5: filter.SortColumnName = "JumlahPPN"; break;
                case 6: filter.SortColumnName = "CreatedBy"; break;
                case 7: filter.SortColumnName = "MasaPajak"; break;
                case 8: filter.SortColumnName = "TahunPajak"; break;
                case 9: filter.SortColumnName = "FillingIndex"; break;
                case 10: filter.SortColumnName = "Dikreditkan"; break;
            }
            int totalItems;

            DateTime? dTglFakturStart = string.IsNullOrEmpty(sTglFakturStart)
                ? (DateTime?) null
                : Convert.ToDateTime(sTglFakturStart);

            DateTime? dTglFakturEnd = string.IsNullOrEmpty(sTglFakturEnd)
                ? (DateTime?)null
                : Convert.ToDateTime(sTglFakturEnd);

            int? iMasaPajak = null;
            int? iTahunPajak = null;
            var strSearchPic = string.Empty;
            if (!string.IsNullOrEmpty(picEntry) && !(picEntry.ToLower() == "undefine" && picEntry.ToLower() == "undefined" && picEntry.ToLower() == "all"))
            {
                strSearchPic = picEntry;
            }
            if (!string.IsNullOrEmpty(masaPajak) && !(masaPajak == "undefine" || masaPajak == "undefined" || masaPajak == "0"))
            {
                int d;
                if (int.TryParse(masaPajak, out d))
                {
                    iMasaPajak = d;
                }
            }
            if (!string.IsNullOrEmpty(tahunPajak) && !(tahunPajak == "undefine" && tahunPajak == "undefined" && tahunPajak == "0"))
            {
                int d;
                if (int.TryParse(tahunPajak, out d))
                {
                    iTahunPajak = d;
                }
            }

            var datList = ReportFakturPajakMasukans.GetList(filter, out totalItems, dTglFakturStart, dTglFakturEnd,
                strSearchPic, fillingIndexStart, fillingIndexEnd, iMasaPajak, iTahunPajak);

            return Json(new
            {
                sEcho = sEcho,
                iTotalRecords = totalItems,
                iTotalDisplayRecords = totalItems,
                aaData = datList
            },
            JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetListFakturPajakOutstanding(string firstLoad, string sEcho, int iDisplayStart, int iDisplayLength, string sPostingDateStart, string sPostingDateEnd, string docSapStart, string docSapEnd)
        {
            if (Convert.ToBoolean(firstLoad))
            {
                return Json(new
                {
                    sEcho = sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = new List<ReportFakturPajakOutstanding>()
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
                SortColumnName = "GLAccount"
            };

            switch (Convert.ToInt32(Request["iSortCol_0"]))
            {
                case 1: filter.SortColumnName = "GLAccount"; break;
                case 2: filter.SortColumnName = "AccountingDocNo"; break;
                case 3: filter.SortColumnName = "PostingDate"; break;
                case 4: filter.SortColumnName = "AmountLocal"; break;
                case 5: filter.SortColumnName = "TaxInvoiceNumber"; break;
                case 6: filter.SortColumnName = "TglFaktur"; break;
                case 7: filter.SortColumnName = "AssignmentNo"; break;
            }
            int totalItems;

            DateTime? dPostingDateStart = string.IsNullOrEmpty(sPostingDateStart)
                ? (DateTime?)null
                : Convert.ToDateTime(sPostingDateStart);

            DateTime? dPostingDateEnd = string.IsNullOrEmpty(sPostingDateEnd)
                ? (DateTime?)null
                : Convert.ToDateTime(sPostingDateEnd);

            var datList = ReportFakturPajakOutstandings.GetList(filter, out totalItems, dPostingDateStart,
                dPostingDateEnd, docSapStart, docSapEnd);

            return Json(new
            {
                sEcho = sEcho,
                iTotalRecords = totalItems,
                iTotalDisplayRecords = totalItems,
                aaData = datList
            },
            JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetListFakturPajakBelumDiJurnal(string firstLoad, string sEcho, int iDisplayStart, int iDisplayLength,string tglFakturStart, string tglFakturEnd, string noFakturStart, string noFakturEnd)
        {
            
            if (Convert.ToBoolean(firstLoad))
            {
                return Json(new
                {
                    sEcho = sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = new List<ReportFakturPajakBelumDiJurnal>()
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
                SortColumnName = "GLAccount"
            };

            switch (Convert.ToInt32(Request["iSortCol_0"]))
            {
                case 1: filter.SortColumnName = "FormatedNpwpPenjual"; break;
                case 2: filter.SortColumnName = "NamaPenjual"; break;
                case 4: filter.SortColumnName = "FormatedNoFaktur"; break;
                case 5: filter.SortColumnName = "CreatedBy"; break;
                case 6: filter.SortColumnName = "FillingIndex"; break;
                case 7: filter.SortColumnName = "TglFaktur"; break;
                case 10: filter.SortColumnName = "JumlahPPN"; break;
            }
            int totalItems;

            DateTime? dTglFakturStart = string.IsNullOrEmpty(tglFakturStart)
                ? (DateTime?) null
                : Convert.ToDateTime(tglFakturStart);

            DateTime? dTglFakturEnd = string.IsNullOrEmpty(tglFakturEnd)
                ? (DateTime?)null
                : Convert.ToDateTime(tglFakturEnd);

            var dats = ReportFakturPajakBelumDiJurnals.GetList(filter, out totalItems, dTglFakturStart, dTglFakturEnd,
                noFakturStart, noFakturEnd);
            return Json(new
            {
                sEcho = sEcho,
                iTotalRecords = totalItems,
                iTotalDisplayRecords = totalItems,
                aaData = dats
            },
            JsonRequestBehavior.AllowGet);

        }

        #region ------------- Report Surat Pemberitahuan Masa --------------
        
        public JsonResult GetSpmInfo(string firstLoad,string masaPajak, string tahunPajak, string versi)
        {
            if (Convert.ToBoolean(firstLoad))
            {
                return Json(new
                {
                    NamaMasaPajak = "",
                    TahunPajak = "",
                    Versi = "",
                    aaData = new List<ReportSuratPemberitahuanMasaDetail>(),
                    IsError = false,
                    MessageError = ""
                },
            JsonRequestBehavior.AllowGet);
            }
            var msgs = new List<string>();
            if (string.IsNullOrEmpty(masaPajak) || masaPajak == "undefined")
            {
                msgs.Add("Masa Pajak Mandatory");
            }
            if (string.IsNullOrEmpty(tahunPajak) || tahunPajak == "undefined")
            {
                msgs.Add("Tahun Pajak Mandatory");
            }
            if (string.IsNullOrEmpty(versi) || versi == "undefined")
            {
                msgs.Add("Versi Mandatory");
            }
            if (msgs.Count > 0)
            {
                return Json(new
                {
                    NamaMasaPajak = "",
                    TahunPajak = "",
                    Versi = "",
                    aaData = new List<ReportSuratPemberitahuanMasaDetail>(),
                    IsError = true,
                    MessageError = string.Join("<br />", msgs)
                }, JsonRequestBehavior.AllowGet); 
            }

            int iMasaPajak;
            if (!int.TryParse(masaPajak, out iMasaPajak))
            {
                msgs.Add("Request Parameter tidak valid [masaPajak]");
            }
            int iTahunPajak;
            if (!int.TryParse(tahunPajak, out iTahunPajak))
            {
                msgs.Add("Request Parameter tidak valid [tahunPajak]");
            }
            int iVersi;
            if (!int.TryParse(versi, out iVersi))
            {
                msgs.Add("Request Parameter tidak valid [versi]");
            }
            if (msgs.Count > 0)
            {
                return Json(new
                {
                    NamaMasaPajak = "",
                    TahunPajak = "",
                    Versi = "",
                    aaData = new List<ReportSuratPemberitahuanMasaDetail>(),
                    IsError = true,
                    MessageError = string.Join("<br />", msgs)
                }, JsonRequestBehavior.AllowGet); 
            }
            var getDataSpm = ReportSuratPemberitahuanMasas.GetSpecificSpm(iMasaPajak, iTahunPajak, iVersi);
            if (getDataSpm == null)
            {
                var monthDat = MasaPajaks.GetByMonthNumber(iMasaPajak);
                return Json(new
                {
                    NamaMasaPajak = monthDat.MonthName,
                    TahunPajak = tahunPajak,
                    Versi = versi,
                    aaData = new List<ReportSuratPemberitahuanMasaDetail>(),
                    IsError = false,
                    MessageError = ""
                }, JsonRequestBehavior.AllowGet); 
            }

            var dats = ReportSuratPemberitahuanMasaDetails.GetBySpmId(getDataSpm.Id);
            
            var jsonResult = Json(new
            {
                getDataSpm.NamaMasaPajak,
                TahunPajak = tahunPajak,
                Versi = versi,
                aaData = dats,
                IsError = false,
                MessageError = ""
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }

        #endregion

    }
}
