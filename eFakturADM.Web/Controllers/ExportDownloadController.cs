using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Web.Mvc;
using eFakturADM.ExternalLib;
using eFakturADM.FileManager;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Shared.Utility;
using eFakturADM.Web.Helpers;
using eFakturADM.Web.Models;

namespace eFakturADM.Web.Controllers
{
    public class ExportDownloadController : BaseController
    {

        [AuthActivity("23")]
        public void ExportCompareEvisVsIwsToExcel(string receivedDateStart, string receivedDateEnd, string statusId, string scanUserName, string scanDateString)
        {
            var userId = Session["UserId"] as string;
            var userInitial = "";
            var userName = "";
            if (userId != null)
            {
                var userData = Users.GetById(Convert.ToInt32(userId));
                userInitial = userData.UserInitial;
                userName = userData.UserName;
            }
            else
            {
                return;
            }
            DateTime? dtScanDate = string.IsNullOrEmpty(scanDateString)
                ? (DateTime?)null
                : Convert.ToDateTime(scanDateString);

            try
            {

                var procRet = ExportFileManager.ExportCompEvisVsIws(receivedDateStart, receivedDateEnd, statusId, userInitial, scanUserName, dtScanDate);

                if (procRet.InfoType != CommonOutputType.Success) throw new Exception(procRet.MessageInfo);

                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);
                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var newFile = new FileInfo(procRet.FilePath);

                    var fileName = Path.GetFileName(procRet.FilePath);
                    SaveDownloadLog(userName, fileName, procRet.FilePath, ApplicationEnums.DownloadModuleType.CompareEvisVsIws);
                    var attachment = string.Format("attachment; filename={0}", fileName);
                    Response.Clear();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.WriteFile(newFile.FullName);
                    Response.Flush();
                    Response.End();
                    
                }
                
            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                throw;
            }
        }

        [AuthActivity("55")]
        public void ExportReportDetailFakturPajakToExcel(string search, string noFaktur1, string noFaktur2, string npwp,
            string nama, string tglStart, string tglEnd, string masaPajak, string tahunPajak, string scanDateAwal, string scanDateAkhir)
        {
            var userId = Session["UserId"] as string;
            var userInitial = "";
            var userName = "";
            if (userId != null)
            {
                var userData = Users.GetById(Convert.ToInt32(userId));
                userInitial = userData.UserInitial;
                userName = userData.UserName;
            }
            else
            {
                return;
            }

            try
            {
                DateTime? tglFakturStart = !string.IsNullOrEmpty(tglStart) ? Convert.ToDateTime(tglStart) : (DateTime?)null;
                DateTime? tglFakturEnd = !string.IsNullOrEmpty(tglEnd) ? Convert.ToDateTime(tglEnd) : (DateTime?)null;

                int? imasaPajak = string.IsNullOrEmpty(masaPajak) || masaPajak == "0" || masaPajak == "undefined" ? (int?)null : int.Parse(masaPajak);
                int? itahunPajak = string.IsNullOrEmpty(tahunPajak) || tahunPajak == "0" || tahunPajak == "undefined" ? (int?)null : int.Parse(tahunPajak);

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

                var procRet = ExportFileManager.ReportDetailFakturPajakToExcel(noFaktur1, noFaktur2, search, userInitial, npwp,
                nama, tglFakturStart, tglFakturEnd, imasaPajak, itahunPajak, dscanDateAwal, dscanDateAkhir);

                if (procRet.InfoType != CommonOutputType.Success)
                {
                    throw new Exception(procRet.MessageInfo);
                }

                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var newFile = new FileInfo(procRet.FilePath);

                    var fileName = Path.GetFileName(procRet.FilePath);
                    SaveDownloadLog(userName, fileName, procRet.FilePath, ApplicationEnums.DownloadModuleType.ReportDetailFakturPajak);
                    var attachment = string.Format("attachment; filename={0}", fileName);
                    Response.Clear();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.WriteFile(newFile.FullName);
                    Response.Flush();
                    Response.End();
                }

            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                throw;
            }
        }

        [AuthActivity("57")]
        public void ExportReportFakturPajakMasukanToExcel(string search, string sTglFakturStart, string sTglFakturEnd, string picEntry, string fillingIndexStart, string fillingIndexEnd, string masaPajak, string tahunPajak)
        {
            var userId = Session["UserId"] as string;
            var userInitial = "";
            var userName = "";
            if (userId != null)
            {
                var userData = Users.GetById(Convert.ToInt32(userId));
                userInitial = userData.UserInitial;
                userName = userData.UserName;
            }
            else
            {
                return;
            }

            int? iMasaPajak = null;
            int? iTahunPajak = null;
            var strSearchPic = string.Empty;
            if (!string.IsNullOrEmpty(picEntry) && !(picEntry.ToLower() == "undefine" && picEntry.ToLower() == "undefined" && picEntry.ToLower() == "all"))
            {
                strSearchPic = picEntry;
            }

            if (!string.IsNullOrEmpty(masaPajak) && !(masaPajak == "undefine" || masaPajak == "undefined" || masaPajak == "0"))
            {
                iMasaPajak = Convert.ToInt32(masaPajak);
            }
            if (!string.IsNullOrEmpty(tahunPajak) && tahunPajak.ToLower() != "undefine" && tahunPajak.ToLower() != "undefined" && tahunPajak != "0")
            {
                iTahunPajak = Convert.ToInt32(tahunPajak);
            }
            try
            {
                var procRet = ExportFileManager.ReportFakturPajakMasukanToExcel(sTglFakturStart, sTglFakturEnd, strSearchPic,
                search, userInitial, fillingIndexStart, fillingIndexEnd, iMasaPajak, iTahunPajak);

                if (procRet.InfoType != CommonOutputType.Success) throw new Exception(procRet.MessageInfo);

                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var newFile = new FileInfo(procRet.FilePath);

                    var fileName = Path.GetFileName(procRet.FilePath);
                    SaveDownloadLog(userName, fileName, procRet.FilePath, ApplicationEnums.DownloadModuleType.ReportDaftarFakturPajakMasukan);
                    var attachment = string.Format("attachment; filename={0}", fileName);
                    Response.Clear();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.WriteFile(newFile.FullName);
                    Response.Flush();
                    Response.End();
                }
            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                throw;
            }

        }

        [AuthActivity("59")]
        public void ExportReportFakturPajakOutstandingToExcel(string search, string sPostingDateStrart, string sPostingDateEnd, string docSapStart, string docSapEnd)
        {
            var userId = Session["UserId"] as string;
            var userInitial = "";
            var userName = "";
            if (userId != null)
            {
                var userData = Users.GetById(Convert.ToInt32(userId));
                userInitial = userData.UserInitial;
                userName = userData.UserName;
            }
            else
            {
                return;
            }

            try
            {
                var procRet = ExportFileManager.ReportFakturPajakOutstandingToExcel(sPostingDateStrart, sPostingDateEnd,
                docSapStart, docSapEnd, search, userInitial);

                if (procRet.InfoType != CommonOutputType.Success) throw new Exception(procRet.MessageInfo);

                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var newFile = new FileInfo(procRet.FilePath);

                    var fileName = Path.GetFileName(procRet.FilePath);
                    SaveDownloadLog(userName, fileName, procRet.FilePath, ApplicationEnums.DownloadModuleType.ReportFakturPajakOutstanding);
                    var attachment = string.Format("attachment; filename={0}", fileName);
                    Response.Clear();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.WriteFile(newFile.FullName);
                    Response.Flush();
                    Response.End();
                }

            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                throw;
            }

        }

        [AuthActivity("61")]
        public void ExportReportFakturPajakBelumDiJurnalToExcel(string search, string tglFakturStart, string tglFakturEnd, string noFakturStart, string noFakturEnd)
        {
            var userId = Session["UserId"] as string;
            var userInitial = "";
            var userName = "";
            if (userId != null)
            {
                var userData = Users.GetById(Convert.ToInt32(userId));
                userInitial = userData.UserInitial;
                userName = userData.UserName;
            }
            else
            {
                return;
            }

            try
            {
                var procRet = ExportFileManager.ReportFakturPajakBelumDiJurnal(search, tglFakturStart, tglFakturEnd,
                    noFakturStart, noFakturEnd, userInitial);

                if (procRet.InfoType != CommonOutputType.Success) throw new Exception(procRet.MessageInfo);

                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var newFile = new FileInfo(procRet.FilePath);

                    var fileName = Path.GetFileName(procRet.FilePath);
                    SaveDownloadLog(userName, fileName, procRet.FilePath, ApplicationEnums.DownloadModuleType.ReportFakturPajakBelumDiJurnal);
                    var attachment = string.Format("attachment; filename={0}", fileName);
                    Response.Clear();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.WriteFile(newFile.FullName);
                    Response.Flush();
                    Response.End(); 
                }

            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                throw;
            }

        }

        public void ExportListOrdnerToExcel(string search, string noFakturStart,
            string noFakturEnd, string tglFakturStart,
            string tglFakturEnd, string bulanPajak, string tahunPajak, string picEntry)
        {
            var userId = Session["UserId"] as string;
            var userInitial = "";
            var userName = "";
            if (userId != null)
            {
                var userData = Users.GetById(Convert.ToInt32(userId));
                userInitial = userData.UserInitial;
                userName = userData.UserName;
            }
            else
            {
                return;
            }

            try
            {

                DateTime? dTglFakturStart = string.IsNullOrEmpty(tglFakturStart) || tglFakturStart == "undefined"
                ? (DateTime?) null
                : Convert.ToDateTime(tglFakturStart);
            DateTime? dTglFakturEnd = string.IsNullOrEmpty(tglFakturEnd) || tglFakturEnd == "undefined" 
                ? (DateTime?) null : Convert.ToDateTime(tglFakturEnd);

            int? iBulanPajak = string.IsNullOrEmpty(bulanPajak) || bulanPajak == "undefined" || bulanPajak == "0"
                ? (int?) null
                : Convert.ToInt32(bulanPajak);
            int? iTahunPajak = string.IsNullOrEmpty(tahunPajak) || tahunPajak == "undefined" || tahunPajak == "0"
                ? (int?) null
                : Convert.ToInt32(tahunPajak);

                var procRet = ExportFileManager.ListOrdnerExportToExcel(search, noFakturStart, noFakturEnd,
                    dTglFakturStart, dTglFakturEnd, iBulanPajak, iTahunPajak, picEntry, userInitial);

                if (procRet.InfoType != CommonOutputType.Success) throw new Exception(procRet.MessageInfo);

                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var newFile = new FileInfo(procRet.FilePath);

                    var fileName = Path.GetFileName(procRet.FilePath);
                    SaveDownloadLog(userName, fileName, procRet.FilePath, ApplicationEnums.DownloadModuleType.ReportFakturPajakBelumDiJurnal);
                    var attachment = string.Format("attachment; filename={0}", fileName);
                    Response.Clear();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.WriteFile(newFile.FullName);
                    Response.Flush();
                    Response.End();
                }

            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                throw;
            }
        }

        #region ----------- Daftar Faktur Pajak --------------

        [AuthActivity("11")]
        public void DaftarFakturPajakDownloadExcel(string noFaktur1, string noFaktur2, string npwp,
            string nama, string tglStart, string tglEnd, string masaPajak, string tahunPajak,
            string status, string fNpwpPenjual, string fNamaPenjual,
            string fNoFaktur, string fTglFaktur, string fMasaPajakName,
            string fTahunPajak, string fDppString, string fPpnString,
            string fPpnBmString, string fStatusFaktur, string dataType,
            string scanDateAwal, string scanDateAkhir, string fillingIndex, string fFillingIndex, string fUserName,
            string source, string statusPayment, string remark, bool? createdCsv, string fSource, string fStatusPayment,
            string fRemark, string fCreatedCsv, string StatusPelaporan, string fNamaPelaporan)
        {

            var userId = Session["UserId"] as string;
            var userInitial = "";
            var userName = "";
            if (userId != null)
            {
                var userData = Users.GetById(Convert.ToInt32(userId));
                userInitial = userData.UserInitial;
                userName = userData.UserName;
            }
            else
            {
                return;
            }
            try
            {

                var procRet = ExportFileManager.FakturPajakExportToExcel(noFaktur1, noFaktur2, npwp,
                nama, tglStart, tglEnd, masaPajak, tahunPajak, status, fNpwpPenjual, fNamaPenjual,
                fNoFaktur, fTglFaktur, fMasaPajakName, fTahunPajak, fDppString, fPpnString, fPpnBmString,
                fStatusFaktur, dataType, scanDateAwal, scanDateAkhir, fillingIndex, userInitial, fFillingIndex, fUserName,
                source, statusPayment, remark, createdCsv, fSource, fStatusPayment, fRemark, fCreatedCsv, StatusPelaporan,fNamaPelaporan);

                if (procRet.InfoType != CommonOutputType.Success) throw new Exception(procRet.MessageInfo);

                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var newFile = new FileInfo(procRet.FilePath);

                    var fileName = Path.GetFileName(procRet.FilePath);

                    SaveDownloadLog(userName, fileName, procRet.FilePath, ApplicationEnums.DownloadModuleType.DaftarFakturPajak);

                    var attachment = string.Format("attachment; filename={0}", fileName);
                    Response.Clear();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.WriteFile(newFile.FullName);
                    Response.Flush();
                    Response.End();
                }

            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                throw;
            }

        }


        [AuthActivity("11")]
        public void DaftarFakturPajakOutstandingDownloadExcel(string noFaktur1, string noFaktur2,
          string npwp, string nama, string tglStart, string tglEnd,
          string status, string scanDateAwal, string scanDateAkhir,
          string source, string StatusFaktur, string ByPass, string remark,
          string fNpwpPenjual, string fNamaPenjual,
          string fNoFaktur, string fTglFaktur, 
          string fDppString, string fPpnString,
          string fPpnBmString, string fStatusFaktur,
          string fSource, string fStatusPayment, string fRemark, string fTglFaktur010, string ReceivedStart, string ReceivedEnd)
        {

            var userId = Session["UserId"] as string;
            var userInitial = "";
            var userName = "";
            if (userId != null)
            {
                var userData = Users.GetById(Convert.ToInt32(userId));
                userInitial = userData.UserInitial;
                userName = userData.UserName;
            }
            else
            {
                return;
            }
            try
            {

                var procRet = ExportFileManager.FakturPajakOutstandingExportToExcel(noFaktur1, noFaktur2,
                    npwp, nama, tglStart, tglEnd,
                    status, scanDateAwal, scanDateAkhir,
                    source, StatusFaktur, ByPass, remark,
                    fNpwpPenjual, fNamaPenjual,
                    fNoFaktur, fTglFaktur,
                    fDppString, fPpnString,
                    fPpnBmString, fStatusFaktur,
                    fSource, fStatusPayment, fRemark, fTglFaktur010, userInitial, ReceivedStart, ReceivedEnd);

                if (procRet.InfoType != CommonOutputType.Success) throw new Exception(procRet.MessageInfo);

                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var newFile = new FileInfo(procRet.FilePath);

                    var fileName = Path.GetFileName(procRet.FilePath);

                    SaveDownloadLog(userName, fileName, procRet.FilePath, ApplicationEnums.DownloadModuleType.DaftarFakturPajak);

                    var attachment = string.Format("attachment; filename={0}", fileName);
                    Response.Clear();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.WriteFile(newFile.FullName);
                    Response.Flush();
                    Response.End();
                }

            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                throw;
            }

        }


        [AuthActivity("10")]
        public JsonResult ValidationDaftarFakturPajakCreateCsv(string noFaktur1, string noFaktur2, string npwp,
            string nama, string tglStart, string tglEnd, string masaPajak, string tahunPajak,
            string status, string fNpwpPenjual, string fNamaPenjual,
            string fNoFaktur, string fTglFaktur, string fMasaPajakName,
            string fTahunPajak, string fDppString, string fPpnString,
            string fPpnBmString, string fStatusFaktur, string dataType,
            string scanDateAwal, string scanDateAkhir, string fillingIndex, string fFillingIndex, string fUserName,
            string source, string statusPayment, string remark, bool? createdCsv, string fSource, string fStatusPayment, string fRemark, string fCreatedCsv
            ,string StatusPelaporan, string fNamaPelaporan)
        {
            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = ""
            };
            var msgs = new List<string>();
            if (string.IsNullOrEmpty(masaPajak) || masaPajak == "undefined" || masaPajak == "0")
            {
                msgs.Add("Masa Pajak Mandatory");
            }
            if (string.IsNullOrEmpty(tahunPajak) || tahunPajak == "undefined" || tahunPajak == "0")
            {
                msgs.Add("Tahun Pajak Mandatory");
            }
            if (string.IsNullOrEmpty(scanDateAwal) || scanDateAwal == "undefined")
            {
                msgs.Add("Scan Date Awal Mandatory");
            }
            if (string.IsNullOrEmpty(scanDateAkhir) || scanDateAkhir == "undefined")
            {
                msgs.Add("Scan Date Akhir Mandatory");
            }
            if (!string.IsNullOrEmpty(scanDateAwal) && !string.IsNullOrEmpty(scanDateAkhir))
            {
                var dStart = Convert.ToDateTime(scanDateAwal);
                var dEnd = Convert.ToDateTime(scanDateAkhir);
                if (dStart > dEnd)
                {
                    msgs.Add("Scan Date Akhir harus lebih besar atau sama dengan Scan Date Awal");
                }
            }
            if (string.IsNullOrEmpty(dataType) || dataType == "undefined")
            {
                msgs.Add("Silahkan Pilih salah satu Data Type");
            }

            if (msgs.Count > 0)
            {
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = string.Join("<br />", msgs);
            }
            else
            {
                DateTime? tglFakturStart = !string.IsNullOrEmpty(tglStart) ? Convert.ToDateTime(tglStart) : (DateTime?)null;
                DateTime? tglFakturEnd = !string.IsNullOrEmpty(tglEnd) ? Convert.ToDateTime(tglEnd) : (DateTime?)null;

                int idataType = 0;
                int? ifillingIndex = null;

                if (!string.IsNullOrEmpty(dataType) && dataType != "undefined")
                {
                    idataType = int.Parse(dataType);
                }

                var dStart = Convert.ToDateTime(scanDateAwal);
                var dEnd = Convert.ToDateTime(scanDateAkhir);

                if (!string.IsNullOrEmpty(fillingIndex) && fillingIndex != "undefined")
                {
                    ifillingIndex = int.Parse(fillingIndex);
                }

                int imasaPajak = string.IsNullOrEmpty(masaPajak) || masaPajak == "0" || masaPajak == "undefined" ? 0 : int.Parse(masaPajak);
                int itahunPajak = string.IsNullOrEmpty(tahunPajak) || tahunPajak == "0" || masaPajak == "undefined" ? 0 : int.Parse(tahunPajak);


                var dats = ExportCsvDomains.ValidateFakturPajakSourceData(noFaktur1, noFaktur2, tglFakturStart, tglFakturEnd,
                    npwp, nama, imasaPajak, itahunPajak, status, fNpwpPenjual, fNamaPenjual, fNoFaktur,
                    fTglFaktur, fMasaPajakName, fTahunPajak, fDppString, fPpnString, fPpnBmString, fStatusFaktur, dStart, dEnd,
                    ifillingIndex, idataType, fFillingIndex, fUserName,
                    source, statusPayment, remark, createdCsv,
                    fSource, fStatusPayment, fRemark, fCreatedCsv,StatusPelaporan, fNamaPelaporan);

                if (dats.Count <= 1)
                {
                    model.InfoType = RequestResultInfoType.ErrorOrDanger;
                    model.Message = "Tidak ada data";
                }

            }
            return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
        }

        public void DaftarFakturPajakCreateCsv(string noFaktur1, string noFaktur2, string npwp,
            string nama, string tglStart, string tglEnd, string masaPajak, string tahunPajak,
            string status, string fNpwpPenjual, string fNamaPenjual,
            string fNoFaktur, string fTglFaktur, string fMasaPajakName,
            string fTahunPajak, string fDppString, string fPpnString,
            string fPpnBmString, string fStatusFaktur, string dataType,
            string scanDateAwal, string scanDateAkhir, string fillingIndex, string fFillingIndex, string fUserName,
            string source, string statusPayment, string remark, bool? createdCsv,
            string fSource, string fStatusPayment, string fRemark, string fCreatedCsv, string StatusPelaporan , string fNamaPelaporan, string modifiedBy)
        {
            var userId = Session["UserId"] as string;
            var userName = "";
            if (userId != null)
            {
                var userData = Users.GetById(Convert.ToInt32(userId));
                userName = userData.UserName;
            }
            else
            {
                return;
            }
            DateTime? tglFakturStart = !string.IsNullOrEmpty(tglStart) ? Convert.ToDateTime(tglStart) : (DateTime?)null;
            DateTime? tglFakturEnd = !string.IsNullOrEmpty(tglEnd) ? Convert.ToDateTime(tglEnd) : (DateTime?)null;

            int idataType = 0;
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

            if (!string.IsNullOrEmpty(fillingIndex) && fillingIndex != "undefined")
            {
                ifillingIndex = int.Parse(fillingIndex);
            }

            int imasaPajak = string.IsNullOrEmpty(masaPajak) || masaPajak == "0" || masaPajak == "undefined" ? 0 : int.Parse(masaPajak);
            int itahunPajak = string.IsNullOrEmpty(tahunPajak) || tahunPajak == "0" || tahunPajak == "undefined" ? 0 : int.Parse(tahunPajak);


            try
            {
                var procRet = ExportFileManager.ExportFakturPajakToCsv(noFaktur1, noFaktur2, tglFakturStart, tglFakturEnd,
                npwp,
                nama, imasaPajak, itahunPajak, status, fNpwpPenjual, fNamaPenjual, fNoFaktur, fTglFaktur, fMasaPajakName,
                fTahunPajak, fDppString, fPpnString, fPpnBmString, fStatusFaktur, dscanDateAwal, dscanDateAkhir, ifillingIndex,
                idataType, fFillingIndex, fUserName, source, statusPayment, remark, createdCsv,
                fSource, fStatusPayment, fRemark, fCreatedCsv,StatusPelaporan, fNamaPelaporan, modifiedBy);

                if (procRet.InfoType != CommonOutputType.Success)
                {
                    throw new Exception(procRet.MessageInfo);
                }

                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var newFile = new FileInfo(procRet.FilePath);

                    var fileName = Path.GetFileName(procRet.FilePath);

                    SaveDownloadLog(userName, fileName, procRet.FilePath, ApplicationEnums.DownloadModuleType.DaftarFakturPajak);

                    var attachment = string.Format("attachment; filename={0}", fileName);
                    Response.Clear();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.WriteFile(newFile.FullName);
                    Response.Flush();
                    Response.End();
                }
            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                throw;
            }
        }

        #endregion

        #region -------------------- Daftar Retur Faktur Pajak --------------------

        [AuthActivity("18")]
        public void DaftarReturFakturPajakCreateCsv(string noFaktur, string noDocRetur,
            string tglFakturReturStart, string tglFakturReturEnd,
            string npwpVendor, string namaVendor, string masaPajak, string tahunPajak,
            string fTglRetur, string fNpwpVendor, string fNamaVendor, string fNoFakturDiRetur, string fTglFaktur,
            string fNomorRetur, string fMasaRetur, string fTahunRetur, string fDpp, string fPpn, string fPpnBm, string fUserName)
        {
            var userId = Session["UserId"] as string;
            var userName = "";
            if (userId != null)
            {
                var userData = Users.GetById(Convert.ToInt32(userId));
                userName = userData.UserName;
            }
            else
            {
                return;
            }

            int? imasaPajak = string.IsNullOrEmpty(masaPajak) || masaPajak == "undefined" || masaPajak == "0" ? (int?)null : int.Parse(masaPajak);
            int? itahunPajak = string.IsNullOrEmpty(tahunPajak) || tahunPajak == "undefined" || tahunPajak == "0" ? (int?)null : int.Parse(tahunPajak);
            DateTime? dttglFakturReturStart = string.IsNullOrEmpty(tglFakturReturStart) ||
                                              tglFakturReturStart == "undefined"
                ? (DateTime?)null
                : DateTime.Parse(tglFakturReturStart);

            DateTime? dttglFakturReturEnd = string.IsNullOrEmpty(tglFakturReturEnd) ||
                                              tglFakturReturStart == "undefined"
                ? (DateTime?)null
                : DateTime.Parse(tglFakturReturEnd);

            try
            {
                var procRet = ExportFileManager.ExportReturToCsv(noFaktur, noDocRetur, dttglFakturReturStart,
                dttglFakturReturEnd, npwpVendor, namaVendor, imasaPajak, itahunPajak, fTglRetur, fNpwpVendor,
                fNamaVendor, fNoFakturDiRetur, fTglFaktur, fNomorRetur, fMasaRetur, fTahunRetur,
                fDpp, fPpn, fPpnBm, fUserName);

                if (procRet.InfoType != CommonOutputType.Success) throw new Exception(procRet.MessageInfo);

                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var newFile = new FileInfo(procRet.FilePath);

                    var fileName = Path.GetFileName(procRet.FilePath);

                    SaveDownloadLog(userName, fileName, procRet.FilePath, ApplicationEnums.DownloadModuleType.DaftarReturFakturPajak);

                    var attachment = string.Format("attachment; filename={0}", fileName);
                    Response.Clear();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.WriteFile(newFile.FullName);
                    Response.Flush();
                    Response.End();
                }
            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                throw;
            }
        }

        [AuthActivity("19")]
        public void DaftarReturFakturPajakDownloadExcel(string noFaktur, string noDocRetur,
            string tglFakturReturStart, string tglFakturReturEnd,
            string npwpVendor, string namaVendor, string masaPajak, string tahunPajak,
            string fTglRetur, string fNpwpVendor, string fNamaVendor, string fNoFakturDiRetur, string fTglFaktur,
            string fNomorRetur, string fMasaRetur, string fTahunRetur, string fDpp, string fPpn, string fPpnBm, string fUserName)
        {
            var userId = Session["UserId"] as string;
            var userName = "";
            var userInitial = "";
            if (userId != null)
            {
                var userData = Users.GetById(Convert.ToInt32(userId));
                userName = userData.UserName;
                userInitial = userData.UserInitial;
            }
            else
            {
                return;
            }

            int? imasaPajak = string.IsNullOrEmpty(masaPajak) || masaPajak == "undefined" || masaPajak == "0" ? (int?)null : int.Parse(masaPajak);
            int? itahunPajak = string.IsNullOrEmpty(tahunPajak) || tahunPajak == "undefined" || tahunPajak == "0" ? (int?)null : int.Parse(tahunPajak);
            DateTime? dttglFakturReturStart = string.IsNullOrEmpty(tglFakturReturStart) ||
                                              tglFakturReturStart == "undefined"
                ? (DateTime?)null
                : DateTime.Parse(tglFakturReturStart);

            DateTime? dttglFakturReturEnd = string.IsNullOrEmpty(tglFakturReturEnd) ||
                                              tglFakturReturStart == "undefined"
                ? (DateTime?)null
                : DateTime.Parse(tglFakturReturEnd);

            try
            {
                var procRet = ExportFileManager.ExportReturToExcel(noFaktur, noDocRetur, dttglFakturReturStart,
                dttglFakturReturEnd, npwpVendor, namaVendor, imasaPajak, itahunPajak, userInitial, fTglFaktur, fNpwpVendor, fNamaVendor, fNoFakturDiRetur,
                fTglFaktur, fNomorRetur, fMasaRetur, fTahunRetur, fDpp, fPpn, fPpnBm, fUserName);

                if (procRet.InfoType != CommonOutputType.Success) throw new Exception(procRet.MessageInfo);

                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var newFile = new FileInfo(procRet.FilePath);

                    var fileName = Path.GetFileName(procRet.FilePath);

                    SaveDownloadLog(userName, fileName, procRet.FilePath, ApplicationEnums.DownloadModuleType.DaftarReturFakturPajak);

                    var attachment = string.Format("attachment; filename={0}", fileName);
                    Response.Clear();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.WriteFile(newFile.FullName);
                    Response.Flush();
                    Response.End();
                }

            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                throw;
            }
        }

        #endregion


        public void ExportCompEvisVsSapToExcel(string tglPostingStart, string tglPostingEnd, string tglFakturStart, string tglFakturEnd, string noFakturStart,
            string noFakturEnd, string scanDate, string masaPajak, string username, string tahunPajak, string statusId, string statusPosting)
        {
            
            var userId = Session["UserId"] as string;
            var userInitial = "";
            var userName = "";
            if (userId != null)
            {
                var userData = Users.GetById(Convert.ToInt32(userId));
                userInitial = userData.UserInitial;
                userName = userData.UserName;
            }
            else
            {
                return;
            }

            int? statusIntId = null;
            if (!(string.IsNullOrEmpty(statusId) || statusId == "0"))
            {
                statusIntId = Convert.ToInt32(statusId);
            }

            int? statusIntPosting = null;
            if (!(string.IsNullOrEmpty(statusPosting) || statusPosting == "0"))
            {
                statusIntPosting = Convert.ToInt32(statusPosting);
            }


            DateTime? dateNull = null;
            int? intNull = null;

            int? iMasaPajak = !string.IsNullOrEmpty(masaPajak) && masaPajak != "0" ? Convert.ToInt32(masaPajak) : intNull;
            int? iTahunPajak = !string.IsNullOrEmpty(tahunPajak) && tahunPajak != "0" ? Convert.ToInt32(tahunPajak) : intNull;
            DateTime? dttglPostingStart = !string.IsNullOrEmpty(tglPostingStart) ? Convert.ToDateTime(tglPostingStart) : dateNull;
            DateTime? dttglPostingEnd = !string.IsNullOrEmpty(tglPostingEnd) ? Convert.ToDateTime(tglPostingEnd) : dateNull;
            DateTime? dttglFakturStart = !string.IsNullOrEmpty(tglFakturStart) ? Convert.ToDateTime(tglFakturStart) : dateNull;
            DateTime? dttglFakturEnd = !string.IsNullOrEmpty(tglFakturEnd) ? Convert.ToDateTime(tglFakturEnd) : dateNull;
            DateTime? dtScanDate = !string.IsNullOrEmpty(scanDate) ? Convert.ToDateTime(scanDate) : dateNull;
            var dats = CompEvisSaps.GetCompareListToDownload(dttglPostingStart, dttglPostingEnd, dttglFakturStart,
                dttglFakturEnd, noFakturStart, noFakturEnd, dtScanDate, username, iMasaPajak, iTahunPajak, statusIntId, statusIntPosting);

            if (dats.Count <= 0)
            {
                throw new Exception("No Data to Export");
            }

            try
            {

                var procRet = ExportFileManager.ExportCompEvisVsSapToExcel(dats, userInitial);

                if (procRet.InfoType != CommonOutputType.Success) throw new Exception(procRet.MessageInfo);

                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var newFile = new FileInfo(procRet.FilePath);

                    var fileName = Path.GetFileName(procRet.FilePath);
                    SaveDownloadLog(userName, fileName, procRet.FilePath, ApplicationEnums.DownloadModuleType.CompareEvisVsIws);
                    var attachment = string.Format("attachment; filename={0}", fileName);
                    Response.Clear();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.WriteFile(newFile.FullName);
                    Response.Flush();
                    Response.End();
                }
                
            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.InnerException + exception.Message , MethodBase.GetCurrentMethod(), exception);
                throw;
            }
        }

        public void ReportSpmExportExcel(string masaPajak, string tahunPajak, string versi)
        {
            int imasaPajak = 0;
            int iVersi = 0;
            int itahunPajak = 0;
            if (string.IsNullOrEmpty(masaPajak) || masaPajak == "undefined")
            {
                throw new Exception("Request Parameter [MasaPajak] not found.");
            }
            if (!int.TryParse(masaPajak, out imasaPajak))
            {
                throw new Exception("Invalid Request Parameter");
            }


            if (string.IsNullOrEmpty(tahunPajak) || tahunPajak == "undefined")
            {
                throw new Exception("Request Parameter [TahunPajak] not found.");
            }
            if (!int.TryParse(tahunPajak, out itahunPajak))
            {
                throw new Exception("Invalid Request Parameter");
            }

            if (string.IsNullOrEmpty(versi) || versi == "undefined")
            {
                throw new Exception("Request Parameter [Versi] not found.");
            }
            if (!int.TryParse(versi, out iVersi))
            {
                throw new Exception("Invalid Request Parameter");
            }

            var userId = Session["UserId"] as string;
            var userInitial = "";
            var userName = "";
            if (userId != null)
            {
                var userData = Users.GetById(Convert.ToInt32(userId));
                userInitial = userData.UserInitial;
                userName = userData.UserName;
            }
            else
            {
                return;
            }
            
            try
            {
                var procRet = ExportFileManager.ReportSpmCreateExcel(imasaPajak, itahunPajak, iVersi, userInitial);

                if (procRet.InfoType != CommonOutputType.Success)
                {
                    throw new Exception(procRet.MessageInfo);
                }

                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var newFile = new FileInfo(procRet.FilePath);

                    var fileName = Path.GetFileName(procRet.FilePath);

                    SaveDownloadLog(userName, fileName, procRet.FilePath, ApplicationEnums.DownloadModuleType.DaftarFakturPajak);

                    var attachment = string.Format("attachment; filename={0}", fileName);
                    Response.Clear();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.WriteFile(newFile.FullName);
                    Response.Flush();
                    Response.End();
                }

            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                throw;
            }
        }

        private void SaveDownloadLog(string userNameLogin, string fileName, string filePath, ApplicationEnums.DownloadModuleType eModuleType)
        {
            string ip = Request.UserHostAddress;
            var dataToSave = new LogDownload()
            {
                Id = 0,
                RequestDate = DateTime.Now,
                Requestor = userNameLogin,
                FileName = fileName,
                FilePath = filePath,
                FileType = EnumHelper.GetDescription(eModuleType),
                ClientIp = ip
            };

            LogDownloads.Insert(dataToSave);

        }

        public void DownloadFile(string type, string fileFolder, string fileName)
        {
            try
            {
                var newFile = DownloadFileManager.GetFileInfo(type, fileFolder, fileName);

                if (newFile == null) return;
                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);
                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    string attachment = string.Format("attachment; filename={0}", fileName);
                    Response.Clear();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.WriteFile(newFile.FullName);
                    Response.Flush();
                    Response.End();
                }
            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                throw;
            }

        }

        public FileStreamResult PreviewFileEcm(long objectID)
        {
            try
            {
                var ecmUrl = WebConfiguration.EcmApiUrl;
                var ecmUsername = WebConfiguration.EcmApiUsername;
                var ecmPassword = WebConfiguration.EcmApiPassword;
                var ecmTempFolder = WebConfiguration.EcmTempFolder;
                var ecmLib = new EcmLib(ecmUrl, ecmUsername, ecmPassword);
                var ecm = ecmLib.GetEcmDownloadFakturPajakV3(objectID, ecmTempFolder, out string msgError, out WebExceptionStatus eStatus, out string logKey);
                if (!string.IsNullOrEmpty(ecm.Error) || string.IsNullOrEmpty(ecm.FilePath))
                    throw new Exception(ecm.Error);

                var stream = System.IO.File.Open(ecm.FilePath, FileMode.Open, FileAccess.Read);
                stream.Position = 0;
                return new FileStreamResult(stream, "application/pdf");
            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                throw;
            }

        }

        public FileResult DownloadFileEcm(long objectID)
        {
            try
            {
                var ecmUrl = WebConfiguration.EcmApiUrl;
                var ecmUsername = WebConfiguration.EcmApiUsername;
                var ecmPassword = WebConfiguration.EcmApiPassword;
                var ecmTempFolder = WebConfiguration.EcmTempFolder;
                var ecmLib = new EcmLib(ecmUrl, ecmUsername, ecmPassword);
                var ecm = ecmLib.GetEcmDownloadFakturPajakV3(objectID, ecmTempFolder, out string msgError, out WebExceptionStatus eStatus, out string logKey);
                if (!string.IsNullOrEmpty(ecm.Error) || string.IsNullOrEmpty(ecm.FilePath))
                    throw new Exception(ecm.Error);

                var stream = System.IO.File.Open(ecm.FilePath, FileMode.Open, FileAccess.Read);
                stream.Position = 0;
                return File(stream, "application/pdf", $"Faktur-Pajak-Document-{objectID}.pdf");
            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                throw;
            }

        }

       public FileResult DownloadLaporFakturPajak(long ID)
        {
            try
            {
                var getData = FakturPajakTerlaporCollections.GetById(ID);
                var stream = System.IO.File.Open(getData.AttachmentPath, FileMode.Open, FileAccess.Read);
                FileInfo fi = new FileInfo(getData.AttachmentPath);
                stream.Position = 0;
                return File(stream, "application/pdf", fi.Name);
            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                throw;
            }

        }

        #region ----------- Daftar Faktur Pajak --------------
        //[AuthActivity("11")]
        public void DaftarFakturPajakDigantiOutstandingDownloadExcel(string noFaktur1, string noFaktur2, string npwp,
            string nama, string tglStart, string tglEnd,
            string status, string fNpwpPenjual, string fNamaPenjual,
            string fNoFaktur, string fTglFaktur, string fMasaPajakName,
            string fTahunPajak, string fDppString, string fPpnString,
            string fPpnBmString, string fStatusFaktur, string dataType,
            string scanDateAwal, string scanDateAkhir, string fillingIndex, string fFillingIndex, string fUserName)
        {

            var userId = Session["UserId"] as string;
            var userInitial = "";
            var userName = "";
            if (userId != null)
            {
                var userData = Users.GetById(Convert.ToInt32(userId));
                userInitial = userData.UserInitial;
                userName = userData.UserName;
            }
            else
            {
                return;
            }
            try
            {

                var procRet = ExportFileManager.FakturPajakDigantiOutstandingExportToExcel(noFaktur1, noFaktur2, npwp,
                nama, tglStart, tglEnd, status, fNpwpPenjual, fNamaPenjual,
                fNoFaktur, fTglFaktur, fMasaPajakName, fTahunPajak, fDppString, fPpnString, fPpnBmString,
                fStatusFaktur, dataType, scanDateAwal, scanDateAkhir, fillingIndex, userInitial, fFillingIndex, fUserName);

                if (procRet.InfoType != CommonOutputType.Success) throw new Exception(procRet.MessageInfo);

                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var newFile = new FileInfo(procRet.FilePath);

                    var fileName = Path.GetFileName(procRet.FilePath);

                    SaveDownloadLog(userName, fileName, procRet.FilePath, ApplicationEnums.DownloadModuleType.DaftarFakturPajak);

                    var attachment = string.Format("attachment; filename={0}", fileName);
                    Response.Clear();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.WriteFile(newFile.FullName);
                    Response.Flush();
                    Response.End();
                }

            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                throw;
            }

        }
        #endregion

        #region Faktur Pajak Penampung
        public void FakturPajakPenampungDownloadExcel(string noFaktur1, string noFaktur2, string npwp,
            string nama, string tglStart, string tglEnd,
            string status, string scanDateAwal, string scanDateAkhir, string source, string remark,
            string fNpwpPenjual, string fNamaPenjual,
            string fNoFaktur, string fTglFaktur, string fDppString, string fPpnString,
            string fPpnBmString, string fStatusFaktur, string fUserName, string fSource, string fStatusPayment, string fRemarks, string fpIds)
        {

            var userId = Session["UserId"] as string;
            var userInitial = "";
            var userName = "";
            if (userId != null)
            {
                var userData = Users.GetById(Convert.ToInt32(userId));
                userInitial = userData.UserInitial;
                userName = userData.UserName;
            }
            else
            {
                return;
            }
            try
            {

                var procRet = ExportFileManager.FakturPajakPenampungExportToExcel(noFaktur1, noFaktur2, npwp,
                                nama, tglStart, tglEnd,
                                status, scanDateAwal, scanDateAkhir, source, remark,
                                fNpwpPenjual, fNamaPenjual,
                                fNoFaktur, fTglFaktur, fDppString, fPpnString,
                                fPpnBmString, fStatusFaktur, fUserName, fSource, fStatusPayment, fRemarks, fpIds);

                if (procRet.InfoType != CommonOutputType.Success) throw new Exception(procRet.MessageInfo);

                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var newFile = new FileInfo(procRet.FilePath);

                    var fileName = Path.GetFileName(procRet.FilePath);

                    SaveDownloadLog(userName, fileName, procRet.FilePath, ApplicationEnums.DownloadModuleType.DaftarFakturPajak);

                    var attachment = string.Format("attachment; filename={0}", fileName);
                    Response.Clear();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.WriteFile(newFile.FullName);
                    Response.Flush();
                    Response.End();
                }

            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                throw;
            }

        }
        #endregion
    }
}