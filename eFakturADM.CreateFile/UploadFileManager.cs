using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Web;
using DocumentFormat.OpenXml.Wordprocessing;
using eFakturADM.Data;
using eFakturADM.DJPLib.Objects;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Shared.Utility;
using SpreadsheetLight;

namespace eFakturADM.FileManager
{
    public class UploadFileManager
    {
        #region --------- class ----------
        public class VendorInfo
        {
            public int VendorId { get; set; }
            public string NPWP { get; set; }
            public string Nama { get; set; }
            public string Alamat { get; set; }
            public string PkpDicabut { get; set; }
            public string TglPkpDicabut { get; set; }
            public string KeteranganTambahan { get; set; }
        }
        public class VendorInfoUpload : VendorInfo
        {
            public bool IsValid { get; set; }
            public string Message { get; set; }
        }

        public class FpDigantiOutstandingInfo
        {
            public string UrlScan { get; set; }
            public string OriginalNoFaktur { get; set; }
            public string NoFaktur { get; set; }
            public int MasaPajak { get; set; }
            public int TahunPajak { get; set; }
            public string StatusFaktur { get; set; }
            public string StatusApproval { get; set; }
            public string Keterangan { get; set; }
            public ApplicationEnums.StatusDigantiOutstanding StatusOutstanding { get; set; }
        }

        public class FpDigantiOutstandingInfoUpload : FpDigantiOutstandingInfo
        {
            public bool IsValid { get; set; }
            public string Message { get; set; }
        }

        #endregion

