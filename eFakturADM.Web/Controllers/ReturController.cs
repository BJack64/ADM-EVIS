using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using eFakturADM.Shared.Utility;
using eFakturADM.Web.Helpers;
using eFakturADM.Logic.Objects;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Collections;
using eFakturADM.Web.Models;

namespace eFakturADM.Web.Controllers
{
    public class ReturController : BaseController
    {
        //
        // GET: /FakturPajakRetur/

        [AuthActivity("20")]
        public ActionResult InputRetur()
        {
            string ppnSetting = System.Configuration.ConfigurationManager.AppSettings["ppnSetting"];
            ViewBag.PpnSetting = ppnSetting;

            return View();

        }

        [AuthActivity("17")]
        public ActionResult ListReturFakturPajak()
        {
            return View();
        }

        public JsonResult BrowseFakturPajakDialog()
        {
            return Json(new
            {
                Html = this.RenderPartialView(@"BrowseFakturPajak", null),

            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ViewReturFakturPajak(string FakturReturPajakId)
        {
            FakturPajakRetur _fpr = FakturPajakReturs.GetById(long.Parse(FakturReturPajakId));

            FakturPajakReturInfoModel _fprModel = new FakturPajakReturInfoModel
            {
                NoDocRetur = _fpr.NoDocRetur,
                TglRetur = _fpr.TglReturString,
                MasaPajakLapor = _fpr.MasaPajakLapor.ToString(),
                TahunPajakLapor = _fpr.TahunPajakLapor.ToString(),
                JumlahDPP = _fpr.DPPString,
                JumlahPPN = _fpr.PPNString,
                JumlahPPNBM = _fpr.PPNBMString,
                Pesan = _fpr.Pesan,
                NoFaktur = _fpr.NoFakturPajak,
                NamaVendor = _fpr.NamaPenjual,
                NPWPVendor = _fpr.FormatedNpwpPenjual,
                TanggalFaktur = _fpr.TglFakturString,
                KdJenisTransaksi = _fpr.KdJenisTransaksi,
                Dikreditkan = _fpr.Dikreditkan.HasValue ? _fpr.Dikreditkan.Value ? "true" : "false" : "false",
                FgPengganti = _fpr.FgPengganti
            };


            return Json(new
            {
                Html = this.RenderPartialView(@"ViewReturFakturPajak", _fprModel),

            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetListReturPajakDataTable(string firstLoad, string sEcho, int iDisplayStart, int iDisplayLength, string NoFaktur, string NoRetur,
            string TglFakturReturStart, string TglFakturReturEnd,
            string NPWPVendor, string NamaVendor, string MasaPajak, string TahunPajak,
            string sSearch_0, string sSearch_1,
            string sSearch_2, string sSearch_3, string sSearch_4, string sSearch_5, string sSearch_6, string sSearch_7, string sSearch_8, string sSearch_9,
            string sSearch_10, string sSearch_11
            )
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
                SortColumnName = "TglRetur"
            };

            string fTglRetur = sSearch_0;
            string fNpwpVendor = sSearch_1;
            string fNamaVendor = sSearch_2;
            string fNoFakturDiRetur = sSearch_3;
            string fTglFaktur = sSearch_4;
            string fNomorRetur = sSearch_5;
            string fMasaRetur = sSearch_6;
            string fTahunRetur = sSearch_7;
            string fDpp = sSearch_8;
            string fPpn = sSearch_9;
            string fPpnBm = sSearch_10;
            string fUserName = sSearch_11;

            switch (Convert.ToInt32(Request["iSortCol_0"]))
            {
                case 0: filter.SortColumnName = "TglRetur"; break;
                case 1: filter.SortColumnName = "NPWPPenjual"; break;
                case 2: filter.SortColumnName = "NamaPenjual"; break;
                case 3: filter.SortColumnName = "NoFakturPajak"; break;
                case 4: filter.SortColumnName = "TglFaktur"; break;
                case 5: filter.SortColumnName = "NoDocRetur"; break;
                case 6: filter.SortColumnName = "MasaPajakLapor"; break;
                case 7: filter.SortColumnName = "TahunPajakLapor"; break;
                case 8: filter.SortColumnName = "JumlahDPP"; break;
                case 9: filter.SortColumnName = "JumlahPPN"; break;
                case 10: filter.SortColumnName = "JumlahPPNBM"; break;
                case 11: filter.SortColumnName = "CreatedBy"; break;
            }
            int totalItems;

            DateTime? tglFakturReturStart = !string.IsNullOrEmpty(TglFakturReturStart) ? Convert.ToDateTime(TglFakturReturStart) : (DateTime?)null;
            DateTime? tglFakturReturEnd = !string.IsNullOrEmpty(TglFakturReturEnd) ? Convert.ToDateTime(TglFakturReturEnd) : (DateTime?)null;
            int? iMasaPajak = string.IsNullOrEmpty(MasaPajak) || MasaPajak == "undefined" || MasaPajak == "0" ? (int?)null : int.Parse(MasaPajak);
            int? iTahunPajak = string.IsNullOrEmpty(TahunPajak) || TahunPajak == "undefined" || TahunPajak == "0" ? (int?)null : int.Parse(TahunPajak);

            List<FakturPajakRetur> listFakturPajak = FakturPajakReturs.GetList(filter, out totalItems, NoFaktur, NoRetur, tglFakturReturStart,
                tglFakturReturEnd, NPWPVendor, NamaVendor, iMasaPajak, iTahunPajak, fTglRetur, fNpwpVendor, fNamaVendor, fNoFakturDiRetur,
                fTglFaktur, fNomorRetur, fMasaRetur, fTahunRetur, fDpp, fPpn, fPpnBm, fUserName);

            return Json(new
            {
                sEcho = sEcho,
                iTotalRecords = totalItems,
                iTotalDisplayRecords = totalItems,
                aaData = listFakturPajak
            },
           JsonRequestBehavior.AllowGet);
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
                case 2: filter.SortColumnName = "NPWPPenjual"; break;
                case 3: filter.SortColumnName = "NamaPenjual"; break;
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

        public JsonResult ValidationFakturPajakRetur(FakturPajakReturInfoModel Info)
        {
            var model = new RequestResultModel
            {
                InfoType = RequestResultInfoType.Success,
                Message = string.Empty
            };

            var validationMessage = "";

            if (Info.NoDocRetur == null || Info.NoDocRetur.Trim().Length == 0)
            {
                validationMessage += "Nomor Document Retur is required, " + "<br/>";
            }
            if (Info.TglRetur == null || Info.TglRetur.Trim().Length == 0)
            {
                validationMessage += "Tanggal Retur is required, " + "<br/>";
            }
            if (Info.MasaPajakLapor == null || Info.MasaPajakLapor.Trim().Length == 0)
            {
                validationMessage += "Masa Pajak is required, " + "<br/>";
            }
            if (Info.TahunPajakLapor == null || Info.TahunPajakLapor.Trim().Length == 0)
            {
                validationMessage += "Tahun Pajak is required, " + "<br/>";
            }
            if ((Info.TglRetur != null) && (Info.MasaPajakLapor != null) && (Info.TahunPajakLapor != null ))
            {
                DateTime tglRetur = Convert.ToDateTime(Info.TglRetur);
                if (!(tglRetur.Month == Convert.ToInt32(Info.MasaPajakLapor) && tglRetur.Year == Convert.ToInt32(Info.TahunPajakLapor)))
                {
                    validationMessage += "Tanggal Retur harus sama dengan masa pajak, " + "<br/>";
                }
            }
            if (Info.FakturPajakId < 1)
            {
                //validationMessage += "Faktur Pajak is required, " + "<br/>";
                if (Info.NoFaktur == null || Info.NoFaktur.Trim().Length == 0)
                {
                    validationMessage += "No Faktur is required, " + "<br/>";
                }
                if (Info.NamaVendor == null || Info.NamaVendor.Trim().Length == 0)
                {
                    validationMessage += "Nama Penjual is required, " + "<br/>";
                }
                if (Info.NPWPVendor == null || Info.NPWPVendor.Trim().Length == 0)
                {
                    validationMessage += "NPWP Penjual is required, " + "<br/>";
                }
                if (Info.TanggalFaktur == null || Info.TanggalFaktur.Trim().Length == 0)
                {
                    validationMessage += "Tanggal Faktur is required, " + "<br/>";
                }
                if (Info.KdJenisTransaksi == null || Info.KdJenisTransaksi.Trim().Length == 0)
                {
                    validationMessage += "Kode Jenis Transaksi is required, " + "<br/>";
                }
                if (Info.FgPengganti == null || Info.FgPengganti.Trim().Length == 0)
                {
                    validationMessage += "Fg Pengganti is required, " + "<br/>";
                }
            }
            decimal nominalValue;
            if (!decimal.TryParse(Info.JumlahDPP, out nominalValue))
            {
                validationMessage += "Jumlah DPP must be numeric, " + "<br/>";
            }
            if (!decimal.TryParse(Info.JumlahPPN, out nominalValue))
            {
                validationMessage += "Jumlah PPN must be numeric, " + "<br/>";
            }
            if (!decimal.TryParse(Info.JumlahPPNBM, out nominalValue))
            {
                validationMessage += "Jumlah PPnBM must be numeric, " + "<br/>";
            }

            if (string.IsNullOrEmpty(validationMessage)) return Json(new { Html = model }, JsonRequestBehavior.AllowGet);

            model.InfoType = RequestResultInfoType.Warning;
            model.Message = validationMessage.Trim().Remove(validationMessage.Trim().Length - 7, 7) + ".";

            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveRetur(FakturPajakReturInfoModel Info)
        {
            var model = new RequestResultModel();
            var userName = Session["UserName"] as string;

            int? _fpId = null;
            if (Info.FakturPajakId > 0)
            {
                _fpId = Info.FakturPajakId;
            }

            using (var scope = new TransactionScope())
            {
                var retur = new FakturPajakRetur
                {
                    FakturPajakReturId = Info.FakturPajakReturId,
                    FakturPajakId = _fpId,
                    FCode = ApplicationConstant.FCodeFm,
                    TglRetur = string.IsNullOrEmpty(Info.TglRetur) ? (DateTime?)null : DateTime.Parse(Info.TglRetur),
                    MasaPajakLapor = int.Parse(Info.MasaPajakLapor),
                    TahunPajakLapor = int.Parse(Info.TahunPajakLapor),
                    JumlahDPP = decimal.Parse(Info.JumlahDPP),
                    JumlahPPN = decimal.Parse(Info.JumlahPPN),
                    JumlahPPNBM = decimal.Parse(Info.JumlahPPNBM),
                    Pesan = Info.Pesan,
                    CreatedBy = userName,
                    ModifiedBy = userName,

                    NPWPPenjual = Info.NPWPVendor.Replace(".", "").Replace("-", ""),
                    NamaPenjual = Info.NamaVendor,
                    AlamatPenjual = Info.AlamatVendor,
                    FormatedNoFakturPajak = Info.NoFaktur,
                    FormatedNpwpPenjual = Info.NPWPVendor,
                    TglFaktur = string.IsNullOrEmpty(Info.TanggalFaktur) ? (DateTime?)null : DateTime.Parse(Info.TanggalFaktur),
                    Dikreditkan = Info.Dikreditkan.ToLower() == "ya" ? true : false,

                    KdJenisTransaksi = Info.KdJenisTransaksi,
                    FgPengganti = Info.FgPengganti,
                    NoFakturPajak = Info.NoFaktur.Replace(".", "").Replace("-", "")
                };

                retur = FakturPajakReturs.Save(retur);

                if (Info.FakturPajakReturId > 0)
                {
                    model.Message = String.Format("Faktur Pajak Retur '{0}' has been updated.", retur.NoDocRetur);
                    model.InfoType = RequestResultInfoType.Success;
                }
                else
                {
                    model.Message = String.Format("Faktur Pajak Retur '{0}' has been created.", retur.NoDocRetur);
                    model.InfoType = RequestResultInfoType.Success;
                }

                scope.Complete();
                scope.Dispose();
            }
            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveFakturReturPajak(string FakturReturPajakId, string noDocRetur)
        {
            RequestResultModel _model = new RequestResultModel();
            if (!string.IsNullOrEmpty(FakturReturPajakId))
            {
                using (TransactionScope scope = new TransactionScope())
                {

                    FakturPajakReturs.Delete(long.Parse(FakturReturPajakId), "System");

                    _model.Message = String.Format("Faktur Pajak Retur '{0}' has been deleted.", noDocRetur);
                    _model.InfoType = RequestResultInfoType.Success;


                    scope.Complete();
                    scope.Dispose();
                }
                return Json(new { Html = _model }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                _model.Message = String.Format("Faktur Pajak Retur '{0}' has not been deleted.", noDocRetur);
                _model.InfoType = RequestResultInfoType.ErrorOrDanger;
                return Json(new { Html = _model }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetKdJenisTransaksiByFCode()
        {
            var dbData = FPKdJenisTransaksis.GetByFCode(EnumHelper.GetDescription(ApplicationEnums.FCodeFpKhusus.Dm));
            return Json(new
            {
                aaData = dbData
            },
            JsonRequestBehavior.AllowGet);
        }
    }
}
