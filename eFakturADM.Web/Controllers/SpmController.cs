using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Transactions;
using System.Web.Mvc;
using eFakturADM.Logic.Collections;
using eFakturADM.Shared.Utility;
using eFakturADM.Web.Models;

namespace eFakturADM.Web.Controllers
{
    public class SpmController : BaseController
    {
        //
        // GET: /Spm/

        public ActionResult Create()
        {
            return View();
        }

        public JsonResult GetSpmInfo(int masaPajak, int tahunPajak)
        {
            var version = 0;
            string namaMasaPajak;
            var getDataSpm = ReportSuratPemberitahuanMasas.GetLastDataByMasaPajak(masaPajak, tahunPajak);
            if (getDataSpm != null)
            {
                //check period
                var getPeriodData = OpenClosePeriods.GetByMasaPajak(masaPajak, tahunPajak);
                if (getPeriodData.StatusRegular)
                {
                    version = getDataSpm.Versi;
                }
                else
                {
                    version = getDataSpm.Versi + 1;
                }
                
                namaMasaPajak = getDataSpm.NamaMasaPajak;
            }
            else
            {
                var mData = MasaPajaks.GetByMonthNumber(masaPajak);
                namaMasaPajak = mData.MonthName;
            }
            var dats = ReportSuratPemberitahuanMasaDetails.GetGenerateSubmitSearch(masaPajak, tahunPajak);
            if (dats.Count > 0)
            {
                version = dats.First().Versi;
            }
            var jsonResult = Json(new
            {
                NamaMasaPajak = namaMasaPajak,
                TahunPajak = tahunPajak,
                Versi = version,
                aaData = dats
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }

        public JsonResult ValidationSubmitSearchSpm(string masaPajak, string tahunPajak)
        {

            int lastVersion;
            var model = ValidationSpm(masaPajak, tahunPajak, out lastVersion);

            return Json(new
            {
                Html = model,
                MasaPajak = masaPajak,
                TahunPajak = tahunPajak,
                LastVersion = lastVersion
            }, JsonRequestBehavior.AllowGet);

        }

        [AuthActivity("53")]
        public JsonResult ValidationCreateSpm(string masaPajak, string tahunPajak)
        {
            int lastVersion;
            var model = ValidationSpm(masaPajak, tahunPajak, out lastVersion);
            if (model.InfoType == RequestResultInfoType.Success)
            {
                //check if any data
                var dats = ReportSuratPemberitahuanMasaDetails.GetGenerateSubmitSearch(int.Parse(masaPajak), int.Parse(tahunPajak));
                if (dats.Count <= 0)
                {
                    //no data
                    model.InfoType = RequestResultInfoType.ErrorOrDanger;
                    model.Message = "No Data";
                }
            }
            return Json(new
            {
                Html = model,
                MasaPajak = masaPajak,
                TahunPajak = tahunPajak,
                LastVersion = lastVersion
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SubmitCreateSpm(int masaPajak, int tahunPajak)
        {

            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = "Create SPM Success"
            };
            long spmId = 0;
            try
            {

                var userName = Session["UserName"] as string;
                using (var eScope = new TransactionScope())
                {
                    ReportSuratPemberitahuanMasas.GenerateCreateSpm(masaPajak, tahunPajak, userName, out spmId);
                    eScope.Complete();
                }
            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = "Create SPM Failed. <br />Log Key " + logKey;
            }

            return Json(new
            {
                Html = model,
                Id = spmId
            }, JsonRequestBehavior.AllowGet);
        }

        private RequestResultModel ValidationSpm(string masaPajak, string tahunPajak, out int lastVersion)
        {
            lastVersion = 0;
            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = ""
            };

            var msgs = new List<string>();
            if (string.IsNullOrEmpty(masaPajak) || masaPajak == "undefined")
            {
                msgs.Add("Masa Pajak Mandatory");
            }
            if (string.IsNullOrEmpty(tahunPajak) || tahunPajak == "undefined")
            {
                msgs.Add("Tahun Pajak Mandatory");
            }
            if (msgs.Count > 0)
            {
                return new RequestResultModel()
                {
                    InfoType = RequestResultInfoType.ErrorOrDanger,
                    Message = string.Join("<br />", msgs)
                };
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
            if (msgs.Count > 0)
            {
                return new RequestResultModel()
                {
                    InfoType = RequestResultInfoType.ErrorOrDanger,
                    Message = string.Join("<br />", msgs)
                };
            }
            
            var chkPeriodMasaPajak = OpenClosePeriods.GetByMasaPajak(iMasaPajak, iTahunPajak);
            if (chkPeriodMasaPajak == null)
            {
                return new RequestResultModel()
                {
                    InfoType = RequestResultInfoType.ErrorOrDanger,
                    Message = "Masa Pajak tidak ditemukan di Open Close Period"
                };
            }

            if (!chkPeriodMasaPajak.StatusSp2)
            {
                return new RequestResultModel()
                {
                    InfoType = RequestResultInfoType.ErrorOrDanger,
                    Message = "Tidak bisa membuat SPM untuk Masa Pajak yang sudah Close SP2"
                };
            }

            //get last version
            var getLastDataSpm = ReportSuratPemberitahuanMasas.GetLastDataByMasaPajak(iMasaPajak, iTahunPajak);
            if (getLastDataSpm != null)
            {
                if (chkPeriodMasaPajak.StatusRegular)
                {
                    lastVersion = getLastDataSpm.Versi;
                }
                else
                {
                    lastVersion = getLastDataSpm.Versi + 1;
                }
            }


            return model;

        }


    }
}
