using AutoMapper;
using eFakturADM.DJPService;
using eFakturADM.ExternalLib.Model;
using eFakturADM.ExternalLib.Model.Base;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Shared.Utility;
using eFakturADM.WebApi.Attributes;
using eFakturADM.WebApi.Controllers.Base;
using eFakturADM.WebApi.Models;
using eFakturADM.WebApi.Models.Base;
using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Security.Policy;
using System.Web;
using System.Web.Http;

namespace eFakturADM.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [CustomAuthorize]
    [CustomExceptionFilter]
    [Log]
    [RoutePrefix("api/faktur_pajak")]
    public class FakturPajakController : BaseApiController
    {
        /// <summary>
        /// 
        /// </summary>
        [Route]
        [HttpPost]
        public RequestResultModel Create(FakturPajakRequestModel input)
        {
            var result = new RequestResultModel()
            {
                Status = false,
                Message = ""
            };
            try
            {
                var getFakturPajak = FakturPajaks.GetFakturPajakNormalByNoFaktur(input.NoFakturPajak);
                if (getFakturPajak != null && getFakturPajak.FakturPajakId != default(long))
                {
                    result.Message = $"Faktur Pajak dengan NoFakturPajak = {input.NoFakturPajak} sudah direkam.";
                    return result;
                }
                var fakturPajak = Mapper.Map<FakturPajak>(input);
                fakturPajak.Created = DateTime.Now;
                fakturPajak.CreatedBy = UserAuth(Request)?.UserName;
                //fakturPajak.Status = !string.IsNullOrEmpty(input.StatusApproval) ? 2 : 3;
                fakturPajak.Status = (int)ApplicationEnums.StatusFakturPajak.Success;
                string Source = UserAuth(Request)?.UserInitial;

                if (String.IsNullOrEmpty(fakturPajak.JenisTransaksi) && String.IsNullOrEmpty(fakturPajak.JenisDokumen))
                {
                   //poin 61
                    //klo asalnya dari delvi , dia set fakturPajak.FPType = (int)FPTypeEnum.IWS;
                    //klo asalnya dari epay , dia set fakturPajak.FPType = (int)FPTypeEnum.NonIWS;
                    if (Source.ToLower().Equals("delvi"))
                        fakturPajak.FPType = (int)FPTypeEnum.IWS;
                    else if (Source.ToLower().Equals("epay"))
                        fakturPajak.FPType = (int)FPTypeEnum.NON_IWS;
                    else
                    fakturPajak.FPType = (int)FPTypeEnum.EXTERNAL;
                }
                else fakturPajak.FPType = (int)FPTypeEnum.KHUSUS;


                fakturPajak.ScanType = (int)ScanTypeEnum.EXTERNAL;
                fakturPajak.Source = Source;
                fakturPajak.FCode = "FM";
                fakturPajak.IsOutstanding = true;

                fakturPajak.IsDeleted = false; //default

                if(string.IsNullOrEmpty(fakturPajak.StatusFaktur) && !string.IsNullOrEmpty(fakturPajak.UrlScan))
                {
                    DjpServiceConfiguration.LoadConfig();

                    var inetProxy = DjpServiceConfiguration.InternetProxy;
                    var inetProxyPort = DjpServiceConfiguration.InternetProxyPort;
                    var inetProxyUseCredential = DjpServiceConfiguration.UseDefaultCredential;
                    var itimeoutsetting = DjpServiceConfiguration.DJPRequestTimeOutSetting;
                    var reqinterval = DjpServiceConfiguration.ServiceRequestDetailFakturPajakDjpRequestInterval;
                    //bool isUseProxy = !string.IsNullOrEmpty(inetProxy);

                    bool isUseProxy = false;


                    var getTimeOutSetting = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.DJPRequestTimeOutSetting);

                    int timeOutSettingInt;
                    int.TryParse(getTimeOutSetting.ConfigValue, out timeOutSettingInt);

                    WebExceptionStatus eStatus;
                    string logkey2;
                    var MsgErrorGetValidate = "";
                    var objXml = DJPLib.ValidateFakturLib.GetValidateFakturObjectV3(fakturPajak.UrlScan, timeOutSettingInt, isUseProxy, inetProxy, inetProxyPort, inetProxyUseCredential, out MsgErrorGetValidate, out eStatus, out logkey2);
                    fakturPajak.StatusFaktur = objXml.StatusFaktur;
                }

                if (fakturPajak.StatusFaktur.ToLower() == "faktur dibatalkan" || fakturPajak.StatusFaktur.ToLower() == "faktur diganti")
                {
                    //3. Perubahan API Posting untuk ada checking FP Diganti Dibatalkan Outstanding berdasarkan CertificateID
                    var getDataByCertificateID = FakturPajaks.GetByCertificateID(fakturPajak.CertificateID);
                    var FormatedNoFaktur = "";
                    if (getDataByCertificateID.Count > 0)
                    {
                        FormatedNoFaktur = getDataByCertificateID.OrderByDescending(a => a.Created).FirstOrDefault().FormatedNoFaktur;
                    }

                    var fpoutcheck = FakturPajakDigantiOutstandings.GetByFormatedNoFaktur(FormatedNoFaktur);
                    if (fpoutcheck == null || fpoutcheck.Id == 0)
                    {

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

                        //var tahunpajaktosave = fakturPajak.TahunPajak.Value;
                        //var masapajaktosave = fakturPajak.MasaPajak.Value;

                        var tahunpajaktocheck = fakturPajak.TglFaktur.Value.Year;
                        var masapajaktocheck = fakturPajak.TglFaktur.Value.Month;

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




                        var formattingFaktur = FormatingDomains.GetFormatNoFaktur(ApplicationEnums.FPType.ScanNonIws,
                                 fakturPajak.KdJenisTransaksi, fakturPajak.FgPengganti, fakturPajak.NoFakturPajak).FormattedField;
                        var fpoutstanding = new FakturPajakDigantiOutstanding()
                        {
                            Id = 0,
                            FormatedNoFaktur = formattingFaktur,
                            TahunPajak = tahunpajaktocheck,
                            MasaPajak = masapajaktocheck,
                            StatusApproval = fakturPajak.StatusApproval,
                            StatusFaktur = fakturPajak.StatusFaktur,
                            Keterangan = null,
                            KeteranganDjp = null,
                            StatusOutstanding = (int)fpoustandingstatus,
                            CreatedBy = UserAuth(Request)?.UserInitial
                        };

                        FakturPajakDigantiOutstandings.Save(fpoutstanding);
                        fakturPajak.MasaPajak = null;
                        fakturPajak.TahunPajak = null;
                    }
                    fakturPajak.IsDeleted = true;
                }
                //di comment dulu untuk publsih ke prod, belum waktunya naik
                else if (fakturPajak.StatusFaktur.ToLower() == "faktur pajak normal-pengganti")
                {
                    //poin  54
                    //else if statusfaktur = Faktur Pajak Normal-Pengganti
                    //dan tidak expired maka di set completed
                    var getDataByCertificateID = FakturPajaks.GetByCertificateID(fakturPajak.CertificateID);
                    var FormatedNoFaktur = "";
                    if (getDataByCertificateID.Count > 0)
                    {
                        FormatedNoFaktur = getDataByCertificateID.OrderByDescending(a => a.Created).FirstOrDefault().FormatedNoFaktur;
                    }

                    var fpoutcheck = FakturPajakDigantiOutstandings.GetByFormatedNoFaktur(FormatedNoFaktur);
                    if (fpoutcheck == null || fpoutcheck.Id == 0)
                    {

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
                                    maxpelaporan = int.Parse(dats[1].Replace("[", newValue: "").Replace("]", ""));
                                }
                                catch (Exception ex)
                                {
                                    Logger.WriteLog(out outlogkey, LogLevel.Warn, ex.Message, MethodBase.GetCurrentMethod(), ex);
                                }
                            }
                        }

                        //var tahunpajaktosave = fakturPajak.TahunPajak.Value;
                        //var masapajaktosave = fakturPajak.MasaPajak.Value;

                        var tahunpajaktocheck = fakturPajak.TglFaktur.Value.Year;
                        var masapajaktocheck = fakturPajak.TglFaktur.Value.Month;

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
                            else fpoustandingstatus = ApplicationEnums.StatusDigantiOutstanding.Complete;
                        }

                        //The parameterized query '(@StatusApproval nvarchar(38),@TahunPajak int,@StatusFaktur nvar' expects the parameter '@ModifiedBy', which was not supplied

                        var IdDigantiOutstanding = FakturPajakDigantiOutstandings.GetByFormatedNoFaktur(FormatedNoFaktur, 1).Id;

                        var fpoutstanding = new FakturPajakDigantiOutstanding()
                        {
                            Id = IdDigantiOutstanding,
                            FormatedNoFaktur = null,
                            TahunPajak = null,
                            MasaPajak = null,
                            StatusApproval = null,
                            StatusFaktur = null,
                            Keterangan = null,
                            KeteranganDjp = null,
                            StatusOutstanding = (int)fpoustandingstatus,
                            ModifiedBy = UserAuth(Request)?.UserInitial
                        };

                        FakturPajakDigantiOutstandings.Save(fpoutstanding);
                        fakturPajak.MasaPajak = null;
                        fakturPajak.TahunPajak = null;
                    }
                }

                var masterResult = FakturPajaks.Save(fakturPajak);

                if (masterResult.WasSaved && input.DetailFakturPajak != null)
                {
                    result.Status = FakturPajakDetails.BulkInsert(input.DetailFakturPajak.Select(x =>
                    {
                        var fakturPajakDetail = Mapper.Map<FakturPajakDetail>(x);
                        fakturPajakDetail.FakturPajakId = fakturPajak.FakturPajakId;
                        fakturPajakDetail.Created = DateTime.Now;
                        fakturPajakDetail.CreatedBy = UserAuth(Request)?.UserName;
                        return fakturPajakDetail;
                    }).ToList());
                }
                else
                    result.Status = masterResult.WasSaved;

                result.Message = "Faktur Pajak berhasil disimpan";
            }
            catch (Exception ex)
            {
                string outlogkey;
                Logger.WriteLog(out outlogkey, LogLevel.Error, "Unhandle exception", MethodBase.GetCurrentMethod(), ex);
                result.Message = "Unhandle Exception : " + ex?.Message;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        [Route]
        [HttpPatch]
        public RequestResultModel Update(FakturPajakUpdateModel input)
        {
            var result = new RequestResultModel()
            {
                Status = false,
                Message = ""
            };
            try
            {
                var getFakturPajak = FakturPajaks.GetFakturPajakNormalByNoFaktur(input.NoFakturPajak);
                if (getFakturPajak != null && getFakturPajak.FakturPajakId != default(long))
                {
                    if (input.ObjectId.HasValue)
                    {
                        if (getFakturPajak.ObjectID == null && getFakturPajak.FakturPajakTerlaporID != null)
                        {
                            var getFakturPajakLapor = FakturPajakTerlaporCollections.GetById(getFakturPajak.FakturPajakTerlaporID.Value);

                            var model = new UpdateCategoryMetadataRequest
                            {
                                ReportingDate = getFakturPajakLapor?.TanggalLapor.ToString("yyyy-MM-dd"),
                            };
                            //DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(UpdateCategoryMetadataRequest));
                            //MemoryStream msObj = new MemoryStream();
                            //js.WriteObject(msObj, model);
                            //msObj.Position = 0;
                            //StreamReader sr = new StreamReader(msObj);
                            //string payload = sr.ReadToEnd();

                            //sr.Close();
                            //msObj.Close();
                            var properties = from p in model.GetType().GetProperties()
                                             where p.GetValue(model, null) != null
                                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(model, null).ToString());

                            // queryString will be set to "Id=1&State=26&Prefix=f&Index=oo"                  
                            string queryString = String.Join("&", properties.ToArray()).Replace("ReportingDate", "339014_8");

                            var logEcm = new LogPostingTanggalLaporan
                            {
                                CreatedBy = UserAuth(Request)?.UserName,
                                Source = UserAuth(Request)?.UserName,
                                Url = $"{GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.EcmApiUrl)?.ConfigValue}/otcs/cs.exe/api/v1/nodes/{input.ObjectId}/categories/{(int)BaseCategoryRequestEnum.FAKTUR_PAJAK}",
                                Method = "PUT",
                                Payload = queryString,
                                Status = 0,
                            };

                            var saveLogEcm = LogPostingTanggalLaporans.Save(logEcm);

                            if (!saveLogEcm.WasSaved)
                                result.Message = $"Cannot process the update.";
                        }

                        getFakturPajak.ObjectID = input.ObjectId;
                    }

                    if (!string.IsNullOrEmpty(input.CertificateID))
                    {
                        getFakturPajak.CertificateID = input.CertificateID;
                    }

                    if (!string.IsNullOrEmpty(input.StatusPayment))
                    {
                        getFakturPajak.StatusPayment = input.StatusPayment;
                    }

                    if (!string.IsNullOrEmpty(input.Remark))
                    {
                        getFakturPajak.Remark = input.Remark;
                    }

                    var updateFakturPajak = FakturPajaks.Save(getFakturPajak);
                    result.Status = updateFakturPajak.WasSaved;
                    result.Message = "Faktur Pajak berhasil diupdate";
                }
                else
                {
                    result.Message = $"Faktur Pajak dengan NoFakturPajak = {input.NoFakturPajak} tidak ditemukan.";
                }
            }
            catch (Exception ex)
            {
                string outlogkey;
                Logger.WriteLog(out outlogkey, LogLevel.Error, "Unhandle exception", MethodBase.GetCurrentMethod(), ex);
                result.Message = "Unhandle Exception : " + ex?.Message;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        [Route]
        [HttpDelete]
        public RequestResultModel Delete(string noFakturPajak)
        {
            var result = new RequestResultModel()
            {
                Status = false,
                Message = ""
            };
            try
            {
                var getFakturPajak = FakturPajaks.GetFakturPajakByNoFaktur(noFakturPajak);
                if (getFakturPajak != null && getFakturPajak.FakturPajakTerlaporID == null && getFakturPajak.FakturPajakId != default(long))
                {
                    result.Status = FakturPajaks.Delete(getFakturPajak.FakturPajakId, UserAuth(Request)?.UserName);
                    if (result.Status)
                    {
                        result.Message = "Faktur Pajak berhasil dihapus";
                        var data  = FakturPajakDigantiOutstandings.GetByOriginalNoFaktur(noFakturPajak);
                        if (data.Count > 0)
                        {
                            FakturPajakDigantiOutstandings.Delete(data.FirstOrDefault().Id, UserAuth(Request)?.UserName);
                        }
                    }
                }
                else
                {
                    if (getFakturPajak != null && getFakturPajak.FakturPajakTerlaporID != null && getFakturPajak.FakturPajakId != default(long))
                        result.Message = $"Faktur Pajak sudah dilaporkan.";
                    else
                        result.Message = $"Faktur Pajak dengan NoFakturPajak = {noFakturPajak} tidak ditemukan.";
                }
            }
            catch (Exception ex)
            {
                string outlogkey;
                Logger.WriteLog(out outlogkey, LogLevel.Error, "Unhandle exception", MethodBase.GetCurrentMethod(), ex);
                result.Message = "Unhandle Exception : " + ex?.Message;
            }
            return result;
        }
    }
}