        public static FileManagerCommonOutput UploadVendor(HttpPostedFileBase file, string userNameLogin)
        {
            var model = new FileManagerCommonOutput()
            {
                InfoType = CommonOutputType.Success,
                FilePath = string.Empty,
                IdNo = string.Empty,
                MessageInfo = string.Empty
            };
            FileInfo fInfo = null;
            try
            {
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    var ext = Path.GetExtension(file.FileName);
                    var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    if (ext != ".xlsx")
                    {
                        model.InfoType = CommonOutputType.Success;
                        model.MessageInfo = "Unsupported file type";
                        return model;
                    }

                    var imp = new ImpersonationHelper();
                    imp.Impersonate(FileManagerConfiguration.RepoUser
                        , FileManagerConfiguration.RepoPassword);

                    var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                        , FileManagerConfiguration.RepoPassword);
                    using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                    {
                        var tempUploadPath = string.Format(@"{0}\Upload", FileManagerConfiguration.RepoRootPath);
                        if (!Directory.Exists(tempUploadPath))
                        {
                            Directory.CreateDirectory(tempUploadPath);
                        }

                        var tempDestFolder = string.Format(@"{0}\{1}", tempUploadPath, "Vendor");

                        if (!Directory.Exists(tempDestFolder))
                        {
                            Directory.CreateDirectory(tempDestFolder);
                        }

                        var destFileName = string.Format("{0}_{1}{2}", fileName, DateTime.Now.ToString("ddMMyyyyHHmmss"),
                            ext);

                        var path = string.Format(@"{0}\{1}", tempDestFolder, destFileName);
                        file.SaveAs(path);

                        fInfo = new FileInfo(path);
                        var sl = new SLDocument();

                        try
                        {
                            sl = new SLDocument(path, "Data Input");
                        }
                        catch
                        {
                            fInfo.Delete();
                            model.InfoType = CommonOutputType.Error;
                            model.MessageInfo = "Sheet Name should be 'Data Input'";
                            return model;
                        }

                        //check nama sheet
                        List<string> sheetname = sl.GetSheetNames();
                        bool isSheetNameExists = false;
                        if (sheetname.Count > 0)
                        {
                            foreach (var t in sheetname.Where(t => t == "Data Input"))
                            {
                                isSheetNameExists = true;
                            }
                        }
                        if (isSheetNameExists == false)
                        {
                            fInfo.Delete();
                            model.InfoType = CommonOutputType.Error;
                            model.MessageInfo = "Nama Sheet seharusnya adalah 'Data Input'";
                            return model;
                        }

                        //get nama column (asumsi nama column pasti baris ke-1 dari file excel)
                        SLWorksheetStatistics stats = sl.GetWorksheetStatistics();
                        int iStartColumnIndex = stats.StartColumnIndex;
                        int iEndColumnIndex = stats.EndColumnIndex;
                        int numcol = stats.NumberOfColumns;

                        if (numcol != 6)
                        {
                            fInfo.Delete();
                            model.InfoType = CommonOutputType.Error;
                            model.MessageInfo = "Jumlah Kolom tidak sesuai";
                            return model;
                        }

                        //Validate Column Name
                        var msgs = new List<string>();
                        var colName = sl.GetCellValueAsString(1, 1);
                        if (string.IsNullOrEmpty(colName) || colName.ToLower() != "npwp")
                        {
                            msgs.Add("Kolom pertama seharusnya 'NPWP'");
                        }
                        colName = sl.GetCellValueAsString(1, 2);
                        if (string.IsNullOrEmpty(colName) || colName.ToLower() != "nama")
                        {
                            msgs.Add("Kolom Kedua seharusnya 'Nama'");
                        }
                        colName = sl.GetCellValueAsString(1, 3);
                        if (string.IsNullOrEmpty(colName) || colName.ToLower() != "alamat")
                        {
                            msgs.Add("Kolom Ketiga seharusnya 'Alamat'");
                        }
                        colName = sl.GetCellValueAsString(1, 4);
                        if (string.IsNullOrEmpty(colName) || colName.ToLower() != "pkp dicabut")
                        {
                            msgs.Add("Kolom Keempat seharusnya 'PKP Dicabut'");
                        }
                        colName = sl.GetCellValueAsString(1, 5);
                        if (string.IsNullOrEmpty(colName) || colName.ToLower() != "tgl pkp")
                        {
                            msgs.Add("Kolom Kelima seharusnya 'Tgl PKP'");
                        }
                        colName = sl.GetCellValueAsString(1, 6);
                        if (string.IsNullOrEmpty(colName) || colName.ToLower() != "keterangan")
                        {
                            msgs.Add("Kolom Keenam seharusnya 'Keterangan'");
                        }

                        if (msgs.Count > 0)
                        {
                            fInfo.Delete();
                            model.InfoType = CommonOutputType.Error;
                            model.MessageInfo = string.Join("<br/>", msgs);
                            return model;
                        }

                        //process row data, start from second row
                        var lstVendor = new List<VendorInfoUpload>();
                        sl.SetCellValue(1, iEndColumnIndex + 1, "Notes");
                        var lstNpwpInList = new List<string>();
                        //Get All Npwp
                        for (int i = 2; i < stats.NumberOfRows + 1; i++)
                        {
                            lstNpwpInList.Add(sl.GetCellValueAsString(i, 1));
                        }

                        for (int i = 2; i < stats.NumberOfRows + 1; i++)
                        {
                            var npwp = sl.GetCellValueAsString(i, 1);
                            var nama = sl.GetCellValueAsString(i, 2);
                            var alamat = sl.GetCellValueAsString(i, 3);
                            var pkpDicabut = sl.GetCellValueAsString(i, 4);
                            var tglPkp = sl.GetCellValueAsString(i, 5);
                            var keterangan = sl.GetCellValueAsString(i, 6);

                            var vToPush = new VendorInfoUpload()
                            {
                                NPWP = npwp,
                                Nama = nama,
                                Alamat = alamat,
                                PkpDicabut = pkpDicabut,
                                TglPkpDicabut = tglPkp,
                                KeteranganTambahan = keterangan
                            };
                            var msgValidate = new List<string>();
                            if (!string.IsNullOrEmpty(tglPkp))
                            {
                                DateTime dt;
                                if (!DateTime.TryParse(tglPkp, out dt))
                                {
                                    msgValidate.Add("Tgl PKP tidak valid.");
                                }
                            }

                            if (string.IsNullOrEmpty(npwp))
                            {
                                msgValidate.Add("NPWP harus diisi.");
                            }
                            else
                            {
                                //check panjang karakter dan harus angka
                                Regex regex = new Regex(@"^[0-9]{16}$");
                                var match = regex.Match(npwp);

                                if (!match.Success)
                                {
                                    msgValidate.Add("NPWP harus 16 karakter dan berupa angka");
                                }
                                else
                                {
                                    var chkIsExists = Vendors.GetByNpwp(npwp);
                                    if (chkIsExists != null)
                                    {
                                        msgValidate.Add("Vendor[" + npwp + "] sudah ada");
                                    }
                                    else
                                    {
                                        //check if double in list
                                        var chk =
                                            lstNpwpInList.Where(
                                                d => String.Equals(d, npwp, StringComparison.CurrentCultureIgnoreCase)).ToList();
                                        if (chk.Count > 1)
                                        {
                                            msgValidate.Add("Vendor[" + npwp + "] double");
                                        }
                                    }

                                }

                            }

                            if (string.IsNullOrEmpty(nama))
                            {
                                msgValidate.Add("Nama harus diisi.");
                            }

                            if (msgValidate.Count > 0)
                            {
                                vToPush.IsValid = false;
                                vToPush.Message = string.Join(" ", msgValidate);
                                sl.SetCellValue(i, iEndColumnIndex + 1, vToPush.Message);
                            }
                            else
                            {
                                vToPush.IsValid = true;
                            }

                            lstVendor.Add(vToPush);

                        }

                        var chkIsAnyInvalidData = lstVendor.Where(c => !c.IsValid).ToList();
                        destFileName = "result_" + destFileName;

                        //var resultpath = Path.Combine(tempDestFolder + "/", destFileName);
                        var resultpath = string.Format("{0}/{1}", tempDestFolder, destFileName);
                        sl.SaveAs(resultpath);

                        if (chkIsAnyInvalidData.Count > 0)
                        {
                            fInfo.Delete();

                            model.InfoType = CommonOutputType.ErrorWithFileDownload;
                            model.FilePath = destFileName;
                            return model;
                        }

                        //Process Data save to Database
                        var dsParamT = new dsParamTable();

                        foreach (var item in lstVendor)
                        {
                            var dRow = dsParamT.Vendor_ParamTable.NewVendor_ParamTableRow();
                            dRow.NPWP = item.NPWP;
                            dRow.Nama = item.Nama;
                            dRow.Alamat = item.Alamat;
                            dRow.PkpDicabut = item.PkpDicabut;
                            dRow.TglPkpDicabut = item.TglPkpDicabut;
                            dRow.KeteranganTambahan = item.KeteranganTambahan;
                            dRow.UserNameLogin = userNameLogin;
                            dsParamT.Vendor_ParamTable.AddVendor_ParamTableRow(dRow);
                        }
                        string logKey;
                        var saveResult = Vendors.SaveUpload(dsParamT.Vendor_ParamTable, out logKey);

                        if (saveResult)
                        {
                            model.InfoType = CommonOutputType.Success;
                            model.MessageInfo = "Upload Vendor Data successfully. ";
                        }
                        else
                        {
                            model.InfoType = CommonOutputType.Error;
                            model.MessageInfo = "Upload Vendor Data failed. See Log with Key " + logKey + " for details.";
                        }

                        fInfo.Delete();
                    }
                }
                else
                {
                    model.InfoType = CommonOutputType.Error;
                    model.MessageInfo = "Empty File";
                }
            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                model.InfoType = CommonOutputType.Error;
                model.MessageInfo = "Upload Vendor Data failed. See Log with Key " + logKey + " for details.";
            }
            finally
            {
                if (fInfo != null && fInfo.Exists)
                {
                    fInfo.Delete();
                }
            }
            return model;
        }



        //public static FileManagerCommonOutput UploadFpDigantiOutstanding(HttpPostedFileBase file, string userNameLogin)
        public static FileManagerCommonOutput UploadFpDigantiOutstanding(HttpPostedFileBase file, string userNameLogin, bool isUseProxy, string inetProxy
            , int? inetProxyPort, bool? inetProxyUseCredential, int timeOutSettingInt)
        {
            var model = new FileManagerCommonOutput()
            {
                InfoType = CommonOutputType.Success,
                FilePath = string.Empty,
                IdNo = string.Empty,
                MessageInfo = string.Empty
            };
            FileInfo fInfo = null;
            try
            {
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    var ext = Path.GetExtension(file.FileName);
                    var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    if (ext != ".xlsx")
                    {
                        model.InfoType = CommonOutputType.Error;
                        model.MessageInfo = "Unsupported file type";
                        return model;
                    }

                    int minpelaporan = 1;
                    int maxpelaporan = 1;

                    var configData = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.PelaporanTglFaktur);
                    if (configData == null)
                    {
                        model.InfoType = CommonOutputType.Error;
                        model.MessageInfo = "GeneralConfig [PelaporanTglFaktur] not found.";
                        return model;
                    }
                    else
                    {
                        var dats = configData.ConfigValue.Split(':').ToList();
                        if (dats.Count != 2)
                        {
                            model.InfoType = CommonOutputType.Error;
                            model.MessageInfo = "GeneralConfig [PelaporanTglFaktur] not valid.";
                            return model;
                        }

                        minpelaporan = int.Parse(dats[0].Replace("[", "").Replace("]", ""));
                        maxpelaporan = int.Parse(dats[1].Replace("[", "").Replace("]", ""));

                    }

                    var imp = new ImpersonationHelper();
                    imp.Impersonate(FileManagerConfiguration.RepoUser
                        , FileManagerConfiguration.RepoPassword);

                    var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                        , FileManagerConfiguration.RepoPassword);
                    using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                    {
                        var tempUploadPath = string.Format(@"{0}\Upload", FileManagerConfiguration.RepoRootPath);
                        if (!Directory.Exists(tempUploadPath))
                        {
                            Directory.CreateDirectory(tempUploadPath);
                        }

                        var tempDestFolder = string.Format(@"{0}\{1}", tempUploadPath, "FpDigantiOutstanding");

                        if (!Directory.Exists(tempDestFolder))
                        {
                            Directory.CreateDirectory(tempDestFolder);
                        }

                        var destFileName = string.Format("{0}_{1}{2}", fileName, DateTime.Now.ToString("ddMMyyyyHHmmss"),
                            ext);

                        var path = string.Format(@"{0}\{1}", tempDestFolder, destFileName);
                        file.SaveAs(path);

                        fInfo = new FileInfo(path);
                        var sl = new SLDocument(path);

                        //get nama column (asumsi nama column pasti baris ke-1 dari file excel)
                        SLWorksheetStatistics stats = sl.GetWorksheetStatistics();
                        int numcol = stats.NumberOfColumns;

                        if (numcol < 17)
                        {
                            fInfo.Delete();
                            model.InfoType = CommonOutputType.Error;
                            model.MessageInfo = "Jumlah Kolom tidak sesuai";
                            return model;
                        }
                        var rowcount = stats.NumberOfRows;
                        //process row data, start from first row
                        var lstdat = new List<FpDigantiOutstandingInfoUpload>();
                        var fpNormalPenggantiToUpdate = new List<FakturPajak>();
                        for (int i = 1; i <= rowcount; i++)
                        {
                            var nofaktur = sl.GetCellValueAsString(i, 3);
                            var masaPajak = sl.GetCellValueAsInt32(i, 5);
                            var tahunPajak = sl.GetCellValueAsInt32(i, 6);
                            var statusfaktur = sl.GetCellValueAsString(i, 7);
                            var statusapproval = sl.GetCellValueAsString(i, 12);
                            var keterangan = sl.GetCellValueAsString(i, 14);
                            var statoutstanding = ApplicationEnums.StatusDigantiOutstanding.Outstanding;
                            var msgValidate = new List<string>();
                            if (string.IsNullOrEmpty(nofaktur))
                            {
                                msgValidate.Add("NOMOR FAKTUR harus diisi.");
                            }
                            else
                            {
                                var chkfpdb = FakturPajaks.GetByFormatedNoFaktur(nofaktur);
                                if (chkfpdb.Count <= 0)
                                {
                                    msgValidate.Add("NOMOR FAKTUR tidak terdaftar di EVIS.");
                                }
                                else
                                {
                                    var fpinfo = chkfpdb.First();
                                    if (fpinfo.FgPengganti == "0")
                                    {
                                        #region ------ FP Normal -----------------
                                        //FP Normal
                                        var vToPush = new FpDigantiOutstandingInfoUpload()
                                        {
                                            NoFaktur = nofaktur,
                                            StatusApproval = statusapproval,
                                            Keterangan = keterangan,
                                            MasaPajak = masaPajak,
                                            TahunPajak = tahunPajak,
                                            StatusFaktur = statusfaktur,
                                            UrlScan = fpinfo.UrlScan,
                                            OriginalNoFaktur = fpinfo.NoFakturPajak
                                        };
                                        if (fpinfo.StatusFaktur.ToLower() == "faktur diganti")
                                        {
                                            statoutstanding = ApplicationEnums.StatusDigantiOutstanding.Complete;
                                        }
                                        else
                                        {
                                            //check apakah termasuk expired atau tidak
                                            //check nya ke Open Close Period yang memungkinkan tgl faktur tersebutd di set
                                            //misal tgl faktur 10 Oktober 2018, maka Masa Pajak (Open Close Period) yang harus di cek adalah 3 bulan ke depan yaitu Oktober, November dan Desember
                                            //yang di cek expired atau tidak nya adalah FP Normal nya
                                            var originFpNumber = chkfpdb.First().NoFakturPajak;
                                            var getfpnormal = FakturPajaks.GetFakturPajakNormal(originFpNumber);

                                            if (getfpnormal.TglFaktur.HasValue)
                                            {
                                                var periodyear = getfpnormal.TglFaktur.Value.Year;
                                                var periodmonth = getfpnormal.TglFaktur.Value.Month;
                                                var dtmin = new DateTime(periodyear, periodmonth, 1).AddMonths(maxpelaporan);
                                                var dtmax =
                                                    new DateTime(periodyear, periodmonth, 1).AddMonths(Math.Abs(minpelaporan));
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

                                            }
                                        }

                                        vToPush.StatusOutstanding = statoutstanding;
                                        if (msgValidate.Count > 0)
                                        {
                                            vToPush.IsValid = false;
                                            vToPush.Message = string.Join(" ", msgValidate);
                                            sl.SetCellValue(i, 18, vToPush.Message);
                                        }
                                        else
                                        {
                                            vToPush.IsValid = true;
                                        }

                                        lstdat.Add(vToPush);

                                        #endregion
                                    }
                                    else
                                    {
                                        //FP Normal Pengganti
                                        #region -------- FP Normal Pengganti --------

                                        var getfpnormal = FakturPajaks.GetFakturPajakNormal(fpinfo.NoFakturPajak);

                                        var vToPush = new FpDigantiOutstandingInfoUpload()
                                        {
                                            NoFaktur = getfpnormal.FormatedNoFaktur,
                                            StatusApproval = statusapproval,
                                            Keterangan = keterangan,
                                            MasaPajak = getfpnormal.MasaPajak.HasValue ? getfpnormal.MasaPajak.Value : 0,
                                            TahunPajak = getfpnormal.TahunPajak.HasValue ? getfpnormal.TahunPajak.Value : 0,
                                            StatusFaktur = statusfaktur,
                                            UrlScan = getfpnormal.UrlScan,
                                            OriginalNoFaktur = getfpnormal.NoFakturPajak
                                        };

                                        if (fpinfo.StatusFaktur.ToLower() == "faktur diganti")
                                        {
                                            statoutstanding = ApplicationEnums.StatusDigantiOutstanding.Complete;
                                        }
                                        else
                                        {
                                            if (getfpnormal.TglFaktur.HasValue)
                                            {
                                                var periodyear = getfpnormal.TglFaktur.Value.Year;
                                                var periodmonth = getfpnormal.TglFaktur.Value.Month;
                                                var dtmin = new DateTime(periodyear, periodmonth, 1).AddMonths(maxpelaporan);
                                                var dtmax =
                                                    new DateTime(periodyear, periodmonth, 1).AddMonths(Math.Abs(minpelaporan));
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
                                            }
                                        }

                                        vToPush.StatusOutstanding = statoutstanding;
                                        if (msgValidate.Count > 0)
                                        {
                                            vToPush.IsValid = false;
                                            vToPush.Message = string.Join(" ", msgValidate);
                                            sl.SetCellValue(i, 18, vToPush.Message);
                                        }
                                        else
                                        {
                                            vToPush.IsValid = true;
                                        }
                                        fpinfo.TahunPajak = null;
                                        fpinfo.MasaPajak = null;
                                        fpinfo.Modified = DateTime.Now;
                                        fpinfo.ModifiedBy = userNameLogin;
                                        fpinfo.StatusFaktur = "Faktur Diganti";
                                        fpNormalPenggantiToUpdate.Add(fpinfo);

                                        lstdat.Add(vToPush);

                                        #endregion
                                    }
                                }
                            }
                        }

                        var chkIsAnyInvalidData = lstdat.Where(c => !c.IsValid).ToList();
                        destFileName = "result_" + destFileName;
                        if (chkIsAnyInvalidData.Count > 0)
                        {
                            var resultpath = string.Format("{0}/{1}", tempDestFolder, destFileName);
                            sl.SaveAs(resultpath);

                            fInfo.Delete();

                            model.InfoType = CommonOutputType.ErrorWithFileDownload;
                            model.FilePath = destFileName;
                            return model;
                        }
                        else
                        {

                            for (var i = 0; i < lstdat.Count; i++)
                            {
                                if (lstdat[i].StatusOutstanding == ApplicationEnums.StatusDigantiOutstanding.Complete)
                                {

                                    var getfpall =
                                                FakturPajaks.GetByOriginalNoFaktur(lstdat[i].OriginalNoFaktur).FirstOrDefault(c => c.FgPengganti == "1"
                                                    && c.IsDeleted == false && c.StatusFaktur.ToLower() != "faktur diganti"
                                                    && c.FormatedNoFaktur != lstdat[i].NoFaktur);

                                    if (getfpall != null)
                                    {
                                        var djpdata = GetDjpData(timeOutSettingInt, inetProxy, inetProxyPort,
                                            inetProxyUseCredential, getfpall.UrlScan);
                                        if (djpdata != null)
                                        {
                                            if (djpdata.StatusFaktur.ToLower() == "faktur diganti" || djpdata.StatusFaktur.ToLower() == "faktur dibatalkan")
                                            {
                                                lstdat[i].StatusOutstanding = ApplicationEnums.StatusDigantiOutstanding.Outstanding;
                                                getfpall.StatusFaktur = djpdata.StatusFaktur;
                                                getfpall.StatusApproval = djpdata.StatusApproval;
                                                getfpall.TahunPajak = null;
                                                getfpall.MasaPajak = null;
                                                getfpall.Modified = DateTime.Now;
                                                getfpall.ModifiedBy = userNameLogin;

                                                if (
                                                    fpNormalPenggantiToUpdate.All(c => c.FakturPajakId != getfpall.FakturPajakId))
                                                {
                                                    fpNormalPenggantiToUpdate.Add(getfpall);
                                                }

                                            }
                                        }
                                        else
                                        {
                                            lstdat[i].IsValid = false;
                                            lstdat[i].Message = "Error Get FP Normal Pengganti from DJP";
                                        }
                                    }
                                    else
                                    {
                                        lstdat[i].StatusOutstanding = ApplicationEnums.StatusDigantiOutstanding.Outstanding;
                                    }
                                }
                            }

                            var chkisvalid2 = lstdat.Where(c => !c.IsValid).ToList();
                            var resultpath = string.Format("{0}/{1}", tempDestFolder, destFileName);
                            sl.SaveAs(resultpath);
                            if (chkisvalid2.Count > 0)
                            {
                                fInfo.Delete();

                                model.InfoType = CommonOutputType.ErrorWithFileDownload;
                                model.FilePath = destFileName;
                                return model;
                            }
                        }

                        //proses save ke database
                        using (var escope = new TransactionScope())
                        {

                            foreach (var x in lstdat)
                            {
                                //No Faktur disini selalu Nomor Faktur FP Normal 
                                //walaupun yang di upload adalah FP Normal Pengganti
                                var chkd = FakturPajakDigantiOutstandings.GetByFormatedNoFaktur(x.NoFaktur);
                                if (chkd == null || chkd.Id <= 0)
                                {

                                    var tosave = new FakturPajakDigantiOutstanding()
                                    {
                                        FormatedNoFaktur = x.NoFaktur,
                                        MasaPajak = x.MasaPajak,
                                        TahunPajak = x.TahunPajak,
                                        StatusFaktur = x.StatusFaktur,
                                        StatusApproval = x.StatusApproval,
                                        KeteranganDjp = x.Keterangan,
                                        StatusOutstanding = (int)x.StatusOutstanding,
                                        CreatedBy = userNameLogin,
                                        Created = DateTime.Now
                                    };

                                    FakturPajakDigantiOutstandings.Save(tosave);

                                    if (x.StatusOutstanding != ApplicationEnums.StatusDigantiOutstanding.Complete)
                                    {
                                        //get by formated no faktur
                                        var fp = FakturPajaks.GetByFormatedNoFaktur(tosave.FormatedNoFaktur);
                                        foreach (var fakturPajak in fp)
                                        {
                                            fakturPajak.MasaPajak = null;
                                            fakturPajak.TahunPajak = null;
                                            fakturPajak.ModifiedBy = userNameLogin;
                                            fakturPajak.Modified = DateTime.Now;
                                            FakturPajaks.Save(fakturPajak);
                                        }
                                    }
                                }
                                else
                                {
                                    chkd.StatusOutstanding = (int)x.StatusOutstanding;
                                    chkd.TahunPajak = x.TahunPajak;
                                    chkd.MasaPajak = x.MasaPajak;
                                    chkd.StatusFaktur = x.StatusFaktur;
                                    chkd.StatusApproval = x.StatusApproval;
                                    chkd.Keterangan = x.Keterangan;
                                    chkd.KeteranganDjp = x.Keterangan;
                                    chkd.Modified = DateTime.Now;
                                    chkd.ModifiedBy = userNameLogin;

                                    FakturPajakDigantiOutstandings.Save(chkd);

                                    //get by formated no faktur
                                    var fp = FakturPajaks.GetByFormatedNoFaktur(chkd.FormatedNoFaktur);
                                    foreach (var fakturPajak in fp)
                                    {
                                        fakturPajak.MasaPajak = null;
                                        fakturPajak.TahunPajak = null;
                                        fakturPajak.ModifiedBy = userNameLogin;
                                        fakturPajak.Modified = DateTime.Now;
                                        FakturPajaks.Save(fakturPajak);
                                    }
                                }
                            }

                            if (fpNormalPenggantiToUpdate.Count > 0)
                            {
                                foreach (var x in fpNormalPenggantiToUpdate)
                                {
                                    FakturPajaks.Save(x);
                                }
                            }

                            escope.Complete();
                            escope.Dispose();
                        }
                        model.InfoType = CommonOutputType.Success;
                        model.MessageInfo = "Upload Vendor Data successfully. ";

                        fInfo.Delete();
                    }
                }
                else
                {
                    model.InfoType = CommonOutputType.Error;
                    model.MessageInfo = "Empty File";
                }
            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                model.InfoType = CommonOutputType.Error;
                model.MessageInfo = "Upload Vendor Data failed. See Log with Key " + logKey + " for details.";
            }
            finally
            {
                if (fInfo != null && fInfo.Exists)
                {
                    fInfo.Delete();
                }
            }
            return model;
        }

        private static ResValidateFakturPm GetDjpData(int iTimeOutSetting,
            string inetProxy
            , int? inetProxyPort
            , bool? inetProxyUseCredential
            , string urlscan)
        {
            bool isUseProxy = !string.IsNullOrEmpty(inetProxy);
            try
            {
                WebExceptionStatus eStatus;

                string msgError;
                string logKey;
                var objXml = DJPLib.ValidateFakturLib.GetValidateFakturObjectV3(urlscan,
                    iTimeOutSetting, isUseProxy, inetProxy, inetProxyPort
                    , inetProxyUseCredential, out msgError, out eStatus, out logKey);
                if (eStatus != WebExceptionStatus.Success)
                {
                    return null;
                }
                return objXml;
            }
            catch (Exception exception)
            {
                string outlogkey;
                Logger.WriteLog(out outlogkey, LogLevel.Error, "Error Request DJP for Url : " + urlscan, MethodBase.GetCurrentMethod(), exception);
                return null;
            }
        }


        public static FileManagerCommonOutput UploadTempFakturPajakTerlapor(HttpPostedFileBase file, string userNameLogin)
        {
            var model = new FileManagerCommonOutput()
            {
                InfoType = CommonOutputType.Success,
                FilePath = string.Empty,
                IdNo = string.Empty,
                MessageInfo = string.Empty
            };
            try
            {
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    var ext = Path.GetExtension(file.FileName);
                    var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    if (ext != ".pdf")
                    {
                        model.InfoType = CommonOutputType.Success;
                        model.MessageInfo = "Unsupported file type";
                        return model;
                    }

                    var imp = new ImpersonationHelper();
                    imp.Impersonate(FileManagerConfiguration.RepoUser
                        , FileManagerConfiguration.RepoPassword);

                    var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                        , FileManagerConfiguration.RepoPassword);
                    using (new NetworkConnectionHelper(FileManagerConfiguration.RepoTempPath, credential))
                    {
                        var tempUploadPath = string.Format(@"{0}\Upload", FileManagerConfiguration.RepoTempPath);
                        if (!Directory.Exists(tempUploadPath))
                        {
                            Directory.CreateDirectory(tempUploadPath);
                        }

                        var tempDestFolder = string.Format(@"{0}\{1}", tempUploadPath, "FakturPajakTerlapor");

                        if (!Directory.Exists(tempDestFolder))
                        {
                            Directory.CreateDirectory(tempDestFolder);
                        }

                        var destFileName = string.Format("{0}_{1}{2}", fileName, DateTime.Now.ToString("ddMMyyyyHHmmss"),
                            ext);

                        var path = string.Format(@"{0}\{1}", tempDestFolder, destFileName);
                        file.SaveAs(path);


                        model.InfoType = CommonOutputType.Success;
                        model.MessageInfo = "Upload Faktur successfully. ";
                        model.FilePath = path;
                    }
                }
                else
                {
                    model.InfoType = CommonOutputType.Error;
                    model.MessageInfo = "Empty File";
                }
            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                model.InfoType = CommonOutputType.Error;
                model.MessageInfo = "Upload Faktur failed. See Log with Key " + logKey + " for details.";
            }
            return model;
        }

        public static FileManagerCommonOutput UploadFakturPajakTerlapor(HttpPostedFileBase file, string userNameLogin)
        {
            var model = new FileManagerCommonOutput()
            {
                InfoType = CommonOutputType.Success,
                FilePath = string.Empty,
                IdNo = string.Empty,
                MessageInfo = string.Empty
            };
            try
            {
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    var ext = Path.GetExtension(file.FileName);
                    var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    if (ext != ".pdf")
                    {
                        model.InfoType = CommonOutputType.Success;
                        model.MessageInfo = "Unsupported file type";
                        return model;
                    }

                    var imp = new ImpersonationHelper();
                    imp.Impersonate(FileManagerConfiguration.RepoUser
                        , FileManagerConfiguration.RepoPassword);

                    var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                        , FileManagerConfiguration.RepoPassword);
                    using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                    {
                        var UploadPath = string.Format(@"{0}\Upload", FileManagerConfiguration.RepoRootPath);
                        if (!Directory.Exists(UploadPath))
                        {
                            Directory.CreateDirectory(UploadPath);
                        }

                        var DestFolder = string.Format(@"{0}\{1}", UploadPath, "FakturPajakTerlapor");

                        if (!Directory.Exists(DestFolder))
                        {
                            Directory.CreateDirectory(DestFolder);
                        }

                        var destFileName = string.Format("{0}_{1}{2}", fileName, DateTime.Now.ToString("ddMMyyyyHHmmss"),
                            ext);

                        var path = string.Format(@"{0}\{1}", DestFolder, destFileName);
                        file.SaveAs(path);


                        model.InfoType = CommonOutputType.Success;
                        model.MessageInfo = "Upload Faktur successfully. ";
                        model.FilePath = path;
                    }
                }
                else
                {
                    model.InfoType = CommonOutputType.Error;
                    model.MessageInfo = "Empty File";
                }
            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                model.InfoType = CommonOutputType.Error;
                model.MessageInfo = "Upload Faktur failed. See Log with Key " + logKey + " for details.";
            }
            return model;
        }

        public static FileManagerCommonOutput DeleteFileFakturPajakTerlapor(string FilePath)
        {
            var model = new FileManagerCommonOutput()
            {
                InfoType = CommonOutputType.Success,
                FilePath = string.Empty,
                IdNo = string.Empty,
                MessageInfo = string.Empty
            };
            try
            {
                if (!string.IsNullOrEmpty(FilePath))
                {
                    var imp = new ImpersonationHelper();
                    imp.Impersonate(FileManagerConfiguration.RepoUser
                        , FileManagerConfiguration.RepoPassword);

                    var credential = new NetworkCredential(FileManagerConfiguration.RepoUser
                        , FileManagerConfiguration.RepoPassword);
                    using (new NetworkConnectionHelper(FileManagerConfiguration.RepoRootPath, credential))
                    {
                        if (File.Exists(FilePath))
                        {
                            File.Delete(FilePath);
                            model.InfoType = CommonOutputType.Success;
                            model.MessageInfo = "Delete File successfully. ";
                        }
                    }

                }
                else
                {
                    model.InfoType = CommonOutputType.Error;
                    model.MessageInfo = "Empty File";
                }
            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                model.InfoType = CommonOutputType.Error;
                model.MessageInfo = "Delete file failed. See Log with Key " + logKey + " for details.";
            }
            return model;
        }

    }
}
