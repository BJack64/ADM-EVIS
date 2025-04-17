using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Transactions;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Shared.Utility;

namespace eFakturADM.DJPService
{
    public class RequestDetailTransaksiLib
    {

        public static void DoJob()
        {
            var inetProxy = DjpServiceConfiguration.InternetProxy;
            var inetProxyPort = DjpServiceConfiguration.InternetProxyPort;
            var inetProxyUseCredential = DjpServiceConfiguration.UseDefaultCredential;
            var itimeoutsetting = DjpServiceConfiguration.DJPRequestTimeOutSetting;
            var reqinterval = DjpServiceConfiguration.ServiceRequestDetailFakturPajakDjpRequestInterval;
            int loopidx = 1;
            var fptoproc = FakturPajakBatchRequestSettings.GetByBatchOrder(loopidx);
            while (fptoproc.Count > 0)
            {
                string outlogkey;
                int successcount;
                Logger.WriteLog(out outlogkey, LogLevel.Info, "[Loop-" + loopidx + "]Get " + fptoproc.Count.ToString() + " data", MethodBase.GetCurrentMethod());
                ProcessGetDetailTransaksiFakturPajak(fptoproc, itimeoutsetting, inetProxy, inetProxyPort,
                    inetProxyUseCredential, reqinterval, out successcount);
                Logger.WriteLog(out outlogkey, LogLevel.Info, "Success Get " + successcount.ToString() + " data", MethodBase.GetCurrentMethod());

                System.Threading.Thread.Sleep(DjpServiceConfiguration.ServiceRequestDetailFakturPajakProcessInterval);

                loopidx++;
                fptoproc = FakturPajakBatchRequestSettings.GetByBatchOrder(loopidx);

            }
        }

        private static void ProcessGetDetailTransaksiFakturPajak(List<FakturPajakBatchRequestSetting> fakturPajakToProcess, int iTimeOutSetting, string inetProxy
            , int? inetProxyPort, bool? inetProxyUseCredential, int reqinterval, out int successcount)
        {
            successcount = 0;
            bool isUseProxy = !string.IsNullOrEmpty(inetProxy);

            // update 15 Mei 2023 -- bug log 14 gb & process data tdk terupdate
            for (int i = 0; i < fakturPajakToProcess.Count; i++)
            {
                try
                {
                    WebExceptionStatus eStatus;
                    var urlScan = fakturPajakToProcess[i].UrlScan;
                    string msgError;
                    string logKey;
                    var objXml = DJPLib.ValidateFakturLib.GetValidateFakturObjectV3(urlScan,
                        iTimeOutSetting, isUseProxy, inetProxy, inetProxyPort
                        , inetProxyUseCredential, out msgError, out eStatus, out logKey);

                    if (eStatus != WebExceptionStatus.Success)
                    {
                        if (eStatus == WebExceptionStatus.Timeout)
                        {
                            i = fakturPajakToProcess.Count;

                            //skip loop
                        }
                    }
                    else
                    {
                        Exception ex;
                        string outlogkeys;

                        var rest = SaveDetailTransaksiToDatabase(objXml, fakturPajakToProcess[i], out ex);

                        if (!string.IsNullOrEmpty(rest))
                        {
                            string outlogkey;
                            Logger.WriteLog(out outlogkey, LogLevel.Error, rest, MethodBase.GetCurrentMethod(),
                                new Exception(rest));
                        }
                        else
                        {
                            successcount++;
                        }
                    }
                    System.Threading.Thread.Sleep(reqinterval);
                }
                catch (Exception exception)
                {
                    string outlogkey;
                    Logger.WriteLog(out outlogkey, LogLevel.Error, "Error Getting Request Detail Transaksi Faktur Pajak for Url : " + fakturPajakToProcess[i].UrlScan, MethodBase.GetCurrentMethod(), exception);
                }
            }
        }

