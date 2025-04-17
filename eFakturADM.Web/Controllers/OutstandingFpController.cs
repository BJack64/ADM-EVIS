using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Objects;
using eFakturADM.Shared.Utility;
using eFakturADM.Web.Helpers;
using eFakturADM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;


namespace eFakturADM.Web.Controllers
{
    public class OutstandingFpController : BaseController
    {

        //
        // GET: /OutstandingFp/
        //#if TOTAL_DEV
        public class IdInfo
        {
            public int FakturPajakIds { get; set; }
        }
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetListFakturPajakOutstandingDataTable(string firstLoad, string sEcho, int iDisplayStart, int iDisplayLength, string NoFaktur1, string NoFaktur2, string NPWP,
            string Nama, string TglFakturStart, string TglFakturEnd, string Status, string scanDateAwal, string scanDateAkhir, string receivedStart, string ReceivedEnd, string source,string statusFaktur, string remark, string isByPass, string sSearch_1,
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
                    aaData = new List<FakturPajakOutstanding>()
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
            string _SSource = sSearch_9;
            string _SStatusPayment = sSearch_10;
            string _Remark = sSearch_11;
            string _TglFakturString010 = sSearch_12;

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

            DateTime? dReceivedStart = null;
            DateTime? dReceivedEnd = null;

            if (!string.IsNullOrEmpty(receivedStart) && receivedStart != "undefined")
            {
                dReceivedStart = Convert.ToDateTime(receivedStart);
            }

            if (!string.IsNullOrEmpty(ReceivedEnd) && ReceivedEnd != "undefined")
            {
                dReceivedEnd = Convert.ToDateTime(ReceivedEnd);
            }

            //int? masaPajak = string.IsNullOrEmpty(MasaPajak) || MasaPajak == "0" ? (int?)null : int.Parse(MasaPajak);
            //int? tahunPajak = string.IsNullOrEmpty(TahunPajak) || TahunPajak == "0" ? (int?)null : int.Parse(TahunPajak);
            string logKey;
            Logger.WriteLog(out logKey, LogLevel.Error, "Start Get FP Outstanding", MethodBase.GetCurrentMethod());
            List<FakturPajakOutstanding> listFakturPajak = FakturPajakOutstandings.GetList(filter, out totalItems, NoFaktur1, NoFaktur2, NPWP, Nama, tglFakturStart, tglFakturEnd, Status, dscanDateAwal, dscanDateAkhir, dReceivedStart, dReceivedEnd, source, statusFaktur, remark,isByPass,
                   _FormatedNpwpPenjual, _NamaPenjual, _FormatedNoFaktur, _TglFakturString, _DPPString, _PPNString, _PPNBMString, _StatusFaktur,  _SSource, _SStatusPayment, _Remark, _TglFakturString010).ToList();

            Logger.WriteLog(out logKey, LogLevel.Error, "End Get FP Outstanding: " + totalItems + " records", MethodBase.GetCurrentMethod());
            return Json(new
            {
                sEcho = sEcho,
                iTotalRecords = totalItems,
                iTotalDisplayRecords = totalItems,
                aaData = listFakturPajak
            },
            JsonRequestBehavior.AllowGet);
        }
        public JsonResult SetMasaPajakDialog(List<long> FakturPajakIds, bool isForceMasaPajak)
        {
            var model = new IdPajakModel()
            {
                FakturPajakId = string.Join(",", FakturPajakIds.Select(x => x.ToString())), isForceMasaPajak = isForceMasaPajak
            };
            return Json(new
            {
                Html = this.RenderPartialView(@"SetMasaPajak", model),

            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteFakturPajakOutstanding(List<long> FakturPajakIds, string NoFaktur1, string NoFaktur2, string NPWP,
            string Nama, string TglFakturStart, string TglFakturEnd, string Status, string scanDateAwal, string scanDateAkhir, string receivedStart, string ReceivedEnd, string source, string statusFaktur, string remark, string sSearch_1,
            string sSearch_2, string sSearch_3, string sSearch_4, string sSearch_5, string sSearch_6, string sSearch_7, string sSearch_8, string sSearch_9, string sSearch_10
            , string sSearch_11, string sSearch_12, bool selectAll)
        {
            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = "Faktur Pajak has been deleted."
            };

            if (FakturPajakIds.Count <= 0)
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

                DateTime? dReceivedStart = null;
                DateTime? dReceivedEnd = null;

                if (!string.IsNullOrEmpty(scanDateAwal) && scanDateAwal != "undefined")
                {
                    dReceivedStart = Convert.ToDateTime(scanDateAwal);
                }

                if (!string.IsNullOrEmpty(scanDateAkhir) && scanDateAkhir != "undefined")
                {
                    dReceivedEnd = Convert.ToDateTime(scanDateAkhir);
                }

                var userName = Session["UserName"] as string;
                FakturPajakOutstandings.Delete(!selectAll ? string.Join(",", FakturPajakIds.Select(x => x.ToString())) : null, userName ?? "SYSTEM", NoFaktur1, NoFaktur2, NPWP, Nama, tglFakturStart,tglFakturEnd, Status, dscanDateAwal, dscanDateAkhir, dReceivedStart, dReceivedEnd, source,statusFaktur, remark,
                   _FormatedNpwpPenjual, _NamaPenjual, _FormatedNoFaktur, _TglFakturString, _DPPString, _PPNString, _PPNBMString, _StatusFaktur, _SUserName, _SSource, _SStatusPayment, _Remark);
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


        public JsonResult Validasi(string FakturPajakIds, int BulanMasaPajak, int TahunMasaPajak, int KreditPajak, string NoFaktur1, string NoFaktur2, string NPWP,
            string Nama, string TglFakturStart, string TglFakturEnd, string Status, string scanDateAwal, string scanDateAkhir, string receivedStart, string ReceivedEnd, string source, string remark, string sSearch_1,
            string sSearch_2, string sSearch_3, string sSearch_4, string sSearch_5, string sSearch_6, string sSearch_7, string sSearch_8, string sSearch_9, string sSearch_10
            , string sSearch_11, string sSearch_12, bool selectAll,bool isForceMasaPajak)
        {
            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = "Set Masa Pajak has been sucessful."
            };

            var checkFakturPajak = FakturPajaks.GetByMultipleId(FakturPajakIds);
            if (checkFakturPajak == null || checkFakturPajak.Count == 0)
            {
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = "Faktur Pajak Tidak Ditemukan";
                return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
            }

            
            //delete data
            var msgs = new List<string>();
            //do delete
            try
            {
                string _FormatedNpwpPenjual = sSearch_1;
                string _NamaPenjual = sSearch_2;
                string _FormatedNoFaktur = sSearch_3;
                string _TglFakturString = sSearch_4;
                string _DPPString = sSearch_5;
                string _PPNString = sSearch_6;
                string _PPNBMString = sSearch_7;
                string _StatusFaktur = sSearch_8;
                string _SSource = sSearch_9;
                string _SStatusPayment = sSearch_10;
                string _Remark = sSearch_11;
                string _TglFakturString010 = sSearch_12;

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

                DateTime? dReceivedStart = null;
                DateTime? dReceivedEnd = null;

                if (!string.IsNullOrEmpty(receivedStart) && receivedStart != "undefined")
                {
                    dReceivedStart = Convert.ToDateTime(receivedStart);
                }

                if (!string.IsNullOrEmpty(ReceivedEnd) && ReceivedEnd != "undefined")
                {
                    dReceivedEnd = Convert.ToDateTime(ReceivedEnd);
                }

                var _SUserName = Session["UserName"] as string;
                var getValidasi = FakturPajakOutstandings.GetExpiredData(!selectAll ? FakturPajakIds : null, BulanMasaPajak, TahunMasaPajak, NoFaktur1, NoFaktur2, NPWP, Nama, tglFakturStart, tglFakturEnd, Status, dscanDateAwal, dscanDateAkhir, dReceivedStart, dReceivedEnd, source, remark,
                   _FormatedNpwpPenjual, _NamaPenjual, _FormatedNoFaktur, _TglFakturString, _DPPString, _PPNString, _PPNBMString, _StatusFaktur, _SUserName, _SSource, _SStatusPayment, _Remark, isForceMasaPajak,KreditPajak, _TglFakturString010);
                msgs.Add("Jumlah Faktur Pajak Berhasil Set Masa Pajak : "+ getValidasi.NotExpiredFakturPajak);
                msgs.Add("Jumlah Faktur Pajak Terkena Validasi Expired : " + getValidasi.ExpiredFakturPajak);
                msgs.Add("Jumlah Faktur Pajak Terkena Validasi Tanggal Tidak Sesuai : " + getValidasi.PajakTidakSesuai);
                if (!isForceMasaPajak)
                {
                    msgs.Add("Jumlah Faktur Pajak Terkena Validasi Status Faktur : " + getValidasi.PajakValidasiStatus);
                }else { 
                    msgs.Add("Jumlah Faktur Pajak Terkena Validasi Status Faktur Blank dan Belum By Pass : " + getValidasi.PajakValidasiStatusBlankDanBelumBypass);
                }
                //FakturPajakOutstandings.OutstandingValidasi(!selectAll ? FakturPajakIds : null, BulanMasaPajak, TahunMasaPajak, KreditPajak, NoFaktur1, NoFaktur2, NPWP, Nama, tglFakturStart, tglFakturEnd, Status, dscanDateAwal, dscanDateAkhir, source, remark,
                //   _FormatedNpwpPenjual, _NamaPenjual, _FormatedNoFaktur, _TglFakturString, _DPPString, _PPNString, _PPNBMString, _StatusFaktur, _SUserName, _SSource, _SStatusPayment, _Remark,isForceMasaPajak);
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
    }
}
