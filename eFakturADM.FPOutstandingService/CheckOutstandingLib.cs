using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Transactions;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Shared.Utility;
using System.Reflection;

namespace eFakturADM.FPOutstandingService
{
    public class CheckOutstandingLib
    {
        public static void DoJob()
        {
            var inetProxy = FPOutstandingServiceConfiguration.InternetProxy;
            var inetProxyPort = FPOutstandingServiceConfiguration.InternetProxyPort;
            var inetProxyUseCredential = FPOutstandingServiceConfiguration.UseDefaultCredential;
            
            var itimeoutsetting = FPOutstandingServiceConfiguration.DJPRequestTimeOutSetting;
            var reqinterval = FPOutstandingServiceConfiguration.ServiceRequestDetailFakturPajakDjpRequestInterval;
            int loopidx = 1;
            var fptoproc = FPDigantiOutstandingBatchRequestSettings.GetByBatchOrder(loopidx);
            while (fptoproc.Count > 0)
            {
                string outlogkey;
                int successcount;
                
                Logger.WriteLog(out outlogkey, LogLevel.Info, "[Loop-" + loopidx + "]Get " + fptoproc.Count.ToString() + " data", MethodBase.GetCurrentMethod());
                DoProcess(fptoproc, itimeoutsetting, inetProxy, inetProxyPort,
                    inetProxyUseCredential, reqinterval, out successcount);
                Logger.WriteLog(out outlogkey, LogLevel.Info, "Success Get " + successcount.ToString() + " data", MethodBase.GetCurrentMethod());
                
                System.Threading.Thread.Sleep(FPOutstandingServiceConfiguration.ServiceRequestDetailFakturPajakProcessInterval);

                loopidx++;
                fptoproc = FPDigantiOutstandingBatchRequestSettings.GetByBatchOrder(loopidx);
            }
        }

        private static void DoProcess(List<FPDigantiOutstandingBatchRequestSetting> fakturPajakToProcess, int iTimeOutSetting, string inetProxy
            , int? inetProxyPort, bool? inetProxyUseCredential, int reqinterval, out int successcount)
        {
            successcount = 0;
            bool isUseProxy = !string.IsNullOrEmpty(inetProxy);
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
                        var rest = SaveToDatabase(objXml, fakturPajakToProcess[i], out ex);
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
                    Logger.WriteLog(out outlogkey, LogLevel.Error, "Error Getting Request Faktur Pajak for Url : " + fakturPajakToProcess[i].UrlScan, MethodBase.GetCurrentMethod(), exception);
                }
            }
        }

        /// <summary>
        /// Hanya memproses MasaPajak-TahunPajak yang StatusReguler nya Open
        /// </summary>
        /// <param name="objData"></param>
        /// <param name="fp"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static string SaveToDatabase(DJPLib.Objects.ResValidateFakturPm objData, FPDigantiOutstandingBatchRequestSetting fpinput, out Exception ex)
        {
            ex = null;
            var msgs = string.Empty;
            const string userName = FPOutstandingServiceConfiguration.Actor;

            try
            {
                using (var eScope = new TransactionScope())
                {
                    var fp = FakturPajaks.GetByUrlScan(fpinput.UrlScan.Trim());
                    if (fp == null || fp.FakturPajakId == 0) return "Skip";
                    if (fp.StatusFaktur != objData.StatusFaktur)
                    {
                        if (objData.StatusFaktur.ToLower() == "faktur diganti" || objData.StatusFaktur.ToLower() == "faktur dibatalkan")
                        {
                            if (fp.FgPengganti == "0")
                            {
                                //check to fp diganti outstanding
                                var fpoutcheck =
                                    FakturPajakDigantiOutstandings.GetByFormatedNoFaktur(fp.FormatedNoFaktur);
                                if (fpoutcheck == null && fp.TahunPajak.HasValue && fp.MasaPajak.HasValue &&
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
                                            FPOutstandingServiceConfiguration.MaxPelaporan);
                                    var dtmax =
                                        new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
                                            Math.Abs(FPOutstandingServiceConfiguration.MinPelaporan));
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
                            else
                            {
                                //FP Normal Pengganti yang berubah status menjadi Faktur Diganti
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
                                                FPOutstandingServiceConfiguration.MaxPelaporan);
                                        var dtmax =
                                            new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
                                                Math.Abs(FPOutstandingServiceConfiguration.MinPelaporan));
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
                                        fp.Modified = DateTime.Now;
                                        fp.ModifiedBy = userName;
                                        //fp.TahunPajak = null;
                                        //fp.MasaPajak = null;
                                        fp.IsDeleted = true;
                                        FakturPajaks.Save(fp);
                                    }
                                    else
                                    {
                                        //sudah ada
                                        //dibalikin jadi outstanding atau expired lagi
                                        var tahunpajaktocheck = getfpnormal.TglFaktur.Value.Year;
                                        var masapajaktocheck = getfpnormal.TglFaktur.Value.Month;

                                        var fpoustandingstatus =
                                            ApplicationEnums.StatusDigantiOutstanding.Outstanding;

                                        var dtmin =
                                            new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
                                                FPOutstandingServiceConfiguration.MaxPelaporan);
                                        var dtmax =
                                            new DateTime(tahunpajaktocheck, masapajaktocheck, 1).AddMonths(
                                                Math.Abs(FPOutstandingServiceConfiguration.MinPelaporan));
                                        var availableperiods = OpenClosePeriods.GetByRange(dtmin, dtmax);
                                        if (availableperiods.Count > 0)
                                        {
                                            //jika semua nya tidak ada yang open maka langsung expired
                                            var chkopencloseperiod =
                                                availableperiods.Where(c => c.StatusRegular).ToList();
                                            if (chkopencloseperiod.Count <= 0)
                                            {
                                                fpoustandingstatus =
                                                    ApplicationEnums.StatusDigantiOutstanding.Expired;
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
                        }
                    }

                    //insert ke log untuk di exclude ketika diproses
                    var logfp = new LogFPDigantiOutstanding()
                    {
                        FormatedNoFaktur = fp.FormatedNoFaktur
                    };

                    LogFPDigantiOutstandings.Add(logfp);

                    //update ProcessStatus di BatchSetting
                    FPDigantiOutstandingBatchRequestSettings.UpdateStatus(fpinput.Id, 1);//done proses

                    eScope.Complete();
                    eScope.Dispose();
                }
            }
            catch (Exception exception)
            {
                ex = exception;
                msgs = "Save to Database Failed for Url : " + fpinput.UrlScan;
            }

            return msgs;
        }

        //public static void CheckExpiredOutstanding()
        //{
        //    try
        //    {
        //        FakturPajakDigantiOutstandings.SetAllExpired(FPOutstandingServiceConfiguration.Actor);
        //    }
        //    catch (Exception ex)
        //    {
        //        string outlogkey;
        //        Logger.WriteLog(out outlogkey, LogLevel.Error, ex.Message, MethodBase.GetCurrentMethod(), ex);
        //    }
        //}
    }
}
