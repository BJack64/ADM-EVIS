using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Xml;
using DocumentFormat.OpenXml.Spreadsheet;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Shared.Utility;
using SpreadsheetLight;

namespace eFakturADM.FileManager
{
    public class ExportFileManager
    {

        public static FileManagerCommonOutput ExportCompEvisVsIws(string receivedDateStart, string receivedDateEnd, string statusId, string userInitial, string scanUserName, DateTime? scanDate)
        {
            var objRet = new FileManagerCommonOutput()
            {
                InfoType = CommonOutputType.Success,
                IdNo = string.Empty,
                MessageInfo = "",
                FilePath = ""
            };
            try
            {
                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);
                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var path = EvisVsIwsCreateExcel(receivedDateStart, receivedDateEnd, statusId, userInitial, scanUserName, scanDate);
                    if (!string.IsNullOrEmpty(path))
                    {
                        objRet.FilePath = path;
                    }
                    else
                    {
                        objRet.InfoType = CommonOutputType.Error;
                        objRet.MessageInfo = "Export Excel Failed with Unknown Error.";
                    }
                }
            }
            catch (Exception exception)
            {
                objRet.InfoType = CommonOutputType.Error;
                objRet.MessageInfo = exception.Message;
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
            }
            return objRet;
        }

        public static FileManagerCommonOutput ExportReturToCsv(string noFaktur, string noDocRetur,
            DateTime? tglFakturReturStart, DateTime? tglFakturReturEnd,
            string npwpVendor, string namaVendor, int? masaPajak, int? tahunPajak,
            string fTglRetur, string fNpwpVendor, string fNamaVendor, string fNoFakturDiRetur, string fTglFaktur,
            string fNomorRetur, string fMasaRetur, string fTahunRetur, string fDpp, string fPpn, string fPpnBm, string fUserName)
        {
            var objRet = new FileManagerCommonOutput()
            {
                InfoType = CommonOutputType.Success,
                IdNo = string.Empty,
                MessageInfo = "",
                FilePath = ""
            };

            try
            {
                var dats = ExportCsvDomains.GetReturFakturPajakSourceData(noFaktur, noDocRetur,
                    tglFakturReturStart, tglFakturReturEnd, npwpVendor, namaVendor, masaPajak, tahunPajak, fTglRetur,
                    fNpwpVendor, fNamaVendor, fNoFakturDiRetur, fTglFaktur, fNomorRetur, fMasaRetur,
                    fTahunRetur, fDpp, fPpn, fPpnBm, fUserName);
                if (dats.Count <= 0)
                {
                    objRet.InfoType = CommonOutputType.Error;
                    objRet.MessageInfo = "Empty Data to Export";
                    return objRet;
                }

                //Process csv
                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);
                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var rootExportPath = string.Format(@"{0}\Export", FileManagerConfiguration.RepoRootPath);
                    if (!Directory.Exists(rootExportPath))
                    {
                        Directory.CreateDirectory(rootExportPath);
                    }
                    var destFolder = string.Format(@"{0}\{1}", rootExportPath, @"Csv");
                    if (!Directory.Exists(destFolder))
                    {
                        Directory.CreateDirectory(destFolder);
                    }

                    var filename = string.Format("{0}_RETUR_FAKTUR_MASUKAN_{1}.csv", FileManagerConfiguration.CompanyCode, DateTime.Now.ToString("yyyyMMddhhmmss"));
                    var destFilePath = string.Format(@"{0}\{1}", destFolder, filename);
                    using (var fs = new FileStream(destFilePath, FileMode.Append))
                    {
                        using (var sw = new StreamWriter(fs))
                        {
                            foreach (var d in dats)
                            {
                                sw.WriteLine(d.RowData);
                            }
                            sw.Close();
                        }
                        fs.Close();
                        objRet.FilePath = destFilePath;
                    }
                }
            }
            catch (Exception exception)
            {
                objRet.InfoType = CommonOutputType.Error;
                objRet.MessageInfo = exception.Message;
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
            }
            return objRet;
        }

        public static FileManagerCommonOutput ExportReturToExcel(string noFaktur, string noDocRetur,
            DateTime? tglFakturReturStart, DateTime? tglFakturReturEnd,
            string npwpVendor, string namaVendor, int? masaPajak, int? tahunPajak, string userInitial, string fTglRetur, string fNpwpVendor, string fNamaVendor, string fNoFakturDiRetur, string fTglFaktur,
            string fNomorRetur, string fMasaRetur, string fTahunRetur, string fDpp, string fPpn, string fPpnBm, string fUserName)
        {
            var objRet = new FileManagerCommonOutput()
            {
                InfoType = CommonOutputType.Success,
                IdNo = string.Empty,
                MessageInfo = "",
                FilePath = ""
            };
            try
            {

                var dats = FakturPajakReturs.GetListToDownloadExcel(noFaktur, noDocRetur, tglFakturReturStart,
                    tglFakturReturEnd, npwpVendor, namaVendor, masaPajak, tahunPajak, fTglRetur, fNpwpVendor, fNamaVendor,
                    fNoFakturDiRetur, fTglFaktur, fNomorRetur, fMasaRetur, fTahunRetur, fDpp, fPpn, fPpnBm, fUserName);

                if (dats.Count <= 0)
                {
                    objRet.InfoType = CommonOutputType.Error;
                    objRet.MessageInfo = "No Data to Export";
                    return objRet;
                }

                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var path = ReturFakturPajakCreateExcel(dats, userInitial);

                    if (!string.IsNullOrEmpty(path))
                    {
                        objRet.FilePath = path;
                    }
                    else
                    {
                        objRet.InfoType = CommonOutputType.Error;
                        objRet.MessageInfo = "Export Excel Failed with Unknown Error.";
                    }
                }
            }
            catch (Exception exception)
            {
                objRet.InfoType = CommonOutputType.Error;
                objRet.MessageInfo = exception.Message;
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
            }
            return objRet;
        }

        public static FileManagerCommonOutput ExportFakturPajakToCsv(string noFaktur1, string noFaktur2, DateTime? tglFakturStart, DateTime? tglFakturEnd,
            string npwpVendor, string namaVendor, int masaPajak, int tahunPajak, string status,
            string sNpwpPenjual, string sNamaPenjual, string sNoFaktur, string sTglFaktur, string sMasaPajak, string sTahunPajak,
            string sDppString, string sPpnString, string sPpnBmString, string sStatusFaktur, DateTime? scanDateAwal, DateTime? scanDateAkhir, int? fillingIndex,
            int iFpType, string fFillingIndex, string fUserName, string source, string statusPayment, string remark, bool? createdCsv,
            string sSource, string sStatusPayment, string sRemark, string sCreatedCsv, string StatusPelaporan, string fNamaPelaporan, string modifiedBy)
        {
            var objRet = new FileManagerCommonOutput()
            {
                InfoType = CommonOutputType.Success,
                IdNo = string.Empty,
                MessageInfo = "",
                FilePath = ""
            };

            try
            {
                var dats = ExportCsvDomains.GetFakturPajakSourceData(noFaktur1, noFaktur2, tglFakturStart, tglFakturEnd,
                    npwpVendor, namaVendor, masaPajak, tahunPajak, status, sNpwpPenjual, sNamaPenjual, sNoFaktur,
                    sTglFaktur, sMasaPajak, sTahunPajak, sDppString, sPpnString, sPpnBmString, sStatusFaktur, scanDateAwal, scanDateAkhir,
                    fillingIndex, iFpType, fFillingIndex, fUserName, source, statusPayment, remark, createdCsv,
                    sSource, sStatusPayment, sRemark, sCreatedCsv, StatusPelaporan, fNamaPelaporan, modifiedBy);
                if (dats.Count <= 1)
                {
                    objRet.InfoType = CommonOutputType.Error;
                    objRet.MessageInfo = "Empty Data to Export";
                    return objRet;
                }

                //Process csv
                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);
                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var rootExportPaht = string.Format(@"{0}\Export", FileManagerConfiguration.RepoRootPath);
                    if (!Directory.Exists(rootExportPaht))
                    {
                        Directory.CreateDirectory(rootExportPaht);
                    }
                    var destFolder = string.Format(@"{0}\{1}", rootExportPaht, @"Csv");
                    if (!Directory.Exists(destFolder))
                    {
                        Directory.CreateDirectory(destFolder);
                    }

                    var rowData2 = dats.First().RowData2;
                    var rowData2S = rowData2.Split(';');
                    var scanDateStart = Convert.ToDateTime(rowData2S[0]);
                    var scanDateEnd = Convert.ToDateTime(rowData2S[1]);

                    //var filename = string.Format("{0}_RETUR_FAKTUR_MASUKAN_{1}.csv", FileManagerConfiguration.CompanyCode, DateTime.Now.ToString("yyyyMMddhhmmss"));
                    var filename = "";
                    switch (iFpType)
                    {
                        case (int)ApplicationEnums.FPType.ScanIws:
                            filename = string.Format(@"FPMS_{0}{1}_IWS_{2}_sd_{3}", tahunPajak.ToString("0000"), masaPajak.ToString("00"), scanDateStart.ToString("yyyyMMdd"), scanDateEnd.ToString("yyyyMMdd"));
                            break;
                        case (int)ApplicationEnums.FPType.ScanNonIws:
                            filename = string.Format(@"FPMS_{0}{1}_NONIWS_{2}_sd_{3}", tahunPajak.ToString("0000"), masaPajak.ToString("00"), scanDateStart.ToString("yyyyMMdd"), scanDateEnd.ToString("yyyyMMdd"));
                            break;
                        case (int)ApplicationEnums.FPType.ScanManual:
                            filename = string.Format(@"FPMK_{0}{1}_KHUSUS_{2}_sd_{3}", tahunPajak.ToString("0000"), masaPajak.ToString("00"), scanDateStart.ToString("yyyyMMdd"), scanDateEnd.ToString("yyyyMMdd"));
                            break;
                    }
                    var destFilePath = string.Format(@"{0}\{1}_{2}.csv", destFolder, filename, DateTime.Now.ToString("yyyyMMddHHmmss"));
                    //check if already exists
                    if (File.Exists(destFilePath))
                    {
                        File.Copy(destFilePath, string.Format(@"{0}\{1}_backup_{2}.csv", destFolder, filename, DateTime.Now.ToString("yyyyMMddHHmmss")));
                        File.Delete(destFilePath);
                    }
                    using (var fs = new FileStream(destFilePath, FileMode.CreateNew))
                    {
                        using (var sw = new StreamWriter(fs))
                        {
                            foreach (var d in dats)
                            {
                                sw.WriteLine(d.RowData);
                            }
                            sw.Close();
                        }
                        fs.Close();
                        objRet.FilePath = destFilePath;
                    }
                }
            }
            catch (Exception exception)
            {
                objRet.InfoType = CommonOutputType.Error;
                objRet.MessageInfo = exception.Message;
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
            }
            return objRet;
        }

        public static FileManagerCommonOutput FakturPajakExportToExcel(string noFaktur1, string noFaktur2, string npwp,
            string nama, string tglStart, string tglEnd, string masaPajak, string tahunPajak, string status, string fNpwpPenjual, string fNamaPenjual,
            string fNoFaktur, string fTglFaktur, string fMasaPajakName, string fTahunPajak, string fDppString, string fPpnString, string fPpnBmString,
            string fStatusFaktur, string dataType, string scanDateAwal, string scanDateAkhir, string fillingIndex, string userInitial, string fFillingIndex, string fUserName,
            string source, string statusPayment, string remark, bool? createdCsv, string fSource, string fStatusPayment, string fRemark, string fCreatedCsv, string StatusPelaporan, string fNamaPelaporan)
        {
            var objRet = new FileManagerCommonOutput()
            {
                InfoType = CommonOutputType.Success,
                IdNo = string.Empty,
                MessageInfo = "",
                FilePath = ""
            };
            try
            {
                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);
                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var path = FakturPajakMasukanCreateExcel(noFaktur1, noFaktur2, npwp,
                            nama, tglStart, tglEnd, masaPajak, tahunPajak, status, fNpwpPenjual, fNamaPenjual,
                            fNoFaktur, fTglFaktur, fMasaPajakName, fTahunPajak, fDppString, fPpnString, fPpnBmString,
                            fStatusFaktur, dataType, scanDateAwal, scanDateAkhir, fillingIndex, userInitial, fFillingIndex, fUserName,
                            source, statusPayment, remark, createdCsv, fSource, fStatusPayment, fRemark, fCreatedCsv,StatusPelaporan, fNamaPelaporan);

                    if (!string.IsNullOrEmpty(path))
                    {
                        objRet.FilePath = path;
                    }
                    else
                    {
                        objRet.InfoType = CommonOutputType.Error;
                        objRet.MessageInfo = "Export Excel Failed with Unknown Error.";
                    }
                }
            }
            catch (Exception exception)
            {
                objRet.InfoType = CommonOutputType.Error;
                objRet.MessageInfo = exception.Message;
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
            }
            return objRet;
        }



        public static FileManagerCommonOutput FakturPajakOutstandingExportToExcel(string noFaktur1, string noFaktur2,
          string npwp, string nama, string tglStart, string tglEnd,
          string status, string scanDateAwal, string scanDateAkhir,
          string source, string StatusFaktur, string ByPass, string remark,
          string fNpwpPenjual, string fNamaPenjual,
          string fNoFaktur, string fTglFaktur,
          string fDppString, string fPpnString,
          string fPpnBmString, string fStatusFaktur,
          string fSource, string fStatusPayment, string fRemark, string fTglFaktur010, string userInitial, string ReceivedStart, string ReceivedEnd)
        {
            var objRet = new FileManagerCommonOutput()
            {
                InfoType = CommonOutputType.Success,
                IdNo = string.Empty,
                MessageInfo = "",
                FilePath = ""
            };
            try
            {
                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);
                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var path = FakturPajakOutstandingCreateExcel(noFaktur1, noFaktur2,
                      npwp, nama, tglStart, tglEnd,
                      status, scanDateAwal, scanDateAkhir,
                      source, StatusFaktur, ByPass, remark,
                      fNpwpPenjual, fNamaPenjual,
                      fNoFaktur, fTglFaktur,
                      fDppString, fPpnString,
                      fPpnBmString, fStatusFaktur,
                      fSource, fStatusPayment, fRemark, fTglFaktur010, userInitial, ReceivedStart, ReceivedEnd);

                    if (!string.IsNullOrEmpty(path))
                    {
                        objRet.FilePath = path;
                    }
                    else
                    {
                        objRet.InfoType = CommonOutputType.Error;
                        objRet.MessageInfo = "Export Excel Failed with Unknown Error.";
                    }
                }
            }
            catch (Exception exception)
            {
                objRet.InfoType = CommonOutputType.Error;
                objRet.MessageInfo = exception.Message;
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
            }
            return objRet;
        }


        public static FileManagerCommonOutput FakturPajakPenampungExportToExcel(string noFaktur1, string noFaktur2, string npwp,
            string nama, string tglStart, string tglEnd,
            string status, string scanDateAwal, string scanDateAkhir, string source, string remark,
            string fNpwpPenjual, string fNamaPenjual,
            string fNoFaktur, string fTglFaktur, string fDppString, string fPpnString,
            string fPpnBmString, string fStatusFaktur, string fUserName, string fSource, string fStatusPayment, string fRemarks, string fpIds)
        {
            var objRet = new FileManagerCommonOutput()
            {
                InfoType = CommonOutputType.Success,
                IdNo = string.Empty,
                MessageInfo = "",
                FilePath = ""
            };
            try
            {
                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);
                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var path = FakturPajakPenampungCreateExcel(noFaktur1, noFaktur2, npwp,
                                nama, tglStart, tglEnd,
                                status, scanDateAwal, scanDateAkhir, source, remark,
                                fNpwpPenjual, fNamaPenjual,
                                fNoFaktur, fTglFaktur, fDppString, fPpnString,
                                fPpnBmString, fStatusFaktur, fUserName, fSource, fStatusPayment, fRemarks, fpIds);

                    if (!string.IsNullOrEmpty(path))
                    {
                        objRet.FilePath = path;
                    }
                    else
                    {
                        objRet.InfoType = CommonOutputType.Error;
                        objRet.MessageInfo = "Export Excel Failed with Unknown Error.";
                    }
                }
            }
            catch (Exception exception)
            {
                objRet.InfoType = CommonOutputType.Error;
                objRet.MessageInfo = exception.Message;
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
            }
            return objRet;
        }

        public static FileManagerCommonOutput FakturPajakDigantiOutstandingExportToExcel(string noFaktur1, string noFaktur2, string npwp,
            string nama, string tglStart, string tglEnd, string status, string fNpwpPenjual, string fNamaPenjual,
            string fNoFaktur, string fTglFaktur, string fMasaPajakName, string fTahunPajak, string fDppString, string fPpnString, string fPpnBmString,
            string fStatusFaktur, string dataType, string scanDateAwal, string scanDateAkhir, string fillingIndex, string userInitial, string fFillingIndex, string fUserName)
        {
            var objRet = new FileManagerCommonOutput()
            {
                InfoType = CommonOutputType.Success,
                IdNo = string.Empty,
                MessageInfo = "",
                FilePath = ""
            };
            try
            {
                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);
                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var path = FakturPajakDigantiOutstandingCreateExcel(noFaktur1, noFaktur2, npwp,
                            nama, tglStart, tglEnd, status, fNpwpPenjual, fNamaPenjual,
                            fNoFaktur, fTglFaktur, fMasaPajakName, fTahunPajak, fDppString, fPpnString, fPpnBmString,
                            fStatusFaktur, dataType, scanDateAwal, scanDateAkhir, fillingIndex, userInitial, fFillingIndex, fUserName);

                    if (!string.IsNullOrEmpty(path))
                    {
                        objRet.FilePath = path;
                    }
                    else
                    {
                        objRet.InfoType = CommonOutputType.Error;
                        objRet.MessageInfo = "Export Excel Failed with Unknown Error.";
                    }
                }
            }
            catch (Exception exception)
            {
                objRet.InfoType = CommonOutputType.Error;
                objRet.MessageInfo = exception.Message;
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
            }
            return objRet;
        }
        public static FileManagerCommonOutput ListOrdnerExportToExcel(string search, string noFaktur1, string noFaktur2,
            DateTime? tglFakturStart, DateTime? tglFakturEnd, int? masaPajak, int? tahunPajak, string picEntry, string userInitial)
        {
            var objRet = new FileManagerCommonOutput()
            {
                InfoType = CommonOutputType.Success,
                IdNo = string.Empty,
                MessageInfo = "",
                FilePath = ""
            };
            try
            {

                var dats = Ordners.GetListWithoutPaging(search, noFaktur1, noFaktur2, tglFakturStart, tglFakturEnd,
                    masaPajak, tahunPajak, picEntry);

                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);
                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var path = ListOrdnerCreateExcel(dats, userInitial);

                    if (!string.IsNullOrEmpty(path))
                    {
                        objRet.FilePath = path;
                    }
                    else
                    {
                        objRet.InfoType = CommonOutputType.Error;
                        objRet.MessageInfo = "Export Excel Failed with Unknown Error.";
                    }
                }
            }
            catch (Exception exception)
            {
                objRet.InfoType = CommonOutputType.Error;
                objRet.MessageInfo = exception.Message;
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
            }
            return objRet;
        }

        public static FileManagerCommonOutput ReportSpmCreateExcel(int masaPajak, int tahunPajak, int versi, string userInitial)
        {
            var objRet = new FileManagerCommonOutput()
            {
                InfoType = CommonOutputType.Success,
                IdNo = string.Empty,
                MessageInfo = "",
                FilePath = ""
            };
            try
            {

                var header = ReportSuratPemberitahuanMasas.GetForDownloadExcel(masaPajak, tahunPajak, versi);

                if (header == null || header.Count <= 0)
                {
                    throw new Exception("No Data");
                }

                var details = ReportSuratPemberitahuanMasaDetails.GetForDownloadExcel(masaPajak, tahunPajak, versi);

                if (details == null || details.Count <= 0)
                {
                    throw new Exception("No Data");
                }

                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);
                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var path = ReportSuratPemberitahuanMasaCreateExcel(header, details, userInitial);

                    if (!string.IsNullOrEmpty(path))
                    {
                        objRet.FilePath = path;
                    }
                    else
                    {
                        objRet.InfoType = CommonOutputType.Error;
                        objRet.MessageInfo = "Export Excel Failed with Unknown Error.";
                    }
                }
            }
            catch (Exception exception)
            {
                objRet.InfoType = CommonOutputType.Error;
                objRet.MessageInfo = exception.Message;
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
            }
            return objRet;
        }

        #region ------------- Report ---------------

        public static FileManagerCommonOutput ReportDetailFakturPajakToExcel(string noFaktur1, string noFaktur2, string search, string userInitial, string npwp,
            string nama,  DateTime? tglStart, DateTime? tglEnd, int? masaPajak, int? tahunPajak,  DateTime? scanDateAwal,  DateTime? scanDateAkhir)
        {
            var objRet = new FileManagerCommonOutput()
            {
                InfoType = CommonOutputType.Success,
                IdNo = string.Empty,
                MessageInfo = "",
                FilePath = ""
            };
            try
            {
                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var dats = ReportDetailFakturPajaks.GetListWithouPaging(search, noFaktur1, noFaktur2, npwp,
                            nama, tglStart, tglEnd, masaPajak, tahunPajak, scanDateAwal, scanDateAkhir);

                //if (dats.Count <= 0)
                //{
                //    return new FileManagerCommonOutput()
                //    {
                //        InfoType = CommonOutputType.Error,
                //        MessageInfo = "No Data Found",
                //        FilePath = "",
                //        IdNo = string.Empty
                //    };
                //}

                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var path = ReportDetailFakturPajakCreateExcel(dats, userInitial);

                    if (!string.IsNullOrEmpty(path))
                    {
                        objRet.FilePath = path;
                    }
                    else
                    {
                        objRet.InfoType = CommonOutputType.Error;
                        objRet.MessageInfo = "Export Excel Failed with Unknown Error.";
                    }
                }
            }
            catch (Exception exception)
            {
                objRet.InfoType = CommonOutputType.Error;
                objRet.MessageInfo = exception.Message;
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
            }
            return objRet;
        }

        public static FileManagerCommonOutput ReportFakturPajakMasukanToExcel(string sTglFakturStart, string sTglFakturEnd, string picEntry, string search, string userInitial, string fillingIndexStart, string fillingIndexEnd,
            int? masaPajak, int? tahunPajak)
        {
            var objRet = new FileManagerCommonOutput()
            {
                InfoType = CommonOutputType.Success,
                IdNo = string.Empty,
                MessageInfo = "",
                FilePath = ""
            };
            try
            {
                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                DateTime? dTglFakturStart = string.IsNullOrEmpty(sTglFakturStart)
                ? (DateTime?)null
                : Convert.ToDateTime(sTglFakturStart);

                DateTime? dTglFakturEnd = string.IsNullOrEmpty(sTglFakturEnd)
                    ? (DateTime?)null
                    : Convert.ToDateTime(sTglFakturEnd);

                var dats = ReportFakturPajakMasukans.GetListWithouPaging(search, dTglFakturStart, dTglFakturEnd,
                    picEntry, fillingIndexStart, fillingIndexEnd, masaPajak, tahunPajak);

                //if (dats.Count <= 0)
                //{
                //    return new FileManagerCommonOutput()
                //    {
                //        InfoType = CommonOutputType.Error,
                //        MessageInfo = "No Data Found",
                //        FilePath = "",
                //        IdNo = string.Empty
                //    };
                //}

                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var path = ReportFakturPajakMasukanCreateExcel(dats, userInitial);

                    if (!string.IsNullOrEmpty(path))
                    {
                        objRet.FilePath = path;
                    }
                    else
                    {
                        objRet.InfoType = CommonOutputType.Error;
                        objRet.MessageInfo = "Export Excel Failed with Unknown Error.";
                    }
                }

            }
            catch (Exception exception)
            {
                objRet.InfoType = CommonOutputType.Error;
                objRet.MessageInfo = exception.Message;
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
            }
            return objRet;
        }


        public static FileManagerCommonOutput ReportFakturPajakOutstandingToExcel(string sPostingDateStart, string sPostingDateEnd, string docSapStart, string docSapEnd, string search, string userInitial)
        {
            var objRet = new FileManagerCommonOutput()
            {
                InfoType = CommonOutputType.Success,
                IdNo = string.Empty,
                MessageInfo = "",
                FilePath = ""
            };
            try
            {
                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                DateTime? dPostingDateStart = string.IsNullOrEmpty(sPostingDateStart)
                ? (DateTime?)null
                : Convert.ToDateTime(sPostingDateStart);

                DateTime? dPostingDateEnd = string.IsNullOrEmpty(sPostingDateEnd)
                    ? (DateTime?)null
                    : Convert.ToDateTime(sPostingDateEnd);

                var dats = ReportFakturPajakOutstandings.GetListWithouPaging(search, dPostingDateStart, dPostingDateEnd,
                    docSapStart, docSapEnd);

                //if (dats.Count <= 0)
                //{
                //    return new FileManagerCommonOutput()
                //    {
                //        InfoType = CommonOutputType.Error,
                //        MessageInfo = "No Data Found",
                //        FilePath = "",
                //        IdNo = string.Empty
                //    };
                //}
                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var path = ReportFakturPajakOutstandingCreateExcel(dats, userInitial);

                    if (!string.IsNullOrEmpty(path))
                    {
                        objRet.FilePath = path;
                    }
                    else
                    {
                        objRet.InfoType = CommonOutputType.Error;
                        objRet.MessageInfo = "Export Excel Failed with Unknown Error.";
                    }
                }
            }
            catch (Exception exception)
            {
                objRet.InfoType = CommonOutputType.Error;
                objRet.MessageInfo = exception.Message;
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
            }
            return objRet;
        }

        public static FileManagerCommonOutput ReportFakturPajakBelumDiJurnal(string search, string tglFakturStart, string tglFakturEnd, string noFakturStart, string noFakturEnd, string userInitial)
        {
            var objRet = new FileManagerCommonOutput()
            {
                InfoType = CommonOutputType.Success,
                IdNo = string.Empty,
                MessageInfo = "",
                FilePath = ""
            };
            try
            {
                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                DateTime? dTglFakturStart = string.IsNullOrEmpty(tglFakturStart)
                ? (DateTime?)null
                : Convert.ToDateTime(tglFakturStart);

                DateTime? dTglFakturEnd = string.IsNullOrEmpty(tglFakturEnd)
                    ? (DateTime?)null
                    : Convert.ToDateTime(tglFakturEnd);

                var dats = ReportFakturPajakBelumDiJurnals.GetListWithoutPaging(search, dTglFakturStart, dTglFakturEnd,
                    noFakturStart, noFakturEnd);
                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var path = ReportFakturPajakBelumDiJurnalCreateExcel(dats, userInitial);

                    if (!string.IsNullOrEmpty(path))
                    {
                        objRet.FilePath = path;
                    }
                    else
                    {
                        objRet.InfoType = CommonOutputType.Error;
                        objRet.MessageInfo = "Export Excel Failed with Unknown Error.";
                    }

                }

            }
            catch (Exception exception)
            {
                objRet.InfoType = CommonOutputType.Error;
                objRet.MessageInfo = exception.Message;
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
            }
            return objRet;
        }

        #endregion

        #region -------------- Private Methods ---------------------

        private static string CreateXlsFile(SLDocument slDocument, int iEndHeaderRow, string pathFolder, string prefixFileName,
            string userInitial, int endColumnIndex, int endRowIndex)
        {
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Alignment.Vertical = VerticalAlignmentValues.Center;

            //set header style
            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            //set border to value cell
            slDocument.SetCellStyle(iEndHeaderRow + 1, 1, endRowIndex, endColumnIndex - 1, valueStyle);

            //set header style
            slDocument.SetCellStyle(1, 1, iEndHeaderRow, endColumnIndex - 1, headerStyle);

            //set auto fit to all column
            slDocument.AutoFitColumn(1, endColumnIndex - 1);

            var fileName = prefixFileName + "_" + userInitial + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

            var rootExportPath = string.Format(@"{0}\Export", FileManagerConfiguration.RepoRootPath);

            if (!Directory.Exists(rootExportPath))
            {
                Directory.CreateDirectory(rootExportPath);
            }

            var destFolder = string.Format(@"{0}\{1}", rootExportPath, pathFolder);
            if (!Directory.Exists(destFolder))
            {
                Directory.CreateDirectory(destFolder);
            }
            var destPath = Path.Combine(FileManagerConfiguration.RepoRootPath, destFolder, fileName);
            //var path = Path.Combine(Server.MapPath(pathFolder), fileName);

            //var outpu = new 
            slDocument.SaveAs(destPath);

            return destPath;
        }

        private static string CreateXlsReportSpmFile(SLDocument slDocument, string userInitial, string pathFolder, string prefixFileName, int iEndRowIndex, int iEndColumnIndex)
        {
            //Style for Report Header
            SLStyle reportHeaderStyle = slDocument.CreateStyle();
            reportHeaderStyle.Alignment.Vertical = VerticalAlignmentValues.Center;
            reportHeaderStyle.Alignment.Horizontal = HorizontalAlignmentValues.Left;
            reportHeaderStyle.Font.Bold = true;

            //Style for Table Header
            SLStyle tableHeaderStyle = slDocument.CreateStyle();
            tableHeaderStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            tableHeaderStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            tableHeaderStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            tableHeaderStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            tableHeaderStyle.Alignment.Vertical = VerticalAlignmentValues.Center;
            tableHeaderStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            tableHeaderStyle.Font.Bold = true;

            //Style for Value Style (Row Data in Table)
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Alignment.Vertical = VerticalAlignmentValues.Center;
            valueStyle.Alignment.Horizontal = HorizontalAlignmentValues.Left;
            valueStyle.Font.Bold = false;

            //set style report header
            slDocument.SetCellStyle(1, 1, 4, iEndColumnIndex - 1, reportHeaderStyle);

            //set style table header
            slDocument.SetCellStyle(5, 1, 6, iEndColumnIndex - 1, tableHeaderStyle);

            //set style row data
            slDocument.SetCellStyle(7, 1, iEndRowIndex - 1, iEndColumnIndex - 1, valueStyle);

            //set auto fit to all column
            slDocument.AutoFitColumn(1, iEndColumnIndex - 1, 50);

            var fileName = prefixFileName + "_" + userInitial + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

            var rootExportPath = string.Format(@"{0}\Export", FileManagerConfiguration.RepoRootPath);

            if (!Directory.Exists(rootExportPath))
            {
                Directory.CreateDirectory(rootExportPath);
            }

            var destFolder = string.Format(@"{0}\{1}", rootExportPath, pathFolder);
            if (!Directory.Exists(destFolder))
            {
                Directory.CreateDirectory(destFolder);
            }
            var destPath = Path.Combine(FileManagerConfiguration.RepoRootPath, destFolder, fileName);
            //var path = Path.Combine(Server.MapPath(pathFolder), fileName);

            //var outpu = new 
            slDocument.SaveAs(destPath);

            return destPath;
        }

        #region ------------------ Compare Evis vs Iws -------------

        private static string EvisVsIwsCreateExcel(string receivedDateStart, string receivedDateEnd, string statusId, string userInitial, string scanUserName, DateTime? scanDate)
        {
            var slDocument = new SLDocument();
            int endColumnIndex;

            //create header
            slDocument = CreateHeaderEvisVsIwsExcel(slDocument, out endColumnIndex);

            var iRow = 3; //starting row data
            int? iStatusIdInt = null;
            if (!(string.IsNullOrEmpty(statusId) || statusId == "0"))
            {
                iStatusIdInt = Convert.ToInt32(statusId);
            }
            var dats = CompEvisIwss.GetByReceivingDateWithoutPaging("VendorCode", true, Convert.ToDateTime(receivedDateStart), Convert.ToDateTime(receivedDateEnd),
                iStatusIdInt, scanUserName, scanDate);

            //Create Row Data Excel
            if (dats.Count > 0)
            {
                foreach (var item in dats)
                {
                    var iColumn = 1;
                    slDocument.SetCellValue(iRow, iColumn, item.ReceivedDateString);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.VendorCode);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.VendorName);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.ScanDateString);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.TaxInvoiceNumberEvis);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.TaxInvoiceNumberIws);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.InvoiceNumber);
                    iColumn++;

                    if (item.VatAmountScanned.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.VatAmountScanned.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    iColumn++;

                    if (item.VatAmountIws.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.VatAmountIws.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    iColumn++;

                    if (item.VatAmountDiff.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.VatAmountDiff.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }

                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.StatusDjp);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.StatusCompare);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.Notes);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.ScanUserName);

                    iRow++;
                }
            }

            var path = CreateXlsFile(slDocument, 2, @"Excel", "comp_evis_iws", userInitial, endColumnIndex, iRow - 1);

            return path;
        }

        private static SLDocument CreateHeaderEvisVsIwsExcel(SLDocument slDocument, out int endColumnIndex)
        {
            int iColumn = 1;
            //first row
            slDocument.SetCellValue(1, iColumn, "Received Date (IWS)");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Vendor Code");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Vendor Name");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Scan Date");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "FP Number EVIS");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "FP Number IWS");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Invoice Number");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "VAT Amount");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn + 2);//No Rowspan, Colspan = 3
            iColumn = iColumn + 3;

            slDocument.SetCellValue(1, iColumn, "Status DJP");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Status Compare");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Notes");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "User Name");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            endColumnIndex = iColumn;

            //second row
            iColumn = 8;
            slDocument.SetCellValue(2, iColumn, "EVIS");
            iColumn++;

            slDocument.SetCellValue(2, iColumn, "IWS");
            iColumn++;

            slDocument.SetCellValue(2, iColumn, "Diff");

            return slDocument;
        }

        #endregion

        #region ----------------- Daftar Faktur Pajak Masukan ------------

        private static string FakturPajakMasukanCreateExcel(string noFaktur1, string noFaktur2, string npwp,
            string nama, string tglStart, string tglEnd, string masaPajak, string tahunPajak, string status, string fNpwpPenjual, string fNamaPenjual,
            string fNoFaktur, string fTglFaktur, string fMasaPajakName, string fTahunPajak, string fDppString, string fPpnString, string fPpnBmString,
            string fStatusFaktur, string dataType,
            string scanDateAwal, string scanDateAkhir, string fillingIndex, string userInitial, string fFillingIndex, string fUserName,
            string source, string statusPayment, string remark, bool? createdCsv, string fSource, string fStatusPayment, string fRemark, string fCreatedCsv, string StatusPelaporan, string fNamaPelaporan)
        {
            var slDocument = new SLDocument();
            int endColumnIndex;

            //create header
            slDocument = CreateHeaderFakturPajakMasukan(slDocument, dataType, out endColumnIndex);

            var iRow = 2; //starting row data
            int totalItems;

            DateTime? tglFakturStart = !string.IsNullOrEmpty(tglStart) ? Convert.ToDateTime(tglStart) : (DateTime?)null;
            DateTime? tglFakturEnd = !string.IsNullOrEmpty(tglEnd) ? Convert.ToDateTime(tglEnd) : (DateTime?)null;

            int? imasaPajak = string.IsNullOrEmpty(masaPajak) || masaPajak == "0" || masaPajak == "undefined" ? (int?)null : int.Parse(masaPajak);
            int? itahunPajak = string.IsNullOrEmpty(tahunPajak) || tahunPajak == "0" || tahunPajak == "undefined" ? (int?)null : int.Parse(tahunPajak);

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

            if (!string.IsNullOrEmpty(fillingIndex) && fillingIndex != "undefined")
            {
                ifillingIndex = int.Parse(fillingIndex);
            }

            if (!string.IsNullOrEmpty(status) && status == "undefined")
            {
                status = "";
            }
            var dats = FakturPajaks.GetListToDownloadExcel(out totalItems, noFaktur1, noFaktur2,
                tglFakturStart, tglFakturEnd, npwp, nama, imasaPajak, itahunPajak
                , status, fNpwpPenjual, fNamaPenjual, fNoFaktur, fTglFaktur, fMasaPajakName, fTahunPajak, fDppString,
                fPpnString, fPpnBmString, fStatusFaktur, idataType, dscanDateAwal, dscanDateAkhir, ifillingIndex, fFillingIndex, fUserName,
                source, statusPayment, remark, createdCsv, fSource, fStatusPayment, fRemark, fCreatedCsv,StatusPelaporan,fNamaPelaporan);

            //Create Row Data Excel
            if (dats.Count > 0)
            {
                foreach (var item in dats)
                {
                    var iColumn = 1;
                    slDocument.SetCellValue(iRow, iColumn, item.FormatedNpwpPenjual);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.NamaPenjual);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.FormatedNoFaktur);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.TglFakturString);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.MasaPajakName);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.TahunPajak.Value);
                    iColumn++;

                    if (item.JumlahDPP.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.JumlahDPP.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    iColumn++;

                    if (item.JumlahPPN.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.JumlahPPN.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    iColumn++;

                    if (item.JumlahPPNBM.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.JumlahPPNBM.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }

                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.StatusFaktur);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, "");

                    iRow++;
                }
            }

            var path = CreateXlsFile(slDocument, 1, @"Excel", "dl_fp_masukan", userInitial, endColumnIndex, iRow - 1);

            return path;
        }

        private static SLDocument CreateHeaderFakturPajakMasukan(SLDocument slDocument,string dataType , out int endColumnIndex)
        {
            int iColumn = 1;
            //first row
            slDocument.SetCellValue(1, iColumn, "NPWP Vendor");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Vendor Code");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "No Faktur");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Tanggal Faktur");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Masa Pajak (Bulan)");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Masa Pajak (Tahun)");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "DPP");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "PPN");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "PPnBM");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Status");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            if (dataType == "3")
            {
                slDocument.SetCellValue(1, iColumn, "FAVR");
                slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
                iColumn = iColumn + 1;
            }
            endColumnIndex = iColumn;

            return slDocument;
        }

        #endregion


        #region ----------------- Daftar Faktur Pajak Outstanding ------------

        private static string FakturPajakOutstandingCreateExcel(string noFaktur1, string noFaktur2,
          string npwp, string nama, string tglStart, string tglEnd,
          string status, string scanDateAwal, string scanDateAkhir,
          string source, string StatusFaktur, string ByPass, string remark,
          string fNpwpPenjual, string fNamaPenjual,
          string fNoFaktur, string fTglFaktur,
          string fDppString, string fPpnString,
          string fPpnBmString, string fStatusFaktur,
          string fSource, string fStatusPayment, string fRemark, string fTglFaktur010, string userInitial, string ReceivedStart, string ReceivedEnd)
        {
            var slDocument = new SLDocument();
            int endColumnIndex;

            //create header
            slDocument = CreateHeaderFakturPajakOutStanding(slDocument, out endColumnIndex);

            var iRow = 2; //starting row data
            int totalItems;

            DateTime? tglFakturStart = !string.IsNullOrEmpty(tglStart) ? Convert.ToDateTime(tglStart) : (DateTime?)null;
            DateTime? tglFakturEnd = !string.IsNullOrEmpty(tglEnd) ? Convert.ToDateTime(tglEnd) : (DateTime?)null;

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


            if (!string.IsNullOrEmpty(ReceivedStart) && ReceivedStart != "undefined")
            {
                dReceivedStart = Convert.ToDateTime(ReceivedStart);
            }

            if (!string.IsNullOrEmpty(ReceivedEnd) && ReceivedEnd != "undefined")
            {
                dReceivedEnd = Convert.ToDateTime(ReceivedEnd);
            }



            if (!string.IsNullOrEmpty(status) && status == "undefined")
            {
                status = "";
            }

            var dats = FakturPajakOutstandings.GetListToDownloadExcel(out totalItems, noFaktur1, noFaktur2, npwp, nama,
            tglFakturStart,  tglFakturEnd, status,  dscanDateAwal,  dscanDateAkhir,  dReceivedStart,  dReceivedEnd, source, StatusFaktur, remark, ByPass,
            fNpwpPenjual, fNamaPenjual, fNoFaktur, fTglFaktur,
            fDppString, fPpnString, fPpnBmString, fStatusFaktur, fSource, fStatusPayment, fRemark, fTglFaktur010);

            //Create Row Data Excel
            if (dats.Count > 0)
            {
                foreach (var item in dats)
                {
                    var iColumn = 1;
                    slDocument.SetCellValue(iRow, iColumn, item.FormatedNpwpPenjual);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.NamaPenjual);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.FormatedNoFaktur);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.TglFakturString);
                    iColumn++;
                                 

                    if (item.JumlahDPP.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.JumlahDPP.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    iColumn++;

                    if (item.JumlahPPN.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.JumlahPPN.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    iColumn++;

                    if (item.JumlahPPNBM.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.JumlahPPNBM.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }

                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.StatusFaktur);
                    iColumn++;


                    slDocument.SetCellValue(iRow, iColumn, item.Source);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.StatusPayment);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.Remark);
                    iColumn++;


                    slDocument.SetCellValue(iRow, iColumn, item.TglFakturString010);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, "");

                    iRow++;
                }
            }

            var path = CreateXlsFile(slDocument, 1, @"Excel", "dl_fp_outstanding", userInitial, endColumnIndex, iRow - 1);

            return path;
        }

        private static SLDocument CreateHeaderFakturPajakOutStanding(SLDocument slDocument,  out int endColumnIndex)
        {
            int iColumn = 1;
            //first row
            slDocument.SetCellValue(1, iColumn, "NPWP Vendor");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Vendor Code");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "No Faktur");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Tanggal Faktur");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "DPP");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "PPN");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "PPnBM");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Status");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Source");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Status Payment");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Remark");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Tanggal Faktur 010");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;


            endColumnIndex = iColumn;

            return slDocument;
        }

        #endregion-


        #region ------------------ Faktur Pajak Penampung ---------------
        private static string FakturPajakPenampungCreateExcel(string noFaktur1, string noFaktur2, string npwp,
            string nama, string tglStart, string tglEnd,
            string status, string scanDateAwal, string scanDateAkhir, string source, string remark,
            string fNpwpPenjual, string fNamaPenjual,
            string fNoFaktur, string fTglFaktur, string fDppString, string fPpnString,
            string fPpnBmString, string fStatusFaktur, string fUserName, string fSource, string fStatusPayment, string fRemarks, string fpIds)
        {
            var slDocument = new SLDocument();
            int endColumnIndex;

            //create header
            slDocument = CreateHeaderFakturPajakPenampung(slDocument, out endColumnIndex);

            var iRow = 2; //starting row data
            int totalItems;

            DateTime? tglFakturStart = !string.IsNullOrEmpty(tglStart) ? Convert.ToDateTime(tglStart) : (DateTime?)null;
            DateTime? tglFakturEnd = !string.IsNullOrEmpty(tglEnd) ? Convert.ToDateTime(tglEnd) : (DateTime?)null;

            //int? idataType = null;
            DateTime? dscanDateAwal = null;
            DateTime? dscanDateAkhir = null;
            //int? ifillingIndex = null;
            
            if (!string.IsNullOrEmpty(scanDateAwal) && scanDateAwal != "undefined")
            {
                dscanDateAwal = Convert.ToDateTime(scanDateAwal);
            }

            if (!string.IsNullOrEmpty(scanDateAkhir) && scanDateAkhir != "undefined")
            {
                dscanDateAkhir = Convert.ToDateTime(scanDateAkhir);
            }

            if (!string.IsNullOrEmpty(status) && status == "undefined")
            {
                status = "";
            }
            var dats = FakturPajakPenampungs.GetListToDownloadExcel(out totalItems, noFaktur1, noFaktur2, npwp, nama,
                tglFakturStart, tglFakturEnd, status, dscanDateAwal, dscanDateAkhir, source, remark, fNpwpPenjual, fNamaPenjual, fNoFaktur,
                fTglFaktur, fDppString, fPpnString, fPpnBmString, fStatusFaktur, fUserName, fSource, fStatusPayment, fRemarks, fpIds);

            //Create Row Data Excel
            if (dats.Count > 0)
            {
                foreach (var item in dats)
                {
                    var iColumn = 1;
                    slDocument.SetCellValue(iRow, iColumn, item.FormatedNpwpPenjual);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.NamaPenjual);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.FormatedNoFaktur);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.TglFakturString);
                    iColumn++;

                    if (item.JumlahDPP.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.JumlahDPP.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    iColumn++;

                    if (item.JumlahPPN.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.JumlahPPN.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    iColumn++;

                    if (item.JumlahPPNBM.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.JumlahPPNBM.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.StatusFaktur);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.CreatedBy);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.Source);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.StatusPayment);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.Remark);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, "");

                    iRow++;
                }
            }

            var path = CreateXlsFile(slDocument, 1, @"Excel", "dl_fp_masukan", fUserName, endColumnIndex, iRow - 1);

            return path;
        }

        private static SLDocument CreateHeaderFakturPajakPenampung(SLDocument slDocument, out int endColumnIndex)
        {
            int iColumn = 1;
            //first row
            slDocument.SetCellValue(1, iColumn, "NPWP Vendor");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Vendor Code");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "No Faktur");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Tanggal Faktur");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "DPP");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "PPN");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "PPnBM");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Status");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "User");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Source");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Status Payment");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Remarks");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            endColumnIndex = iColumn;

            return slDocument;
        }
        #endregion

        #region ------------------------- Faktur Pajak Diganti Outstanding ----------------------------
        private static string FakturPajakDigantiOutstandingCreateExcel(string noFaktur1, string noFaktur2, string npwp,
            string nama, string tglStart, string tglEnd, string status, string fNpwpPenjual, string fNamaPenjual,
            string fNoFaktur, string fTglFaktur, string fMasaPajakName, string fTahunPajak, string fDppString, string fPpnString, string fPpnBmString,
            string fStatusFaktur, string dataType,
            string scanDateAwal, string scanDateAkhir, string fillingIndex, string userInitial, string fFillingIndex, string fUserName)
        {
            var slDocument = new SLDocument();
            int endColumnIndex;

            //create header
            slDocument = CreateHeaderFakturPajakDigantiOutstanding(slDocument, dataType, out endColumnIndex);

            var iRow = 2; //starting row data
            int totalItems;

            DateTime? tglFakturStart = !string.IsNullOrEmpty(tglStart) ? Convert.ToDateTime(tglStart) : (DateTime?)null;
            DateTime? tglFakturEnd = !string.IsNullOrEmpty(tglEnd) ? Convert.ToDateTime(tglEnd) : (DateTime?)null;

            int? idataType = null;
            DateTime? dscanDateAwal = null;
            DateTime? dscanDateAkhir = null;
            string ifillingIndex = null;
            int? iStatus = null;

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
                ifillingIndex = fillingIndex;
            }            
            if (status != "")
            {
                iStatus = Int32.Parse(status);
            }
            var dats = FakturPajakDigantiOutstandings.GetListWithoutPaging(out totalItems, noFaktur1, noFaktur2,
                tglFakturStart, tglFakturEnd, npwp, nama
                , iStatus, fNpwpPenjual, fNamaPenjual, fNoFaktur, fTglFaktur, fMasaPajakName, fTahunPajak, fDppString,
                fPpnString, fPpnBmString, fStatusFaktur, idataType, dscanDateAwal, dscanDateAkhir, ifillingIndex, fFillingIndex, fUserName);

            //Create Row Data Excel
            if (dats.Count > 0)
            {
                foreach (var item in dats)
                {
                    var iColumn = 1;
                    slDocument.SetCellValue(iRow, iColumn, item.FormatedNpwpPenjual);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.NamaPenjual);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.FormatedNoFaktur);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.TglFakturString);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.OriginMasaPajakName);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.OriginTahunPajak.HasValue ? item.OriginTahunPajak.Value.ToString() : "");
                    iColumn++;

                    if (item.JumlahDPP.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.JumlahDPP.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    iColumn++;

                    if (item.JumlahPPN.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.JumlahPPN.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    iColumn++;

                    if (item.JumlahPPNBM.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.JumlahPPNBM.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }

                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.StatusOutstandingName);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.Keterangan);

                    iRow++;
                }
            }

            var path = CreateXlsFile(slDocument, 1, @"Excel", "dl_fp_outstanding", userInitial, endColumnIndex, iRow - 1);

            return path;
        }

        private static SLDocument CreateHeaderFakturPajakDigantiOutstanding(SLDocument slDocument, string dataType, out int endColumnIndex)
        {
            int iColumn = 1;
            //first row
            slDocument.SetCellValue(1, iColumn, "NPWP Vendor");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Vendor Code");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "No Faktur");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Tanggal Faktur");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Masa Pajak (Bulan)");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Masa Pajak (Tahun)");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "DPP");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "PPN");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "PPnBM");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Status");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;
            
            slDocument.SetCellValue(1, iColumn, "Remark");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            endColumnIndex = iColumn;

            return slDocument;
        }

        #endregion
        #region ----------------- Compare Evis vs SAP ------------

        private static string CompEvisVsSapCreateExcel(List<CompEvisSap> dats, string userInitial)
        {
            var slDocument = new SLDocument();
            int endColumnIndex;

            //create header
            slDocument = CreateHeaderCompEvisVsSap(slDocument, out endColumnIndex);

            var iRow = 3; //starting row data

            //Create Row Data Excel
            if (dats.Count > 0)
            {
                foreach (var item in dats)
                {
                    var iColumn = 1;
                    slDocument.SetCellValue(iRow, iColumn, item.PostingDateString);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.AccountingDocNo);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.ItemNo);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.TglFakturString);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.NamaVendor);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.ScanDateString);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.TaxInvoiceNumberEvis);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.TaxInvoiceNumberSap);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.DocumentHeaderText);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.Npwp);
                    iColumn++;

                    if (item.AmountEvis.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.AmountEvis.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    
                    iColumn++;

                    if (item.AmountSap.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.AmountSap.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    
                    iColumn++;

                    if (item.AmountDiff.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.AmountDiff.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, ""); 
                    }
                    
                    iColumn++;
                    if (item.MasaPajak.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.MasaPajak.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    iColumn++;

                    if (item.TahunPajak.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.TahunPajak.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.StatusCompare);
                    iColumn++;

                    //slDocument.SetCellValue(iRow, iColumn, item.Notes);
                    //iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.UserNameCreator);

                    iRow++;
                }
            }

            var path = CreateXlsFile(slDocument, 1, @"Excel", "dl_comp_evis_sap", userInitial, endColumnIndex, iRow - 1);

            return path;
        }

        private static SLDocument CreateHeaderCompEvisVsSap(SLDocument slDocument, out int endColumnIndex)
        {
            int iColumn = 1;
            //first row
            slDocument.SetCellValue(1, iColumn, "Posting Date");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Acc Doc No");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Item No");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Tgl Faktur");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Nama Vendor");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Scan Date");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Tax Invoice Number EVIS");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Tax Invoice Number SAP");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Document Header Text");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "NPWP");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Amount");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn + 2);//No Rowspan, Colspan = 3
            iColumn = iColumn + 3;

            slDocument.SetCellValue(1, iColumn, "Masa Pajak");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Tahun Pajak");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Status Compare");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            //slDocument.SetCellValue(1, iColumn, "Notes");
            //slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            //iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Username");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//RowSpan = 2
            iColumn = iColumn + 1;

            endColumnIndex = iColumn;

            //second row
            iColumn = 11;
            slDocument.SetCellValue(2, iColumn, "EVIS");
            iColumn++;

            slDocument.SetCellValue(2, iColumn, "SAP");
            iColumn++;

            slDocument.SetCellValue(2, iColumn, "Diff");

            return slDocument;
        }

        #endregion

        #region --------------- Daftar Retur Faktur Pajak -----------
        
        private static string ReturFakturPajakCreateExcel(List<FakturPajakRetur> dats, string userInitial)
        {
            var slDocument = new SLDocument();
            int endColumnIndex;

            //create header
            slDocument = CreateHeaderReturFakturPajakToExcel(slDocument, out endColumnIndex);

            var iRow = 2; //starting row data

            //Create Row Data Excel
            if (dats.Count > 0)
            {
                foreach (var item in dats)
                {
                    var iColumn = 1;
                    slDocument.SetCellValue(iRow, iColumn, item.TglReturString);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.FormatedNpwpPenjual);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.NamaPenjual);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.NoFakturPajak);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.TglFakturString);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.NoDocRetur);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.MasaPajakLapor);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.TahunPajakLapor);
                    iColumn++;

                    if (item.JumlahDPP.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.JumlahDPP.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    
                    iColumn++;
                    if (item.JumlahPPN.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.JumlahPPN.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    
                    iColumn++;

                    if (item.JumlahPPNBM.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.JumlahPPNBM.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.CreatedBy);
                    
                    iRow++;
                }
            }

            var path = CreateXlsFile(slDocument, 1, @"Excel", "dl_retur_fp_masukan", userInitial, endColumnIndex, iRow - 1);

            return path;
        }

        private static SLDocument CreateHeaderReturFakturPajakToExcel(SLDocument slDocument, out int endColumnIndex)
        {
            int iColumn = 1;
            slDocument.SetCellValue(1, iColumn, "Tanggal Retur");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "NPWP Vendor");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Nama Vendor");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "No Faktur yang diretur");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Tanggal Faktur");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "No Retur");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Masa Retur (Bulan)");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Masa Retur (Tahun)");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "DPP");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "PPn");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "PPnBM");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "UserName");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            endColumnIndex = iColumn;

            return slDocument;
        }
        
        #endregion

        #region ----------------- Report Detail Faktur Pajak -------------
        
        private static string ReportDetailFakturPajakCreateExcel(List<ReportDetailFakturPajak> dats, string userInitial)
        {
            var slDocument = new SLDocument();
            int endColumnIndex;

            //create header
            slDocument = CreateHeaderReportDetailFakturPajakToExcel(slDocument, out endColumnIndex);

            var iRow = 2; //starting row data

            //Create Row Data Excel
            if (dats.Count > 0)
            {
                foreach (var item in dats)
                {
                    var iColumn = 1;
                    slDocument.SetCellValue(iRow, iColumn, item.VSequenceNumber);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.FormatedNoFaktur);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.Nama);
                    iColumn++;

                    if (item.HargaSatuan.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.HargaSatuan.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    
                    iColumn++;

                    if (item.Diskon.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.Diskon.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    
                    iColumn++;

                    if (item.JumlahDPP.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.JumlahDPP.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    
                    iColumn++;

                    if (item.JumlahPPN.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.JumlahPPN.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    
                    iColumn++;

                    if (item.JumlahPPNBM.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.JumlahPPNBM.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    
                    iColumn++;

                    if (item.Dpp.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.Dpp.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    
                    iColumn++;

                    if (item.Ppn.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.Ppn.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    
                    iColumn++;

                    if (item.Ppnbm.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.Ppnbm.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    
                    iColumn++;

                    if (item.TarifPpnbm.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.TarifPpnbm.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.FormatedNpwpPenjual);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.NamaPenjual);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.FillingIndex);

                    iRow++;
                }
            }

            var path = CreateXlsFile(slDocument, 1, @"Excel", "dl_rpt_detailfp", userInitial, endColumnIndex, iRow - 1);

            return path;
        }

        private static SLDocument CreateHeaderReportDetailFakturPajakToExcel(SLDocument slDocument, out int endColumnIndex)
        {
            int iColumn = 1;

            slDocument.SetCellValue(1, iColumn, "No");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;
            slDocument.SetCellValue(1, iColumn, "Nomor FP");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Nama Barang / Jasa");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Harga Satuan");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Discount");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "DPP (Header)");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "PPN (Header)");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "PPnBM (Header)");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "DPP (Detail)");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "PPN (Detail)");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "PPnBM (Detail)");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Tarif PPnBM (Detail)");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "NPWP Penjual");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Nama Penjual");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Filling Index");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            endColumnIndex = iColumn;

            return slDocument;
        }
        
        #endregion

        #region ----------------- Report Faktur Pajak Masukan -------------

        private static string ReportFakturPajakMasukanCreateExcel(List<ReportFakturPajakMasukan> dats, string userInitial)
        {
            var slDocument = new SLDocument();
            int endColumnIndex;

            //create header
            slDocument = CreateHeaderReportFakturPajakMasukanToExcel(slDocument, out endColumnIndex);

            var iRow = 2; //starting row data

            //Create Row Data Excel
            if (dats.Count > 0)
            {
                foreach (var item in dats)
                {
                    var iColumn = 1;
                    slDocument.SetCellValue(iRow, iColumn, item.VSequenceNumber);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.NamaPenjual);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.FormatedNpwpPenjual);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.FormatedNoFaktur);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.TglFakturString);
                    iColumn++;

                    if (item.JumlahPPN.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.JumlahPPN.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.CreatedBy);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.MasaPajak);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.TahunPajak);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.FillingIndex);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.Dikreditkan.HasValue ? item.Dikreditkan.Value.ToString() : "");

                    iRow++;
                }
            }

            var path = CreateXlsFile(slDocument, 1, @"Excel", "dl_rpt_fp_masukan", userInitial, endColumnIndex, iRow - 1);

            return path;
        }

        private static SLDocument CreateHeaderReportFakturPajakMasukanToExcel(SLDocument slDocument, out int endColumnIndex)
        {
            int iColumn = 1;
            slDocument.SetCellValue(1, iColumn, "No");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Nama Penjual");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "NPWP Penjual");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "No. FP");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Tanggal FP");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "PPN");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "PIC Entry (Kode)");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Masa Pajak");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Tahun Pajak");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Filling Index");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Is Creditable");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;
            
            endColumnIndex = iColumn;

            return slDocument;
        }

        #endregion

        #region ----------------- Report Faktur Pajak Outstanding -------------

        private static string ReportFakturPajakOutstandingCreateExcel(List<ReportFakturPajakOutstanding> dats, string userInitial)
        {
            var slDocument = new SLDocument();
            int endColumnIndex;

            //create header
            slDocument = CreateHeaderReportFakturPajakOutstandingToExcel(slDocument, out endColumnIndex);

            var iRow = 3; //starting row data

            //Create Row Data Excel
            if (dats.Count > 0)
            {
                foreach (var item in dats)
                {
                    var iColumn = 1;
                    slDocument.SetCellValue(iRow, iColumn, item.VSequenceNumber);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.GLAccount);
                    iColumn++;

                    //slDocument.SetCellValue(iRow, iColumn, item.DocumentHeaderText);
                    //iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.AccountingDocNo);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.PostingDateString);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.AmountLocalString);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.TaxInvoiceNumber);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.TglFakturString);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.AssignmentNo);
                    //iColumn++;

                    //slDocument.SetCellValue(iRow, iColumn, item.UserName);
                    iRow++;
                }
            }

            var path = CreateXlsFile(slDocument, 1, @"Excel", "dl_rpt_fp_outstanding", userInitial, endColumnIndex, iRow - 1);

            return path;
        }

        private static SLDocument CreateHeaderReportFakturPajakOutstandingToExcel(SLDocument slDocument, out int endColumnIndex)
        {
            int iColumn = 1;
            slDocument.SetCellValue(1, iColumn, "No");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//rowspan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "GL");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn + 3);
            iColumn = iColumn + 4;

            slDocument.SetCellValue(1, iColumn, "Keterangan");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn + 2);

            iColumn = 2;
            slDocument.SetCellValue(2, iColumn, "GL Account");
            slDocument.MergeWorksheetCells(2, iColumn, 2, iColumn);
            iColumn = iColumn + 1;

            //slDocument.SetCellValue(2, iColumn, "Doc. Header Text");
            //slDocument.MergeWorksheetCells(2, iColumn, 2, iColumn);
            //iColumn = iColumn + 1;

            slDocument.SetCellValue(2, iColumn, "Doc. No");
            slDocument.MergeWorksheetCells(2, iColumn, 2, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(2, iColumn, "Posting Date");
            slDocument.MergeWorksheetCells(2, iColumn, 2, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(2, iColumn, "Amount in GL");
            slDocument.MergeWorksheetCells(2, iColumn, 2, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(2, iColumn, "FP No.");
            slDocument.MergeWorksheetCells(2, iColumn, 2, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(2, iColumn, "FP Date");
            slDocument.MergeWorksheetCells(2, iColumn, 2, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(2, iColumn, "Assignment");
            slDocument.MergeWorksheetCells(2, iColumn, 2, iColumn);
            iColumn = iColumn + 1;

            //slDocument.SetCellValue(2, iColumn, "Username");
            //slDocument.MergeWorksheetCells(2, iColumn, 2, iColumn);
            //iColumn = iColumn + 1;

            endColumnIndex = iColumn;

            return slDocument;
        }

        #endregion

        #region ----------------- Report Faktur Pajak Belum Di Jurnal -------------

        private static string ReportFakturPajakBelumDiJurnalCreateExcel(List<ReportFakturPajakBelumDiJurnal> dats, string userInitial)
        {
            var slDocument = new SLDocument();
            int endColumnIndex;

            //create header
            slDocument = CreateHeaderReportFakturPajakBelumDiJurnalToExcel(slDocument, out endColumnIndex);

            var iRow = 3; //starting row data

            //Create Row Data Excel
            if (dats.Count > 0)
            {
                foreach (var item in dats)
                {
                    var iColumn = 1;
                    slDocument.SetCellValue(iRow, iColumn, item.VSequenceNumber);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.FormatedNpwpPenjual);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.NamaPenjual);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, (item.MasaPajak + "-" + item.TahunPajak));
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.FormatedNoFaktur);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.CreatedBy);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.FillingIndex);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.TglFakturString);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, (item.MasaPajakSpm.HasValue && item.TahunPajakSpm.HasValue ? item.MasaPajakSpm.Value + " - " + item.TahunPajakSpm.Value : "-"));
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, (string.IsNullOrEmpty(item.FpNoSpm) ? "-" : item.FpNoSpm));
                    iColumn++;

                    if (item.JumlahPPN.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.JumlahPPN.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    

                    iRow++;
                }
            }

            var path = CreateXlsFile(slDocument, 1, @"Excel", "dl_rpt_fp_belumdijurnal", userInitial, endColumnIndex, iRow - 1);

            return path;
        }

        private static SLDocument CreateHeaderReportFakturPajakBelumDiJurnalToExcel(SLDocument slDocument, out int endColumnIndex)
        {
            int iColumn = 1;
            slDocument.SetCellValue(1, iColumn, "No");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//rowspan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "NPWP Penjual");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//rowspan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Nama Penjual");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//rowspan = 2
            iColumn = iColumn + 1;
            
            slDocument.SetCellValue(1, iColumn, "Tax Period");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//rowspan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "FP No.");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//rowspan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "PIC Entry");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//rowspan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Filling Index");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//rowspan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "FP Date");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//rowspan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Surat Pemberitahuan Masa");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn + 1);//colspan = 2
            iColumn = iColumn + 2;

            slDocument.SetCellValue(1, iColumn, "Amount in FP");
            slDocument.MergeWorksheetCells(1, iColumn, 2, iColumn);//rowspan = 2
            endColumnIndex = iColumn + 1;
            iColumn = iColumn - 2;

            slDocument.SetCellValue(2, iColumn, "Tax Period");
            slDocument.MergeWorksheetCells(2, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(2, iColumn, "FP No.");
            slDocument.MergeWorksheetCells(2, iColumn, 1, iColumn);

            return slDocument;
        }

        #endregion
        
        #region ----------------------- Report Surat Pemberitahuan Masa -------------------

        private static string ReportSuratPemberitahuanMasaCreateExcel(List<ReportSuratPemberitahuanMasa> header, List<ReportSuratPemberitahuanMasaDetail> details, string userInitial)
        {
            var slDocument = new SLDocument();
            int endColumnIndex;
            int endHeaderRowIndex;
            var firstData = header.First();
            string taxPeriod = firstData.NamaMasaPajak + " " + firstData.TahunPajak;
            string version = string.Join(",", header.Select(d => d.Versi));
            
            //create header
            slDocument = CreateHeaderReportSuratPemberitahuanMasaExcel(slDocument, out endColumnIndex, version, taxPeriod, out endHeaderRowIndex);

            var iRow = endHeaderRowIndex + 1; //starting row data

            //Create Row Data Excel
            if (details.Count > 0)
            {
                foreach (var item in details)
                {
                    var iColumn = 1;
                    slDocument.SetCellValue(iRow, iColumn, item.VSequenceNumber);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.Versi);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.FCode);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.FormatedNoFaktur);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.MasaPajak.ToString("00"));
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.TahunPajak);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.TglFakturString);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.FormatedNpwpPenjual);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.NamaPenjual);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.AlamatPenjual);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.FormatedNpwpLawanTransaksi);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.NamaLawanTransaksi);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.AlamatLawanTransaksi);
                    iColumn++;

                    if (item.JumlahDPP.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.JumlahDPP.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    
                    iColumn++;

                    if (item.JumlahPPN.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.JumlahPPN.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    
                    iColumn++;

                    if (item.JumlahPPNBM.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.JumlahPPNBM.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.KeteranganTambahan);
                    iColumn++;

                    if (item.FgUangMuka.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.FgUangMuka.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    
                    iColumn++;

                    if (item.UangMukaDPP.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.UangMukaDPP.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    
                    iColumn++;

                    if (item.UangMukaPPN.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.UangMukaPPN.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    
                    iColumn++;

                    if (item.UangMukaPPnBM.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.UangMukaPPnBM.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.Referensi);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.FillingIndex);
                    
                    iRow++;
                }
            }

            var path = CreateXlsReportSpmFile(slDocument, userInitial, @"Export", "dl_rpt_spm", iRow, endColumnIndex);

            return path;
        }

        private static SLDocument CreateHeaderReportSuratPemberitahuanMasaExcel(SLDocument slDocument, out int endColumnIndex, string pembetulanKe, string taxPeriod, out int endHeaderRowIndex)
        {
            //CREATE HEADER
            slDocument.SetCellValue(1, 1, "PT Astra Daihatsu Motor");
            slDocument.MergeWorksheetCells(1, 1, 1, 3); //colspan = 2
            slDocument.SetCellValue(2, 1, "Summary TAX VAT IN");
            slDocument.MergeWorksheetCells(2, 1, 2, 3); //colspan = 2
            slDocument.SetCellValue(3, 1, "Pembetulan");
            slDocument.MergeWorksheetCells(3, 1, 3, 3); //colspan = 2
            slDocument.SetCellValue(4, 1, "Tax Period");
            slDocument.MergeWorksheetCells(4, 1, 4, 3); //colspan = 2
            slDocument.SetCellValue(3, 4, ": " + pembetulanKe);
            slDocument.SetCellValue(4, 4, ": " + taxPeriod);

            int iColumn = 1;
            int iRow = 5;
            
            slDocument.SetCellValue(iRow, iColumn, "No.");
            slDocument.MergeWorksheetCells(iRow, iColumn, (iRow + 1), iColumn); //rowspan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Versi SPM");
            slDocument.MergeWorksheetCells(iRow, iColumn, (iRow + 1), iColumn); //rowspan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Kode Jenis");
            slDocument.MergeWorksheetCells(iRow, iColumn, (iRow + 1), iColumn); //rowspan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Nomor FP");
            slDocument.MergeWorksheetCells(iRow, iColumn, (iRow + 1), iColumn); //rowspan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Masa Pajak");
            slDocument.MergeWorksheetCells(iRow, iColumn, (iRow + 1), iColumn); //rowspan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Tahun Pajak");
            slDocument.MergeWorksheetCells(iRow, iColumn, (iRow + 1), iColumn); //rowspan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Tanggal FP");
            slDocument.MergeWorksheetCells(iRow, iColumn, (iRow + 1), iColumn); //rowspan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Identitas Penjual");
            slDocument.MergeWorksheetCells(iRow, iColumn, iRow, (iColumn + 2)); //Colspan 3
            iColumn = iColumn + 3;

            slDocument.SetCellValue(iRow, iColumn, "Identitas Pembeli");
            slDocument.MergeWorksheetCells(iRow, iColumn, iRow, (iColumn + 2)); //Colspan = 3
            iColumn = iColumn + 3;

            slDocument.SetCellValue(iRow, iColumn, "Total DPP");
            slDocument.MergeWorksheetCells(iRow, iColumn, (iRow + 1), iColumn); //rowspan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Total PPN");
            slDocument.MergeWorksheetCells(iRow, iColumn, (iRow + 1), iColumn); //rowspan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Total PPnBM");
            slDocument.MergeWorksheetCells(iRow, iColumn, (iRow + 1), iColumn); //rowspan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Keterangan Tambahan");
            slDocument.MergeWorksheetCells(iRow, iColumn, (iRow + 1), iColumn); //rowspan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "FG Uang Muka");
            slDocument.MergeWorksheetCells(iRow, iColumn, (iRow + 1), iColumn); //rowspan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Uang Muka DPP");
            slDocument.MergeWorksheetCells(iRow, iColumn, (iRow + 1), iColumn); //rowspan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Uang Muka PPN");
            slDocument.MergeWorksheetCells(iRow, iColumn, (iRow + 1), iColumn); //rowspan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Uang Muka PPnBM");
            slDocument.MergeWorksheetCells(iRow, iColumn, (iRow + 1), iColumn); //rowspan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Referensi");
            slDocument.MergeWorksheetCells(iRow, iColumn, (iRow + 1), iColumn); //rowspan = 2
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Filling Index");
            slDocument.MergeWorksheetCells(iRow, iColumn, (iRow + 1), iColumn); //rowspan = 2
            iColumn = iColumn + 1;

            endColumnIndex = iColumn;
            iRow = iRow + 1;

            iColumn = 8;
            slDocument.SetCellValue(iRow, iColumn, "NPWP Penjual");
            slDocument.MergeWorksheetCells(iRow, iColumn, iRow, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Nama Penjual");
            slDocument.MergeWorksheetCells(iRow, iColumn, iRow, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Alamat Penjual");
            slDocument.MergeWorksheetCells(iRow, iColumn, iRow, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "NPWP Pembeli");
            slDocument.MergeWorksheetCells(iRow, iColumn, iRow, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Nama Pembeli");
            slDocument.MergeWorksheetCells(iRow, iColumn, iRow, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(iRow, iColumn, "Alamat Pembeli");
            slDocument.MergeWorksheetCells(iRow, iColumn, iRow, iColumn);

            endHeaderRowIndex = iRow;

            return slDocument;
        }


        #endregion

        #region ------------ List Ordner -----------


        private static string ListOrdnerCreateExcel(List<Ordner> dats, string userInitial)
        {
            var slDocument = new SLDocument();
            int endColumnIndex;

            //create header
            slDocument = CreateHeaderListOrdnerToExcel(slDocument, out endColumnIndex);

            var iRow = 2; //starting row data

            //Create Row Data Excel
            if (dats.Count > 0)
            {
                foreach (var item in dats)
                {
                    var iColumn = 1;
                    slDocument.SetCellValue(iRow, iColumn, item.FormatedNpwpPenjual);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.NamaPenjual);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.FormatedNoFaktur);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.TglFakturString);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.MasaPajak);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.TahunPajak);
                    iColumn++;

                    if (item.JumlahDPP.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.JumlahDPP.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    
                    iColumn++;

                    if (item.JumlahPPN.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.JumlahPPN.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    
                    iColumn++;
                    if (item.JumlahPPNBM.HasValue)
                    {
                        slDocument.SetCellValue(iRow, iColumn, item.JumlahPPNBM.Value);
                    }
                    else
                    {
                        slDocument.SetCellValue(iRow, iColumn, "");
                    }
                    
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.FillingIndex);
                    iColumn++;

                    slDocument.SetCellValue(iRow, iColumn, item.PrintCount);

                    iRow++;
                }
            }

            var path = CreateXlsFile(slDocument, 1, @"Excel", "dl_ordner", userInitial, endColumnIndex, iRow - 1);

            return path;
        }

        private static SLDocument CreateHeaderListOrdnerToExcel(SLDocument slDocument, out int endColumnIndex)
        {
            int iColumn = 1;
            slDocument.SetCellValue(1, iColumn, "NPWP Vendor");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Nama Vendor");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Nomor Faktur");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Tanggal Faktur");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Masa Pajak (Bulan)");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Masa Pajak (Tahun)");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "DPP");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "PPN");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "PPnBM");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Filling Index");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            slDocument.SetCellValue(1, iColumn, "Print Count");
            slDocument.MergeWorksheetCells(1, iColumn, 1, iColumn);
            iColumn = iColumn + 1;

            endColumnIndex = iColumn;

            return slDocument;
        }


        #endregion

        #endregion

        #region --------- Compare Evis vs SAP ---------------------

        public static FileManagerCommonOutput ExportCompEvisVsSapToExcel(List<CompEvisSap> dats, string userInitial)
        {
            var objRet = new FileManagerCommonOutput()
            {
                InfoType = CommonOutputType.Success,
                IdNo = string.Empty,
                MessageInfo = "",
                FilePath = ""
            };
            try
            {
                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);
                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var path = CompEvisVsSapCreateExcel(dats, userInitial);

                    if (!string.IsNullOrEmpty(path))
                    {
                        objRet.FilePath = path;
                    }
                    else
                    {
                        objRet.InfoType = CommonOutputType.Error;
                        objRet.MessageInfo = "Export Excel Failed with Unknown Error.";
                    }
                    
                }
            }
            catch (Exception exception)
            {
                objRet.InfoType = CommonOutputType.Error;
                objRet.MessageInfo = exception.Message;
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
            }
            return objRet;
        }

        public static FileManagerCommonOutput CompEvisVsSapCreateSubmitXml(string idNo, string RepoUser
            , string RepoPassword, string RepoRootPath, string DataFolderWatcherInboxService, string PrefixUploadPpnCredit)
        {
            var objRet = new FileManagerCommonOutput()
            {
                InfoType = CommonOutputType.Success,
                IdNo = string.Empty,
                MessageInfo = "",
                FilePath = ""
            };
            
            try
            {
                
                var dats = XmlUploadPpnCredits.ByIdNo(idNo);
                if (dats.Count <= 0)
                {
                    objRet.InfoType = CommonOutputType.Error;
                    objRet.MessageInfo = "Empty Data to Export";
                    return objRet;
                }

                //Process csv
                var imp = new ImpersonationHelper();
                imp.Impersonate(RepoUser, RepoPassword);

                var credential = new NetworkCredential(RepoUser, RepoPassword);
                using (new NetworkConnectionHelper(RepoRootPath, credential))
                {
                    var destFolder = string.Format(@"{0}", DataFolderWatcherInboxService);
                    if (!Directory.Exists(destFolder))
                    {
                        Directory.CreateDirectory(destFolder);
                    }

                    var dtNow = DateTime.Now;
                    var filename = string.Format("{0}_{1}_{2}.xml", PrefixUploadPpnCredit, dtNow.ToString("yyyyMMdd"), dtNow.ToString("HHmmss-fff"));
                    var destFilePath = string.Format(@"{0}\{1}", destFolder, filename);

                    //var totalAmount = dats.Sum(d => d.AmountSap.HasValue ? d.AmountSap.Value : 0);
                    var totalAmount = dats.Sum(d => d.AmountEvis.HasValue ? d.AmountEvis.Value : 0);

                    objRet.FilePath = destFilePath;
                    objRet.IdNo = idNo;

                    //Create Xml File
                    using (XmlWriter writer = XmlWriter.Create(destFilePath))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("ns0", "MT_UploadPPNCredit_Req", "http://admevis");

                        writer.WriteElementString("IDNo", idNo);
                        writer.WriteElementString("PostingDate", dtNow.ToString("yyyy-MM-dd"));
                        writer.WriteElementString("AmountTotal", totalAmount.ToString("N0").Replace(",", "").Replace(".", ""));

                        writer.WriteStartElement("Input");

                        //Item
                        foreach (var item in dats)
                        {
                            writer.WriteStartElement("Item");
                            writer.WriteElementString("FP", item.ItemText);
                            writer.WriteElementString("NPWP", item.Npwp);
                            writer.WriteElementString("PembetulanKe", item.Pembetulan.ToString("00"));
                            writer.WriteElementString("MasaPajakBulan", item.MasaPajak.HasValue ? item.MasaPajak.Value.ToString("00") : "");
                            writer.WriteElementString("MasaPajakTahun", item.TahunPajak.HasValue ? item.TahunPajak.Value.ToString() : "");
                            writer.WriteElementString("AccountingDocDebet", item.AccountingDocNo);
                            writer.WriteElementString("FiscalYearDebet", item.FiscalYearDebet.HasValue ? item.FiscalYearDebet.Value.ToString() : "");
                            writer.WriteElementString("LineItem", item.ItemNo);
                            writer.WriteElementString("GLAccount", item.GLAccount);
                            //writer.WriteElementString("AmountPPN", item.AmountSap.HasValue ? item.AmountSap.Value.ToString("N0").Replace(",", "").Replace(".", "") : "");
                            writer.WriteElementString("AmountPPN", item.AmountEvis.HasValue ? item.AmountEvis.Value.ToString("N0").Replace(",", "").Replace(".", "") : "");/*Feedback 2019-10-07*/
                            writer.WriteElementString("FPOriginal", item.ItemTextOriginal);
                            writer.WriteElementString("NPWPOriginal", item.NpwpOriginal);
                            writer.WriteElementString("PembetulanKeOriginal", item.PembetulanOriginal.HasValue ? item.PembetulanOriginal.Value.ToString("00") : "");
                            writer.WriteElementString("AccountingDocDebetOriginal", item.AccountingDocDebetOriginal);
                            writer.WriteElementString("FiscalYearDebetOriginal", item.FiscalYearDebetOriginal.HasValue ? item.FiscalYearDebetOriginal.Value.ToString() : "");
                            writer.WriteElementString("GLAccountOriginal", item.GLAccountOriginal);
                            writer.WriteElementString("AmountPPNOriginal", item.AmountPPNOriginal.HasValue ? item.AmountPPNOriginal.Value.ToString("N0").Replace(".", "").Replace(",", "") : "");
                            writer.WriteEndElement();

                            //    if (item.ItemText2 != null && item.ItemText2 != "" && item.ItemText2.Length > 0)
                            //    {
                            //        writer.WriteStartElement("Item");
                            //        writer.WriteElementString("FP", item.ItemText2);
                            //        writer.WriteElementString("NPWP", item.NpwpPenjual2);
                            //        writer.WriteElementString("PembetulanKe", "");
                            //        writer.WriteElementString("MasaPajakBulan", item.MasaPajak2.HasValue ? item.MasaPajak2.Value.ToString("00") : "");
                            //        writer.WriteElementString("MasaPajakTahun", item.TahunPajak2.HasValue ? item.TahunPajak2.Value.ToString() : "");
                            //        //writer.WriteElementString("AccountingDocDebet", "");
                            //        writer.WriteElementString("AccountingDocDebet", "");/*fixing feedback 2019-10-07*/
                            //        writer.WriteElementString("FiscalYearDebet", "");
                            //        writer.WriteElementString("LineItem", "");
                            //        writer.WriteElementString("GLAccount", "");
                            //        writer.WriteElementString("AmountPPN", item.AmountEvis2.HasValue ? item.AmountEvis2.Value.ToString("N0").Replace(",", "").Replace(".", "") : "");
                            //        writer.WriteElementString("FPOriginal", "");
                            //        writer.WriteElementString("NPWPOriginal", "");
                            //        writer.WriteElementString("PembetulanKeOriginal", "");
                            //        writer.WriteElementString("AccountingDocDebetOriginal", "");
                            //        writer.WriteElementString("FiscalYearDebetOriginal", "");
                            //        writer.WriteElementString("GLAccountOriginal", "");
                            //        writer.WriteElementString("AmountPPNOriginal", "");
                            //        writer.WriteEndElement();
                            //    }
                        }

                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                    }
                }
            }
            catch (Exception exception)
            {
                objRet.InfoType = CommonOutputType.Error;
                objRet.MessageInfo = exception.Message;
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
            }
            return objRet;
        }

        public static FileManagerCommonOutput CompEvisVsSapCreateForceSubmitXml(string idNo, string RepoUser
            , string RepoPassword, string RepoRootPath, string DataFolderWatcherInboxService, string PrefixUploadPpnCredit)
        {
            var objRet = new FileManagerCommonOutput()
            {
                InfoType = CommonOutputType.Success,
                IdNo = string.Empty,
                MessageInfo = "",
                FilePath = ""
            };
            try
            {
                var dats = XmlUploadPpnCredits.ByIdNo(idNo);
                if (dats.Count <= 0)
                {
                    objRet.InfoType = CommonOutputType.Error;
                    objRet.MessageInfo = "Empty Data to Export";
                    return objRet;
                }

                var glAccount = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.GLAccountForceSubmitSAP);
                if (glAccount == null)
                {
                    objRet.InfoType = CommonOutputType.Error;
                    objRet.MessageInfo = "GL Account default not found on General Config";
                    return objRet;
                }

                //Process csv
                var imp = new ImpersonationHelper();
                imp.Impersonate(RepoUser,RepoPassword);

                var credential = new NetworkCredential(RepoUser, RepoPassword);
                using (new NetworkConnectionHelper(RepoRootPath, credential))
                {
                    var destFolder = string.Format(@"{0}", DataFolderWatcherInboxService);
                    if (!Directory.Exists(destFolder))
                    {
                        Directory.CreateDirectory(destFolder);
                    }

                    var dtNow = DateTime.Now;
                    var filename = string.Format("{0}_{1}_{2}.xml", PrefixUploadPpnCredit, dtNow.ToString("yyyyMMdd"), dtNow.ToString("HHmmss-fff"));
                    var destFilePath = string.Format(@"{0}\{1}", destFolder, filename);

                    var totalAmount = dats.Sum(d => d.AmountEvis.HasValue ? d.AmountEvis.Value : 0);

                    objRet.FilePath = destFilePath;
                    objRet.IdNo = idNo;

                    //var sysDate = DateTime.Now.ToString("yyyyMMdd");

                    //Create Xml File
                    using (XmlWriter writer = XmlWriter.Create(destFilePath))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("ns0", "MT_UploadPPNCredit_Req", "http://admevis");

                        writer.WriteElementString("IDNo", idNo);
                        writer.WriteElementString("PostingDate", dtNow.ToString("yyyy-MM-dd"));
                        writer.WriteElementString("AmountTotal", totalAmount.ToString("N0").Replace(",", "").Replace(".", ""));

                        writer.WriteStartElement("Input");
                        //Item
                        foreach (var item in dats)
                        {
                            writer.WriteStartElement("Item");
                            writer.WriteElementString("FP", item.ItemText);
                            writer.WriteElementString("NPWP", item.NpwpPenjual);
                            writer.WriteElementString("PembetulanKe", item.Pembetulan.ToString("00"));
                            writer.WriteElementString("MasaPajakBulan", item.MasaPajak.HasValue ? item.MasaPajak.Value.ToString("00") : "");
                            writer.WriteElementString("MasaPajakTahun", item.TahunPajak.HasValue ? item.TahunPajak.Value.ToString() : "");
                            //writer.WriteElementString("AccountingDocDebet", "");
                            writer.WriteElementString("AccountingDocDebet", !string.IsNullOrEmpty(item.AccountingDocNo) ? item.AccountingDocNo : "");/*fixing feedback 2019-10-07*/
                            writer.WriteElementString("FiscalYearDebet", "");
                            writer.WriteElementString("LineItem", "");
                            writer.WriteElementString("GLAccount", item.GLAccount);
                            writer.WriteElementString("AmountPPN", item.AmountEvis.HasValue ? item.AmountEvis.Value.ToString("N0").Replace(",", "").Replace(".", "") : "");
                            writer.WriteElementString("FPOriginal", item.ItemTextOriginal);
                            writer.WriteElementString("NPWPOriginal", item.NpwpOriginal);
                            writer.WriteElementString("PembetulanKeOriginal", item.PembetulanOriginal.HasValue ? item.PembetulanOriginal.Value.ToString("00") : "");
                            writer.WriteElementString("AccountingDocDebetOriginal", item.AccountingDocDebetOriginal);
                            writer.WriteElementString("FiscalYearDebetOriginal", item.FiscalYearDebetOriginal.HasValue ? item.FiscalYearDebetOriginal.Value.ToString() : "");
                            writer.WriteElementString("GLAccountOriginal", item.GLAccountOriginal);
                            writer.WriteElementString("AmountPPNOriginal", item.AmountPPNOriginal.HasValue ? item.AmountPPNOriginal.Value.ToString("N0").Replace(".", "").Replace(",", "") : "");
                            writer.WriteEndElement();

                        }

                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                    }
                }
            }
            catch (Exception exception)
            {
                objRet.InfoType = CommonOutputType.Error;
                objRet.MessageInfo = exception.Message;
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
            }
            return objRet;
        }

        public static FileManagerCommonOutput XmlRetryFileTransferToSap(List<long> logSapId, string userNameLogin)
        {

            var objRet = new FileManagerCommonOutput()
            {
                InfoType = CommonOutputType.Success,
                MessageInfo = string.Empty
            };

            if (logSapId.Count <= 0)
            {
                return new FileManagerCommonOutput()
                {
                    InfoType = CommonOutputType.Error,
                    MessageInfo = "No Data"
                };
            }

            var msgs = new List<string>();

            IntPtr token;

            if (!NativeMethods.LogonUser(
                FileManagerSapConfiguration.UserNameWatcherInboxService,
                FileManagerSapConfiguration.ServerAddessWatcherInboxService,
                FileManagerSapConfiguration.PasswordWatcherInboxService,
                NativeMethods.LogonType.NewCredentials,
                NativeMethods.LogonProvider.Default,
                out token))
            {
                throw new Win32Exception();
            }
            try
            {
                IntPtr tokenDuplicate;

                if (!NativeMethods.DuplicateToken(
                    token,
                    NativeMethods.SecurityImpersonationLevel.Impersonation,
                    out tokenDuplicate))
                {
                    throw new Win32Exception();
                }
                try
                {
                    using (WindowsImpersonationContext impersonationContext =
                        new WindowsIdentity(tokenDuplicate).Impersonate())
                    {
                        //get file to process
                        var ids = string.Join(",", logSapId);
                        var dats = LogSaps.GetByIds(ids);
                        string destinationFolder = FileManagerSapConfiguration.DataFolder2WatcherInboxService;//Share Folder SAP
                        foreach (var logSap in dats)
                        {
                            bool isSuccessTransfer = false;
                            var destFile = string.Format(@"{0}\{1}", destinationFolder, logSap.FileName);
                            var msgTransferFile = "";
                            try
                            {
                                File.Copy(logSap.LocalPath, destFile, true);
                                isSuccessTransfer = true;
                                msgs.Add("Success Transfer " + logSap.FileName + " to " + destinationFolder);
                            }
                            catch (Exception exceptionCopy)
                            {
                                string logKey;
                                Logger.WriteLog(out logKey, LogLevel.Error, exceptionCopy.Message, MethodBase.GetCurrentMethod(), exceptionCopy);
                                msgTransferFile = "Failed Transfer " + logSap.FileName + " to " + destinationFolder;
                                msgs.Add(msgTransferFile);
                            }

                            //Update LogSap
                            logSap.LocalExecution = DateTime.Now;
                            logSap.Status = isSuccessTransfer
                                ? (int)ApplicationEnums.SapStatusLog.Success
                                : (int)ApplicationEnums.SapStatusLog.Error;
                            logSap.ModifiedBy = userNameLogin;
                            logSap.TransferDate = isSuccessTransfer ? DateTime.Now : (DateTime?)null;
                            logSap.Note = msgTransferFile;
                            logSap.SapPath = isSuccessTransfer ? destFile : null;

                            LogSaps.Save(logSap);

                        }

                        impersonationContext.Undo();
                    }
                }
                catch (Exception ex2)
                {
                    string logKey;
                    Logger.WriteLog(out logKey, LogLevel.Error, "Error File Transfer to SAP with info : " + ex2.Message, MethodBase.GetCurrentMethod(), ex2);
                    objRet.InfoType = CommonOutputType.Error;
                    objRet.MessageInfo = "Error File Transfer. Log Key : " + logKey;
                }
                finally
                {
                    if (tokenDuplicate != IntPtr.Zero)
                    {
                        if (!NativeMethods.CloseHandle(tokenDuplicate))
                        {

                            // Uncomment if you need to know this case.
                            throw new Win32Exception();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, "Error File Transfer to SAP with info : " + ex.Message, MethodBase.GetCurrentMethod(), ex);
                objRet.InfoType = CommonOutputType.Error;
                objRet.MessageInfo = "Error File Transfer. Log Key : " + logKey;
            }
            finally
            {
                if (token != IntPtr.Zero)
                {
                    if (!NativeMethods.CloseHandle(token))
                    {
                        // Uncomment if you need to know this case.
                        throw new Win32Exception();
                    }
                }
            }

            if (msgs.Count > 0)
            {
                objRet.MessageInfo = string.Join("<br />", msgs);
            }

            return objRet;
        }

        #endregion

        #region -------- Test Dev Compare Evis vs SAP -----

        public static FileManagerCommonOutput GenerateResponUploadPppnCredit(string idNo, bool isSuccess, string message, string accountingDocNoKredit, int fiscalYearKredit)
        {
            var objRet = new FileManagerCommonOutput()
            {
                InfoType = CommonOutputType.Success,
                IdNo = string.Empty,
                MessageInfo = "",
                FilePath = ""
            };
            
            try
            {
                //var dats = CompEvisSaps.GetByIdNo(idNo);
                var dats = XmlUploadPpnCredits.ByIdNo(idNo);
                if (dats.Count <= 0)
                {
                    objRet.InfoType = CommonOutputType.Error;
                    objRet.MessageInfo = "Empty Data to Generate";
                    return objRet;
                }

                //Process csv
                var imp = new ImpersonationHelper();
                imp.Impersonate(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);

                var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                    , FileManagerConfiguration.RepoPassword);
                using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                {
                    var destFolder = string.Format(@"{0}", FileManagerSapConfiguration.DataFolderWatcherInboxService);
                    if (!Directory.Exists(destFolder))
                    {
                        Directory.CreateDirectory(destFolder);
                    }

                    var dtNow = DateTime.Now;
                    var filename = string.Format("{0}_{1}_{2}.xml", FileManagerSapConfiguration.PrefixOutUploadPpnCredit, dtNow.ToString("yyyyMMdd"), dtNow.ToString("HHmmss-fff"));
                    var destFilePath = string.Format(@"{0}\{1}", destFolder, filename);

                    objRet.FilePath = destFilePath;
                    objRet.IdNo = idNo;

                    //Create Xml File
                    using (XmlWriter writer = XmlWriter.Create(destFilePath))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("ns1", "MT_UploadPPNCredit_Resp", "http://admevis");

                        writer.WriteElementString("IDNo", idNo);
                        writer.WriteElementString("Confirm", isSuccess ? "X" : string.Empty);
                        writer.WriteElementString("Message", isSuccess ? string.Empty : message);
                        writer.WriteElementString("AccountingDocNo", isSuccess ? accountingDocNoKredit : string.Empty);
                        writer.WriteElementString("FiscalYear", isSuccess ? fiscalYearKredit.ToString("0000") : "0000");
                        writer.WriteStartElement("Output");

                        //Item
                        foreach (var item in dats)
                        {
                            writer.WriteStartElement("Item");
                            writer.WriteElementString("FP", item.ItemText);
                            writer.WriteElementString("NPWP", string.IsNullOrEmpty(item.TaxInvoiceNumberSap) ? item.NpwpPenjual : item.Npwp);
                            writer.WriteElementString("PembetulanKe", item.Pembetulan.ToString("00"));
                            writer.WriteElementString("MasaPajakBulan", item.MasaPajak.HasValue ? item.MasaPajak.Value.ToString("00") : "");
                            writer.WriteElementString("MasaPajakTahun", item.TahunPajak.HasValue ? item.TahunPajak.Value.ToString() : "");
                            writer.WriteElementString("AccountingDocDebet", item.AccountingDocNo);
                            writer.WriteElementString("FiscalYearDebet", item.FiscalYearDebet.HasValue ? item.FiscalYearDebet.Value.ToString() : "");
                            writer.WriteElementString("LineItem", item.ItemNo);
                            writer.WriteElementString("GLAccount", item.GLAccount);
                            writer.WriteElementString("AmountPPN", item.AmountSap.HasValue ? item.AmountSap.Value.ToString("N0").Replace(",", "").Replace(".", "") : item.AmountEvis.HasValue ? item.AmountEvis.Value.ToString("N0").Replace(",", "").Replace(".", "") : "");
                            writer.WriteElementString("FPOriginal", item.ItemTextOriginal);
                            writer.WriteElementString("NPWPOriginal", item.NpwpOriginal);
                            writer.WriteElementString("PembetulanKeOriginal", item.PembetulanOriginal.HasValue ? item.PembetulanOriginal.Value.ToString("00") : "");
                            writer.WriteElementString("AccountingDocDebetOriginal", item.AccountingDocDebetOriginal);
                            writer.WriteElementString("FiscalYearDebetOriginal", item.FiscalYearDebetOriginal.HasValue ? item.FiscalYearDebetOriginal.Value.ToString() : "");
                            writer.WriteElementString("GLAccountOriginal", item.GLAccountOriginal);
                            writer.WriteElementString("AmountPPNOriginal", item.AmountPPNOriginal.HasValue ? item.AmountPPNOriginal.Value.ToString("N0").Replace(".", "").Replace(",", "") : "");
                            writer.WriteElementString("Message", isSuccess ? string.Empty : message + "-" + item.TaxInvoiceNumberEvis);
                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                    }
                }
            }
            catch (Exception exception)
            {
                objRet.InfoType = CommonOutputType.Error;
                objRet.MessageInfo = exception.Message;
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
            }
            return objRet;
        }

        #endregion

    }
}