        public static string SaveDetailTransaksiToDatabase(DJPLib.Objects.ResValidateFakturPm objData, FakturPajakBatchRequestSetting fpinput,  out Exception ex)
        {
            ex = null;
            var msgs = string.Empty;
            const string userName = DjpServiceConfiguration.Actor;
            
            try
            {
                TransactionOptions options = new TransactionOptions();
                options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                options.Timeout = new TimeSpan(0, 15, 0);
                using (var eScope = new TransactionScope(TransactionScopeOption.Required, options))
                {
                    
                    var fp = FakturPajaks.GetByUrlScan(fpinput.UrlScan.Trim());
                    if (fp == null || fp.FakturPajakId == 0) return "Skip";
                    if (fp.StatusFaktur != objData.StatusFaktur)
                    {
                        if (objData.StatusFaktur.ToLower() == "faktur diganti" || objData.StatusFaktur.ToLower() == "faktur dibatalkan")
                        {
                            if (fp.FgPengganti == "0")
                            {

                                var fpoutcheck =
                                    FakturPajakDigantiOutstandings.GetByFormatedNoFaktur(fp.FormatedNoFaktur);
                                if (fpoutcheck == null || fpoutcheck.Id == 0)
                                {
                                    if (fp.TahunPajak.HasValue && fp.MasaPajak.HasValue &&
                                        fp.TglFaktur.HasValue)
                                    {
                                        //belum ada
                                        //Add to FP Diganti Outstanding
                                        var tahunpajaktosave = fp.TahunPajak.Value;
                                        var masapajaktosave = fp.MasaPajak.Value;
                                        var tahunpajaktocheck = fp.TglFaktur.Value.Year;
                                        var masapajaktocheck = fp.TglFaktur.Value.Month;

                                        var fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Outstanding;

                                        var dtmin =
                                            new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
                                                DjpServiceConfiguration.MaxPelaporan);
                                        var dtmax =
                                            new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
                                                Math.Abs(DjpServiceConfiguration.MinPelaporan));
                                        var availableperiods = OpenClosePeriods.GetByRange(dtmin, dtmax);
                                        if (availableperiods.Count > 0)
                                        {
                                            //jika semua nya tidak ada yang open maka langsung expired
                                            var chkopencloseperiod =
                                                availableperiods.Where(c => c.StatusRegular).ToList();
                                            if (chkopencloseperiod.Count <= 0)
                                            {
                                                fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Expired;
                                            }
                                        }
                                        var fpoutstanding = new FakturPajakDigantiOutstanding()
                                        {
                                            Id = 0,
                                            FormatedNoFaktur = fp.FormatedNoFaktur,
                                            TahunPajak = tahunpajaktosave,
                                            MasaPajak = masapajaktosave,
                                            StatusApproval = objData.StatusApproval,
                                            StatusFaktur = objData.StatusFaktur,
                                            Keterangan = null,
                                            KeteranganDjp = null,
                                            StatusOutstanding = (int) fpoustandingstatus,
                                            CreatedBy = userName
                                        };

                                        FakturPajakDigantiOutstandings.Save(fpoutstanding);


                                        //fp.MasaPajak = null;
                                        //fp.TahunPajak = null;
                                        fp.StatusFaktur = objData.StatusFaktur;
                                        fp.IsDeleted = true;
                                        fp.ModifiedBy = userName;
                                        fp.Modified = DateTime.Now;
                                        FakturPajaks.Save(fp);
                                    }
                                }
                                if (fp.StatusReconcile == 2)
                                {
                                    fp.IsDeleted = false;
                                    fp.StatusFaktur = objData.StatusFaktur;
                                    fp.StatusReconcile = 4;
                                    fp.ModifiedBy = userName;
                                    fp.Modified = DateTime.Now;
                                    FakturPajaks.Save(fp);
                                }
                            }
                            else
                            {
                                //FP Normal Pengganti yang berubah Status Faktur menjadi Faktur Diganti
                                //get fp normal
                                var getfpnormal = FakturPajaks.GetFakturPajakNormal(fp.NoFakturPajak);
                                if (getfpnormal != null && getfpnormal.TglFaktur.HasValue &&
                                    getfpnormal.TahunPajak.HasValue && getfpnormal.MasaPajak.HasValue)
                                {

                                    var fpoutchek =
                                        FakturPajakDigantiOutstandings.GetByFormatedNoFaktur(
                                            getfpnormal.FormatedNoFaktur);
                                    if (fpoutchek == null || fpoutchek.Id == 0)
                                    {
                                        var tahunpajaktocheck = getfpnormal.TglFaktur.Value.Year;
                                        var masapajaktocheck = getfpnormal.TglFaktur.Value.Month;
                                        var tahunpajaktosave = getfpnormal.TahunPajak.Value;
                                        var masapajaktosave = getfpnormal.MasaPajak.Value;

                                        var fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Outstanding;

                                        var dtmin =
                                            new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
                                                DjpServiceConfiguration.MaxPelaporan);
                                        var dtmax =
                                            new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
                                                Math.Abs(DjpServiceConfiguration.MinPelaporan));
                                        var availableperiods = OpenClosePeriods.GetByRange(dtmin, dtmax);
                                        if (availableperiods.Count > 0)
                                        {
                                            //jika semua nya tidak ada yang open maka langsung expired
                                            var chkopencloseperiod =
                                                availableperiods.Where(c => c.StatusRegular).ToList();
                                            if (chkopencloseperiod.Count <= 0)
                                            {
                                                fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Expired;
                                            }
                                        }
                                        var fpoutstanding = new FakturPajakDigantiOutstanding()
                                        {
                                            Id = 0,
                                            FormatedNoFaktur = getfpnormal.FormatedNoFaktur,
                                            TahunPajak = tahunpajaktosave,
                                            MasaPajak = masapajaktosave,
                                            StatusApproval = getfpnormal.StatusApproval,
                                            StatusFaktur = getfpnormal.StatusFaktur,
                                            Keterangan = null,
                                            KeteranganDjp = null,
                                            StatusOutstanding = (int) fpoustandingstatus,
                                            CreatedBy = userName
                                        };

                                        FakturPajakDigantiOutstandings.Save(fpoutstanding);

                                        //getfpnormal.TahunPajak = null;
                                        //getfpnormal.MasaPajak = null;
                                        getfpnormal.IsDeleted = true;
                                        getfpnormal.Modified = DateTime.Now;
                                        getfpnormal.ModifiedBy = userName;

                                        FakturPajaks.Save(getfpnormal);

                                        //update Status Faktur FP Normal Pengganti sesuai dari DJP
                                        fp.StatusFaktur = objData.StatusFaktur;
                                        fp.StatusApproval = objData.StatusApproval;
                                        //fp.TahunPajak = null;
                                        //fp.MasaPajak = null;
                                        fp.IsDeleted = true;
                                        fp.Modified = DateTime.Now;
                                        fp.ModifiedBy = userName;
                                        FakturPajaks.Save(fp);
                                        
                                    }
                                    else
                                    {
                                        //sudah ada
                                        //dibalikin jadi outstanding atau expired lagi
                                        var tahunpajaktocheck = getfpnormal.TglFaktur.Value.Year;
                                        var masapajaktocheck = getfpnormal.TglFaktur.Value.Month;

                                        var fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Outstanding;

                                        var dtmin =
                                            new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
                                                DjpServiceConfiguration.MaxPelaporan);
                                        var dtmax =
                                            new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
                                                Math.Abs(DjpServiceConfiguration.MinPelaporan));
                                        var availableperiods = OpenClosePeriods.GetByRange(dtmin, dtmax);
                                        if (availableperiods.Count > 0)
                                        {
                                            //jika semua nya tidak ada yang open maka langsung expired
                                            var chkopencloseperiod =
                                                availableperiods.Where(c => c.StatusRegular).ToList();
                                            if (chkopencloseperiod.Count <= 0)
                                            {
                                                fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Expired;
                                            }
                                        }

                                        fpoutchek.StatusOutstanding = (int)fpoustandingstatus;
                                        fpoutchek.StatusApproval = getfpnormal.StatusApproval;
                                        fpoutchek.StatusFaktur = getfpnormal.StatusFaktur;
                                        fpoutchek.TahunPajak = getfpnormal.TahunPajak;
                                        fpoutchek.MasaPajak = getfpnormal.MasaPajak;
                                        fpoutchek.Modified = DateTime.Now;
                                        fpoutchek.ModifiedBy = userName;

                                        FakturPajakDigantiOutstandings.Save(fpoutchek);

                                        //Reset Masa-Tahun Pajak FP Normal
                                        //getfpnormal.TahunPajak = null;
                                        //getfpnormal.MasaPajak = null;
                                        getfpnormal.IsDeleted = true;
                                        getfpnormal.Modified = DateTime.Now;
                                        getfpnormal.ModifiedBy = userName;

                                        FakturPajaks.Save(getfpnormal);

                                        //update Status Faktur FP Normal Pengganti sesuai dari DJP
                                        //Reset Masa-Tahun Pajak FP Normal Pengganti yang sudah berubah menjadi Faktur Diganti
                                        fp.StatusFaktur = objData.StatusFaktur;
                                        fp.StatusApproval = objData.StatusApproval;
                                        //fp.TahunPajak = null;
                                        //fp.MasaPajak = null;
                                        fp.IsDeleted = true;
                                        fp.Modified = DateTime.Now;
                                        fp.ModifiedBy = userName;
                                        FakturPajaks.Save(fp);

                                    }
                                }
                            }
                            //FakturPajaks.UpdateStatusFakturDelvi(fp.FormatedNoFaktur, fp.StatusFaktur);
                        }
                        else
                        {
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
                            fp.Modified = DateTime.Now;

                            FakturPajaks.Save(fp);
                        }
                    }
                    else
                    {
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
                        fp.Modified = DateTime.Now;

                        FakturPajaks.Save(fp);
                    }

                    //Delimite FakturPajakDetail by FakturPajakId
                    FakturPajakDetails.DeleteByFakturPajakId(fp.FakturPajakId, userName);

                    #region -------------- Save Detail Transaksi --------------
                    if (fp.FakturPajakId > 0 && objData.DetailTransaksi != null && objData.DetailTransaksi.Count > 0)
                    {
                        foreach (var dataitem in objData.DetailTransaksi)
                        {
                            var item = dataitem;
                            var fakturPajakDetail = new FakturPajakDetail()
                            {
                                Nama = item.Nama,
                                HargaSatuan = Convert.ToDecimal(item.HargaSatuan),
                                JumlahBarang = Convert.ToDecimal(item.JumlahBarang),
                                HargaTotal = Convert.ToDecimal(item.HargaTotal),
                                Diskon = Convert.ToDecimal(item.Diskon),
                                Dpp = Convert.ToDecimal(item.Dpp),
                                Ppn = Convert.ToDecimal(item.Ppn),
                                TarifPpnbm = Convert.ToDecimal(item.TarifPpnbm),
                                Ppnbm = Convert.ToDecimal(item.Ppnbm),
                                CreatedBy = userName,
                                FakturPajakId = fp.FakturPajakId
                            };

                            FakturPajakDetails.Insert(fakturPajakDetail);
                        }
                       

                    }

                    #endregion

                    //insert ke log untuk di exclude ketika diproses
                    var logfp = new LogFPDigantiOutstanding()
                    {
                        FormatedNoFaktur = fp.FormatedNoFaktur
                    };

                    LogFPDigantiOutstandings.Add(logfp);

                    //update ProcessStatus di BatchSetting
                    FakturPajakBatchRequestSettings.UpdateStatus(fpinput.Id, 1);//done proses

                    eScope.Complete();
                    eScope.Dispose();

                }
            }
            catch (Exception exception)
            {
                ex = exception;
                msgs = "Save to Database Failed for Url : " + fpinput.UrlScan + ex.StackTrace+ ex.Message;
            }

            return msgs;
        }
        
    }
}
