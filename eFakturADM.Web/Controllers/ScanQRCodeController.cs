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
    public class ScanQRCodeController : BaseController
    {
        [AuthActivity("1")]
        public ActionResult ScanQRCodeSatuanIWS()
        {
            return View();
        }

        [AuthActivity("2")]
        public ActionResult ScanQRCodeBulkIWS()
        {
            return View();
        }

        [AuthActivity("4")]
        public ActionResult ScanQRCodeSatuanNonIWS()
        {
            return View();
        }

        [AuthActivity("5")]
        public ActionResult ScanQRCodeBulkNonIWS()
        {
            return View();
        }

        [AuthActivity("7")]
        public ActionResult ScanManual()
        {
            string ppnSetting = System.Configuration.ConfigurationManager.AppSettings["ppnSetting"];
            ViewBag.PpnSetting = ppnSetting;

            return View();
        }

        [AuthActivity("3")]
        public ActionResult PembetulanQRCodeSatuanIWS()
        {
            return View();
        }

        [AuthActivity("6")]
        public ActionResult PembetulanQRCodeSatuanNonIWS()
        {
            return View();
        }

        [AuthActivity("8")]
        public ActionResult PembetulanScanManual()
        {
            string ppnSetting = System.Configuration.ConfigurationManager.AppSettings["ppnSetting"];
            ViewBag.PpnSetting = ppnSetting;

            return View();
        }

        [AuthActivity("9")]
        public ActionResult ListFakturPajak()
        {
            return View();
        }

        ////[AuthActivity("10")]
        //public ActionResult ListFakturPajakDigantiOutstanding()
        //{
        //    return View();
        //}
        public JsonResult BrowseFakturPajakDialog()
        {
            return Json(new
            {
                Html = this.RenderPartialView(@"BrowseFakturPajak", null),

            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BrowseFakturPajakKhususDialog()
        {
            return Json(new
            {
                Html = this.RenderPartialView(@"BrowseFakturPajakKhusus", null),

            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetListBrowseFakturPajakDataTable(string sEcho, int iDisplayStart, int iDisplayLength, string TglFaktur, string NPWP, string Nama)
        {
            var filter = new Logic.Utilities.Filter
            {
                CurrentPage = (iDisplayStart / iDisplayLength) + 1,
                ItemsPerPage = iDisplayLength,
                SortOrderAsc = Request["sSortDir_0"] == "desc",
                SortColumn = Convert.ToInt32(Request["iSortCol_0"]),
                Search = HttpUtility.UrlDecode(Request["sSearch"]),
                SortColumnName = "TglFaktur"
            };


            switch (Convert.ToInt32(Request["iSortCol_0"]))
            {
                case 0: filter.SortColumnName = "TglFaktur"; break;
                case 1: filter.SortColumnName = "NoFakturPajak"; break;
                case 2: filter.SortColumnName = "NPWPLawanTransaksi"; break;
                case 3: filter.SortColumnName = "NamaLawanTransaksi"; break;
                case 4: filter.SortColumnName = "KdJenisTransaksi"; break;
                case 5: filter.SortColumnName = "MasaPajak"; break;
                case 6: filter.SortColumnName = "TahunPajak"; break;
                case 7: filter.SortColumnName = "JumlahDPP"; break;
                case 8: filter.SortColumnName = "JumlahPPN"; break;
                case 9: filter.SortColumnName = "JumlahPPNBM"; break;
            }
            int totalItems;

            DateTime? dtNull = null;
            DateTime? tglFaktur = !string.IsNullOrEmpty(TglFaktur) ? Convert.ToDateTime(TglFaktur) : dtNull;

            List<FakturPajak> listFakturPajak = FakturPajaks.GetListBrowse(filter, out totalItems, tglFaktur, NPWP, Nama);

            return Json(new
            {
                sEcho = sEcho,
                iTotalRecords = totalItems,
                iTotalDisplayRecords = totalItems,
                aaData = listFakturPajak
            },
           JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetListBrowseFakturPajakKhususDataTable(string sEcho, int iDisplayStart, int iDisplayLength, string TglFaktur, string NPWP, string Nama)
        {
            var filter = new Logic.Utilities.Filter
            {
                CurrentPage = (iDisplayStart / iDisplayLength) + 1,
                ItemsPerPage = iDisplayLength,
                SortOrderAsc = Request["sSortDir_0"] == "desc",
                SortColumn = Convert.ToInt32(Request["iSortCol_0"]),
                Search = HttpUtility.UrlDecode(Request["sSearch"]),
                SortColumnName = "TglFaktur"
            };


            switch (Convert.ToInt32(Request["iSortCol_0"]))
            {
                case 0: filter.SortColumnName = "TglFaktur"; break;
                case 1: filter.SortColumnName = "NoFakturPajak"; break;
                case 2: filter.SortColumnName = "NPWPLawanTransaksi"; break;
                case 3: filter.SortColumnName = "NamaLawanTransaksi"; break;
                case 4: filter.SortColumnName = "KdJenisTransaksi"; break;
                case 5: filter.SortColumnName = "MasaPajak"; break;
                case 6: filter.SortColumnName = "TahunPajak"; break;
                case 7: filter.SortColumnName = "JumlahDPP"; break;
                case 8: filter.SortColumnName = "JumlahPPN"; break;
                case 9: filter.SortColumnName = "JumlahPPNBM"; break;
            }
            int totalItems;

            DateTime? dtNull = null;
            DateTime? tglFaktur = !string.IsNullOrEmpty(TglFaktur) ? Convert.ToDateTime(TglFaktur) : dtNull;

            List<FakturPajak> listFakturPajak = FakturPajaks.GetListBrowseFpKhusus(filter, out totalItems, tglFaktur, NPWP, Nama);

            return Json(new
            {
                sEcho = sEcho,
                iTotalRecords = totalItems,
                iTotalDisplayRecords = totalItems,
                aaData = listFakturPajak
            },
           JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetListFakturPajakDataTable(string firstLoad, string sEcho, int iDisplayStart, int iDisplayLength, string NoFaktur1, string NoFaktur2, string NPWP,
            string Nama, string TglFakturStart, string TglFakturEnd, string MasaPajak, string TahunPajak, string Status,
            string dataType, string scanDateAwal, string scanDateAkhir, string fillingIndex, string source, string statusPayment, string remark, bool? createdCsv, string StatusPelaporan, string sSearch_1,
            string sSearch_2, string sSearch_3, string sSearch_4, string sSearch_5, string sSearch_6, string sSearch_7, string sSearch_8, string sSearch_9, string sSearch_10
            , string sSearch_11, string sSearch_12, string sSearch_13, string sSearch_14, string sSearch_15, string sSearch_16, string sSearch_17, string sSearch_18)
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
                SortColumnName = "NPWPLawanTransaksi"
            };

            switch (Convert.ToInt32(Request["iSortCol_0"]))
            {
                case 0: filter.SortColumnName = "NPWPLawanTransaksi"; break;
                case 1: filter.SortColumnName = "NamaLawanTransaksi"; break;
                case 2: filter.SortColumnName = "NoFakturPajak"; break;
                case 3: filter.SortColumnName = "TglFaktur"; break;
                case 4: filter.SortColumnName = "MasaPajak"; break;
                case 5: filter.SortColumnName = "TahunPajak"; break;
                case 6: filter.SortColumnName = "JumlahDPP"; break;
                case 7: filter.SortColumnName = "JumlahPPN"; break;
                case 8: filter.SortColumnName = "JumlahPPNBM"; break;
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
            string _SSource = sSearch_13;
            string _SStatusPayment = sSearch_14;
            string _SRemark = sSearch_15;
            string _SCreatedCsv = sSearch_16;
            string _SNamaPelaporan = sSearch_17;
            string _TglFakturString010 = sSearch_18;

            int totalItems;

            DateTime? tglFakturStart = !string.IsNullOrEmpty(TglFakturStart) ? Convert.ToDateTime(TglFakturStart) : (DateTime?)null;
            DateTime? tglFakturEnd = !string.IsNullOrEmpty(TglFakturEnd) ? Convert.ToDateTime(TglFakturEnd) : (DateTime?)null;

            int? idataType = null;
            DateTime? dscanDateAwal = null;
            DateTime? dscanDateAkhir = null;
            int? ifillingIndex = null;

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
                ifillingIndex = int.Parse(fillingIndex);
            }

            int? masaPajak = string.IsNullOrEmpty(MasaPajak) || MasaPajak == "0" ? (int?)null : int.Parse(MasaPajak);
            int? tahunPajak = string.IsNullOrEmpty(TahunPajak) || TahunPajak == "0" ? (int?)null : int.Parse(TahunPajak);
            string logKey;
            Logger.WriteLog(out logKey, LogLevel.Error, "Start Get FP", MethodBase.GetCurrentMethod());
            List<FakturPajak> listFakturPajak = FakturPajaks.GetList(filter, out totalItems, NoFaktur1, NoFaktur2, tglFakturStart, tglFakturEnd, NPWP,
                Nama, masaPajak, tahunPajak, Status, source, statusPayment, remark, createdCsv, StatusPelaporan, _FormatedNpwpPenjual, _NamaPenjual, _FormatedNoFaktur, _TglFakturString, _MasaPajakName, _TahunPajak, _DPPString, _PPNString,
                _PPNBMString, _StatusFaktur, idataType, dscanDateAwal, dscanDateAkhir, ifillingIndex, _SFillingIndex, _SUserName, _SSource, _SStatusPayment, _SRemark, _SCreatedCsv, _SNamaPelaporan, _TglFakturString010).ToList();
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

        public JsonResult MasaPajakDialog(long FakturPajakId, int MasaPajak, int TahunPajak)
        {
            var model = new MasaPajakInfoModel
            {
                FakturPajakId = FakturPajakId,
                MasaPajak = MasaPajak,
                TahunPajak = TahunPajak
            };
            return Json(new
            {
                Html = this.RenderPartialView(@"MasaPajakDialog", model),

            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetMasaPajak(SetMasaPajakModel input)
        {

            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = "Set Masa Pajak Success."
            };
            var checkFakturPajak = FakturPajaks.GetById(input.FakturPajakId);
            if (checkFakturPajak == null)
            {
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = "Faktur Pajak Tidak Ditemukan";
                return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
            }

            if (checkFakturPajak.FakturPajakTerlaporID != null || !checkFakturPajak.StatusRegular)
            {
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = "Faktur Pajak Sudah Dilaporkan";
                return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
            }

            //delete data
            var userName = Session["UserName"] as string;

            try
            {
                checkFakturPajak.MasaPajak = input.MasaPajak;
                checkFakturPajak.TahunPajak = input.TahunPajak;
                FakturPajaks.Save(checkFakturPajak);
            }
            catch (Exception ex)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, ex.Message, MethodBase.GetCurrentMethod(), ex);
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = "Update Masa Pajak failed. See log with Log Key " + logKey + " for details.";
            }
            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult SetMasaPajakDialog(List<long> FakturPajakIds)
        {
            var model = new IdPajakModel()
            {
                FakturPajakId = string.Join(",", FakturPajakIds.Select(x => x.ToString()))
            };
            return Json(new
            {
                Html = this.RenderPartialView(@"MasaPajakDialog", model),

            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetMasaPajakMultiple(IdPajakModel Ids, int MasaPajak, int TahunPajak)
        {

            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = "Set Masa Pajak Success."
            };
            var checkFakturPajak = FakturPajaks.GetByMultipleId(Ids.FakturPajakId);
            if (checkFakturPajak == null || checkFakturPajak.Count == 0)
            {
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = "Faktur Pajak Tidak Ditemukan";
                return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
            }

            //var dTglFakturDariMasaPajak = Convert.ToDateTime(string.Concat(TahunPajak, "-", MasaPajak, "-1"));
            //var checkFakturPajakMin = Convert.ToDateTime(checkFakturPajak.OrderBy(a => a.TahunPajak).ThenBy(a=> a.MasaPajak).FirstOrDefault().TglFaktur.ToString());
            //checkFakturPajakMin = Convert.ToDateTime(string.Format("{0}-{1}-1", checkFakturPajakMin.Year, checkFakturPajakMin.Month));
            //if (dTglFakturDariMasaPajak < checkFakturPajakMin) {
            //    model.InfoType = RequestResultInfoType.ErrorOrDanger;
            //    model.Message = "Ganti Masa Pajak tidak bisa lebih kecil dari masa pajak yg dipilih";
            //    return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
            //}

            //var checkFakturPajakMax = Convert.ToDateTime(checkFakturPajak.OrderByDescending(a => a.TahunPajak).ThenByDescending(a => a.MasaPajak).FirstOrDefault().TglFaktur.ToString());
            //checkFakturPajakMax = Convert.ToDateTime(string.Format("{0}-{1}-1", checkFakturPajakMax.Year, checkFakturPajakMax.Month)).AddMonths(3);
            //if (dTglFakturDariMasaPajak > checkFakturPajakMax)
            //{
            //    model.InfoType = RequestResultInfoType.ErrorOrDanger;
            //    model.Message = "Ganti Masa Pajak tidak bisa melebihi 3 bulan dari masa pajak yg dipilih";
            //    return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
            //}


            //var dtget = checkFakturPajak.Where(a => a.FakturPajakTerlaporID != null || !a.StatusRegular);
            ////if (checkFakturPajak.FakturPajakTerlaporID != null || !checkFakturPajak.StatusRegular)
            //if (dtget.ToList().Count > 0)
            //{
            //    model.InfoType = RequestResultInfoType.ErrorOrDanger;
            //    model.Message = string.Concat("Faktur Pajak ", string.Join(", ", dtget.ToList().Select(a => a.FormatedNoFaktur)), " Sudah Dilaporkan");
            //    return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
            //}

            //var configData = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.PelaporanTglFaktur);
            //var dats = configData.ConfigValue.Split(':').ToList();

            //int min = int.Parse(dats[0].Replace("[", "").Replace("]", "")); // -3
            //int max = int.Parse(dats[1].Replace("[", "").Replace("]", "")); // 0
            //var dtMin = new DateTime(TahunPajak, MasaPajak, 1).AddMonths(min); // ex: oktober -> 1 juli


            //var maxperiode = OpenClosePeriods.GetOpenReguler().OrderByDescending(a => a.OpenClosePeriodId).FirstOrDefault();
            //var maxMasaPajak = maxperiode.MasaPajak;
            //var maxTahunPajak = maxperiode.TahunPajak;
            //var dtMax = new DateTime(maxTahunPajak, maxMasaPajak, 1).AddMonths(max + 1).AddDays(-1); // ex: oktober -> tgl terakhir bulan oktober




            //var dtMaxValidity = new DateTime(dtMax.Year, dtMax.Month, DateTime.DaysInMonth(dtMax.Year, dtMax.Month)); // ex: 30 oktober

            //var cekTglFakturMin = checkFakturPajak.Where(a => a.TglFaktur < dtMin);
            //if (cekTglFakturMin.ToList().Count > 0) // juli < april
            //{
            //    model.InfoType = RequestResultInfoType.ErrorOrDanger;
            //    model.Message = string.Concat("Tanggal Faktur Pajak " + string.Join(", ", cekTglFakturMin.ToList().Select(a => a.FormatedNoFaktur)) + " sudah kadaluarsa");
            //    return Json(new { Html = model }, JsonRequestBehavior.AllowGet);

            //}
            //else
            //{
            //    var cekTglFakturMax = checkFakturPajak.Where(a => a.TglFaktur > dtMaxValidity);
            //    if (cekTglFakturMax.ToList().Count > 0)
            //    {
            //        model.InfoType = RequestResultInfoType.ErrorOrDanger;
            //        model.Message = string.Concat("Tanggal Faktur Pajak " + string.Join(", ", cekTglFakturMax.ToList().Select(a => a.FormatedNoFaktur)) + " tidak sesuai ketentuan");
            //        return Json(new { Html = model }, JsonRequestBehavior.AllowGet);

            //    }
            //}

            //delete data
            var userName = Session["UserName"] as string;
            var msgs = new List<string>();
            try
            {
                bool isValid = false;
                var getValidasi = FakturPajaks.SetMasaPajakMultiple(Ids.FakturPajakId, MasaPajak, TahunPajak, userName, out isValid);
                msgs.Add("Jumlah Faktur Pajak Berhasil Set Masa Pajak : " + getValidasi.NotExpiredFakturPajak);
                msgs.Add("Jumlah Faktur Pajak Terkena Validasi Expired : " + getValidasi.ExpiredFakturPajak);
                msgs.Add("Jumlah Faktur Pajak Terkena Validasi Tanggal Tidak Sesuai : " + getValidasi.PajakTidakSesuai);
                msgs.Add("Jumlah Faktur Pajak Terkena Validasi Sudah dilaporkan : " + getValidasi.PajakSudahDilaporkan);

            }
            catch (Exception ex)
            {
                msgs.Add(ex.Message);
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, model.Message, MethodBase.GetCurrentMethod());
            }

            if (msgs.Count > 0)
            {
                model.Message = string.Join("<br />", msgs);
            }

            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetListFakturPajakBulkDataTableNonIws(string sEcho, int iDisplayStart, int iDisplayLength, int? masaPajak, int? tahunPajak)
        {
            if (!masaPajak.HasValue || !tahunPajak.HasValue)
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

            int totalItems;

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
                case 2: filter.SortColumnName = "FillingIndex"; break;
                case 3: filter.SortColumnName = "ErrorMessage"; break;
            }

            //var userName = Session["UserName"] as string;

            var listFakturPajak = FakturPajaks.GetScanBulk(filter, out totalItems, ApplicationEnums.FPType.ScanNonIws, masaPajak.Value,
                tahunPajak.Value, null);

            return Json(new
            {
                sEcho = sEcho,
                iTotalRecords = totalItems,
                iTotalDisplayRecords = totalItems,
                aaData = listFakturPajak
            },
            JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetListFakturPajakBulkDataTableIws(string sEcho, int iDisplayStart, int iDisplayLength, int? masaPajak, int? tahunPajak, string receivingDate)
        {

            if (!masaPajak.HasValue || !tahunPajak.HasValue)
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

            int totalItems;

            DateTime? dtReceivingDate = null;
            if (!string.IsNullOrEmpty(receivingDate))
            {
                dtReceivingDate = Convert.ToDateTime(receivingDate);
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
                case 2: filter.SortColumnName = "FillingIndex"; break;
                case 3: filter.SortColumnName = "ErrorMessage"; break;
            }

            //var userName = Session["UserName"] as string;

            var listFakturPajak = FakturPajaks.GetScanBulk(filter, out totalItems, ApplicationEnums.FPType.ScanIws, masaPajak.Value,
                tahunPajak.Value, dtReceivingDate);

            return Json(new
            {
                sEcho = sEcho,
                iTotalRecords = totalItems,
                iTotalDisplayRecords = totalItems,
                aaData = listFakturPajak
            },
            JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetDataFromDJP(string URL)
        {
            URL = URL.Trim();
            //check if exists url on database
            var chkByUrl = FakturPajaks.GetByUrlScan(URL);

            var inetProxy = WebConfiguration.InternetProxy;
            var inetProxyPort = WebConfiguration.InternetProxyPort;
            var inetProxyUseCredential = WebConfiguration.UseDefaultCredential;

            bool isUseProxy = !string.IsNullOrEmpty(inetProxy);

            //bool isUseProxy = false;


            //var isUseProxy = false;
            //if (Environment.MachineName != "LAPTOP-FG63PUP7")
            //{
            //    isUseProxy = true;
            //}

            if (!string.IsNullOrEmpty(URL))
            {
                string msgErr = "x";
                try
                {
                    WebExceptionStatus eStatus;

                    var getTimeOutSetting = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.DJPRequestTimeOutSetting);

                    if (getTimeOutSetting == null)
                    {
                        const string msgError = "Config Data not found for [DJPRequestTimeOutSetting]";
                        string logKey;
                        Logger.WriteLog(out logKey, LogLevel.Error, msgError, MethodBase.GetCurrentMethod());

                        var model = new
                        {
                            InfoType = RequestResultInfoType.Warning,
                            Data = (ResValidateFakturPm)null,
                            ErrorMsg = msgError
                        };
                        return Json(model, JsonRequestBehavior.AllowGet);
                    }

                    int timeOutSettingInt;
                    if (!int.TryParse(getTimeOutSetting.ConfigValue, out timeOutSettingInt))
                    {
                        const string msgError = "Invalid value Config Data [DJPRequestTimeOutSetting]";
                        string logKey;
                        Logger.WriteLog(out logKey, LogLevel.Error, msgError, MethodBase.GetCurrentMethod());
                        throw new Exception(msgError);
                    }
                    string logkey2;

                    var objXml = DJPLib.ValidateFakturLib.GetValidateFakturObjectV3(URL, timeOutSettingInt, isUseProxy, 
                                                                                    inetProxy, inetProxyPort, inetProxyUseCredential, 
                                                                                    out msgErr, out eStatus, out logkey2);
                    //CR DZ moved from line 4 (start from method) to here

                    if (chkByUrl != null && eStatus == WebExceptionStatus.Success)
                    {
                        string s = null;
                        if (chkByUrl.Status == (int)ApplicationEnums.StatusFakturPajak.Success && chkByUrl.IsOutstanding == true)
                        {
                            s = "Faktur Pajak  " + chkByUrl.FormatedNoFaktur + " sudah ada pada menu Outstanding Faktur Pajak ";
                            var model = new
                            {
                                InfoType = RequestResultInfoType.Warning,
                                Data = (ResValidateFakturPm)null,
                                ErrorMsg = s
                            };
                            return Json(model, JsonRequestBehavior.AllowGet);
                        }
                    }

                    if (chkByUrl != null && chkByUrl.MasaPajak.HasValue && chkByUrl.TahunPajak.HasValue && eStatus == WebExceptionStatus.Success)
                    {
                        string s = null;

                        if (chkByUrl.Status == (int)ApplicationEnums.StatusFakturPajak.Success &&
                            chkByUrl.StatusFaktur != EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti))
                        {

                            if (!(chkByUrl.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturPajakNormal) &&
                                objXml.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti)
                                ))
                            {

                                s = "Sudah discan pada " +
                                ConvertHelper.DateTimeConverter.ToLongDateString(chkByUrl.Created) + " oleh " +
                                chkByUrl.CreatedBy +
                                ". No FP " + chkByUrl.FormatedNoFaktur + " sudah ada di Masa Pajak " + chkByUrl.MasaPajakName +
                                " " + chkByUrl.TahunPajak
                                + ", Nomor Filling Index " + chkByUrl.FillingIndex;

                                var model = new
                                {
                                    InfoType = RequestResultInfoType.Warning,
                                    Data = (ResValidateFakturPm)null,
                                    ErrorMsg = s
                                };
                                return Json(model, JsonRequestBehavior.AllowGet);
                            }

                        }

                        if (chkByUrl.Status == (int)ApplicationEnums.StatusFakturPajak.ErrorRequest || chkByUrl.Status == (int)ApplicationEnums.StatusFakturPajak.ErrorValidation)// jika status 3/4 baru masuk sini
                        {
                            s = "Sudah discan pada "
                                + ConvertHelper.DateTimeConverter.ToLongDateString(chkByUrl.Created) + " oleh " +
                                chkByUrl.CreatedBy +
                                ". Di Masa Pajak " +
                                chkByUrl.MasaPajakName + " " + chkByUrl.TahunPajak
                                + ", Nomor Filling Index " + chkByUrl.FillingIndex + ". Tapi belum request ke DJP atau ada error validasi.";
                            var model = new
                            {
                                InfoType = RequestResultInfoType.Warning,
                                Data = (ResValidateFakturPm)null,
                                ErrorMsg = s
                            };
                            return Json(model, JsonRequestBehavior.AllowGet);

                        }
                    }
                    else if (chkByUrl == null && eStatus == WebExceptionStatus.Success)
                    {
                        var frmNoFaktur = FormatingDomains.GetFormatNoFaktur(ApplicationEnums.FPType.ScanIws, objXml.KdJenisTransaksi, objXml.FgPengganti, objXml.NomorFaktur);
                        var cekFormatted = FakturPajaks.GetByFormatedNoFaktur(frmNoFaktur.FormattedField);
                        if (cekFormatted.Count > 0)
                        {
                            if (String.IsNullOrEmpty(cekFormatted.FirstOrDefault().UrlScan))
                            {
                                var s = "Sudah discan pada " +
                                   ConvertHelper.DateTimeConverter.ToLongDateString(cekFormatted.FirstOrDefault().Created) + " oleh " +
                                   cekFormatted.FirstOrDefault().CreatedBy +
                                   ". No FP " + cekFormatted.FirstOrDefault().FormatedNoFaktur + " sudah ada di Masa Pajak " + cekFormatted.FirstOrDefault().MasaPajakName +
                                   " " + cekFormatted.FirstOrDefault().TahunPajak
                                   + ", Nomor Filling Index " + cekFormatted.FirstOrDefault().FillingIndex;

                                var model = new
                                {
                                    InfoType = RequestResultInfoType.Warning,
                                    Data = (ResValidateFakturPm)null,
                                    ErrorMsg = s
                                };
                                return Json(model, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }


                    if (eStatus == WebExceptionStatus.Success)
                    {
                        if (objXml.NomorFaktur != null)
                        {
                            objXml.DetailTransaksi = new List<DetailTransaksi>();//reset kosong, tidak perlu di lempar ke UI, karena yang diproses pada saat scan hanya header saja
                            var model = new
                            {
                                InfoType = RequestResultInfoType.Success,
                                Data = objXml,
                                ScanUrl = URL
                            };
                            var jsonResult = Json(model, JsonRequestBehavior.AllowGet);
                            jsonResult.MaxJsonLength = int.MaxValue;
                            return jsonResult;
                        }
                        else
                        {
                            var model = new
                            {
                                InfoType = RequestResultInfoType.Warning,
                                Data = (ResValidateFakturPm)null,
                                ErrorMsg = objXml.StatusApproval == null ? msgErr : objXml.StatusApproval
                            };
                            return Json(model, JsonRequestBehavior.AllowGet);

                        }

                    }
                    if (eStatus == WebExceptionStatus.Timeout)
                    {
                        var mh = new MailHelper();
                        bool isErrorSendMail;
                        mh.DjpRequestErrorSendMail(out isErrorSendMail, URL, logkey2);

                        if (isErrorSendMail)
                        {
                            var model = new
                            {
                                InfoType = RequestResultInfoType.Warning,
                                ErrorMsg = "Failed Get Data From DJP <br />Send Email Notification error." + msgErr
                            };

                            return Json(model, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            var model = new
                            {
                                InfoType = RequestResultInfoType.Warning,
                                ErrorMsg = "Failed Get Data From DJP <br />" + msgErr
                            };

                            return Json(model, JsonRequestBehavior.AllowGet);
                        }

                    }
                    else
                    {
                        var model = new
                        {
                            InfoType = RequestResultInfoType.Warning,
                            ErrorMsg = "Failed Get Data From DJP <br />" + msgErr
                        };

                        return Json(model, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception)
                {
                    var model = new
                    {
                        InfoType = RequestResultInfoType.Warning,
                        //ErrorMsg = msgErr
                        ErrorMsg = "Failed Get Data From DJP a"
                    };

                    return Json(model, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var model = new
                {
                    InfoType = RequestResultInfoType.Warning,
                    ErrorMsg = "URL Mandatory"
                };

                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }

        #region ------------- Scan Bulk IWS / Non-IWS -----------

        public JsonResult ValidationScanBulkIws(FakturPajakInfoModel info)
        {
            info.ScanType = ApplicationEnums.ScanType.Bulk;
            info.FPType = ApplicationEnums.FPType.ScanIws;
            var model = ValidationScanBulk(info);

            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PushScanBulkIws(FakturPajakInfoModel info)
        {
            info.ScanType = ApplicationEnums.ScanType.Bulk;
            info.FPType = ApplicationEnums.FPType.ScanIws;
            long fakturPajakId;
            var model = PushScanBulk(info, out fakturPajakId);
            return Json(new { Html = model, FakturPajakId = fakturPajakId }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidationScanBulkNonIws(FakturPajakInfoModel info)
        {
            info.ScanType = ApplicationEnums.ScanType.Bulk;
            info.FPType = ApplicationEnums.FPType.ScanNonIws;
            var model = ValidationScanBulk(info);

            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PushScanBulkNonIws(FakturPajakInfoModel info)
        {
            info.ScanType = ApplicationEnums.ScanType.Bulk;
            info.FPType = ApplicationEnums.FPType.ScanNonIws;
            long fakturPajakId;
            var model = PushScanBulk(info, out fakturPajakId);
            return Json(new { Html = model, FakturPajakId = fakturPajakId }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SubmitScanBulkIws(int? masaPajak, int? tahunPajak, string receivingDate)
        {
            if (!masaPajak.HasValue)
            {
                return Json(new
                {
                    Html = new RequestResultModel()
                    {
                        InfoType = RequestResultInfoType.Warning,
                        Message = "Masa Pajak Mandatory"
                    }
                }, JsonRequestBehavior.DenyGet);
            }

            if (!tahunPajak.HasValue)
            {
                return Json(new
                {
                    Html = new RequestResultModel()
                    {
                        InfoType = RequestResultInfoType.Warning,
                        Message = "Tahun Pajak Mandatory"
                    }
                }, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(receivingDate))
            {
                return Json(new
                {
                    Html = new RequestResultModel()
                    {
                        InfoType = RequestResultInfoType.Warning,
                        Message = "Receiving Date Mandatory"
                    }
                }, JsonRequestBehavior.DenyGet);
            }

            return SubmitScanBulk(masaPajak.Value, tahunPajak.Value, receivingDate, ApplicationEnums.FPType.ScanIws);
        }

        public JsonResult SubmitScanBulkNonIws(int? masaPajak, int? tahunPajak)
        {
            if (!masaPajak.HasValue)
            {
                return Json(new
                {
                    Html = new RequestResultModel()
                    {
                        InfoType = RequestResultInfoType.Warning,
                        Message = "Masa Pajak Mandatory"
                    }
                }, JsonRequestBehavior.DenyGet);
            }

            if (!tahunPajak.HasValue)
            {
                return Json(new
                {
                    Html = new RequestResultModel()
                    {
                        InfoType = RequestResultInfoType.Warning,
                        Message = "Tahun Pajak Mandatory"
                    }
                }, JsonRequestBehavior.DenyGet);
            }

            return SubmitScanBulk(masaPajak.Value, tahunPajak.Value, null, ApplicationEnums.FPType.ScanNonIws);
        }

        private JsonResult SubmitScanBulk(int masaPajak, int tahunPajak, string receivingDate, ApplicationEnums.FPType fpType)
        {

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

            DateTime? dtReceivingDate = null;
            if (!string.IsNullOrEmpty(receivingDate))
            {
                dtReceivingDate = Convert.ToDateTime(receivingDate);
            }

            var userName = Session["UserName"] as string;
            var getData = FakturPajaks.GetScanBulkToSubmit(fpType, masaPajak, tahunPajak, dtReceivingDate);

            //var dataToSubmit =
            //    getData.Where(c => c.Status != (int) ApplicationEnums.StatusFakturPajak.ErrorValidation).ToList();

            if (getData.Count <= 0)
            {
                return Json(new
                {
                    Html = new RequestResultModel()
                    {
                        InfoType = RequestResultInfoType.Warning,
                        Message = "Tidak ada data yang perlu di Submit Bulk"
                    }
                }, JsonRequestBehavior.DenyGet);
            }

            var ids = string.Join(",", getData.Select(d => d.FakturPajakId));
            var fakturPajakToProcess = FakturPajaks.GetByIds(ids);

            //proses request to DJP
            var processResult = ProcessSubmitBulkFakturPajak(fakturPajakToProcess, iTimeOutSetting, userName, fpType);

            if (processResult.Count > 0)
            {
                //Write Log for Details
                string logKey;
                var msgToLog = string.Join(Environment.NewLine,
                    processResult.Select(
                        d => d.Message + Environment.NewLine + "Exception : " + (d.ExceptionDetails != null ? d.ExceptionDetails.Message +
                            Environment.NewLine + "StackTrace : " + d.ExceptionDetails.StackTrace : "NULL")));
                Logger.WriteLog(out logKey, LogLevel.Error, msgToLog, MethodBase.GetCurrentMethod());
                return Json(new
                {
                    Html = new RequestResultModel()
                    {
                        InfoType = RequestResultInfoType.Warning,
                        Message = "Submit Bulk Faktur Pajak Failed. See Error Message Column."
                    }
                }, JsonRequestBehavior.DenyGet);
            }

            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = "Submit Bulk Faktur Pajak Succeed"
            };

            return Json(new { Html = model }, JsonRequestBehavior.DenyGet);
        }

        #endregion

        #region -------------------- Scan Satuan IWS / Non-IWS --------------

        public JsonResult PushScanPembetulanIws(FakturPajakInfoModel info)
        {
            info.ScanType = ApplicationEnums.ScanType.Pembetulan;
            info.FPType = ApplicationEnums.FPType.ScanIws;
            long fakturPajakId;
            var model = PushScanPembetulan(info, out fakturPajakId);
            var dbDataFp = FakturPajaks.GetById(fakturPajakId);
            return Json(new { Html = model, FakturPajakId = fakturPajakId, aaData = dbDataFp }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PushScanSatuanIws(FakturPajakInfoModel info)
        {
            info.ScanType = ApplicationEnums.ScanType.Satuan;
            info.FPType = ApplicationEnums.FPType.ScanIws;
            long fakturPajakId;
            var model = PushScanSatuan(info, out fakturPajakId);
            var dbDataFp = FakturPajaks.GetById(fakturPajakId);
            return Json(new { Html = model, FakturPajakId = fakturPajakId, aaData = dbDataFp }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PushScanPembetulanNonIws(FakturPajakInfoModel info)
        {
            info.ScanType = ApplicationEnums.ScanType.Pembetulan;
            info.FPType = ApplicationEnums.FPType.ScanNonIws;
            long fakturPajakId;
            var model = PushScanPembetulan(info, out fakturPajakId);
            var dbDataFp = FakturPajaks.GetById(fakturPajakId);
            return Json(new { Html = model, FakturPajakId = fakturPajakId, aaData = dbDataFp }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PushScanSatuanNonIws(FakturPajakInfoModel info)
        {
            info.ScanType = ApplicationEnums.ScanType.Satuan;
            info.FPType = ApplicationEnums.FPType.ScanNonIws;
            long fakturPajakId;
            var model = PushScanSatuan(info, out fakturPajakId);
            var dbDataFp = FakturPajaks.GetById(fakturPajakId);
            return Json(new { Html = model, FakturPajakId = fakturPajakId, aaData = dbDataFp }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidationScanSatuanIws(FakturPajakInfoModel info)
        {
            info.ScanType = ApplicationEnums.ScanType.Satuan;
            info.FPType = ApplicationEnums.FPType.ScanIws;
            var model = ValidationScanSatuan(info);

            if (model.InfoType != RequestResultInfoType.Success)
            {
                var npwpToFormat = new List<string>()
                {
                    info.NPWPPenjual,
                    info.NPWPLawanTransaksi
                };

                var formatedNpwp = FormatingDomains.GetFormatNpwp(npwpToFormat);

                info.FormatedNpwpPenjual = formatedNpwp.First(c => c.OriginalField == info.NPWPPenjual).FormattedField;
                info.FormatedNpwpLawanTransaksi = formatedNpwp.First(c => c.OriginalField == info.NPWPLawanTransaksi).FormattedField;

                var formatedNoFaktur = FormatingDomains.GetFormatNoFaktur(ApplicationEnums.FPType.ScanIws, info.KdJenisTransaksi, info.FgPengganti,
                    info.NoFakturPajak);
                info.FormatedNoFaktur = formatedNoFaktur.FormattedField;
            }

            return Json(new { Html = model, aaDat = info }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidationScanSatuanNonIws(FakturPajakInfoModel info)
        {

            info.ScanType = ApplicationEnums.ScanType.Satuan;
            info.FPType = ApplicationEnums.FPType.ScanNonIws;
            var model = ValidationScanSatuan(info);
            if (model.InfoType != RequestResultInfoType.Success)
            {
                var npwpToFormat = new List<string>()
                {
                    info.NPWPPenjual,
                    info.NPWPLawanTransaksi
                };


                var formatedNpwp = FormatingDomains.GetFormatNpwp(npwpToFormat);
                info.FormatedNpwpPenjual = formatedNpwp.First(c => c.OriginalField == info.NPWPPenjual).FormattedField;
                info.FormatedNpwpLawanTransaksi = formatedNpwp.First(c => c.OriginalField == info.NPWPLawanTransaksi).FormattedField;

                var formatedNoFaktur = FormatingDomains.GetFormatNoFaktur(ApplicationEnums.FPType.ScanNonIws, info.KdJenisTransaksi, info.FgPengganti,
                    info.NoFakturPajak);
                info.FormatedNoFaktur = formatedNoFaktur.FormattedField;
            }
            return Json(new { Html = model, aaDat = info }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidationScanPembetulanIws(FakturPajakInfoModel info)
        {
            info.ScanType = ApplicationEnums.ScanType.Pembetulan;
            info.FPType = ApplicationEnums.FPType.ScanIws;
            var model = ValidationScanPembetulan(info);

            if (model.InfoType != RequestResultInfoType.Success)
            {
                var npwpToFormat = new List<string>()
                {
                    info.NPWPPenjual,
                    info.NPWPLawanTransaksi
                };

                var formatedNpwp = FormatingDomains.GetFormatNpwp(npwpToFormat);

                info.FormatedNpwpPenjual = formatedNpwp.First(c => c.OriginalField == info.NPWPPenjual).FormattedField;
                info.FormatedNpwpLawanTransaksi = formatedNpwp.First(c => c.OriginalField == info.NPWPLawanTransaksi).FormattedField;

                var formatedNoFaktur = FormatingDomains.GetFormatNoFaktur(ApplicationEnums.FPType.ScanIws, info.KdJenisTransaksi, info.FgPengganti,
                    info.NoFakturPajak);
                info.FormatedNoFaktur = formatedNoFaktur.FormattedField;
            }

            return Json(new { Html = model, aaDat = info }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidationScanPembetulanNonIws(FakturPajakInfoModel info)
        {
            info.ScanType = ApplicationEnums.ScanType.Pembetulan;
            info.FPType = ApplicationEnums.FPType.ScanNonIws;
            var model = ValidationScanPembetulan(info);
            if (model.InfoType != RequestResultInfoType.Success)
            {
                var npwpToFormat = new List<string>()
                {
                    info.NPWPPenjual,
                    info.NPWPLawanTransaksi
                };

                var formatedNpwp = FormatingDomains.GetFormatNpwp(npwpToFormat);

                info.FormatedNpwpPenjual = formatedNpwp.First(c => c.OriginalField == info.NPWPPenjual).FormattedField;
                info.FormatedNpwpLawanTransaksi = formatedNpwp.First(c => c.OriginalField == info.NPWPLawanTransaksi).FormattedField;

                var formatedNoFaktur = FormatingDomains.GetFormatNoFaktur(ApplicationEnums.FPType.ScanNonIws, info.KdJenisTransaksi, info.FgPengganti,
                    info.NoFakturPajak);
                info.FormatedNoFaktur = formatedNoFaktur.FormattedField;
            }
            return Json(new { Html = model, aaDat = info }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region --------------------- Scan Manual -------------------

        public JsonResult ValidationScanManual(FakturPajakInfoModel Info)
        {
            var model = new RequestResultModel
            {
                InfoType = RequestResultInfoType.Success,
                Message = string.Empty
            };




            var msgs = new List<string>();
            int tahunPajak = 0;
            int checkNoFP = 0;
            var createDate = DateTime.Now.ToString();
            if (string.IsNullOrEmpty(Info.NoFakturPajak))
            {
                msgs.Add("No Faktur Pajak Mandatory");
            }
            else
            {
                checkNoFP = FakturPajaks.CheckNoFPKhusus(Info.NoFakturPajak);
                createDate = FakturPajaks.GetCreateDateNoFPKhusus(Info.NoFakturPajak);
            }

            if (string.IsNullOrEmpty(Info.TahunPajak.Trim()))
            {
                msgs.Add("Tahun Pajak Mandatory");
            }
            else
            {
                if (!int.TryParse(Info.TahunPajak.Trim(), out tahunPajak))
                {
                    msgs.Add("Invalid Tahun Pajak");
                }
            }
            int masaPajak = 0;
            if (string.IsNullOrEmpty(Info.MasaPajak.Trim()))
            {
                msgs.Add("Masa Pajak Mandatory");
            }
            else
            {
                if (!int.TryParse(Info.MasaPajak.Trim(), out masaPajak))
                {
                    msgs.Add("Invalid Tahun Pajak");
                }
            }

            if (Info.NoFakturPajak == null || Info.NoFakturPajak.Trim().Length == 0)
            {
                msgs.Add("No Faktur Pajak Mandatory");
            }
            if (Info.TglFaktur == null || Info.TglFaktur.Trim().Length == 0)
            {
                msgs.Add("Tanggal Faktur Mandatory");
            }
            if (checkNoFP >= 1)
            {
                msgs.Add(string.Format("Faktur Pajak Tersebut Sudah Pernah di Input Pada [{0}] ", createDate));
            }
            else
            {
                //getconfig
                var configData = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.PelaporanTglFaktur);
                if (configData == null)
                {
                    msgs.Add("GeneralConfig [PelaporanTglFaktur] not found.");
                }
                else
                {
                    var dats = configData.ConfigValue.Split(':').ToList();
                    if (dats.Count != 2)
                    {
                        msgs.Add("GeneralConfig [PelaporanTglFaktur] not valid.");
                    }
                    else
                    {
                        if (masaPajak != 0 && tahunPajak != 0)
                        {
                            //BPM No. ASMO3-201847620
                            int min = int.Parse(dats[0].Replace("[", "").Replace("]", ""));
                            int max = int.Parse(dats[1].Replace("[", "").Replace("]", ""));
                            var dtMin = new DateTime(tahunPajak, masaPajak, 1).AddMonths(min);
                            var dtMax = new DateTime(tahunPajak, masaPajak, 1).AddMonths(max);
                            var dTglFaktur = Convert.ToDateTime(Info.TglFaktur);
                            var dtMaxValidity = new DateTime(dtMax.Year, dtMax.Month, DateTime.DaysInMonth(dtMax.Year, dtMax.Month));
                            if (dTglFaktur < dtMin)
                            {
                                msgs.Add("Tanggal Faktur Pajak sudah kadaluarsa");
                            }
                            else
                            {
                                if (dTglFaktur > dtMaxValidity)
                                {
                                    msgs.Add("Tanggal Faktur Pajak tidak sesuai ketentuan");
                                }
                            }
                        }

                    }
                }

            }

            if (!(string.IsNullOrEmpty(Info.MasaPajak) || Info.MasaPajak == "0") &&
                !(string.IsNullOrEmpty(Info.TahunPajak) || Info.TahunPajak == "0"))
            {
                var getOpenClosePeriod = OpenClosePeriods.GetByMasaPajak(int.Parse(Info.MasaPajak),
                    int.Parse(Info.TahunPajak));

                if (getOpenClosePeriod != null)
                {
                    if (!getOpenClosePeriod.StatusRegular)
                    {
                        msgs.Add("Status Masa Pajak Close Reguler");
                    }
                    else
                    {
                        if (!getOpenClosePeriod.StatusSp2)
                        {
                            msgs.Add("Status Masa Pajak Close SP2");
                        }
                    }
                }
                else
                {
                    msgs.Add("Masa dan Tahun Pajak tidak tersedia di Data Open Close Periode");
                }
            }

            decimal decJumlahDpp;
            if (!decimal.TryParse(Info.JumlahDPP, out decJumlahDpp))
            {
                msgs.Add("Jumlah DPP harus angka");
            }
            decimal decJumlahPpn;
            if (!decimal.TryParse(Info.JumlahPPN, out decJumlahPpn))
            {
                msgs.Add("Jumlah PPN harus angka");
            }
            decimal decJumlahPpnBm;
            if (!decimal.TryParse(Info.JumlahPPNBM, out decJumlahPpnBm))
            {
                msgs.Add("Jumlah PPnBM harus angka");
            }

            if (string.IsNullOrEmpty(Info.JenisTransaksi))
            {
                msgs.Add("Jenis Transaksi Mandatory");
            }
            else
            {
                var chkJnsTransaksiConfig =
                    GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.FpKhususJenisTransaksiEmptyNpwp);
                if (chkJnsTransaksiConfig == null)
                {
                    string logKeyd;
                    Logger.WriteLog(out logKeyd, LogLevel.Error, "General Config [FpKhususJenisTransaksiEmptyNpwp] tidak ditemukan di database.", MethodBase.GetCurrentMethod());
                    msgs.Add("General Config tidak ditemukan (Log Key " + logKeyd + ")");
                }
                else
                {
                    if (Info.JenisTransaksi == chkJnsTransaksiConfig.ConfigValue)
                    {
                        if (!string.IsNullOrEmpty(Info.NPWPPenjual))
                        {
                            msgs.Add("NPWP Penjual tidak perlu diisi untuk Jenis Transaksi = " +
                                     chkJnsTransaksiConfig.ConfigValue);
                        }
                        //if (string.IsNullOrEmpty(Info.NamaPenjual))
                        //{
                        //    msgs.Add("Nama Penjual tidak perlu diisi untuk Jenis Transaksi = " +
                        //             chkJnsTransaksiConfig.ConfigValue);
                        //}
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Info.NPWPPenjual))
                        {
                            msgs.Add("NPWP Penjual harus diisi untuk Jenis Transaksi = " +
                                     Info.JenisTransaksi);
                        }
                        else
                        {
                            var v = Vendors.GetByFormatedNpwp(Info.NPWPPenjual);
                            if (v == null)
                            {
                                msgs.Add("NPWP Penjual tidak terdaftar");
                            }
                            else
                            {
                                if (v.PkpDicabut)
                                {
                                    msgs.Add("PKP Dicabut atas NPWP Penjual");
                                }
                            }
                        }
                        //if (string.IsNullOrEmpty(Info.NamaPenjual))
                        //{
                        //    msgs.Add("Nama Penjual harus diisi untuk Jenis Transaksi = " +
                        //             Info.JenisTransaksi);
                        //}
                    }
                }
            }

            if (string.IsNullOrEmpty(Info.JenisDokumen))
            {
                msgs.Add("Jenis Dokumen Mandatory");
            }
            else
            {
                if (!string.IsNullOrEmpty(Info.JenisTransaksi))
                {
                    var chkData = FPJenisDokumens.GetByFCodeAndJnsTransaksi(EnumHelper.GetDescription(ApplicationEnums.FCodeFpKhusus.Dm), Info.JenisTransaksi);
                    if (chkData.Count <= 0)
                    {
                        msgs.Add("Data Mapping Jenis Transaksi - Jenis Dokumen tidak tersedia.");
                    }
                    else
                    {
                        var isExists = chkData.Where(c => c.Id == Info.JenisDokumen).ToList();
                        if (isExists.Count <= 0)
                        {
                            var jnsDokAvailable = string.Join(",", chkData.Select(d => d.Id));
                            msgs.Add("Jenis Dokumen yang diperbolehkan : " + jnsDokAvailable);
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(Info.KdJenisTransaksi))
            {
                msgs.Add("Kode Jenis Transaksi Mandatory");
            }
            else
            {
                if (!string.IsNullOrEmpty(Info.JenisTransaksi))
                {
                    var chkData =
                        FPKdJenisTransaksis.GetByFCodeAndJnsTransaksi(
                            EnumHelper.GetDescription(ApplicationEnums.FCodeFpKhusus.Dm), Info.JenisTransaksi);
                    if (chkData.Count <= 0)
                    {
                        msgs.Add("Data Mapping Jenis Transaksi - Kode Jenis Transaksi tidak tersedia.");
                    }
                    else
                    {
                        var isExists = chkData.Where(c => c.Id == Info.KdJenisTransaksi).ToList();
                        if (isExists.Count <= 0)
                        {
                            var availableData = string.Join(",", chkData.Select(d => d.Id));
                            msgs.Add("Kode Jenis Transaksi yang diperbolehkan : " + availableData);
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(Info.FgPengganti))
            {
                msgs.Add("Faktur Pengganti Mandatory");
            }
            else
            {
                if (Info.FgPengganti.Trim() == "1")
                {
                    if (string.IsNullOrEmpty(Info.NoFakturYangDiganti))
                    {
                        msgs.Add("No FP yang diganti Mandatory");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(Info.NoFakturYangDiganti))
                    {
                        msgs.Add("No FP yang diganti harus kosong");
                    }
                }
            }

            if (!string.IsNullOrEmpty(Info.FgPengganti) && Info.FgPengganti.Trim() == "1" &&
                !string.IsNullOrEmpty(Info.NoFakturYangDiganti))
            {
                //check if exists
                var getData = FakturPajaks.GetByFormatedNoFakturFpKhusus(Info.NoFakturYangDiganti);
                if (getData == null || getData.Count <= 0)
                {
                    model.InfoType = RequestResultInfoType.Warning;
                    model.Message = "No FP yang diganti belum terdaftar sebagai Faktur Pajak Khusus.";

                    return Json(new { Html = model, IsClose = "close" }, JsonRequestBehavior.AllowGet);
                }

                var chkIsAlreadyReplace =
                    getData.FirstOrDefault(
                        c =>
                            c.StatusFaktur ==
                            EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti));

                if (chkIsAlreadyReplace != null)
                {
                    var getDataPengganti = FakturPajaks.GetFpKhususPenggantiByNoFaktur(chkIsAlreadyReplace.NoFakturPajak);
                    if (getDataPengganti.Count > 0)
                    {
                        //check apakah pengganti nya sudah diganti alias statusfaktur "Faktur Diganti" atau tidak alias kolom StatusFaktur IS NULL
                        var chkTo =
                            getDataPengganti.FirstOrDefault(
                                c =>
                                    c.StatusFaktur !=
                                    EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti));
                        if (chkTo != null)
                        {
                            if (chkTo.FormatedNoFaktur == Info.NoFakturPajak)
                            {
                                model.InfoType = RequestResultInfoType.ErrorWithConfirmation;
                                model.Message =
                                    "No Faktur Pengganti sudah terdaftar, apakah anda yakin untuk EDIT/REPLACE data berdasarkan 'No Faktur'?";
                            }
                            else
                            {
                                model.InfoType = RequestResultInfoType.Warning;
                                model.Message = "No Faktur yang diganti sudah diganti oleh " + chkTo.FormatedNoFaktur +
                                                ", silahkan menggunakan No Faktur tesebut sebagai No Faktur pengganti.";
                            }
                            return Json(new { Html = model, IsClose = "close" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var getToError =
                                getDataPengganti.Where(
                                    c =>
                                        c.StatusFaktur ==
                                        EnumHelper.GetDescription(
                                            ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti))
                                    .OrderByDescending(o => o.FakturPajakId)
                                    .FirstOrDefault();
                            if (getToError != null)
                            {
                                model.InfoType = RequestResultInfoType.Warning;
                                model.Message = "No Faktur yang diganti sudah diganti oleh " +
                                                getToError.FormatedNoFaktur +
                                                ", silahkan menggunakan No Faktur tesebut sebagai No Faktur yang diganti.";
                                return Json(new { Html = model, IsClose = "close" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
                else
                {
                    if (Info.NoFakturPajak == Info.NoFakturYangDiganti)
                    {
                        var chkGetError =
                        getData.FirstOrDefault(
                            c =>
                                c.StatusFaktur !=
                                EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti));
                        if (chkGetError != null)
                        {
                            model.InfoType = RequestResultInfoType.ErrorWithConfirmation;
                            model.Message =
                                "No Faktur Pengganti sudah terdaftar, apakah anda yakin untuk EDIT/REPLACE data berdasarkan 'No Faktur'?";
                            return Json(new { Html = model, IsClose = "close" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }

            if (Info.FakturPajakId > 0)
            {
                var fpm = FakturPajaks.GetById(Info.FakturPajakId);

                if (fpm.IsDeleted == true)
                {
                    msgs.Add(string.Format("Data sudah dihapus oleh '{0}', ", fpm.ModifiedBy));

                    model.InfoType = RequestResultInfoType.Warning;
                    model.Message = string.Join("<br />", msgs);

                    return Json(new { Html = model, IsClose = "close" }, JsonRequestBehavior.AllowGet);
                }
            }



            if (msgs.Count <= 0) return Json(new { Html = model }, JsonRequestBehavior.AllowGet);

            model.InfoType = RequestResultInfoType.Warning;
            model.Message = string.Join("<br />", msgs);

            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PushScanManual(FakturPajakInfoModel Info)
        {
            var model = new RequestResultModel
            {
                InfoType = RequestResultInfoType.Success,
                Message = string.Empty
            };

            //untuk menghindari input dari UI yang tidak diperlukan berkaitan dengan Faktur Pajak Scan Manual
            var dataToSave = new FakturPajakInfoModel()
            {
                FakturPajakId = Info.FakturPajakId,
                NoFakturPajak = Info.NoFakturPajak,
                TglFaktur = Info.TglFaktur,
                VendorId = Info.VendorId,
                NamaPenjual = Info.NamaPenjual,
                NPWPPenjual = string.IsNullOrEmpty(Info.NPWPPenjual) ? "" : Info.NPWPPenjual.Replace(".", "").Replace("-", ""),
                AlamatPenjual = Info.AlamatPenjual,
                JumlahDPP = Info.JumlahDPP,
                JumlahPPN = Info.JumlahPPN,
                JumlahPPNBM = Info.JumlahPPNBM,
                Pesan = Info.Pesan,
                FgPengganti = Info.FgPengganti,
                MasaPajak = Info.MasaPajak,
                TahunPajak = Info.TahunPajak,
                FPType = ApplicationEnums.FPType.ScanManual,
                ScanType = ApplicationEnums.ScanType.Manual,
                JenisTransaksi = Info.JenisTransaksi,
                JenisDokumen = Info.JenisDokumen,
                KdJenisTransaksi = Info.KdJenisTransaksi,
                NoFakturYangDiganti = Info.NoFakturYangDiganti
            };

            try
            {
                var userName = Session["UserName"] as string;
                var isToDelete = false;
                FakturPajak fpYgDiganti = null;
                if (!string.IsNullOrEmpty(Info.NoFakturYangDiganti))
                {
                    var datCheck = FakturPajaks.GetByFormatedNoFakturFpKhusus(Info.NoFakturYangDiganti);
                    if (datCheck != null && datCheck.Count > 0)
                    {
                        fpYgDiganti =
                            datCheck.FirstOrDefault(
                                c =>
                                    c.StatusFaktur !=
                                    EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti));
                    }
                }

                var getToDelete = FakturPajaks.GetByFormatedNoFakturFpKhusus(Info.NoFakturPajak);

                if (getToDelete != null && getToDelete.Count > 0)
                {
                    isToDelete = true;
                }
                long fakturpajakid = 0;
                using (var eScope = new TransactionScope())
                {
                    if (isToDelete)
                    {
                        var toDelete = getToDelete.OrderByDescending(o => o.FakturPajakId).First();
                        FakturPajaks.Delete(toDelete.FakturPajakId, userName);
                    }
                    else
                    {
                        if (fpYgDiganti != null)
                        {
                            //update status menjadi Faktur Diganti
                            var newStatusFaktur =
                                EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti);
                            FakturPajaks.UpdateStatusFaktur(fpYgDiganti.FakturPajakId, newStatusFaktur, userName);
                        }
                    }
                    var fakturPajakId = SaveFakturPajakKhusus(dataToSave, userName);
                    fakturpajakid = fakturPajakId;

                    eScope.Complete();
                    eScope.Dispose();
                }
                if (Info.FakturPajakId > 0)
                {
                    model.Message = String.Format("Faktur Pajak '{0}' has been updated.", Info.NoFakturPajak);
                }
                else
                {
                    //var dbDataFp = FakturPajaks.GetById(fakturpajakid);
                    //model.Message = String.Format("Faktur Pajak '{0}' has been created. Filling Index : " + dbDataFp.FillingIndex, Info.NoFakturPajak);
                    model.Message = String.Format("Faktur Pajak '{0}' has been created. ", Info.NoFakturPajak);
                }

            }
            catch (Exception ex)
            {
                model.Message = "Submit data failed";
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, ex.Message, MethodBase.GetCurrentMethod(), ex);
            }

            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidationPembetulanScanManual(FakturPajakInfoModel Info)
        {

            var model = new RequestResultModel
            {
                InfoType = RequestResultInfoType.Success,
                Message = string.Empty
            };

            var msgs = new List<string>();
            int tahunPajak = 0;
            if (string.IsNullOrEmpty(Info.TahunPajak))
            {
                msgs.Add("Tahun Pajak Mandatory");
            }
            else
            {
                if (!int.TryParse(Info.TahunPajak.Trim(), out tahunPajak))
                {
                    msgs.Add("Invalid Tahun Pajak");
                }
            }
            int masaPajak = 0;
            if (string.IsNullOrEmpty(Info.MasaPajak))
            {
                msgs.Add("Masa Pajak Mandatory");
            }
            else
            {
                if (!int.TryParse(Info.MasaPajak.Trim(), out masaPajak))
                {
                    msgs.Add("Invalid Tahun Pajak");
                }
            }

            if (Info.NoFakturPajak == null || Info.NoFakturPajak.Trim().Length == 0)
            {
                msgs.Add("No Faktur Pajak Mandatory");
            }

            if (Info.TglFaktur == null || Info.TglFaktur.Trim().Length == 0)
            {
                msgs.Add("Tanggal Faktur Mandatory");
            }
            else
            {
                //getconfig
                var configData = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.PelaporanTglFaktur);
                if (configData == null)
                {
                    msgs.Add("GeneralConfig [PelaporanTglFaktur] not found.");
                }
                else
                {
                    var dats = configData.ConfigValue.Split(':').ToList();
                    if (dats.Count != 2)
                    {
                        msgs.Add("GeneralConfig [PelaporanTglFaktur] not valid.");
                    }
                    else
                    {
                        //BPM No. ASMO3-201847620
                        if (masaPajak != 0 && tahunPajak != 0)
                        {
                            //BPM No. ASMO3-201847620
                            int min = int.Parse(dats[0].Replace("[", "").Replace("]", ""));
                            int max = int.Parse(dats[1].Replace("[", "").Replace("]", ""));
                            var dtMin = new DateTime(tahunPajak, masaPajak, 1).AddMonths(min);
                            var dtMax = new DateTime(tahunPajak, masaPajak, 1).AddMonths(max);
                            var dTglFaktur = Convert.ToDateTime(Info.TglFaktur);
                            var dtMaxValidity = new DateTime(dtMax.Year, dtMax.Month, DateTime.DaysInMonth(dtMax.Year, dtMax.Month));
                            if (dTglFaktur < dtMin)
                            {
                                msgs.Add("Tanggal Faktur Pajak sudah kadaluarsa");
                            }
                            else
                            {
                                if (dTglFaktur > dtMaxValidity)
                                {
                                    msgs.Add("Tanggal Faktur Pajak tidak sesuai ketentuan");
                                }
                            }
                        }
                    }
                }

            }

            if (!(string.IsNullOrEmpty(Info.MasaPajak) || Info.MasaPajak == "0") &&
                !(string.IsNullOrEmpty(Info.TahunPajak) || Info.TahunPajak == "0"))
            {
                var getOpenClosePeriod = OpenClosePeriods.GetByMasaPajak(int.Parse(Info.MasaPajak),
                    int.Parse(Info.TahunPajak));

                if (getOpenClosePeriod != null)
                {
                    if (getOpenClosePeriod.StatusRegular)
                    {
                        msgs.Add("Status Masa Pajak Open Reguler");
                    }
                    else
                    {
                        if (!getOpenClosePeriod.StatusSp2)
                        {
                            msgs.Add("Status Masa Pajak Close SP2");
                        }
                    }
                }
                else
                {
                    msgs.Add("Masa dan Tahun Pajak tidak tersedia di Data Open Close Periode");
                }
            }

            if (Info.VendorId < 1)
            {
                msgs.Add("Vendor Mandatory");
            }

            decimal decJumlahDpp;
            if (!decimal.TryParse(Info.JumlahDPP, out decJumlahDpp))
            {
                msgs.Add("Jumlah DPP harus angka");
            }
            decimal decJumlahPpn;
            if (!decimal.TryParse(Info.JumlahPPN, out decJumlahPpn))
            {
                msgs.Add("Jumlah PPN harus angka");
            }
            decimal decJumlahPpnBm;
            if (!decimal.TryParse(Info.JumlahPPNBM, out decJumlahPpnBm))
            {
                msgs.Add("Jumlah PPnBM harus angka");
            }

            if (string.IsNullOrEmpty(Info.JenisTransaksi))
            {
                msgs.Add("Jenis Transaksi Mandatory");
            }
            else
            {
                var chkJnsTransaksiConfig =
                    GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.FpKhususJenisTransaksiEmptyNpwp);
                if (chkJnsTransaksiConfig == null)
                {
                    string logKeyd;
                    Logger.WriteLog(out logKeyd, LogLevel.Error, "General Config [FpKhususJenisTransaksiEmptyNpwp] tidak ditemukan di database.", MethodBase.GetCurrentMethod());
                    msgs.Add("General Config tidak ditemukan (Log Key " + logKeyd + ")");
                }
                else
                {
                    if (Info.JenisTransaksi == chkJnsTransaksiConfig.ConfigValue)
                    {
                        if (!string.IsNullOrEmpty(Info.NPWPPenjual))
                        {
                            msgs.Add("NPWP Penjual tidak perlu diisi untuk Jenis Transaksi = " +
                                     chkJnsTransaksiConfig.ConfigValue);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Info.NPWPPenjual))
                        {
                            msgs.Add("NPWP Penjual harus diisi untuk Jenis Transaksi = " +
                                     Info.JenisTransaksi);
                        }
                        else
                        {
                            var v = Vendors.GetByFormatedNpwp(Info.NPWPPenjual);
                            if (v == null)
                            {
                                msgs.Add("NPWP Penjual tidak terdaftar");
                            }
                            else
                            {
                                if (v.PkpDicabut)
                                {
                                    msgs.Add("PKP Dicabut atas NPWP Penjual");
                                }
                            }
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(Info.JenisDokumen))
            {
                msgs.Add("Jenis Dokumen Mandatory");
            }
            else
            {
                if (!string.IsNullOrEmpty(Info.JenisTransaksi))
                {
                    var chkData = FPJenisDokumens.GetByFCodeAndJnsTransaksi(EnumHelper.GetDescription(ApplicationEnums.FCodeFpKhusus.Dm), Info.JenisTransaksi);
                    if (chkData.Count <= 0)
                    {
                        msgs.Add("Data Mapping Jenis Transaksi - Jenis Dokumen tidak tersedia.");
                    }
                    else
                    {
                        var isExists = chkData.Where(c => c.Id == Info.JenisDokumen).ToList();
                        if (isExists.Count <= 0)
                        {
                            var jnsDokAvailable = string.Join(",", chkData.Select(d => d.Id));
                            msgs.Add("Jenis Dokumen yang diperbolehkan : " + jnsDokAvailable);
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(Info.KdJenisTransaksi))
            {
                msgs.Add("Kode Jenis Transaksi Mandatory");
            }
            else
            {
                if (!string.IsNullOrEmpty(Info.JenisTransaksi))
                {
                    var chkData =
                        FPKdJenisTransaksis.GetByFCodeAndJnsTransaksi(
                            EnumHelper.GetDescription(ApplicationEnums.FCodeFpKhusus.Dm), Info.JenisTransaksi);
                    if (chkData.Count <= 0)
                    {
                        msgs.Add("Data Mapping Jenis Transaksi - Kode Jenis Transaksi tidak tersedia.");
                    }
                    else
                    {
                        var isExists = chkData.Where(c => c.Id == Info.KdJenisTransaksi).ToList();
                        if (isExists.Count <= 0)
                        {
                            var availableData = string.Join(",", chkData.Select(d => d.Id));
                            msgs.Add("Kode Jenis Transaksi yang diperbolehkan : " + availableData);
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(Info.FgPengganti))
            {
                msgs.Add("Faktur Pengganti Mandatory");
            }
            else
            {
                if (Info.FgPengganti.Trim() == "1")
                {
                    if (string.IsNullOrEmpty(Info.NoFakturYangDiganti))
                    {
                        msgs.Add("No FP yang diganti Mandatory");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(Info.NoFakturYangDiganti))
                    {
                        msgs.Add("No FP yang diganti harus kosong");
                    }
                }
            }

            if (Info.FakturPajakId > 0)
            {
                var fpm = FakturPajaks.GetById(Info.FakturPajakId);

                if (fpm.IsDeleted == true)
                {
                    msgs.Add(string.Format("Data sudah dihapus oleh '{0}', ", fpm.ModifiedBy));

                    model.InfoType = RequestResultInfoType.Warning;
                    model.Message = string.Join("<br />", msgs);

                    return Json(new { Html = model, IsClose = "close" }, JsonRequestBehavior.AllowGet);
                }
            }

            if (msgs.Count > 0)
            {
                model.InfoType = RequestResultInfoType.Warning;
                model.Message = string.Join("<br />", msgs);
                return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
            }

            if (!string.IsNullOrEmpty(Info.FgPengganti) && Info.FgPengganti.Trim() == "1" &&
                !string.IsNullOrEmpty(Info.NoFakturYangDiganti))
            {
                //check if exists
                var getData = FakturPajaks.GetByFormatedNoFakturFpKhusus(Info.NoFakturYangDiganti);
                if (getData == null || getData.Count <= 0)
                {
                    model.InfoType = RequestResultInfoType.Warning;
                    model.Message = "No FP yang diganti belum terdaftar sebagai Faktur Pajak Khusus.";

                    return Json(new { Html = model, IsClose = "close" }, JsonRequestBehavior.AllowGet);
                }

                var chkIsAlreadyReplace =
                    getData.FirstOrDefault(
                        c =>
                            c.StatusFaktur ==
                            EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti));

                if (chkIsAlreadyReplace != null)
                {
                    var getDataPengganti = FakturPajaks.GetFpKhususPenggantiByNoFaktur(chkIsAlreadyReplace.NoFakturPajak);
                    if (getDataPengganti.Count > 0)
                    {
                        //check apakah pengganti nya sudah diganti alias statusfaktur "Faktur Diganti" atau tidak alias kolom StatusFaktur IS NULL
                        var chkTo =
                            getDataPengganti.FirstOrDefault(
                                c =>
                                    c.StatusFaktur !=
                                    EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti));
                        if (chkTo != null)
                        {
                            if (chkTo.FormatedNoFaktur == Info.NoFakturPajak)
                            {
                                model.InfoType = RequestResultInfoType.ErrorWithConfirmation;
                                model.Message =
                                    "No Faktur Pengganti sudah terdaftar, apakah anda yakin untuk EDIT/REPLACE data berdasarkan 'No Faktur'?";
                            }
                            else
                            {
                                model.InfoType = RequestResultInfoType.Warning;
                                model.Message = "No Faktur yang diganti sudah diganti oleh " + chkTo.FormatedNoFaktur +
                                                ", silahkan menggunakan No Faktur tesebut sebagai No Faktur pengganti.";
                            }
                            return Json(new { Html = model, IsClose = "close" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var getToError =
                                getDataPengganti.Where(
                                    c =>
                                        c.StatusFaktur ==
                                        EnumHelper.GetDescription(
                                            ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti))
                                    .OrderByDescending(o => o.FakturPajakId)
                                    .FirstOrDefault();
                            if (getToError != null)
                            {
                                model.InfoType = RequestResultInfoType.Warning;
                                model.Message = "No Faktur yang diganti sudah diganti oleh " +
                                                getToError.FormatedNoFaktur +
                                                ", silahkan menggunakan No Faktur tesebut sebagai No Faktur yang diganti.";
                                return Json(new { Html = model, IsClose = "close" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
                else
                {
                    if (Info.NoFakturPajak == Info.NoFakturYangDiganti)
                    {
                        var chkGetError =
                        getData.FirstOrDefault(
                            c =>
                                c.StatusFaktur !=
                                EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti));
                        if (chkGetError != null)
                        {
                            model.InfoType = RequestResultInfoType.ErrorWithConfirmation;
                            model.Message =
                                "No Faktur Pengganti sudah terdaftar, apakah anda yakin untuk EDIT/REPLACE data berdasarkan 'No Faktur'?";
                            return Json(new { Html = model, IsClose = "close" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(Info.NoFakturPajak))
            {
                var chkData = FakturPajaks.GetByFormatedNoFaktur(Info.NoFakturPajak.Trim());
                if (chkData != null && chkData.Count > 0)
                {
                    model.InfoType = RequestResultInfoType.ErrorWithConfirmation;
                    model.Message = "No Faktur sudah terdaftar, apakah anda yakin untuk menimpa data yang sudah ada ?";

                    return Json(new { Html = model, IsClose = "close" }, JsonRequestBehavior.AllowGet);

                }
                return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PushPembetulanScanManual(FakturPajakInfoModel Info)
        {
            var userName = Session["UserName"] as string;
            var model = new RequestResultModel
            {
                InfoType = RequestResultInfoType.Success,
                Message = string.Empty
            };

            //untuk menghindari input dari UI yang tidak diperlukan berkaitan dengan Faktur Pajak Scan Manual
            var dataToSave = new FakturPajakInfoModel()
            {
                FakturPajakId = Info.FakturPajakId,
                NoFakturPajak = Info.NoFakturPajak,
                TglFaktur = Info.TglFaktur,
                VendorId = Info.VendorId,
                NamaPenjual = Info.NamaPenjual,
                NPWPPenjual = Info.NPWPPenjual.Replace(".", "").Replace("-", ""),
                AlamatPenjual = Info.AlamatPenjual,
                JumlahDPP = Info.JumlahDPP,
                JumlahPPN = Info.JumlahPPN,
                JumlahPPNBM = Info.JumlahPPNBM,
                Pesan = Info.Pesan,
                FgPengganti = Info.FgPengganti,
                MasaPajak = Info.MasaPajak,
                TahunPajak = Info.TahunPajak,
                FPType = ApplicationEnums.FPType.ScanManual,
                ScanType = ApplicationEnums.ScanType.Manual,
                JenisTransaksi = Info.JenisTransaksi,
                JenisDokumen = Info.JenisDokumen,
                KdJenisTransaksi = Info.KdJenisTransaksi,
                NoFakturYangDiganti = Info.NoFakturYangDiganti
            };

            try
            {
                var isToDelete = false;
                FakturPajak fpYgDiganti = null;
                if (!string.IsNullOrEmpty(Info.NoFakturYangDiganti))
                {
                    var datCheck = FakturPajaks.GetByFormatedNoFakturFpKhusus(Info.NoFakturYangDiganti);
                    if (datCheck != null && datCheck.Count > 0)
                    {
                        fpYgDiganti =
                            datCheck.FirstOrDefault(
                                c =>
                                    c.StatusFaktur !=
                                    EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti));
                    }
                }

                var getToDelete = FakturPajaks.GetByFormatedNoFakturFpKhusus(Info.NoFakturPajak);

                if (getToDelete != null && getToDelete.Count > 0)
                {
                    isToDelete = true;
                }

                using (var eScope = new TransactionScope())
                {
                    //Jika No Faktur sudah ada maka EDIT = Flag Delete yang lama, insert yang baru
                    //Jika No Faktur belum ada maka insert baru saja
                    if (isToDelete)
                    {
                        var toDelete = getToDelete.OrderByDescending(o => o.FakturPajakId).First();
                        FakturPajaks.Delete(toDelete.FakturPajakId, userName);

                    }
                    else
                    {
                        if (fpYgDiganti != null)
                        {
                            //update status menjadi Faktur Diganti
                            var newStatusFaktur =
                                EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti);
                            FakturPajaks.UpdateStatusFaktur(fpYgDiganti.FakturPajakId, newStatusFaktur, userName);
                        }
                    }

                    if (fpYgDiganti != null)
                    {
                        //update status menjadi Faktur Diganti
                        var newStatusFaktur =
                            EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti);
                        FakturPajaks.UpdateStatusFaktur(fpYgDiganti.FakturPajakId, newStatusFaktur, userName);
                    }

                    var fakturPajakId = SaveFakturPajakKhusus(dataToSave, userName);
                    Info.FakturPajakId = fakturPajakId;

                    eScope.Complete();
                    eScope.Dispose();
                }
                if (isToDelete)
                {
                    model.Message = String.Format("Faktur Pajak '{0}' has been updated.", Info.NoFakturPajak);
                }
                else
                {
                    model.Message = String.Format("Faktur Pajak '{0}' has been created.", Info.NoFakturPajak);
                }
            }
            catch (Exception ex)
            {
                model.Message = "Submit data failed";
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, ex.Message, MethodBase.GetCurrentMethod(), ex);
            }

            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region -------------- Private Method --------------

        private RequestResultModel PushScanBulk(FakturPajakInfoModel info, out long fakturPajakId)
        {
            var model = new RequestResultModel
            {
                InfoType = RequestResultInfoType.Success,
                Message = string.Empty
            };

            fakturPajakId = 0;
            info.UrlScan = info.UrlScan.Trim();
            var dataToSave = new FakturPajakInfoModel()
            {
                FakturPajakId = info.FakturPajakId,
                UrlScan = info.UrlScan,
                Dikreditkan = info.Dikreditkan,
                MasaPajak = info.MasaPajak,
                ReceivingDate = info.ReceivingDate,
                TahunPajak = info.TahunPajak,
                FPType = info.FPType,
                ScanType = info.ScanType,
                VendorId = null
            };

            try
            {
                using (var eScope = new TransactionScope())
                {
                    fakturPajakId = SaveScanBulkFakturPajak(dataToSave);
                    info.FakturPajakId = fakturPajakId;
                    eScope.Complete();
                    eScope.Dispose();
                }
                model.Message = "Faktur Pajak has been created.";
            }
            catch (Exception ex)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Fatal, ex.Message, MethodBase.GetCurrentMethod(), ex);
                model.Message = "Submit data failed";
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
            }
            return model;
        }

        private RequestResultModel PushScanSatuan(FakturPajakInfoModel info, out long fakturPajakId)
        {
            var model = new RequestResultModel
            {
                InfoType = RequestResultInfoType.Success,
                Message = string.Empty
            };
            fakturPajakId = 0;
            try
            {
                var getData = FakturPajaks.GetByOriginalNoFaktur(info.NoFakturPajak);
                var datToUpdate = new List<FakturPajak>();
                //CR DZ
                var datToDelete = new List<FakturPajak>();
                //maka collect data
                if (getData.Count > 0)
                {
                    //get data yang akan di-update jika 
                    datToUpdate =
                        getData.Where(
                            c =>
                                c.StatusFaktur !=
                                EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti))
                            .ToList();

                    //CR DZ
                    if (info.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti))
                    {
                        datToDelete =
                       getData.Where(
                           x =>
                               x.StatusFaktur ==
                               EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturPajakNormal))
                           .ToList();
                    }
                    //

                }

                //get to djp for datToUpdate
                var dbDataToUpdate = new List<FakturPajakToUpdated>();
                if (datToUpdate.Count > 0)
                {
                    List<RequestResultModel> resultModel;
                    var djpData = CollectUpdatedDataFromDjp(datToUpdate, out resultModel);
                    var chkIsAnyError = resultModel.Where(c => c.InfoType != RequestResultInfoType.Success).ToList();
                    if (chkIsAnyError.Count > 0)
                    {
                        //Ada error pada saat getdata ke djp
                        var msgToRet = string.Join("<br />", chkIsAnyError.Select(d => d.Message));
                        model.InfoType = RequestResultInfoType.Warning;
                        model.Message = msgToRet;
                        return model;
                    }
                    dbDataToUpdate = djpData;
                }
                var userName = Session["UserName"] as string;
                using (var eScope = new TransactionScope())
                {
                    //CR DZ
                    if (datToDelete.Count > 0)
                    {
                        foreach (var item in datToDelete)
                        {
                            FakturPajaks.Delete(item.FakturPajakId, userName);
                        }
                    }
                    if (dbDataToUpdate.Count > 0)
                    {
                        foreach (var item in dbDataToUpdate)
                        {
                            FakturPajaks.UpdateStatusFakturNotDeleted(item.FakturPajakId, item.StatusFaktur, userName, item.IsDeleted);
                        }
                    }
                    string outMsgs = "";
                    fakturPajakId = SaveFakturPajak(info, userName, out outMsgs);
                    if (string.IsNullOrEmpty(outMsgs))
                    {

                        eScope.Complete();
                    }
                    else
                    {
                        model.InfoType = RequestResultInfoType.ErrorOrDanger;
                        model.Message = outMsgs;
                    }

                    eScope.Dispose();
                }
                if (model.InfoType == RequestResultInfoType.Success)
                {
                    var dbData = FakturPajaks.GetById(fakturPajakId);
                    //if (info.StatusFaktur.ToLower() == "faktur diganti")
                    //{
                    //    model.Message = "Faktur Pajak Sudah Diganti";
                    //}
                    //else
                    //{
                    //    if (info.FakturPajakId > 0)
                    //    {
                    //        model.Message = String.Format("Faktur Pajak '{0}' has been updated.", dbData.FormatedNoFaktur);
                    //    }
                    //    else
                    //    {
                    //        model.Message = String.Format("Faktur Pajak '{0}' has been created.", dbData.FormatedNoFaktur);
                    //    }
                    //}

                    var frmNoFaktur = FormatingDomains.GetFormatNoFaktur(ApplicationEnums.FPType.ScanIws, info.KdJenisTransaksi, info.FgPengganti, info.NoFakturPajak);
                    if (info.FakturPajakId > 0)
                    {
                        model.Message = String.Format("Faktur Pajak '{0}' has been updated.", frmNoFaktur.FormattedField);
                    }
                    else
                    {
                        model.Message = String.Format("Faktur Pajak '{0}' has been created.", frmNoFaktur.FormattedField);
                    }
                }
            }
            catch (Exception ex)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, ex.Message, MethodBase.GetCurrentMethod(), ex);
                model.Message = "Submit data failed. See Log with Key " + logKey + " for details.";
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
            }

            return model;
        }

        private List<FakturPajakToUpdated> CollectUpdatedDataFromDjp(IEnumerable<FakturPajak> dataToUpdated, out List<RequestResultModel> resultModel)
        {
            resultModel = new List<RequestResultModel>();
            var isUseProxy = false;
            var inetProxy = WebConfiguration.InternetProxy;
            var inetProxyPort = WebConfiguration.InternetProxyPort;
            var inetProxyUseCredential = WebConfiguration.UseDefaultCredential;
            if (!string.IsNullOrEmpty(inetProxy) && inetProxyPort.HasValue && inetProxyUseCredential.HasValue)
            {
                isUseProxy = true;
            }

            var getTimeOutSetting = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.DJPRequestTimeOutSetting);

            if (getTimeOutSetting == null)
            {
                const string msgError = "Config Data not found for [DJPRequestTimeOutSetting]";
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, msgError, MethodBase.GetCurrentMethod());
                resultModel.Add(new RequestResultModel()
                {
                    InfoType = RequestResultInfoType.Warning,
                    Message = msgError
                });
                return null;
            }

            int timeOutSettingInt;

            if (!int.TryParse(getTimeOutSetting.ConfigValue, out timeOutSettingInt))
            {
                const string msgError = "Invalid value Config Data [DJPRequestTimeOutSetting]";
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, msgError, MethodBase.GetCurrentMethod());
                resultModel.Add(new RequestResultModel()
                {
                    InfoType = RequestResultInfoType.Warning,
                    Message = msgError
                });
                return null;
            }

            var toReturn = new List<FakturPajakToUpdated>();
            foreach (var dat in dataToUpdated)
            {
                if (!string.IsNullOrEmpty(dat.UrlScan))
                {
                    try
                    {
                        WebExceptionStatus eStatus;
                        string msgErr;


                        string logkey2;
                        var objXml = DJPLib.ValidateFakturLib.GetValidateFakturObjectV3(dat.UrlScan, timeOutSettingInt, isUseProxy, inetProxy, inetProxyPort, inetProxyUseCredential, out msgErr, out eStatus, out logkey2);

                        if (eStatus == WebExceptionStatus.Success)
                        {
                            if (objXml != null)
                            {
                                objXml.DetailTransaksi = new List<DetailTransaksi>();//reset kosong, tidak perlu di lempar ke UI, karena yang diproses pada saat scan hanya header saja
                                toReturn.Add(new FakturPajakToUpdated()
                                {
                                    FakturPajakId = dat.FakturPajakId,
                                    StatusFaktur = objXml.StatusFaktur,
                                    //CR DZ
                                    IsDeleted = dat.IsDeleted
                                });
                            }
                            else
                            {
                                resultModel.Add(new RequestResultModel()
                                {
                                    InfoType = RequestResultInfoType.Warning,
                                    Message = msgErr
                                });
                            }
                        }
                        else
                        {
                            //if (eStatus == WebExceptionStatus.Timeout)
                            //{
                            resultModel.Add(new RequestResultModel()
                            {
                                InfoType = RequestResultInfoType.Warning,
                                Message = msgErr
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        var msgToError = "Failed Get Data From DJP for URL : " + dat.UrlScan;
                        string logKey;
                        Logger.WriteLog(out logKey, LogLevel.Error, msgToError, MethodBase.GetCurrentMethod(), ex);
                        resultModel.Add(new RequestResultModel()
                        {
                            InfoType = RequestResultInfoType.Warning,
                            Message = msgToError
                        });
                    }
                }
            }
            return toReturn;
        }

        private RequestResultModel PushScanPembetulan(FakturPajakInfoModel info, out long fakturPajakId)
        {
            var model = new RequestResultModel
            {
                InfoType = RequestResultInfoType.Success,
                Message = string.Empty
            };
            fakturPajakId = 0;
            try
            {
                var getData = FakturPajaks.GetByOriginalNoFaktur(info.NoFakturPajak);
                var datToUpdate = new List<FakturPajak>();

                //CR DZ
                var datToDelete = new List<FakturPajak>();
                //maka collect data
                if (getData.Count > 0)
                {
                    //get data yang akan di-update jika 
                    datToUpdate =
                        getData.Where(
                            c =>
                                c.StatusFaktur !=
                                EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti))
                            .ToList();
                    //CR DZ
                    if (info.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti))
                    {
                        datToDelete =
                       getData.Where(
                           x =>
                               x.StatusFaktur ==
                               EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturPajakNormal))
                           .ToList();
                    }
                }

                //get to djp for datToUpdate
                var dbDataToUpdate = new List<FakturPajakToUpdated>();
                if (datToUpdate.Count > 0)
                {
                    List<RequestResultModel> resultModel;
                    var djpData = CollectUpdatedDataFromDjp(datToUpdate, out resultModel);
                    var chkIsAnyError = resultModel.Where(c => c.InfoType != RequestResultInfoType.Success).ToList();
                    if (chkIsAnyError.Count > 0)
                    {
                        //Ada error pada saat getdata ke djp
                        var msgToRet = string.Join("<br />", chkIsAnyError.Select(d => d.Message));
                        model.InfoType = RequestResultInfoType.Warning;
                        model.Message = msgToRet;
                        return model;
                    }
                    dbDataToUpdate = djpData;
                }
                var userName = Session["UserName"] as string;
                using (var eScope = new TransactionScope())
                {
                    //CR DZ
                    if (datToDelete.Count > 0)
                    {
                        foreach (var item in datToDelete)
                        {
                            FakturPajaks.Delete(item.FakturPajakId, userName);
                        }
                    }
                    if (dbDataToUpdate.Count > 0)
                    {
                        foreach (var item in dbDataToUpdate)
                        {
                            FakturPajaks.UpdateStatusFakturNotDeleted(item.FakturPajakId, item.StatusFaktur, userName, item.IsDeleted);
                        }
                    }
                    //
                    string outMsgs = string.Empty;
                    fakturPajakId = SaveFakturPajak(info, userName, out outMsgs);
                    if (string.IsNullOrEmpty(outMsgs))
                    {
                        eScope.Complete();
                    }
                    else
                    {
                        model.InfoType = RequestResultInfoType.ErrorOrDanger;
                        model.Message = outMsgs;
                    }
                    eScope.Dispose();
                }
                if (model.InfoType == RequestResultInfoType.Success)
                {
                    var dbData = FakturPajaks.GetById(fakturPajakId);
                    if (info.StatusFaktur.ToLower() == "faktur diganti")
                    {
                        model.Message = "Faktur Sudah Diganti";
                    }
                    else
                    {
                        if (info.FakturPajakId > 0)
                        {
                            model.Message = String.Format("Faktur Pajak '{0}' has been updated.", dbData.FormatedNoFaktur);
                        }
                        else
                        {
                            model.Message = String.Format("Faktur Pajak '{0}' has been created.", dbData.FormatedNoFaktur);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, ex.Message, MethodBase.GetCurrentMethod(), ex);
                model.Message = "Submit data failed. See Log with Key " + logKey + " for details.";
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
            }

            return model;
        }

        private long SaveFakturPajak(FakturPajakInfoModel info, string userName, out string msgerror)
        {
            msgerror = string.Empty;
            info.UrlScan = info.UrlScan.Trim();

            int minpelaporan = -3;
            int maxpelaporan = 0;
            var configData = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.PelaporanTglFaktur);
            if (configData != null)
            {
                var dats = configData.ConfigValue.Split(':').ToList();
                if (dats.Count == 2)
                {
                    string outlogkey;
                    try
                    {
                        minpelaporan = int.Parse(dats[0].Replace("[", "").Replace("]", ""));
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog(out outlogkey, LogLevel.Warn, ex.Message, MethodBase.GetCurrentMethod(), ex);
                    }
                    try
                    {
                        maxpelaporan = int.Parse(dats[1].Replace("[", "").Replace("]", ""));
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog(out outlogkey, LogLevel.Warn, ex.Message, MethodBase.GetCurrentMethod(), ex);
                    }
                }
            }

            var masapajak = Convert.ToInt32(info.MasaPajak);
            var tahunpajak = Convert.ToInt32(info.TahunPajak);

            if (info.FgPengganti == "0")
            {
                //FP Normal
                #region Logic FP Normal ----------
                var fp = FakturPajaks.GetByUrlScan(info.UrlScan);
                //var fp = FakturPajaks.GetByOriginalNoFaktur(info.NoFakturPajak).Where(
                //            c =>
                //                c.StatusFaktur !=
                //                EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti)).FirstOrDefault();
                if (fp != null)
                {
                    //if (!fp.MasaPajak.HasValue && !fp.TahunPajak.HasValue)
                    //{
                    //Masa Pajak - Tahun Pajak di reset oleh logic Faktur Pajak Diganti Outstanding
                    fp.KdJenisTransaksi = info.KdJenisTransaksi;
                    fp.FgPengganti = info.FgPengganti;
                    fp.NoFakturPajak = info.NoFakturPajak;
                    fp.TglFaktur = null;
                    fp.NPWPPenjual = info.NPWPPenjual;
                    fp.NamaPenjual = info.NamaPenjual;
                    fp.AlamatPenjual = info.AlamatPenjual;
                    fp.VendorId = info.VendorId;
                    fp.FCode = ApplicationConstant.FCodeFm;
                    fp.FPType = (int)info.FPType;
                    fp.ScanType = (int)info.ScanType;
                    fp.NPWPLawanTransaksi = info.NPWPLawanTransaksi;
                    fp.NamaLawanTransaksi = info.NamaLawanTransaksi;
                    fp.AlamatLawanTransaksi = info.AlamatLawanTransaksi;
                    fp.JumlahDPP = Convert.ToDecimal(info.JumlahDPP);
                    fp.JumlahPPN = Convert.ToDecimal(info.JumlahPPN);
                    fp.JumlahPPNBM = Convert.ToDecimal(info.JumlahPPNBM);
                    fp.StatusApproval = info.StatusApproval;
                    fp.StatusFaktur = info.StatusFaktur;
                    fp.Pesan = info.StatusApproval;
                    fp.Dikreditkan = info.Dikreditkan == "Ya" || info.Dikreditkan.ToLower() == "ya";
                    fp.MasaPajak = masapajak;
                    fp.TahunPajak = tahunpajak;
                    fp.Referensi = info.Referensi;
                    fp.UrlScan = info.UrlScan;
                    if (info.FPType == ApplicationEnums.FPType.ScanIws)
                    {
                        fp.ReceivingDate = Convert.ToDateTime(info.ReceivingDate);
                    }
                    else
                    {
                        fp.ReceivingDate = null;
                    }

                    if (!string.IsNullOrEmpty(info.TglFaktur))
                    {
                        fp.TglFaktur = DateTime.ParseExact(info.TglFaktur.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    fp.Status = (int)ApplicationEnums.StatusFakturPajak.Success;
                    fp.CreatedBy = userName;
                    fp.ModifiedBy = userName;

                    var masterResult = FakturPajaks.Save(fp);
                    info.FakturPajakId = masterResult.FakturPajakId;
                    //}
                }
                else
                {
                    //data belum ada di System EVIS
                    //add ke System EVIS

                    fp = new FakturPajak
                    {
                        FakturPajakId = info.FakturPajakId,
                        UrlScan = info.UrlScan,
                        KdJenisTransaksi = info.KdJenisTransaksi,
                        FgPengganti = info.FgPengganti,
                        NoFakturPajak = info.NoFakturPajak,
                        TglFaktur = null,
                        NPWPPenjual = info.NPWPPenjual,
                        NamaPenjual = info.NamaPenjual,
                        AlamatPenjual = info.AlamatPenjual,
                        VendorId = info.VendorId,
                        FCode = ApplicationConstant.FCodeFm,
                        FPType = (int)info.FPType,
                        ScanType = (int)info.ScanType,
                        NPWPLawanTransaksi = info.NPWPLawanTransaksi,
                        NamaLawanTransaksi = info.NamaLawanTransaksi,
                        AlamatLawanTransaksi = info.AlamatLawanTransaksi,
                        JumlahDPP = Convert.ToDecimal(info.JumlahDPP),
                        JumlahPPN = Convert.ToDecimal(info.JumlahPPN),
                        JumlahPPNBM = Convert.ToDecimal(info.JumlahPPNBM),
                        StatusApproval = info.StatusApproval,
                        StatusFaktur = info.StatusFaktur,
                        Pesan = info.StatusApproval,
                        Dikreditkan = info.Dikreditkan == "Ya" || info.Dikreditkan.ToLower() == "ya",
                        MasaPajak = masapajak,
                        TahunPajak = tahunpajak,
                        Referensi = info.Referensi
                    };
                    if (info.FPType == ApplicationEnums.FPType.ScanIws)
                    {
                        fp.ReceivingDate = Convert.ToDateTime(info.ReceivingDate);
                    }
                    else
                    {
                        fp.ReceivingDate = null;
                    }
                    if (!string.IsNullOrEmpty(info.TglFaktur))
                    {
                        fp.TglFaktur = DateTime.ParseExact(info.TglFaktur.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }

                    fp.Status = (int)ApplicationEnums.StatusFakturPajak.Success;
                    fp.CreatedBy = userName;
                    fp.ModifiedBy = userName;

                    if (info.StatusFaktur.ToLower() == "faktur diganti" || info.StatusFaktur.ToLower() == "faktur dibatalkan")
                    {
                        //FP Normal yang sudah diganti
                        //langsung masuk ke FakturPajakDigantiOutstanding
                        //Set Status Expired atau Outstanding
                        var dtmin = new DateTime(tahunpajak, masapajak, 1).AddMonths(maxpelaporan);
                        var dtmax =
                            new DateTime(tahunpajak, masapajak, 1).AddMonths(Math.Abs(minpelaporan));

                        var statoutstanding = ApplicationEnums.StatusDigantiOutstanding.Outstanding;

                        var availableperiods = OpenClosePeriods.GetByRange(dtmin, dtmax);

                        if (availableperiods.Count > 0)
                        {
                            //jika semua nya tidak ada yang open maka langsung expired
                            var chkopencloseperiod =
                                availableperiods.Where(c => c.StatusRegular).ToList();
                            if (chkopencloseperiod.Count <= 0)
                            {
                                statoutstanding = ApplicationEnums.StatusDigantiOutstanding.Expired;
                            }
                        }

                        fp.TahunPajak = null;
                        fp.MasaPajak = null;

                        var masterResult = FakturPajaks.Save(fp);
                        info.FakturPajakId = masterResult.FakturPajakId;

                        var dtsaved = FakturPajaks.GetById(info.FakturPajakId);

                        var fpoutstanding = new FakturPajakDigantiOutstanding()
                        {
                            Id = 0,
                            FormatedNoFaktur = dtsaved.FormatedNoFaktur,
                            TahunPajak = tahunpajak,
                            MasaPajak = masapajak,
                            StatusApproval = info.StatusApproval,
                            StatusFaktur = info.StatusFaktur,
                            Keterangan = null,
                            KeteranganDjp = null,
                            StatusOutstanding = (int)statoutstanding,
                            CreatedBy = userName
                        };

                        FakturPajakDigantiOutstandings.Save(fpoutstanding);

                    }
                    else
                    {

                        var masterResult = FakturPajaks.Save(fp);
                        info.FakturPajakId = masterResult.FakturPajakId;
                    }

                }

                #endregion
            }
            else
            {
                //FP Normal Pengganti
                #region Logic FP Normal Pengganti

                //var fp = FakturPajaks.GetByUrlScan(info.UrlScan);
                var fp = FakturPajaks.GetByOriginalNoFaktur(info.NoFakturPajak).Where(
                            c =>
                                c.StatusFaktur !=
                                EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti)).FirstOrDefault();

                if (fp == null)
                {

                    //belum ada di system evis
                    if (info.StatusFaktur.ToLower() != "faktur diganti")
                    {
                        //save
                        fp = new FakturPajak
                        {
                            FakturPajakId = info.FakturPajakId,
                            UrlScan = info.UrlScan,
                            KdJenisTransaksi = info.KdJenisTransaksi,
                            FgPengganti = info.FgPengganti,
                            NoFakturPajak = info.NoFakturPajak,
                            TglFaktur = null,
                            NPWPPenjual = info.NPWPPenjual,
                            NamaPenjual = info.NamaPenjual,
                            AlamatPenjual = info.AlamatPenjual,
                            VendorId = info.VendorId,
                            FCode = ApplicationConstant.FCodeFm,
                            FPType = (int)info.FPType,
                            ScanType = (int)info.ScanType,
                            NPWPLawanTransaksi = info.NPWPLawanTransaksi,
                            NamaLawanTransaksi = info.NamaLawanTransaksi,
                            AlamatLawanTransaksi = info.AlamatLawanTransaksi,
                            JumlahDPP = Convert.ToDecimal(info.JumlahDPP),
                            JumlahPPN = Convert.ToDecimal(info.JumlahPPN),
                            JumlahPPNBM = Convert.ToDecimal(info.JumlahPPNBM),
                            StatusApproval = info.StatusApproval,
                            StatusFaktur = info.StatusFaktur,
                            Pesan = info.StatusApproval,
                            Dikreditkan = info.Dikreditkan == "Ya" || info.Dikreditkan.ToLower() == "ya",
                            MasaPajak = masapajak,
                            TahunPajak = tahunpajak,
                            Referensi = info.Referensi
                        };
                        if (info.FPType == ApplicationEnums.FPType.ScanIws)
                        {
                            fp.ReceivingDate = Convert.ToDateTime(info.ReceivingDate);
                        }
                        else
                        {
                            fp.ReceivingDate = null;
                        }

                        if (!string.IsNullOrEmpty(info.TglFaktur))
                        {
                            fp.TglFaktur = DateTime.ParseExact(info.TglFaktur.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }

                        fp.Status = (int)ApplicationEnums.StatusFakturPajak.Success;
                        fp.CreatedBy = userName;
                        fp.ModifiedBy = userName;

                        var masterResult = FakturPajaks.Save(fp);
                        info.FakturPajakId = masterResult.FakturPajakId;

                        //FP Pengganti
                        //check apakah FP Diganti nya ada di FP Diganti Outstanding
                        var chks = FakturPajakDigantiOutstandings.GetByOriginalNoFaktur(fp.NoFakturPajak);
                        if (chks.Count > 0 &&
                            chks.Any(c => c.StatusOutstanding == (int)ApplicationEnums.StatusDigantiOutstanding.Outstanding || c.StatusOutstanding == (int)ApplicationEnums.StatusDigantiOutstanding.Expired))
                        {
                            var listFpDigantiOutstanding =
                                chks.Where(
                                    c => c.StatusOutstanding == (int)ApplicationEnums.StatusDigantiOutstanding.Outstanding || c.StatusOutstanding == (int)ApplicationEnums.StatusDigantiOutstanding.Expired).ToList();
                            //update menjadi completed dan keterangan sesuai CR
                            var keterangantosave = @"FP Diganti dilaporkan SPT Pembetulan"; //Nilai Default jika dari GeneralCategory tidak ditemukan
                            var cfgketerangan =
                                GeneralCategories.GetByCategories(ApplicationEnums.EnumGeneralCategory.FpDigantiOutstandingRemarks);
                            if (cfgketerangan.Count > 0 && cfgketerangan.Any(c => c.Code == "1"))
                            {
                                keterangantosave = cfgketerangan.First(c => c.Code == "1").Name;
                            }
                            foreach (var x in listFpDigantiOutstanding)
                            {
                                FakturPajakDigantiOutstandings.UpdateStatusByFormatedNoFaktur(x.FormatedNoFaktur,
                                    ApplicationEnums.StatusDigantiOutstanding.Complete, keterangantosave, userName);
                            }
                        }

                    }
                }

                #endregion
            }
            return info.FakturPajakId;
        }

        private long SaveFakturPajakKhusus(FakturPajakInfoModel info, string userName)
        {

            var fp = new FakturPajak
            {
                FakturPajakId = info.FakturPajakId,
                KdJenisTransaksi = info.KdJenisTransaksi,
                FgPengganti = info.FgPengganti,
                NoFakturPajak = info.NoFakturPajak,
                TglFaktur = null,
                NPWPPenjual = info.NPWPPenjual,
                NamaPenjual = info.NamaPenjual,
                AlamatPenjual = info.AlamatPenjual,
                VendorId = info.VendorId,
                FCode = EnumHelper.GetDescription(ApplicationEnums.FCodeFpKhusus.Dm),
                FPType = (int)ApplicationEnums.FPType.ScanManual,
                ScanType = (int)ApplicationEnums.ScanType.Manual,
                JumlahDPP = Convert.ToDecimal(info.JumlahDPP),
                JumlahPPN = Convert.ToDecimal(info.JumlahPPN),
                JumlahPPNBM = Convert.ToDecimal(info.JumlahPPNBM),
                Pesan = info.Pesan,
                MasaPajak = Convert.ToInt32(info.MasaPajak),
                TahunPajak = Convert.ToInt32(info.TahunPajak),
                JenisTransaksi = info.JenisTransaksi,
                JenisDokumen = info.JenisDokumen,
                NoFakturYangDiganti = info.NoFakturYangDiganti
            };

            if (!string.IsNullOrEmpty(info.TglFaktur))
            {
                fp.TglFaktur = DateTime.ParseExact(info.TglFaktur.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            fp.Status = (int)ApplicationEnums.StatusFakturPajak.Success;
            fp.CreatedBy = userName;
            fp.ModifiedBy = userName;

            var masterResult = FakturPajaks.SaveFakturPajakKhusus(fp);

            return masterResult.FakturPajakId;

        }

        private long SaveScanBulkFakturPajak(FakturPajakInfoModel info)
        {
            var userName = Session["UserName"] as string;

            var fp = new FakturPajak
            {
                UrlScan = info.UrlScan,
                FCode = ApplicationConstant.FCodeFm,
                FPType = (int)info.FPType,
                ScanType = (int)info.ScanType,
                Dikreditkan = info.Dikreditkan == "Ya" || info.Dikreditkan.ToLower() == "ya",
                MasaPajak = Convert.ToInt32(info.MasaPajak),
                TahunPajak = Convert.ToInt32(info.TahunPajak)
            };
            if (info.FPType == ApplicationEnums.FPType.ScanIws)
            {
                fp.ReceivingDate = Convert.ToDateTime(info.ReceivingDate);
            }
            else
            {
                fp.ReceivingDate = null;
            }

            fp.Status = (int)ApplicationEnums.StatusFakturPajak.Scanned;
            fp.CreatedBy = userName;
            fp.ModifiedBy = userName;

            var masterResult = FakturPajaks.SaveScanBulk(fp);

            return masterResult.FakturPajakId;
        }

        private RequestResultModel ValidationScanBulk(FakturPajakInfoModel info)
        {
            var model = new RequestResultModel
            {
                InfoType = RequestResultInfoType.Success,
                Message = string.Empty
            };
            info.UrlScan = info.UrlScan.Trim();
            var msgs = new List<string>();
            if (string.IsNullOrEmpty(info.MasaPajak) || info.MasaPajak == "0")
            {
                msgs.Add("Masa Pajak Mandatory");
            }
            if (string.IsNullOrEmpty(info.TahunPajak) || info.TahunPajak == "0")
            {
                msgs.Add("Tahun Pajak Mandatory");
            }

            if (!(string.IsNullOrEmpty(info.MasaPajak) || info.MasaPajak == "0") &&
                !(string.IsNullOrEmpty(info.TahunPajak) || info.TahunPajak == "0"))
            {
                var getOpenClosePeriod = OpenClosePeriods.GetByMasaPajak(int.Parse(info.MasaPajak),
                    int.Parse(info.TahunPajak));

                if (getOpenClosePeriod != null)
                {
                    if (!getOpenClosePeriod.StatusRegular)
                    {
                        msgs.Add("Status Masa Pajak Close Reguler");
                    }
                    else
                    {
                        if (!getOpenClosePeriod.StatusSp2)
                        {
                            msgs.Add("Status Masa Pajak Close SP2");
                        }
                    }
                }
                else
                {
                    msgs.Add("Masa dan Tahun Pajak tidak tersedia di Data Open Close Periode");
                }
            }

            if (info.FPType == ApplicationEnums.FPType.ScanIws)
            {
                if (info.ReceivingDate == null || info.ReceivingDate.Trim().Length == 0)
                {
                    msgs.Add("Receiving Date tidak boleh kosong");
                }
            }
            if (info.UrlScan == null || info.UrlScan.Trim().Length == 0)
            {
                msgs.Add("Scan Url Mandatory");
            }
            else
            {
                var checkByUrl = FakturPajaks.GetByUrlScan(info.UrlScan);
                if (checkByUrl != null && checkByUrl.MasaPajak.HasValue && checkByUrl.TahunPajak.HasValue)
                {
                    string s;
                    if (checkByUrl.Status == (int)ApplicationEnums.StatusFakturPajak.Success)
                    {
                        s = "Sudah discan pada " +
                            ConvertHelper.DateTimeConverter.ToLongDateString(checkByUrl.Created) + " oleh " +
                            checkByUrl.CreatedBy +
                            ". No FP " + checkByUrl.FormatedNoFaktur + " sudah ada di Masa Pajak " + checkByUrl.MasaPajakName +
                            " " + checkByUrl.TahunPajak
                            + ", Nomor Filling Index " + checkByUrl.FillingIndex;
                    }
                    else
                    {
                        s = "Sudah discan pada "
                            + ConvertHelper.DateTimeConverter.ToLongDateString(checkByUrl.Created) + " oleh " +
                            checkByUrl.CreatedBy +
                            ". Di Masa Pajak " +
                            checkByUrl.MasaPajakName + " " + checkByUrl.TahunPajak
                            + ", Nomor Filling Index " + checkByUrl.FillingIndex + ". Tapi belum request ke DJP atau ada error validasi.";
                    }
                    msgs.Add(s);
                }

            }

            if (msgs.Count > 0)
            {
                model.InfoType = RequestResultInfoType.Warning;
                model.Message = string.Join("<br />", msgs);
            }

            return model;

        }
        #region "Methode yg sebelum dicopy dari branch utama"
        private RequestResultModel ValidationScanSatuan(FakturPajakInfoModel info)
        {
            var model = new RequestResultModel
            {
                InfoType = RequestResultInfoType.Success,
                Message = string.Empty
            };
            info.UrlScan = info.UrlScan.Trim();
            var msgs = new List<string>();
            int tahunPajak = 0;
            int masaPajak = 0;
            if (string.IsNullOrEmpty(info.MasaPajak) || info.MasaPajak == "0")
            {
                msgs.Add("Masa Pajak Mandatory");
            }
            else
            {
                if (!int.TryParse(info.MasaPajak.Trim(), out masaPajak))
                {
                    msgs.Add("Invalid Tahun Pajak");
                }
            }
            if (string.IsNullOrEmpty(info.TahunPajak) || info.TahunPajak == "0")
            {
                msgs.Add("Tahun Pajak Mandatory");
            }
            else
            {
                if (!int.TryParse(info.TahunPajak.Trim(), out tahunPajak))
                {
                    msgs.Add("Invalid Tahun Pajak");
                }
            }

            if (!(string.IsNullOrEmpty(info.MasaPajak) || info.MasaPajak == "0") &&
                !(string.IsNullOrEmpty(info.TahunPajak) || info.TahunPajak == "0"))
            {
                var getOpenClosePeriod = OpenClosePeriods.GetByMasaPajak(int.Parse(info.MasaPajak),
                    int.Parse(info.TahunPajak));

                if (getOpenClosePeriod != null)
                {
                    if (!getOpenClosePeriod.StatusRegular)
                    {
                        msgs.Add("Status Masa Pajak Close Reguler");
                    }
                    else
                    {
                        if (!getOpenClosePeriod.StatusSp2)
                        {
                            msgs.Add("Status Masa Pajak Close SP2");
                        }
                    }
                }
                else
                {
                    msgs.Add("Masa dan Tahun Pajak tidak tersedia di Data Open Close Periode");
                }
            }

            if (info.FPType == ApplicationEnums.FPType.ScanIws)
            {
                if (info.ReceivingDate == null || info.ReceivingDate.Trim().Length == 0)
                {
                    msgs.Add("Receiving Date tidak boleh kosong");
                }
            }
            if (info.UrlScan == null || info.UrlScan.Trim().Length == 0)
            {
                msgs.Add("Scan Url Mandatory");
            }
            if (info.NoFakturPajak == null || info.NoFakturPajak.Trim().Length == 0)
            {
                msgs.Add("No Faktur Pajak Mandatory");
            }

            if (string.IsNullOrEmpty(info.TglFaktur))
            {
                msgs.Add("Tanggal Faktur Pajak Mandatory");
            }
            else
            {
                //getconfig
                var configData = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.PelaporanTglFaktur);
                if (configData == null)
                {
                    msgs.Add("GeneralConfig [PelaporanTglFaktur] not found.");
                }
                else
                {
                    var dats = configData.ConfigValue.Split(':').ToList();
                    if (dats.Count != 2)
                    {
                        msgs.Add("GeneralConfig [PelaporanTglFaktur] not valid.");
                    }
                    else
                    {
                        if (masaPajak != 0 && tahunPajak != 0)
                        {
                            if (!string.IsNullOrEmpty(info.FgPengganti) && info.FgPengganti == "0")
                            {
                                //BPM No. ASMO3-201847620
                                int min = int.Parse(dats[0].Replace("[", "").Replace("]", "")); // -3
                                int max = int.Parse(dats[1].Replace("[", "").Replace("]", "")); // 0
                                var dtMin = new DateTime(tahunPajak, masaPajak, 1).AddMonths(min); // ex: oktober -> 1 juli
                                var dtMax = new DateTime(tahunPajak, masaPajak, 1).AddMonths(max + 1).AddDays(-1); // ex: oktober -> tgl terakhir bulan oktober
                                var dTglFaktur = Convert.ToDateTime(info.TglFaktur);
                                var dtMaxValidity = new DateTime(dtMax.Year, dtMax.Month, DateTime.DaysInMonth(dtMax.Year, dtMax.Month)); // ex: 30 oktober
                                if (dTglFaktur < dtMin) // juli < april
                                {
                                    msgs.Add("Tanggal Faktur Pajak sudah kadaluarsa");
                                }
                                else
                                {
                                    if (dTglFaktur > dtMaxValidity)
                                    {
                                        msgs.Add("Tanggal Faktur Pajak tidak sesuai ketentuan");
                                    }
                                }
                            }
                        }

                    }
                }
            }

            //Check NPWP Adm
            if (string.IsNullOrEmpty(info.NPWPLawanTransaksi))
            {
                msgs.Add("NPWP Pembeli Mandatory");
            }
            else
            {
                var chkConfig = GeneralConfigs.GetConfigCheckNpwpAdm(info.NPWPLawanTransaksi);
                if (chkConfig == null)
                {
                    var npwpAdm = GeneralConfigs.GetByKeyId(ApplicationEnums.GeneralConfig.NpwpAdm);
                    msgs.Add("NPWP Pembeli bukan " + npwpAdm.ConfigValue);
                }
            }

            //Check NPWP Adm
            if (string.IsNullOrEmpty(info.NamaLawanTransaksi))
            {
                msgs.Add("Nama Pembeli Mandatory");
            }
            else
            {
                if (!string.IsNullOrEmpty(info.FgPengganti) && info.StatusFaktur != "Faktur Diganti")
                {
                    if (info.FgPengganti != "2")
                    {
                        var d = GeneralConfigs.GetByKeyId(ApplicationEnums.GeneralConfig.NamaNpwpAdm);

                        var dList = d.ConfigValue.Split(';').Where(c => !string.IsNullOrEmpty(c) && !string.IsNullOrEmpty(c.Trim())).ToList();
                        var availableNamaPembeli = (string.Join(",", dList));

                        var chk = dList.Where(dItem => info.NamaLawanTransaksi.Replace(" ", "").ToLower().Contains(dItem.Replace(" ", "").ToLower())).ToList();

                        if (chk.Count <= 0)
                        {
                            msgs.Add("Nama Pembeli bukan yang diperbolehkan [" + availableNamaPembeli + "]");
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(info.NPWPPenjual))
            {
                msgs.Add("NPWP Penjual Mandatory");
            }
            else
            {
                var v = Vendors.GetByNpwp(info.NPWPPenjual);
                if (v != null && v.PkpDicabut)
                {
                    msgs.Add("PKP Dicabut atas NPWP Penjual");
                }
            }

            if (string.IsNullOrEmpty(info.KdJenisTransaksi))
            {
                msgs.Add("KdJenisTransaksi Mandatory");
            }

            if (string.IsNullOrEmpty(info.FgPengganti))
            {
                msgs.Add("FgPengganti Mandatory");
            }

            if (string.IsNullOrEmpty(info.StatusFaktur))
            {
                msgs.Add("Status Faktur Mandatory");
            }

            if (msgs.Count <= 0)
            {
                /*
                 * Disable berdasarkan CR Perubahan dari sisi e-Faktur V2.0
                 * Terkait scan FP Pengganti (Kode yang digit ke -3 ‘1’ contoh: 011,021,031,041) maka tidak dibutuhkan scan FP awal (Kode yang digit ke -3 ‘0’ contoh: 010,020,030,040)"
                 */
                /*
                 * Rollback berdasarkan CR terbaru, tetap ada validasi harus scan faktur pajak normal (Kode yang digit ke-3 nya '0'
                 * BPM No. ASMO3-201847620 
                 */
                if (info.FgPengganti != "0")
                {
                    /* BPM No. ASMO3-201847620
                     * (c) Saat user scan FP normal-pengganti, maka akan muncul request untuk scan FP diganti terlebih
                     * dahulu agar dapat mengetahui kapan FP diganti tersebut kadaluarsa (jika FP diganti
                     * expired/kadaluarsa maka ditolak oleh EVIS). - jika 010 belum di scan di EVIS 
                     */
                    var msgValidatePengganti = FakturPajaks.ValidateScanPengganti(info.NoFakturPajak, info.FgPengganti,
                        info.FPType);
                    if (!string.IsNullOrEmpty(msgValidatePengganti))
                    {
                        msgs.Add(msgValidatePengganti);
                    }
                    else
                    {
                        /* BPM No. ASMO3-201847620
                         * (a) Jika FP diganti (kode awal 010) ada dan belum expired, maka FP normal-pengganti (kode awal 011) dapat diterima/discan. 
                         * - langsung masuk (dan masih open), FP 010 & 011 harus discan dimasa yang sama. 
                        */
                        var fpNormal = FakturPajaks.GetFakturPajakNormal(info.NoFakturPajak);
                        if (fpNormal != null && fpNormal.FakturPajakId != 0)
                        {
                            var configData = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.PelaporanTglFaktur);
                            if (configData == null)
                            {
                                msgs.Add("GeneralConfig [PelaporanTglFaktur] not found.");
                            }
                            else
                            {
                                //check apakah Faktur Normal sudah Expired
                                var dats = configData.ConfigValue.Split(':').ToList();
                                if (dats.Count != 2)
                                {
                                    msgs.Add("GeneralConfig [PelaporanTglFaktur] not valid.");
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
                                            msgs.Add("Faktur Pajak Diganti sudah kadarluasa");
                                        }
                                        //else
                                        //{
                                        //    if (!(tahunPajak == fpNormal.TahunPajak && masaPajak == fpNormal.MasaPajak))
                                        //    {
                                        //        msgs.Add("Faktur Pajak Pengganti harus di Masa-Tahun Pajak yang sama dengan Faktur Pajak Normal");
                                        //    }
                                        //}

                                        //if (!(tahunPajak == fpNormal.TahunPajak && masaPajak == fpNormal.MasaPajak))
                                        //{
                                        //    msgs.Add("Faktur Pajak Pengganti harus di Masa-Tahun Pajak yang sama dengan Faktur Pajak Normal");
                                        //}

                                        //CR DZ - Validasi Scan Pengganti (11) Jika FP Digantinya masih berstatus FP-Nornal


                                        DateTime monthName = new DateTime(2020, fpNormal.MasaPajak.Value, 1);
                                        if (fpNormal.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturPajakNormal))
                                        {
                                            msgs.Add("Faktur Pajak " + fpNormal.FormatedNoFaktur + " sudah dilaporkan di masa pajak " + monthName.ToString("MMMM") + " " + fpNormal.TahunPajak.ToString());
                                            // msgs.Add("No FP Diganti [" + fpNormal.FormatedNoFaktur + "] belum discan");

                                        }

                                    }

                                }
                            }
                        }
                    }
                }

                /*
                 * Penambahan validasi berdasarkan CR Perubahan dari sisi e-Faktur V2.0
                 * Ada validasi tambahan ketika semua Faktur Pajak (apapun kode FP nya) yang di scan adalah “Faktur Diganti” maka akan muncul notifikasi “Faktur Pajak sudah diganti”
                 */
                //if (info.StatusFaktur.ToLower().Trim() == "faktur diganti")
                //{
                //    msgs.Add("Faktur Pajak sudah diganti");
                //}

                /*
                 * CR Penambahan validasi untuk faktur pajak yang sudah dibatalkan
                 * BPM No. ASMO3-201847620 
                 */
                if (info.StatusFaktur.ToLower().Trim() == "faktur dibatalkan")
                {
                    msgs.Add("Faktur Pajak Dibatalkan");
                }

                /* BPM No. ASMO3-201847620
                 * (d) Jika FP dengan kode 011 statusnya adalah FP diganti maka akan ditolak oleh EVIS - jika ada 2
                 * FP pengganti
                 */
                if (info.StatusFaktur.ToLower().Trim() == "faktur diganti" && info.FgPengganti != "0")
                {
                    msgs.Add("Faktur Pajak Normal - Pengganti sudah diganti");
                }

                //CR DZ -- Validasi Scan Diganti (10), jika FP Normal sudah dilaporkan, lalu dari DJP mengeluarkan FP Digantinya.
                if (info.FgPengganti == "0" && info.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti))
                {
                    var fpNormal = FakturPajaks.GetFakturPajakNormal(info.NoFakturPajak);
                    if (fpNormal != null && fpNormal.FakturPajakId != 0)
                    {
                        if (fpNormal.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturPajakNormal) && fpNormal.MasaPajak.ToString() != info.MasaPajak.ToString())
                        {
                            var getOpenClosePeriod = OpenClosePeriods.GetByMasaPajak(fpNormal.MasaPajak.Value, fpNormal.TahunPajak.Value);
                            if (getOpenClosePeriod != null)
                            {
                                if (!getOpenClosePeriod.StatusRegular || !getOpenClosePeriod.StatusSp2)
                                {
                                    //                                    msgs.Add("Faktur Pajak-Nomal sudah dilaporkan");
                                    //  DateTime monthName = new DateTime(2020, fpNormal.MasaPajak.Value, 1);
                                    //  msgs.Add("Faktur Pajak " + fpNormal.FormatedNoFaktur + " sudah dilaporkan di masa pajak " + monthName.ToString("MMMM") + " " + fpNormal.TahunPajak.ToString());

                                    msgs.Add("Sudah discan pada " +
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

            if (info.FakturPajakId > 0)
            {
                FakturPajak fp = FakturPajaks.GetById(info.FakturPajakId);

                if (fp.IsDeleted)
                {
                    msgs.Add(String.Format("Data Sudah dihapus oleh '{0}'", fp.ModifiedBy));

                    model.InfoType = RequestResultInfoType.Warning;
                    model.Message = string.Join("<br />", msgs);
                    return model;
                }
            }


            if (msgs.Count <= 0) return model;

            model.InfoType = RequestResultInfoType.Warning;
            model.Message = string.Join("<br />", msgs);
            return model;
        }
        #endregion

        private RequestResultModel ValidationScanSatuanDariMerging(FakturPajakInfoModel info)
        {
            var model = new RequestResultModel
            {
                InfoType = RequestResultInfoType.Success,
                Message = string.Empty
            };
            info.UrlScan = info.UrlScan.Trim();
            var msgs = new List<string>();
            int tahunPajak = 0;
            int masaPajak = 0;
            if (string.IsNullOrEmpty(info.MasaPajak) || info.MasaPajak == "0")
            {
                msgs.Add("Masa Pajak Mandatory");
            }
            else
            {
                if (!int.TryParse(info.MasaPajak.Trim(), out masaPajak))
                {
                    msgs.Add("Invalid Tahun Pajak");
                }
            }
            if (string.IsNullOrEmpty(info.TahunPajak) || info.TahunPajak == "0")
            {
                msgs.Add("Tahun Pajak Mandatory");
            }
            else
            {
                if (!int.TryParse(info.TahunPajak.Trim(), out tahunPajak))
                {
                    msgs.Add("Invalid Tahun Pajak");
                }
            }

            if (!(string.IsNullOrEmpty(info.MasaPajak) || info.MasaPajak == "0") &&
                !(string.IsNullOrEmpty(info.TahunPajak) || info.TahunPajak == "0"))
            {
                var getOpenClosePeriod = OpenClosePeriods.GetByMasaPajak(int.Parse(info.MasaPajak),
                    int.Parse(info.TahunPajak));

                if (getOpenClosePeriod != null)
                {
                    if (!getOpenClosePeriod.StatusRegular)
                    {
                        msgs.Add("Status Masa Pajak Close Reguler");
                    }
                    else
                    {
                        if (!getOpenClosePeriod.StatusSp2)
                        {
                            msgs.Add("Status Masa Pajak Close SP2");
                        }
                    }
                }
                else
                {
                    msgs.Add("Masa dan Tahun Pajak tidak tersedia di Data Open Close Periode");
                }
            }

            if (info.FPType == ApplicationEnums.FPType.ScanIws)
            {
                if (info.ReceivingDate == null || info.ReceivingDate.Trim().Length == 0)
                {
                    msgs.Add("Receiving Date tidak boleh kosong");
                }
            }
            if (info.UrlScan == null || info.UrlScan.Trim().Length == 0)
            {
                msgs.Add("Scan Url Mandatory");
            }
            if (info.NoFakturPajak == null || info.NoFakturPajak.Trim().Length == 0)
            {
                msgs.Add("No Faktur Pajak Mandatory");
            }

            if (string.IsNullOrEmpty(info.TglFaktur))
            {
                msgs.Add("Tanggal Faktur Pajak Mandatory");
            }
            else
            {
                //getconfig
                var configData = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.PelaporanTglFaktur);
                if (configData == null)
                {
                    msgs.Add("GeneralConfig [PelaporanTglFaktur] not found.");
                }
                else
                {
                    var dats = configData.ConfigValue.Split(':').ToList();
                    if (dats.Count != 2)
                    {
                        msgs.Add("GeneralConfig [PelaporanTglFaktur] not valid.");
                    }
                    else
                    {
                        if (masaPajak != 0 && tahunPajak != 0)
                        {
                            //BPM No. ASMO3-201847620
                            int min = int.Parse(dats[0].Replace("[", "").Replace("]", ""));
                            int max = int.Parse(dats[1].Replace("[", "").Replace("]", ""));
                            var dtMin = new DateTime(tahunPajak, masaPajak, 1).AddMonths(min);
                            var dtMax = new DateTime(tahunPajak, masaPajak, 1).AddMonths(max + 1).AddDays(-1);
                            var dTglFaktur = Convert.ToDateTime(info.TglFaktur);
                            var dtMaxValidity = new DateTime(dtMax.Year, dtMax.Month, DateTime.DaysInMonth(dtMax.Year, dtMax.Month));

                            if (!string.IsNullOrEmpty(info.FgPengganti) && info.FgPengganti == "0")
                            {

                                if (dTglFaktur < dtMin)
                                {
                                    msgs.Add("Tanggal Faktur Pajak sudah kadaluarsa");
                                }
                                else
                                {
                                    if (dTglFaktur > dtMaxValidity)
                                    {
                                        msgs.Add("Tanggal Faktur Pajak tidak sesuai ketentuan");
                                    }
                                }
                            }
                            else
                            {
                                if (info.StatusFaktur == "Faktur Pajak Normal-Pengganti")
                                {
                                    var fpDiganti = FakturPajaks.GetFakturPajakByNoFakturPajak(info.NoFakturPajak);

                                    dTglFaktur = Convert.ToDateTime(fpDiganti.TglFaktur);

                                    if (dTglFaktur < dtMin)
                                    {
                                        msgs.Add("Tanggal Faktur Pajak sudah kadaluarsa");
                                    }

                                }
                            }
                        }

                    }
                }
            }

            //Check NPWP Adm
            if (string.IsNullOrEmpty(info.NPWPLawanTransaksi))
            {
                msgs.Add("NPWP Pembeli Mandatory");
            }
            else
            {
                var chkConfig = GeneralConfigs.GetConfigCheckNpwpAdm(info.NPWPLawanTransaksi);
                if (chkConfig == null)
                {
                    var npwpAdm = GeneralConfigs.GetByKeyId(ApplicationEnums.GeneralConfig.NpwpAdm);
                    msgs.Add("NPWP Pembeli bukan " + npwpAdm.ConfigValue);
                }
            }

            //Check NPWP Adm
            if (string.IsNullOrEmpty(info.NamaLawanTransaksi))
            {
                msgs.Add("Nama Pembeli Mandatory");
            }
            else
            {
                if (!string.IsNullOrEmpty(info.FgPengganti))
                {
                    if (info.FgPengganti != "2")
                    {
                        if (info.StatusFaktur != "Faktur Diganti")
                        {
                            var d = GeneralConfigs.GetByKeyId(ApplicationEnums.GeneralConfig.NamaNpwpAdm);

                            var dList = d.ConfigValue.Split(';').Where(c => !string.IsNullOrEmpty(c) && !string.IsNullOrEmpty(c.Trim())).ToList();
                            var availableNamaPembeli = (string.Join(",", dList));

                            var chk = dList.Where(dItem => info.NamaLawanTransaksi.Replace(" ", "").ToLower().Contains(dItem.Replace(" ", "").ToLower())).ToList();

                            if (chk.Count <= 0)
                            {
                                msgs.Add("Nama Pembeli bukan yang diperbolehkan [" + availableNamaPembeli + "]");
                            }
                        }


                    }
                }
            }

            if (string.IsNullOrEmpty(info.NPWPPenjual))
            {
                msgs.Add("NPWP Penjual Mandatory");
            }
            else
            {
                var v = Vendors.GetByNpwp(info.NPWPPenjual);
                if (v != null && v.PkpDicabut)
                {
                    msgs.Add("PKP Dicabut atas NPWP Penjual");
                }
            }

            if (string.IsNullOrEmpty(info.KdJenisTransaksi))
            {
                msgs.Add("KdJenisTransaksi Mandatory");
            }

            if (string.IsNullOrEmpty(info.FgPengganti))
            {
                msgs.Add("FgPengganti Mandatory");
            }

            if (string.IsNullOrEmpty(info.StatusFaktur))
            {
                msgs.Add("Status Faktur Mandatory");
            }

            if (msgs.Count <= 0)
            {
                /*
                 * Disable berdasarkan CR Perubahan dari sisi e-Faktur V2.0
                 * Terkait scan FP Pengganti (Kode yang digit ke -3 ‘1’ contoh: 011,021,031,041) maka tidak dibutuhkan scan FP awal (Kode yang digit ke -3 ‘0’ contoh: 010,020,030,040)"
                 */
                /*
                 * Rollback berdasarkan CR terbaru, tetap ada validasi harus scan faktur pajak normal (Kode yang digit ke-3 nya '0'
                 * BPM No. ASMO3-201847620 
                 */
                if (info.FgPengganti != "0")
                {
                    /* BPM No. ASMO3-201847620
                     * (c) Saat user scan FP normal-pengganti, maka akan muncul request untuk scan FP diganti terlebih
                     * dahulu agar dapat mengetahui kapan FP diganti tersebut kadaluarsa (jika FP diganti
                     * expired/kadaluarsa maka ditolak oleh EVIS). - jika 010 belum di scan di EVIS 
                     */
                    var msgValidatePengganti = FakturPajaks.ValidateScanPengganti(info.NoFakturPajak, info.FgPengganti,
                        info.FPType);
                    if (!string.IsNullOrEmpty(msgValidatePengganti))
                    {
                        msgs.Add(msgValidatePengganti);
                    }
                    else
                    {
                        /* BPM No. ASMO3-201847620
                         * (a) Jika FP diganti (kode awal 010) ada dan belum expired, maka FP normal-pengganti (kode awal 011) dapat diterima/discan. 
                         * - langsung masuk (dan masih open), FP 010 & 011 harus discan dimasa yang sama. 
                        */
                        var fpNormal = FakturPajaks.GetFakturPajakNormal(info.NoFakturPajak);
                        if (fpNormal != null && fpNormal.FakturPajakId != 0)
                        {
                            var configData = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.PelaporanTglFaktur);
                            if (configData == null)
                            {
                                msgs.Add("GeneralConfig [PelaporanTglFaktur] not found.");
                            }
                            else
                            {
                                //check apakah Faktur Normal sudah Expired
                                var dats = configData.ConfigValue.Split(':').ToList();
                                if (dats.Count != 2)
                                {
                                    msgs.Add("GeneralConfig [PelaporanTglFaktur] not valid.");
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

                                        //int min = int.Parse(dats[0].Replace("[", "").Replace("]", ""));
                                        //var dtMin = new DateTime(tahunPajak, masaPajak, 1).AddMonths(min);
                                        //if (fpNormal.TglFaktur < dtMin)
                                        //{
                                        //    msgs.Add("Faktur Pajak Normal sudah kadarluasa");
                                        //}
                                        //else
                                        //{
                                        //    if (!(tahunPajak == fpNormal.TahunPajak && masaPajak == fpNormal.MasaPajak))
                                        //    {
                                        //        msgs.Add("Faktur Pajak Pengganti harus di Masa-Tahun Pajak yang sama dengan Faktur Pajak Normal");
                                        //    }
                                        //}

                                        //change by bram - 25 March 2022
                                        if (!(tahunPajak == fpNormal.TahunPajak && masaPajak == fpNormal.MasaPajak))
                                        {
                                            if (info.StatusFaktur != "Faktur Pajak Normal-Pengganti")
                                            {
                                                msgs.Add("Faktur Pajak Pengganti harus di Masa-Tahun Pajak yang sama dengan Faktur Pajak Normal");
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                }

                /*
                 * Penambahan validasi berdasarkan CR Perubahan dari sisi e-Faktur V2.0
                 * Ada validasi tambahan ketika semua Faktur Pajak (apapun kode FP nya) yang di scan adalah “Faktur Diganti” maka akan muncul notifikasi “Faktur Pajak sudah diganti”
                 */
                //if (info.StatusFaktur.ToLower().Trim() == "faktur diganti")
                //{
                //    msgs.Add("Faktur Pajak sudah diganti");
                //}

                /*
                 * CR Penambahan validasi untuk faktur pajak yang sudah dibatalkan
                 * BPM No. ASMO3-201847620 
                 */
                if (info.StatusFaktur.ToLower().Trim() == "faktur dibatalkan")
                {
                    msgs.Add("Faktur Pajak Dibatalkan");
                }

                /* BPM No. ASMO3-201847620
                 * (d) Jika FP dengan kode 011 statusnya adalah FP diganti maka akan ditolak oleh EVIS - jika ada 2
                 * FP pengganti
                 */
                if (info.StatusFaktur.ToLower().Trim() == "faktur diganti" && info.FgPengganti != "0")
                {
                    msgs.Add("Faktur Pajak Normal - Pengganti sudah diganti");
                }

            }

            if (info.FakturPajakId > 0)
            {
                FakturPajak fp = FakturPajaks.GetById(info.FakturPajakId);

                if (fp.IsDeleted)
                {
                    msgs.Add(String.Format("Data Sudah dihapus oleh '{0}'", fp.ModifiedBy));

                    model.InfoType = RequestResultInfoType.Warning;
                    model.Message = string.Join("<br />", msgs);
                    return model;
                }
            }


            if (msgs.Count <= 0) return model;

            model.InfoType = RequestResultInfoType.Warning;
            model.Message = string.Join("<br />", msgs);
            return model;
        }

        private RequestResultModel ValidationScanPembetulan(FakturPajakInfoModel info)
        {
            var model = new RequestResultModel
            {
                InfoType = RequestResultInfoType.Success,
                Message = string.Empty
            };
            info.UrlScan = info.UrlScan.Trim();
            var msgs = new List<string>();
            int tahunPajak = 0;
            int masaPajak = 0;
            if (string.IsNullOrEmpty(info.MasaPajak) || info.MasaPajak == "0")
            {
                msgs.Add("Masa Pajak Mandatory");
            }
            else
            {
                if (!int.TryParse(info.MasaPajak.Trim(), out masaPajak))
                {
                    msgs.Add("Invalid Tahun Pajak");
                }
            }
            if (string.IsNullOrEmpty(info.TahunPajak) || info.TahunPajak == "0")
            {
                msgs.Add("Tahun Pajak Mandatory");
            }
            else
            {
                if (!int.TryParse(info.TahunPajak.Trim(), out tahunPajak))
                {
                    msgs.Add("Invalid Tahun Pajak");
                }
            }

            if (!(string.IsNullOrEmpty(info.MasaPajak) || info.MasaPajak == "0") &&
                !(string.IsNullOrEmpty(info.TahunPajak) || info.TahunPajak == "0"))
            {
                var getOpenClosePeriod = OpenClosePeriods.GetByMasaPajak(int.Parse(info.MasaPajak),
                    int.Parse(info.TahunPajak));

                if (getOpenClosePeriod != null)
                {
                    if (getOpenClosePeriod.StatusRegular)
                    {
                        msgs.Add("Status Masa Pajak Open Reguler, seharusnya dalam Close Reguler");
                    }
                    else
                    {
                        if (!getOpenClosePeriod.StatusSp2)
                        {
                            msgs.Add("Status Masa Pajak Close SP2");
                        }
                    }
                }
                else
                {
                    msgs.Add("Masa dan Tahun Pajak tidak tersedia di Data Open Close Periode");
                }
            }

            if (info.FPType == ApplicationEnums.FPType.ScanIws)
            {
                if (info.ReceivingDate == null || info.ReceivingDate.Trim().Length == 0)
                {
                    msgs.Add("Receiving Date Mandatory");
                }
            }
            if (info.UrlScan == null || info.UrlScan.Trim().Length == 0)
            {
                msgs.Add("Scan Url Mandatory");
            }

            if (info.NoFakturPajak == null || info.NoFakturPajak.Trim().Length == 0)
            {
                msgs.Add("No Faktur Pajak Mandatory");
            }

            if (string.IsNullOrEmpty(info.TglFaktur))
            {
                msgs.Add("Tanggal Faktur Pajak Mandatory");
            }
            else
            {
                //getconfig
                var configData = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.PelaporanTglFaktur);
                if (configData == null)
                {
                    msgs.Add("GeneralConfig [PelaporanTglFaktur] not found.");
                }
                else
                {
                    var dats = configData.ConfigValue.Split(':').ToList();
                    if (dats.Count != 2)
                    {
                        msgs.Add("GeneralConfig [PelaporanTglFaktur] not valid.");
                    }
                    else
                    {
                        if (masaPajak != 0 && tahunPajak != 0)
                        {
                            if (!string.IsNullOrEmpty(info.FgPengganti) && info.FgPengganti == "0")
                            {
                                //BPM No. ASMO3-201847620
                                int min = int.Parse(dats[0].Replace("[", "").Replace("]", ""));
                                int max = int.Parse(dats[1].Replace("[", "").Replace("]", ""));
                                var dtMin = new DateTime(tahunPajak, masaPajak, 1).AddMonths(min);
                                var dtMax = new DateTime(tahunPajak, masaPajak, 1).AddMonths(max + 1).AddDays(-1);
                                var dTglFaktur = Convert.ToDateTime(info.TglFaktur);
                                var dtMaxValidity = new DateTime(dtMax.Year, dtMax.Month, DateTime.DaysInMonth(dtMax.Year, dtMax.Month));
                                if (dTglFaktur < dtMin)
                                {
                                    msgs.Add("Tanggal Faktur Pajak sudah kadaluarsa");
                                }
                                else
                                {
                                    if (dTglFaktur > dtMaxValidity)
                                    {
                                        msgs.Add("Tanggal Faktur Pajak tidak sesuai ketentuan");
                                    }
                                }
                            }

                        }

                    }
                }
            }

            //Check NPWP Adm
            if (string.IsNullOrEmpty(info.NPWPLawanTransaksi))
            {
                msgs.Add("NPWP Pembeli Mandatory");
            }
            else
            {
                var chkConfig = GeneralConfigs.GetConfigCheckNpwpAdm(info.NPWPLawanTransaksi);
                if (chkConfig == null)
                {
                    var npwpAdm = GeneralConfigs.GetByKeyId(ApplicationEnums.GeneralConfig.NpwpAdm);
                    msgs.Add("NPWP Pembeli bukan " + npwpAdm.ConfigValue);
                }
            }

            //Check NPWP Adm
            if (string.IsNullOrEmpty(info.NamaLawanTransaksi))
            {
                msgs.Add("Nama Pembeli Mandatory");
            }
            else
            {
                if (!string.IsNullOrEmpty(info.FgPengganti))
                {
                    if (info.FgPengganti != "0")
                    {
                        var d = GeneralConfigs.GetByKeyId(ApplicationEnums.GeneralConfig.NamaNpwpAdm);

                        var dList = d.ConfigValue.Split(';').Where(c => !string.IsNullOrEmpty(c) && !string.IsNullOrEmpty(c.Trim())).ToList();
                        var availableNamaPembeli = (string.Join(",", dList));

                        var chk = dList.Where(dItem => info.NamaLawanTransaksi.Replace(" ", "").ToLower().Contains(dItem.Replace(" ", "").ToLower())).ToList();

                        if (chk.Count <= 0)
                        {
                            msgs.Add("Nama Pembeli bukan yang diperbolehkan [" + availableNamaPembeli + "]");
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(info.NPWPPenjual))
            {
                msgs.Add("NPWP Penjual Mandatory");
            }
            else
            {
                var v = Vendors.GetByNpwp(info.NPWPPenjual);
                if (v != null && v.PkpDicabut)
                {
                    msgs.Add("PKP Dicabut atas NPWP Penjual");
                }
            }

            if (string.IsNullOrEmpty(info.KdJenisTransaksi))
            {
                msgs.Add("KdJenisTransaksi Mandatory");
            }

            if (string.IsNullOrEmpty(info.FgPengganti))
            {
                msgs.Add("FgPengganti Mandatory");
            }

            if (msgs.Count <= 0)
            {
                /*
                 * Disable berdasarkan CR Perubahan dari sisi e-Faktur V2.0
                 * Terkait scan FP Pengganti (Kode yang digit ke -3 ‘1’ contoh: 011,021,031,041) maka tidak dibutuhkan scan FP awal (Kode yang digit ke -3 ‘0’ contoh: 010,020,030,040)"
                 */
                /*
                 * Rollback berdasarkan CR terbaru, tetap ada validasi harus scan faktur pajak normal (Kode yang digit ke-3 nya '0'
                 * BPM No. ASMO3-201847620 
                 */
                if (info.FgPengganti != "0")
                {
                    /* BPM No. ASMO3-201847620
                     * (c) Saat user scan FP normal-pengganti, maka akan muncul request untuk scan FP diganti terlebih
                     * dahulu agar dapat mengetahui kapan FP diganti tersebut kadaluarsa (jika FP diganti
                     * expired/kadaluarsa maka ditolak oleh EVIS). - jika 010 belum di scan di EVIS 
                     */
                    var msgValidatePengganti = FakturPajaks.ValidateScanPengganti(info.NoFakturPajak, info.FgPengganti,
                        info.FPType);
                    if (!string.IsNullOrEmpty(msgValidatePengganti))
                    {
                        msgs.Add(msgValidatePengganti);
                    }
                    else
                    {
                        /* BPM No. ASMO3-201847620
                         * (a) Jika FP diganti (kode awal 010) ada dan belum expired, maka FP normal-pengganti (kode awal 011) dapat diterima/discan. 
                         * - langsung masuk (dan masih open), FP 010 & 011 harus discan dimasa yang sama. 
                        */
                        var fpNormal = FakturPajaks.GetFakturPajakNormal(info.NoFakturPajak);
                        if (fpNormal != null && fpNormal.FakturPajakId != 0)
                        {
                            var configData = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.PelaporanTglFaktur);
                            if (configData == null)
                            {
                                msgs.Add("GeneralConfig [PelaporanTglFaktur] not found.");
                            }
                            else
                            {
                                //check apakah Faktur Normal sudah Expired
                                var dats = configData.ConfigValue.Split(':').ToList();
                                if (dats.Count != 2)
                                {
                                    msgs.Add("GeneralConfig [PelaporanTglFaktur] not valid.");
                                }
                                else
                                {
                                    /* BPM No. ASMO3-201847620
                                     * (b) Jika FP diganti (kode awal 010) sudah expired, maka FP normal-pengganti (kode awal 011) akan
                                     * ditolak oleh EVIS. 
                                     * Tgl Faktur dibandingkan dengan TahunPajak-MasaPajak
                                    // */
                                    //if (tahunPajak != 0 && masaPajak != 0)
                                    //{
                                    //    if (!(tahunPajak == fpNormal.TahunPajak && masaPajak == fpNormal.MasaPajak))
                                    //    {
                                    //        msgs.Add("Faktur Pajak Pengganti harus di Masa-Tahun Pajak yang sama dengan Faktur Pajak Normal");
                                    //    }
                                    //}

                                    int min = int.Parse(dats[0].Replace("[", "").Replace("]", ""));
                                    var dtMin = new DateTime(tahunPajak, masaPajak, 1).AddMonths(min);
                                    if (fpNormal.TglFaktur < dtMin)
                                    {
                                        msgs.Add("Faktur Pajak Diganti sudah kadarluasa");
                                    }
                                    //else
                                    //{
                                    //    if (!(tahunPajak == fpNormal.TahunPajak && masaPajak == fpNormal.MasaPajak))
                                    //    {
                                    //        msgs.Add("Faktur Pajak Pengganti harus di Masa-Tahun Pajak yang sama dengan Faktur Pajak Normal");
                                    //    }
                                    //}

                                    //if (!(tahunPajak == fpNormal.TahunPajak && masaPajak == fpNormal.MasaPajak))
                                    //{
                                    //    msgs.Add("Faktur Pajak Pengganti harus di Masa-Tahun Pajak yang sama dengan Faktur Pajak Normal");
                                    //}

                                    //CR DZ - Validasi Scan Pengganti (11) Jika FP Digantinya masih berstatus FP-Nornal


                                    DateTime monthName = new DateTime(2020, fpNormal.MasaPajak.Value, 1);
                                    if (fpNormal.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturPajakNormal))
                                    {
                                        msgs.Add("Faktur Pajak " + fpNormal.FormatedNoFaktur + " sudah dilaporkan di masa pajak " + monthName.ToString("MMMM") + " " + fpNormal.TahunPajak.ToString());
                                        // msgs.Add("No FP Diganti [" + fpNormal.FormatedNoFaktur + "] belum discan");

                                    }

                                }
                            }
                        }
                    }
                }

                /*
                 * Penambahan validasi berdasarkan CR Perubahan dari sisi e-Faktur V2.0
                 * Ada validasi tambahan ketika semua Faktur Pajak (apapun kode FP nya) yang di scan adalah “Faktur Diganti” maka akan muncul notifikasi “Faktur Pajak sudah diganti”
                 */
                /*
                 * CR Penambahan validasi untuk faktur pajak yang sudah dibatalkan
                 * BPM No. ASMO3-201847620 
                 */
                if (info.StatusFaktur.ToLower().Trim() == "faktur dibatalkan")
                {
                    msgs.Add("Faktur Pajak Dibatalkan");
                }

                /* BPM No. ASMO3-201847620
                 * (d) Jika FP dengan kode 011 statusnya adalah FP diganti maka akan ditolak oleh EVIS - jika ada 2
                 * FP pengganti
                 */
                if (info.StatusFaktur.ToLower().Trim() == "faktur diganti" && info.FgPengganti != "0")
                {
                    msgs.Add("Faktur Pajak Normal - Pengganti sudah diganti");
                }


                //CR DZ -- Validasi Scan Diganti (10), jika FP Normal sudah dilaporkan, lalu dari DJP mengeluarkan FP Digantinya.
                if (info.FgPengganti == "0" && info.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti))
                {
                    var fpNormal = FakturPajaks.GetFakturPajakNormal(info.NoFakturPajak);
                    if (fpNormal != null && fpNormal.FakturPajakId != 0)
                    {
                        if (fpNormal.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturPajakNormal) && fpNormal.MasaPajak.ToString() != info.MasaPajak.ToString())
                        {
                            var getOpenClosePeriod = OpenClosePeriods.GetByMasaPajak(fpNormal.MasaPajak.Value, fpNormal.TahunPajak.Value);
                            if (getOpenClosePeriod != null)
                            {
                                if (!getOpenClosePeriod.StatusRegular || !getOpenClosePeriod.StatusSp2)
                                {
                                    //                                    msgs.Add("Faktur Pajak-Nomal sudah dilaporkan");
                                    //  DateTime monthName = new DateTime(2020, fpNormal.MasaPajak.Value, 1);
                                    //  msgs.Add("Faktur Pajak " + fpNormal.FormatedNoFaktur + " sudah dilaporkan di masa pajak " + monthName.ToString("MMMM") + " " + fpNormal.TahunPajak.ToString());

                                    msgs.Add("Sudah discan pada " +
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



            if (info.FakturPajakId > 0)
            {
                FakturPajak fp = FakturPajaks.GetById(info.FakturPajakId);

                if (fp.IsDeleted)
                {
                    msgs.Add(String.Format("Data Sudah dihapus oleh '{0}'", fp.ModifiedBy));

                    model.InfoType = RequestResultInfoType.Warning;
                    model.Message = string.Join("<br />", msgs);
                    return model;
                }
            }


            if (msgs.Count <= 0) return model;

            model.InfoType = RequestResultInfoType.Warning;
            model.Message = string.Join("<br />", msgs);
            return model;
        }

        //val scan bulk disini - CR DZ
        private List<RequestFakturPajakResultModel> ProcessSubmitBulkFakturPajak(List<FakturPajak> fakturPajakToProcess, int iTimeOutSetting,
            string userNameLogin, ApplicationEnums.FPType eFpType)
        {
            var msgs = new List<RequestFakturPajakResultModel>();

            var npwpAdm = GeneralConfigs.GetByKeyId(ApplicationEnums.GeneralConfig.NpwpAdm);
            var namaAdm = GeneralConfigs.GetByKeyId(ApplicationEnums.GeneralConfig.NamaNpwpAdm);
            var maxIndexProceed = -1;
            var networkErrorMessage = string.Empty;

            var fpErrorFpNormalNotFound = new List<DataSubmitScanBulk>();
            bool isTimeOut = false;
            string logkeytimeout = "";
            string urlTimeout = "";
            var isUseProxy = false;
            var inetProxy = WebConfiguration.InternetProxy;
            var inetProxyPort = WebConfiguration.InternetProxyPort;
            var inetProxyUseCredential = WebConfiguration.UseDefaultCredential;
            if (!string.IsNullOrEmpty(inetProxy) || inetProxyPort.HasValue || inetProxyUseCredential.HasValue)
            {
                isUseProxy = true;
            }
            for (int i = 0; i < fakturPajakToProcess.Count; i++)
            {
                try
                {
                    WebExceptionStatus eStatus;
                    string msgError;
                    string logkey2;
                    var objXml = DJPLib.ValidateFakturLib.GetValidateFakturObjectV3(fakturPajakToProcess[i].UrlScan,
                        iTimeOutSetting, isUseProxy, inetProxy, inetProxyPort, inetProxyUseCredential, out msgError, out eStatus, out logkey2);

                    if (eStatus != WebExceptionStatus.Success)
                    {
                        msgs.Add(new RequestFakturPajakResultModel()
                        {
                            FillingIndex = fakturPajakToProcess[i].FillingIndex,
                            Message = msgError,
                            ExceptionDetails = null
                        });
                        networkErrorMessage = msgError;
                        maxIndexProceed = i;

                        //save LogRequestFakturPajak
                        var datLogToSaves = new LogRequestFakturPajak()
                        {
                            LogRequestFakturPajakId = 0,
                            FakturPajakId = fakturPajakToProcess[i].FakturPajakId,
                            CreatedBy = userNameLogin,
                            ErrorMessage = networkErrorMessage,
                            Status = 0,
                            RequestUrl = fakturPajakToProcess[i].UrlScan
                        };

                        LogRequestFakturPajaks.Save(datLogToSaves);

                        //update status error request
                        FakturPajaks.UpdateStatusById(fakturPajakToProcess[i].FakturPajakId,
                            ApplicationEnums.StatusFakturPajak.ErrorRequest, userNameLogin);

                        if (eStatus == WebExceptionStatus.Timeout)
                        {
                            //Send Email
                            isTimeOut = true;
                            urlTimeout = fakturPajakToProcess[i].UrlScan;
                            logkeytimeout = logkey2;
                        }

                        break;
                    }

                    //validasi
                    var validateError = new List<string>();

                    if (string.IsNullOrEmpty(objXml.NamaLawanTransaksi))
                    {
                        validateError.Add("NPWP Lawan Transaksi kosong");
                    }
                    else
                    {
                        if (objXml.NpwpLawanTransaksi != npwpAdm.ConfigValue.Replace(".", "").Replace("-", ""))
                        {
                            validateError.Add("NPWP Lawan Transaksi bukan " + npwpAdm.ConfigValue);
                        }
                    }

                    if (string.IsNullOrEmpty(objXml.NamaLawanTransaksi))
                    {
                        validateError.Add("Nama Lawan Transaksi kosong");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(objXml.FgPengganti))
                        {
                            if (objXml.FgPengganti != "0")
                            {
                                var dList = namaAdm.ConfigValue.Split(';').Where(d => !string.IsNullOrEmpty(d) && !string.IsNullOrEmpty(d.Trim())).ToList();
                                var availableNamaPembeli = (string.Join(",", dList));

                                var chk = dList.Where(dItem => objXml.NamaLawanTransaksi.Replace(" ", "").ToLower().Contains(dItem.Replace(" ", "").ToLower())).ToList();

                                if (chk.Count <= 0)
                                {
                                    validateError.Add("Nama Pembeli bukan yang diperbolehkan [" + availableNamaPembeli + "]");
                                }
                            }
                        }
                    }

                    var getOpenClosePeriod = OpenClosePeriods.GetByMasaPajak(fakturPajakToProcess[i].MasaPajak.Value, fakturPajakToProcess[i].TahunPajak.Value);

                    if (getOpenClosePeriod != null)
                    {
                        if (!getOpenClosePeriod.StatusRegular)
                        {
                            validateError.Add("Status Masa Pajak Close Reguler");
                        }
                        else
                        {
                            if (!getOpenClosePeriod.StatusSp2)
                            {
                                validateError.Add("Status Masa Pajak Close SP2");
                            }
                        }
                    }
                    else
                    {
                        validateError.Add("Masa dan Tahun Pajak tidak tersedia di Data Open Close Periode");
                    }

                    if (string.IsNullOrEmpty(objXml.TanggalFaktur))
                    {
                        validateError.Add("Tanggal Faktur Pajak Mandatory");
                    }
                    else
                    {
                        //getconfig
                        var configData = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.PelaporanTglFaktur);
                        if (configData == null)
                        {
                            validateError.Add("GeneralConfig [PelaporanTglFaktur] not found.");
                        }
                        else
                        {
                            var dats = configData.ConfigValue.Split(':').ToList();
                            if (dats.Count != 2)
                            {
                                validateError.Add("GeneralConfig [PelaporanTglFaktur] not valid.");
                            }
                            else
                            {
                                //BPM No. ASMO3-201847620
                                if (fakturPajakToProcess[i].MasaPajak.Value != 0 && fakturPajakToProcess[i].TahunPajak.Value != 0 && fakturPajakToProcess[i].FgPengganti == "0")
                                {
                                    if (!string.IsNullOrEmpty(fakturPajakToProcess[i].FgPengganti) &&
                                        fakturPajakToProcess[i].FgPengganti == "0")
                                    {
                                        //BPM No. ASMO3-201847620
                                        int min = int.Parse(dats[0].Replace("[", "").Replace("]", ""));
                                        int max = int.Parse(dats[1].Replace("[", "").Replace("]", ""));
                                        var dtMin = new DateTime(fakturPajakToProcess[i].TahunPajak.Value, fakturPajakToProcess[i].MasaPajak.Value, 1).AddMonths(min);
                                        var dtMax = new DateTime(fakturPajakToProcess[i].TahunPajak.Value, fakturPajakToProcess[i].MasaPajak.Value, 1).AddMonths(max + 1).AddDays(-1);
                                        var dTglFaktur = Convert.ToDateTime(objXml.TanggalFaktur);
                                        var dtMaxValidity = new DateTime(dtMax.Year, dtMax.Month, DateTime.DaysInMonth(dtMax.Year, dtMax.Month));
                                        if (dTglFaktur < dtMin)
                                        {
                                            validateError.Add("Tanggal Faktur Pajak sudah kadaluarsa");
                                        }
                                        else
                                        {
                                            if (dTglFaktur > dtMaxValidity)
                                            {
                                                validateError.Add("Tanggal Faktur Pajak tidak sesuai ketentuan");
                                            }
                                        }
                                    }

                                }

                            }
                        }
                    }

                    if (string.IsNullOrEmpty(objXml.NpwpPenjual))
                    {
                        validateError.Add("NPWP Penjual Mandatory");
                    }
                    else
                    {
                        var v = Vendors.GetByNpwp(objXml.NpwpPenjual);
                        if (v != null && v.PkpDicabut)
                        {
                            validateError.Add("PKP Dicabut atas NPWP Penjual");
                        }
                    }

                    if (string.IsNullOrEmpty(objXml.KdJenisTransaksi))
                    {
                        validateError.Add("KdJenisTransaksi Mandatory");
                    }

                    if (string.IsNullOrEmpty(objXml.FgPengganti))
                    {
                        validateError.Add("FgPengganti Mandatory");
                    }

                    if (validateError.Count <= 0)
                    {
                        /*
                         * Disable berdasarkan CR Perubahan dari sisi e-Faktur V2.0
                         * Terkait scan FP Pengganti (Kode yang digit ke -3 ‘1’ contoh: 011,021,031,041) maka tidak dibutuhkan scan FP awal (Kode yang digit ke -3 ‘0’ contoh: 010,020,030,040)"
                         */
                        /*
                         * Rollback berdasarkan CR terbaru, tetap ada validasi harus scan faktur pajak normal (Kode yang digit ke-3 nya '0'
                         * BPM No. ASMO3-201847620 
                         */
                        if (objXml.FgPengganti != "0")
                        {
                            /* BPM No. ASMO3-201847620
                             * (c) Saat user scan FP normal-pengganti, maka akan muncul request untuk scan FP diganti terlebih
                             * dahulu agar dapat mengetahui kapan FP diganti tersebut kadaluarsa (jika FP diganti
                             * expired/kadaluarsa maka ditolak oleh EVIS). - jika 010 belum di scan di EVIS 
                             */
                            var msgValidatePengganti = FakturPajaks.ValidateScanPengganti(objXml.NomorFaktur, objXml.FgPengganti, eFpType);
                            if (!string.IsNullOrEmpty(msgValidatePengganti))
                            {
                                //msgs.Add(msgValidatePengganti);
                                fpErrorFpNormalNotFound.Add(new DataSubmitScanBulk()
                                {
                                    FakturPajakToProcess = fakturPajakToProcess[i],
                                    XmlFakturPajakDjp = objXml
                                });
                                continue; //next loop, pending process
                            }
                            /* BPM No. ASMO3-201847620
                             * (a) Jika FP diganti (kode awal 010) ada dan belum expired, maka FP normal-pengganti (kode awal 011) dapat diterima/discan. 
                             * langsung masuk (dan masih open), FP 010 & 011 harus discan dimasa yang sama. 
                             */
                            var fpNormal = FakturPajaks.GetFakturPajakNormal(objXml.NomorFaktur);
                            if (fpNormal != null && fpNormal.FakturPajakId != 0)
                            {
                                var configData = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.PelaporanTglFaktur);
                                if (configData == null)
                                {
                                    validateError.Add("GeneralConfig [PelaporanTglFaktur] not found.");
                                }
                                else
                                {
                                    //check apakah Faktur Normal sudah Expired
                                    var dats = configData.ConfigValue.Split(':').ToList();
                                    if (dats.Count != 2)
                                    {
                                        validateError.Add("GeneralConfig [PelaporanTglFaktur] not valid.");
                                    }
                                    else
                                    {
                                        //if (fakturPajakToProcess[i].TahunPajak.Value != 0 &&
                                        //    fakturPajakToProcess[i].MasaPajak.Value != 0)
                                        //{
                                        //    if (!(fakturPajakToProcess[i].TahunPajak.Value == fpNormal.TahunPajak && fakturPajakToProcess[i].MasaPajak.Value == fpNormal.MasaPajak))
                                        //    {
                                        //        validateError.Add("Faktur Pajak Pengganti harus di Masa-Tahun Pajak yang sama dengan Faktur Pajak Normal");
                                        //    }
                                        //}

                                        //CR DZ
                                        int min = int.Parse(dats[0].Replace("[", "").Replace("]", ""));
                                        var dtMin = new DateTime(fakturPajakToProcess[i].TahunPajak.Value, fakturPajakToProcess[i].MasaPajak.Value, 1).AddMonths(min);
                                        if (fpNormal.TglFaktur < dtMin)
                                        {
                                            validateError.Add("Faktur Pajak Diganti sudah kadarluasa");
                                        }
                                        //CR DZ - Validasi Scan Pengganti (11) Jika FP Digantinya masih berstatus FP-Nornal
                                        DateTime monthName = new DateTime(2020, fpNormal.MasaPajak.Value, 1);
                                        if (fpNormal.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturPajakNormal))
                                        {
                                            validateError.Add("Faktur Pajak " + fpNormal.FormatedNoFaktur + " sudah dilaporkan di masa pajak " + monthName.ToString("MMMM") + " " + fpNormal.TahunPajak.ToString());
                                            // msgs.Add("No FP Diganti [" + fpNormal.FormatedNoFaktur + "] belum discan");

                                        }

                                    }
                                }
                            }
                        }

                        /*
                         * CR Penambahan validasi untuk faktur pajak yang sudah dibatalkan
                         * BPM No. ASMO3-201847620 
                         */
                        if (objXml.StatusFaktur.ToLower().Trim() == "faktur dibatalkan")
                        {
                            validateError.Add("Faktur Pajak Dibatalkan");
                        }

                        /* BPM No. ASMO3-201847620
                         * (d) Jika FP dengan kode 011 statusnya adalah FP diganti maka akan ditolak oleh EVIS - jika ada 2
                         * FP pengganti
                         */
                        if (objXml.StatusFaktur.ToLower().Trim() == "faktur diganti" && objXml.FgPengganti != "0")
                        {
                            validateError.Add("Faktur Pajak Normal - Pengganti sudah diganti");
                        }

                        //CR DZ -- Validasi Scan Diganti (10), jika FP Normal sudah dilaporkan, lalu dari DJP mengeluarkan FP Digantinya.
                        if (objXml.FgPengganti == "0" && objXml.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti))
                        {
                            var fpNormal = FakturPajaks.GetFakturPajakNormal(objXml.NomorFaktur);
                            if (fpNormal != null && fpNormal.FakturPajakId != 0)
                            {
                                if (fpNormal.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturPajakNormal) && fpNormal.MasaPajak.ToString() != fakturPajakToProcess[i].MasaPajak.Value.ToString())
                                {
                                    var getOpenClosePeriodNormal = OpenClosePeriods.GetByMasaPajak(fpNormal.MasaPajak.Value, fpNormal.TahunPajak.Value);
                                    if (getOpenClosePeriodNormal != null)
                                    {
                                        if (!getOpenClosePeriodNormal.StatusRegular || !getOpenClosePeriodNormal.StatusSp2)
                                        {
                                            //                                    msgs.Add("Faktur Pajak-Nomal sudah dilaporkan");
                                            //  DateTime monthName = new DateTime(2020, fpNormal.MasaPajak.Value, 1);
                                            //  msgs.Add("Faktur Pajak " + fpNormal.FormatedNoFaktur + " sudah dilaporkan di masa pajak " + monthName.ToString("MMMM") + " " + fpNormal.TahunPajak.ToString());

                                            validateError.Add("Sudah discan pada " +
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

                    //save LogRequestFakturPajak
                    var datLogToSave = new LogRequestFakturPajak()
                    {
                        LogRequestFakturPajakId = 0,
                        FakturPajakId = fakturPajakToProcess[i].FakturPajakId,
                        CreatedBy = userNameLogin,
                        ErrorMessage = validateError.Count <= 0 ? "" : string.Join(";", validateError),
                        Status = validateError.Count <= 0 ? 1 : 0,
                        RequestUrl = fakturPajakToProcess[i].UrlScan
                    };

                    LogRequestFakturPajaks.Save(datLogToSave);

                    if (validateError.Count > 0)
                    {
                        msgs.Add(new RequestFakturPajakResultModel()
                        {
                            FillingIndex = fakturPajakToProcess[i].FillingIndex,
                            Message = string.Join(";", validateError),
                            ExceptionDetails = null
                        });

                        //update status error validasi
                        FakturPajaks.UpdateStatusById(fakturPajakToProcess[i].FakturPajakId,
                            ApplicationEnums.StatusFakturPajak.ErrorValidation, userNameLogin);

                        continue;
                    }//next, tidak perlu simpan ke database

                    Exception ex;
                    var rest = SaveSubmitBulkToDatabase(objXml, fakturPajakToProcess[i], out ex);
                    if (!string.IsNullOrEmpty(rest))
                    {
                        msgs.Add(new RequestFakturPajakResultModel()
                        {
                            FillingIndex = fakturPajakToProcess[i].FillingIndex,
                            Message = rest,
                            ExceptionDetails = ex
                        });
                    }

                }
                catch (Exception exception)
                {
                    msgs.Add(new RequestFakturPajakResultModel()
                    {
                        Message = "Unknown Error",
                        ExceptionDetails = exception
                    });
                    maxIndexProceed = i;
                    break;//Berhenti loop ketika sekali gagal
                }
            }

            if (fpErrorFpNormalNotFound.Count > 0)
            {
                //Pending Process
                for (int i = 0; i < fpErrorFpNormalNotFound.Count; i++)
                {
                    /* BPM No. ASMO3-201847620
                     * (c) Saat user scan FP normal-pengganti, maka akan muncul request untuk scan FP diganti terlebih
                     * dahulu agar dapat mengetahui kapan FP diganti tersebut kadaluarsa (jika FP diganti
                     * expired/kadaluarsa maka ditolak oleh EVIS). - jika 010 belum di scan di EVIS 
                     */
                    //check by formated no faktur
                    var msgValidatePengganti =
                        FakturPajaks.ValidateScanPengganti(fpErrorFpNormalNotFound[i].XmlFakturPajakDjp.NomorFaktur,
                            fpErrorFpNormalNotFound[i].XmlFakturPajakDjp.FgPengganti,
                            (ApplicationEnums.FPType)fpErrorFpNormalNotFound[i].FakturPajakToProcess.FPType);

                    var isError = false;
                    var msgError = new List<string>();

                    if (!string.IsNullOrEmpty(msgValidatePengganti))
                    {
                        msgError.Add(msgValidatePengganti);
                        isError = true;
                    }
                    else
                    {
                        /* BPM No. ASMO3-201847620
                             * (a) Jika FP diganti (kode awal 010) ada dan belum expired, maka FP normal-pengganti (kode awal 011) dapat diterima/discan. 
                             * langsung masuk (dan masih open), FP 010 & 011 harus discan dimasa yang sama. 
                             */
                        var fpNormal = FakturPajaks.GetFakturPajakNormal(fpErrorFpNormalNotFound[i].XmlFakturPajakDjp.NomorFaktur);
                        if (fpNormal != null && fpNormal.FakturPajakId != 0)
                        {
                            var configData = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.PelaporanTglFaktur);
                            if (configData == null)
                            {
                                msgError.Add("GeneralConfig [PelaporanTglFaktur] not found.");
                            }
                            else
                            {
                                //check apakah Faktur Normal sudah Expired
                                var dats = configData.ConfigValue.Split(':').ToList();
                                if (dats.Count != 2)
                                {
                                    msgError.Add("GeneralConfig [PelaporanTglFaktur] not valid.");
                                }
                                else
                                {
                                    //if (fakturPajakToProcess[i].TahunPajak.Value != 0 &&
                                    //    fakturPajakToProcess[i].MasaPajak.Value != 0)
                                    //{
                                    //    if (!(fakturPajakToProcess[i].TahunPajak.Value == fpNormal.TahunPajak && fakturPajakToProcess[i].MasaPajak.Value == fpNormal.MasaPajak))
                                    //    {
                                    //        msgError.Add("Faktur Pajak Pengganti harus di Masa-Tahun Pajak yang sama dengan Faktur Pajak Normal");
                                    //    }
                                    //}

                                    //CR DZ
                                    int min = int.Parse(dats[0].Replace("[", "").Replace("]", ""));
                                    var dtMin = new DateTime(fakturPajakToProcess[i].TahunPajak.Value, fakturPajakToProcess[i].MasaPajak.Value, 1).AddMonths(min);
                                    if (fpNormal.TglFaktur < dtMin)
                                    {
                                        msgError.Add("Faktur Pajak Diganti sudah kadarluasa");
                                    }
                                    //CR DZ - Validasi Scan Pengganti (11) Jika FP Digantinya masih berstatus FP-Nornal
                                    DateTime monthName = new DateTime(2020, fpNormal.MasaPajak.Value, 1);
                                    if (fpNormal.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturPajakNormal))
                                    {
                                        msgError.Add("Faktur Pajak " + fpNormal.FormatedNoFaktur + " sudah dilaporkan di masa pajak " + monthName.ToString("MMMM") + " " + fpNormal.TahunPajak.ToString());
                                        // msgs.Add("No FP Diganti [" + fpNormal.FormatedNoFaktur + "] belum discan");

                                    }

                                }
                            }
                        }
                    }
                    /*
                     * CR Penambahan validasi untuk faktur pajak yang sudah dibatalkan
                     * BPM No. ASMO3-201847620 
                     */
                    if (fpErrorFpNormalNotFound[i].XmlFakturPajakDjp.StatusFaktur.ToLower().Trim() == "faktur dibatalkan")
                    {
                        msgError.Add("Faktur Pajak Dibatalkan");
                        isError = true;
                    }

                    /* BPM No. ASMO3-201847620
                     * (d) Jika FP dengan kode 011 statusnya adalah FP diganti maka akan ditolak oleh EVIS - jika ada 2
                     * FP pengganti
                     */
                    if (fpErrorFpNormalNotFound[i].XmlFakturPajakDjp.StatusFaktur.ToLower().Trim() == "faktur diganti" && fpErrorFpNormalNotFound[i].XmlFakturPajakDjp.FgPengganti != "0")
                    {
                        msgError.Add("Faktur Pajak Normal - Pengganti sudah diganti");
                    }

                    //CR DZ -- Validasi Scan Diganti (10), jika FP Normal sudah dilaporkan, lalu dari DJP mengeluarkan FP Digantinya.
                    if (fpErrorFpNormalNotFound[i].XmlFakturPajakDjp.FgPengganti == "0" && fpErrorFpNormalNotFound[i].XmlFakturPajakDjp.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti))
                    {
                        var fpNormal = FakturPajaks.GetFakturPajakNormal(fpErrorFpNormalNotFound[i].XmlFakturPajakDjp.NomorFaktur);
                        if (fpNormal != null && fpNormal.FakturPajakId != 0)
                        {
                            if (fpNormal.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturPajakNormal) && fpNormal.MasaPajak.ToString() != fakturPajakToProcess[i].MasaPajak.Value.ToString())
                            {
                                var getOpenClosePeriodNormal = OpenClosePeriods.GetByMasaPajak(fpNormal.MasaPajak.Value, fpNormal.TahunPajak.Value);
                                if (getOpenClosePeriodNormal != null)
                                {
                                    if (!getOpenClosePeriodNormal.StatusRegular || !getOpenClosePeriodNormal.StatusSp2)
                                    {
                                        //                                    msgs.Add("Faktur Pajak-Nomal sudah dilaporkan");
                                        //  DateTime monthName = new DateTime(2020, fpNormal.MasaPajak.Value, 1);
                                        //  msgs.Add("Faktur Pajak " + fpNormal.FormatedNoFaktur + " sudah dilaporkan di masa pajak " + monthName.ToString("MMMM") + " " + fpNormal.TahunPajak.ToString());

                                        msgError.Add("Sudah discan pada " +
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

                    if (!isError)
                    {
                        //double checking formated no faktur
                        var chkNoFaktur = FakturPajaks.GetSpecificFakturPajak((ApplicationEnums.FPType)fpErrorFpNormalNotFound[i].FakturPajakToProcess.FPType, fpErrorFpNormalNotFound[i].XmlFakturPajakDjp.NomorFaktur,
                            fpErrorFpNormalNotFound[i].XmlFakturPajakDjp.KdJenisTransaksi, fpErrorFpNormalNotFound[i].XmlFakturPajakDjp.FgPengganti);

                        if (chkNoFaktur.Count > 0)
                        {
                            var chkIsSameUrl = chkNoFaktur.FirstOrDefault(c => c.UrlScan.Trim() == fpErrorFpNormalNotFound[i].FakturPajakToProcess.UrlScan.Trim());
                            if (chkIsSameUrl != null)
                            {
                                var d = "Sudah discan pada " +
                                     ConvertHelper.DateTimeConverter.ToLongDateString(chkIsSameUrl.Created) + " oleh " +
                                     chkIsSameUrl.CreatedBy +
                                     ". No FP " + chkIsSameUrl.FormatedNoFaktur + " sudah ada di Masa Pajak " + chkIsSameUrl.MasaPajakName +
                                     " " + chkIsSameUrl.TahunPajak
                                     + ", Nomor Filling Index " + chkIsSameUrl.FillingIndex;

                                msgError.Add(d);
                                isError = true;
                            }
                        }
                    }
                    //save LogRequestFakturPajak
                    var datLogToSave = new LogRequestFakturPajak()
                    {
                        LogRequestFakturPajakId = 0,
                        FakturPajakId = fakturPajakToProcess[i].FakturPajakId,
                        CreatedBy = userNameLogin,
                        ErrorMessage = !isError ? "" : string.Join(",", msgError),
                        Status = !isError ? 1 : 0,
                        RequestUrl = fakturPajakToProcess[i].UrlScan
                    };

                    LogRequestFakturPajaks.Save(datLogToSave);
                    if (isError)
                    {
                        msgs.Add(new RequestFakturPajakResultModel()
                        {
                            FillingIndex = fakturPajakToProcess[i].FillingIndex,
                            Message = string.Join(",", msgError),
                            ExceptionDetails = null
                        });

                        //update status error validasi
                        FakturPajaks.UpdateStatusById(fakturPajakToProcess[i].FakturPajakId,
                            ApplicationEnums.StatusFakturPajak.ErrorValidation, userNameLogin);

                        continue;
                    }//next, tidak perlu simpan ke database

                    Exception ex;
                    var rest = SaveSubmitBulkToDatabase(fpErrorFpNormalNotFound[i].XmlFakturPajakDjp, fpErrorFpNormalNotFound[i].FakturPajakToProcess, out ex);
                    if (!string.IsNullOrEmpty(rest))
                    {
                        msgs.Add(new RequestFakturPajakResultModel()
                        {
                            FillingIndex = fakturPajakToProcess[i].FillingIndex,
                            Message = rest,
                            ExceptionDetails = ex
                        });
                    }
                }
            }

            if (maxIndexProceed != -1
                && maxIndexProceed < fakturPajakToProcess.Count
                && !string.IsNullOrEmpty(networkErrorMessage))
            {
                //generate error untuk sisa data yang belum di proses
                for (int i = maxIndexProceed + 1; i < fakturPajakToProcess.Count; i++)
                {
                    msgs.Add(new RequestFakturPajakResultModel()
                    {
                        FillingIndex = fakturPajakToProcess[i].FillingIndex,
                        Message = networkErrorMessage,
                        ExceptionDetails = null
                    });

                    //save LogRequestFakturPajak
                    var datLogToSaves = new LogRequestFakturPajak()
                    {
                        LogRequestFakturPajakId = 0,
                        FakturPajakId = fakturPajakToProcess[i].FakturPajakId,
                        CreatedBy = userNameLogin,
                        ErrorMessage = networkErrorMessage,
                        Status = 0,
                        RequestUrl = fakturPajakToProcess[i].UrlScan
                    };

                    LogRequestFakturPajaks.Save(datLogToSaves);

                    //update status error request
                    FakturPajaks.UpdateStatusById(fakturPajakToProcess[i].FakturPajakId,
                        ApplicationEnums.StatusFakturPajak.ErrorRequest, userNameLogin);

                }
            }

            if (isTimeOut)
            {
                //send email
                var mh = new MailHelper();
                bool isErrorSendMail;
                mh.DjpRequestErrorSendMail(out isErrorSendMail, urlTimeout, logkeytimeout);

                if (isErrorSendMail)
                {
                    msgs.Add(new RequestFakturPajakResultModel()
                    {
                        Message = "Send Email Notification error."
                    });
                }

            }

            return msgs;
        }

        //yepyep
        private string SaveSubmitBulkToDatabase(ResValidateFakturPm objData, FakturPajak fp, out Exception ex)
        {
            ex = null;
            var msgs = string.Empty;
            var userName = Session["UserName"] as string;
            try
            {
                var getData = FakturPajaks.GetByOriginalNoFaktur(objData.NomorFaktur);
                var datToUpdate = new List<FakturPajak>();
                var datToDelete = new List<FakturPajak>();
                //maka collect data
                if (getData.Count > 0)
                {
                    //get data yang akan di-update jika 
                    datToUpdate =
                        getData.Where(
                            c =>
                                c.StatusFaktur !=
                                EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti))
                            .ToList();

                    //CR DZ
                    if (objData.StatusFaktur == EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturDiganti))
                    {
                        datToDelete =
                       getData.Where(
                           x =>
                               x.StatusFaktur ==
                               EnumHelper.GetDescription(ApplicationEnums.StatusFakturPajakFromDjp.FakturPajakNormal))
                           .ToList();
                    }
                    //
                }

                //get to djp for datToUpdate
                var dbDataToUpdate = new List<FakturPajakToUpdated>();
                if (datToUpdate.Count > 0)
                {
                    List<RequestResultModel> resultModel;
                    var djpData = CollectUpdatedDataFromDjp(datToUpdate, out resultModel);
                    var chkIsAnyError = resultModel.Where(c => c.InfoType != RequestResultInfoType.Success).ToList();
                    if (chkIsAnyError.Count > 0)
                    {
                        //Ada error pada saat getdata ke djp
                        var msgToRet = string.Join("<br />", chkIsAnyError.Select(d => d.Message));
                        ex = new Exception(msgToRet);
                        msgs = msgToRet;
                    }
                    dbDataToUpdate = djpData;
                }

                using (var eScope = new TransactionScope())
                {

                    //CR DZ
                    if (datToDelete.Count > 0)
                    {
                        foreach (var item in datToDelete)
                        {
                            FakturPajaks.Delete(item.FakturPajakId, userName);
                        }
                    }

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
                    fp.Referensi = objData.Referensi;

                    if (dbDataToUpdate.Count > 0)
                    {
                        foreach (var item in dbDataToUpdate)
                        {
                            FakturPajaks.UpdateStatusFakturNotDeleted(item.FakturPajakId, item.StatusFaktur, userName, item.IsDeleted);
                        }
                    }

                    var saveresult = FakturPajaks.Save(fp);

                    if (string.IsNullOrEmpty(saveresult.UIMessage))
                    {
                        eScope.Complete();
                    }
                    else
                    {

                        ex = new Exception(saveresult.UIMessage);
                        msgs = saveresult.UIMessage;
                    }

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

        public JsonResult GetCountByReceivingDate(string receivingDate)
        {
            if (string.IsNullOrEmpty(receivingDate))
            {
                return Json(new { aaDat = 0 }, JsonRequestBehavior.AllowGet);
            }

            var getDatas = VwDataIWSReqEfises.GetCountDataByReceivingDate(Convert.ToDateTime(receivingDate));
            var fakturPajakIwsScanned = FakturPajaks.GetCountByFpTypeAndReceivingDate(ApplicationEnums.FPType.ScanIws,
                Convert.ToDateTime(receivingDate));

            return Json(new { aaDat = (getDatas - fakturPajakIwsScanned) }, JsonRequestBehavior.AllowGet);

            //testing only
            //return Json(new { aaDat = 0 }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult RemoveFakturPajak(long fakturPajakId)
        {

            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = "Faktur Pajak has been deleted."
            };
            string noFakturPajak;
            string outMessage;
            var isAbleToDele = CheckIsAbleToDelete(fakturPajakId, out noFakturPajak, out outMessage);
            if (!isAbleToDele)
            {
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = "No Faktur Pajak " + noFakturPajak + " tidak bisa dihapus. Message Info : <br />" +
                                outMessage;
                return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
            }

            //delete data
            var userName = Session["UserName"] as string;

            try
            {
                FakturPajaks.Delete(fakturPajakId, userName);
            }
            catch (Exception ex)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, ex.Message, MethodBase.GetCurrentMethod(), ex);
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = "Delete failed. See log with Log Key " + logKey + " for details.";
            }
            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }

        [AuthActivity("12")]
        public JsonResult RemoveFakturPajakByIds(List<long> fakturPajakIds)
        {
            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = "Faktur Pajak has been deleted."
            };

            if (fakturPajakIds.Count <= 0)
            {
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = "No Data Selected";
                return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
            }
            //delete data
            var userName = Session["UserName"] as string;
            var msgs = new List<string>();
            foreach (var item in fakturPajakIds)
            {
                string noFakturPajak;
                string outMessage;
                var isAbleToDele = CheckIsAbleToDelete(item, out noFakturPajak, out outMessage);
                if (!isAbleToDele)
                {
                    msgs.Add("No Faktur Pajak " + noFakturPajak + " tidak bisa dihapus. Message Info : <br />" +
                                outMessage);
                    continue;
                }
                //do delete
                try
                {
                    using (var escope = new TransactionScope())
                    {
                        //FakturPajaks.Delete(item, userName);
                        //CompEvisIwss.DeleteByTaxInvoiceNumberEvis(noFakturPajak, userName);
                        FakturPajaks.DeleteDaftarFakturPajak(item, userName);
                        escope.Complete();

                    }
                }
                catch (Exception ex)
                {
                    msgs.Add(ex.Message);
                }
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

        private bool CheckIsAbleToDelete(long fakturPajakId, out string formatedNoFaktur, out string msgResult)
        {
            msgResult = "";
            var msgResults = new List<string>();
            var chkData = FakturPajaks.GetById(fakturPajakId);
            formatedNoFaktur = chkData.FormatedNoFaktur;
            bool isAbleToRemove = true;

            //check is open period
            var getOpenPeriodInfo = OpenClosePeriods.GetByMasaPajak(chkData.MasaPajak.Value, chkData.TahunPajak.Value);
            if (getOpenPeriodInfo != null)
            {
                if (!getOpenPeriodInfo.StatusRegular)
                {
                    msgResults.Add("Period FP Close Period Regular");
                    isAbleToRemove = false;
                }
                else
                {
                    if (!getOpenPeriodInfo.StatusSp2)
                    {
                        msgResults.Add("Period FP Close SP2");
                        isAbleToRemove = false;
                    }
                }
            }
            else
            {
                msgResults.Add("Data Open Close Period tidak ditemukan");
                isAbleToRemove = false;
            }


            var chkSpmData = ReportSuratPemberitahuanMasaDetails.GetByFormatedNoFaktur(formatedNoFaktur);
            if (chkSpmData != null)
            {
                msgResults.Add("Faktur Pajak " + formatedNoFaktur + " sudah Create SPM");
                isAbleToRemove = false;
            }

            if (!isAbleToRemove)
            {
                msgResult = string.Join("<br/>", msgResults);
            }

            return isAbleToRemove;
        }

        public JsonResult ClearLogsBulkIws(int? masaPajak, int? tahunPajak, string receivingDate)
        {
            var userName = Session["UserName"] as string;
            if (string.IsNullOrEmpty(userName))
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, "Error GetSession for [UserName]", MethodBase.GetCurrentMethod());
                return Json(new
                {
                    Html = new RequestResultModel()
                    {
                        InfoType = RequestResultInfoType.ErrorOrDanger,
                        Message = "See Log : " + logKey
                    }
                }, JsonRequestBehavior.AllowGet);

            }
            var msgs = new List<string>();
            if (string.IsNullOrEmpty(receivingDate))
            {
                msgs.Add("Receiving Date Mandatory");
            }
            if (!masaPajak.HasValue)
            {
                msgs.Add("Masa Pajak Mandatory");
            }

            if (!tahunPajak.HasValue)
            {
                msgs.Add("Tahun Pajak Mandatory");
            }

            if (msgs.Count > 0)
            {
                return Json(new
                {
                    Html = new RequestResultModel()
                    {
                        InfoType = RequestResultInfoType.ErrorOrDanger,
                        Message = string.Join("<br />", msgs)
                    }
                }, JsonRequestBehavior.AllowGet);
            }

            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = "Clear Logs Successfull"
            };

            try
            {
                // ReSharper disable once PossibleInvalidOperationException, sudah pasti tidak NULL, sudah di handle di code atas
                var getData = FakturPajaks.GetScanBulkToSubmit(ApplicationEnums.FPType.ScanIws, masaPajak.Value,
                    // ReSharper disable PossibleInvalidOperationException, sudah pasti tidak NULL, sudah dihandle di code atas
                    tahunPajak.Value, Convert.ToDateTime(receivingDate));
                // ReSharper restore PossibleInvalidOperationException

                var toDelete =
                    getData.Where(c => c.Status == (int)ApplicationEnums.StatusFakturPajak.ErrorValidation).ToList();
                if (toDelete.Count <= 0)
                {
                    return Json(new
                    {
                        Html = new RequestResultModel()
                        {
                            InfoType = RequestResultInfoType.ErrorOrDanger,
                            Message = "Tidak ada Faktur Pajak yang Error Validasi"
                        }
                    }, JsonRequestBehavior.AllowGet);
                }

                //do delete
                var idsToDelete = string.Join(",",
                    toDelete.Select(d => d.FakturPajakId));

                if (!string.IsNullOrEmpty(idsToDelete))
                {
                    using (var eScope = new TransactionScope())
                    {
                        FakturPajaks.DeleteByIds(idsToDelete, userName);
                        eScope.Complete();
                    }
                }

            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = "Clear Logs Failed.";
            }

            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult ClearLogsBulkNonIws(int? masaPajak, int? tahunPajak)
        {
            var userName = Session["UserName"] as string;
            if (string.IsNullOrEmpty(userName))
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, "Error GetSession for [UserName]", MethodBase.GetCurrentMethod());
                return Json(new
                {
                    Html = new RequestResultModel()
                    {
                        InfoType = RequestResultInfoType.ErrorOrDanger,
                        Message = "See Log : " + logKey
                    }
                }, JsonRequestBehavior.AllowGet);

            }
            var msgs = new List<string>();
            if (!masaPajak.HasValue)
            {
                msgs.Add("Masa Pajak Mandatory");
            }

            if (!tahunPajak.HasValue)
            {
                msgs.Add("Tahun Pajak Mandatory");
            }

            if (msgs.Count > 0)
            {
                return Json(new
                {
                    Html = new RequestResultModel()
                    {
                        InfoType = RequestResultInfoType.ErrorOrDanger,
                        Message = string.Join("<br />", msgs)
                    }
                }, JsonRequestBehavior.AllowGet);
            }

            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = "Clear Logs Successfull"
            };

            try
            {
                // ReSharper disable once PossibleInvalidOperationException, sudah pasti tidak NULL, sudah di handle di code atas
                var getData = FakturPajaks.GetScanBulkToSubmit(ApplicationEnums.FPType.ScanNonIws, masaPajak.Value,
                    // ReSharper disable PossibleInvalidOperationException, sudah pasti tidak NULL, sudah dihandle di code atas
                    tahunPajak.Value, null);
                // ReSharper restore PossibleInvalidOperationException

                var toDelete =
                    getData.Where(c => c.Status == (int)ApplicationEnums.StatusFakturPajak.ErrorValidation).ToList();
                if (toDelete.Count <= 0)
                {
                    return Json(new
                    {
                        Html = new RequestResultModel()
                        {
                            InfoType = RequestResultInfoType.ErrorOrDanger,
                            Message = "Tidak ada Faktur Pajak yang Error Validasi"
                        }
                    }, JsonRequestBehavior.AllowGet);
                }

                //do delete
                var idsToDelete = string.Join(",",
                    toDelete.Select(d => d.FakturPajakId));

                if (!string.IsNullOrEmpty(idsToDelete))
                {
                    using (var eScope = new TransactionScope())
                    {
                        FakturPajaks.DeleteByIds(idsToDelete, userName);
                        eScope.Complete();
                    }
                }

            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = "Clear Logs Failed.";
            }

            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetFakturPajakKhususByNoFaktur(string noFaktur)
        {
            var dats = FakturPajaks.GetByFormatedNoFakturFpKhusus(noFaktur);
            return Json(new { aaData = dats }, JsonRequestBehavior.AllowGet);
        }

        private class DataSubmitScanBulk
        {
            public FakturPajak FakturPajakToProcess { get; set; }
            public ResValidateFakturPm XmlFakturPajakDjp { get; set; }
        }

        private class FakturPajakToUpdated
        {
            public long FakturPajakId { get; set; }
            public string StatusFaktur { get; set; }

            //CR DZ
            public bool IsDeleted { get; set; }

        }

    }
}
