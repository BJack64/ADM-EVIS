using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;

namespace eFakturADM.Logic.Collections
{
    public class ExportCsvDomains : ApplicationCollection<ExportCsvDomain, SpBase>
    {
        public static List<ExportCsvDomain> GetReturFakturPajakSourceData(string noFaktur, string noDocRetur,
            DateTime? tglFakturReturStart, DateTime? tglFakturReturEnd,
            string npwpVendor, string namaVendor, int? masaPajak, int? tahunPajak,
            string fTglRetur, string fNpwpVendor, string fNamaVendor, string fNoFakturDiRetur, string fTglFaktur,
            string fNomorRetur, string fMasaRetur, string fTahunRetur, string fDpp, string fPpn, string fPpnBm, string fUserName)
        {

            var sp = new SpBase(@"EXEC sp_ReturPajakMasukanExportCsv 
                                    @NoFakturPajak, 
                                    @NoDocRetur, 
                                    @NpwpVendor, 
                                    @NamaVendor, 
                                    @TglFakturReturStart,
                                    @TglFakturReturEnd,                                     
                                    @MasaPajak, 
                                    @TahunPajak,
                                    @fTglRetur,
                                    @fNpwpVendor,
                                    @fNamaVendor,
                                    @fNoFakturDiRetur,
                                    @fTglFaktur,
                                    @fNomorRetur,
                                    @fMasaRetur,
                                    @fTahunRetur,
                                    @fDpp,
                                    @fPpn,
                                    @fPpnBm,
                                    @fUserName");
            sp.AddParameter("NoDocRetur", string.IsNullOrEmpty(noDocRetur) ? SqlString.Null : noDocRetur);
            sp.AddParameter("NoFakturPajak", string.IsNullOrEmpty(noFaktur) ? SqlString.Null : noFaktur);
            sp.AddParameter("NpwpVendor", string.IsNullOrEmpty(npwpVendor) ? SqlString.Null : npwpVendor);
            sp.AddParameter("NamaVendor", string.IsNullOrEmpty(namaVendor) ? SqlString.Null : namaVendor);
            sp.AddParameter("MasaPajak", masaPajak.HasValue ? masaPajak.Value : SqlInt32.Null);
            sp.AddParameter("TahunPajak", tahunPajak.HasValue ? tahunPajak.Value : SqlInt32.Null);
            sp.AddParameter("TglFakturReturStart", tglFakturReturStart.HasValue ? tglFakturReturStart.Value : SqlDateTime.Null);
            sp.AddParameter("tglFakturReturEnd", tglFakturReturEnd.HasValue ? tglFakturReturEnd.Value : SqlDateTime.Null);
            sp.AddParameter("fTglRetur", string.IsNullOrEmpty(fTglRetur) ? SqlString.Null : fTglRetur);
            sp.AddParameter("fNpwpVendor", string.IsNullOrEmpty(fNpwpVendor) ? SqlString.Null : fNpwpVendor);
            sp.AddParameter("fNamaVendor", string.IsNullOrEmpty(fNamaVendor) ? SqlString.Null : fNamaVendor);
            sp.AddParameter("fNoFakturDiRetur", string.IsNullOrEmpty(fNoFakturDiRetur) ? SqlString.Null : fNoFakturDiRetur);
            sp.AddParameter("fTglFaktur", string.IsNullOrEmpty(fTglFaktur) ? SqlString.Null : fTglFaktur);
            sp.AddParameter("fNomorRetur", string.IsNullOrEmpty(fNomorRetur) ? SqlString.Null : fNomorRetur);
            sp.AddParameter("fMasaRetur", string.IsNullOrEmpty(fMasaRetur) ? SqlString.Null : fMasaRetur);
            sp.AddParameter("fTahunRetur", string.IsNullOrEmpty(fTahunRetur) ? SqlString.Null : fTahunRetur);
            sp.AddParameter("fDpp", string.IsNullOrEmpty(fDpp) ? SqlString.Null : fDpp);
            sp.AddParameter("fPpn", string.IsNullOrEmpty(fPpn) ? SqlString.Null : fPpn);
            sp.AddParameter("fPpnBm", string.IsNullOrEmpty(fPpnBm) ? SqlString.Null : fPpnBm);
            sp.AddParameter("fUserName", string.IsNullOrEmpty(fUserName) ? SqlString.Null : fUserName);
            return GetApplicationCollection(sp);

        }

        public static List<ExportCsvDomain> GetFakturPajakSourceData(string noFaktur1, string noFaktur2, DateTime? tglFakturStart, DateTime? tglFakturEnd,
            string npwpVendor, string namaVendor, int? masaPajak, int? tahunPajak, string status,
            string sNpwpPenjual, string sNamaPenjual, string sNoFaktur, string sTglFaktur, string sMasaPajak, string sTahunPajak,
            string sDppString, string sPpnString, string sPpnBmString, string sStatusFaktur, DateTime? scanDateAwal, DateTime? scanDateAkhir, int? fillingIndex,
            int iFpType, string sFillingIndex, string sUserName, string source, string statusPayment, string remark, bool? createdCsv, 
            string sSource, string sStatusPayment, string sRemark, string sCreatedCsv, string StatusPelaporan, string sNamaPelaporan, string modifiedBy)
        {
            var sp = new SpBase(@"EXEC sp_PajakMasukanExportCsv 
		        @NoFakturPajak1, 
		        @NoFakturPajak2, 
		        @TglFakturStart, 
		        @TglFakturEnd,
		        @NpwpVendor, 
		        @NamaVendor, 
		        @MasaPajak, 
		        @TahunPajak, 
		        @Status, 
		        @sFormatedNpwpPenjual, 
		        @sNamaPenjual,
		        @sFormatedNoFaktur, 
		        @sTglFakturString,
		        @sMasaPajakName, 
		        @sTahunPajak, 
		        @sDppString, 
		        @sPpnString, 
		        @sPpnBmString, 
		        @sStatusFaktur, 
		        @scanDateAwal, 
                @scanDateAkhir, 
		        @fillingIndex, 
		        @dataType,
                @sFillingIndex,
                @sUserName,
                @Source,
                @StatusPayment,
                @Remark,
                @CreatedCsv,
                @fSource,
                @fStatusPayment,
                @fRemark,
                @fCreatedCsv,
                @StatusPelaporan,
                @sNamaPelaporan,
                @modifiedBy
        ");

            sp.AddParameter("NoFakturPajak1", string.IsNullOrEmpty(noFaktur1) ? SqlString.Null : noFaktur1);
            sp.AddParameter("NoFakturPajak2", string.IsNullOrEmpty(noFaktur2) ? SqlString.Null : noFaktur2);
            sp.AddParameter("NpwpVendor", string.IsNullOrEmpty(npwpVendor) ? SqlString.Null : npwpVendor);
            sp.AddParameter("NamaVendor", string.IsNullOrEmpty(namaVendor) ? SqlString.Null : namaVendor);
            sp.AddParameter("MasaPajak", masaPajak.HasValue ? masaPajak.Value : SqlInt32.Null);
            sp.AddParameter("TahunPajak", tahunPajak.HasValue ? tahunPajak.Value : SqlInt32.Null);
            sp.AddParameter("Status", string.IsNullOrEmpty(status) ? SqlString.Null : status);
            sp.AddParameter("TglFakturStart", tglFakturStart.HasValue ? tglFakturStart.Value : SqlDateTime.Null);
            sp.AddParameter("TglFakturEnd", tglFakturEnd.HasValue ? tglFakturEnd.Value : SqlDateTime.Null);

            sp.AddParameter("sFormatedNpwpPenjual", string.IsNullOrEmpty(sNpwpPenjual) ? SqlString.Null : sNpwpPenjual);
            sp.AddParameter("sNamaPenjual", string.IsNullOrEmpty(sNamaPenjual) ? SqlString.Null : sNamaPenjual);
            sp.AddParameter("sFormatedNoFaktur", string.IsNullOrEmpty(sNoFaktur) ? SqlString.Null : sNoFaktur);
            sp.AddParameter("sTglFakturString", string.IsNullOrEmpty(sTglFaktur) ? SqlString.Null : sTglFaktur);
            sp.AddParameter("sMasaPajakName", string.IsNullOrEmpty(sMasaPajak) ? SqlString.Null : sMasaPajak);
            sp.AddParameter("sTahunPajak", string.IsNullOrEmpty(sTahunPajak) ? SqlString.Null : sTahunPajak);
            sp.AddParameter("sDPPString", string.IsNullOrEmpty(sDppString) ? SqlString.Null : sDppString);
            sp.AddParameter("sPPNString", string.IsNullOrEmpty(sPpnString) ? SqlString.Null : sPpnString);
            sp.AddParameter("sPPNBMString", string.IsNullOrEmpty(sPpnBmString) ? SqlString.Null : sPpnBmString);
            sp.AddParameter("sStatusFaktur", string.IsNullOrEmpty(sStatusFaktur) ? SqlString.Null : sStatusFaktur);
            sp.AddParameter("scanDateAwal", scanDateAwal.HasValue ? scanDateAwal.Value : SqlDateTime.Null);
            sp.AddParameter("scanDateAkhir", scanDateAkhir.HasValue ? scanDateAkhir.Value : SqlDateTime.Null);
            sp.AddParameter("dataType",iFpType);
            sp.AddParameter("fillingIndex", fillingIndex.HasValue ? fillingIndex.Value : SqlInt32.Null);
            sp.AddParameter("sFillingIndex", string.IsNullOrEmpty(sFillingIndex) ? SqlString.Null : sFillingIndex);
            sp.AddParameter("sUserName", string.IsNullOrEmpty(sUserName) ? SqlString.Null : sUserName);

            sp.AddParameter("Source", string.IsNullOrEmpty(source) ? SqlString.Null : source);
            sp.AddParameter("StatusPayment", string.IsNullOrEmpty(statusPayment) ? SqlString.Null : statusPayment);
            sp.AddParameter("Remark", string.IsNullOrEmpty(remark) ? SqlString.Null : remark);
            sp.AddParameter("CreatedCsv", !createdCsv.HasValue ? SqlBoolean.Null : createdCsv.Value);
            sp.AddParameter("fSource", string.IsNullOrEmpty(sSource) ? SqlString.Null : sSource);
            sp.AddParameter("fStatusPayment", string.IsNullOrEmpty(sStatusPayment) ? SqlString.Null : sStatusPayment);
            sp.AddParameter("fRemark", string.IsNullOrEmpty(sRemark) ? SqlString.Null : sRemark);
            sp.AddParameter("StatusPelaporan", string.IsNullOrEmpty(StatusPelaporan) ? SqlString.Null : StatusPelaporan);
            sp.AddParameter("sNamaPelaporan", string.IsNullOrEmpty(sNamaPelaporan) ? SqlString.Null : sNamaPelaporan);
            sp.AddParameter("modifiedBy", string.IsNullOrEmpty(modifiedBy) ? SqlString.Null : modifiedBy);

            bool? isCreated = null;
            if (!string.IsNullOrEmpty(sCreatedCsv))
            {
                isCreated = sCreatedCsv.ToLowerInvariant() == "done";
            }

            sp.AddParameter("fCreatedCsv", !isCreated.HasValue ? SqlBoolean.Null : isCreated.Value);

            return GetApplicationCollection(sp);
        }


        public static List<ExportCsvDomain> ValidateFakturPajakSourceData(string noFaktur1, string noFaktur2, DateTime? tglFakturStart, DateTime? tglFakturEnd,
           string npwpVendor, string namaVendor, int? masaPajak, int? tahunPajak, string status,
           string sNpwpPenjual, string sNamaPenjual, string sNoFaktur, string sTglFaktur, string sMasaPajak, string sTahunPajak,
           string sDppString, string sPpnString, string sPpnBmString, string sStatusFaktur, DateTime? scanDateAwal, DateTime? scanDateAkhir, int? fillingIndex,
           int iFpType, string sFillingIndex, string sUserName, string source, string statusPayment, string remark, bool? createdCsv,
           string sSource, string sStatusPayment, string sRemark, string sCreatedCsv, string StatusPelaporan, string sNamaPelaporan)
        {
            var sp = new SpBase(@"EXEC sp_validation_PajakMasukanExportCsv 
		        @NoFakturPajak1, 
		        @NoFakturPajak2, 
		        @TglFakturStart, 
		        @TglFakturEnd,
		        @NpwpVendor, 
		        @NamaVendor, 
		        @MasaPajak, 
		        @TahunPajak, 
		        @Status, 
		        @sFormatedNpwpPenjual, 
		        @sNamaPenjual,
		        @sFormatedNoFaktur, 
		        @sTglFakturString,
		        @sMasaPajakName, 
		        @sTahunPajak, 
		        @sDppString, 
		        @sPpnString, 
		        @sPpnBmString, 
		        @sStatusFaktur, 
		        @scanDateAwal, 
                @scanDateAkhir, 
		        @fillingIndex, 
		        @dataType,
                @sFillingIndex,
                @sUserName,
                @Source,
                @StatusPayment,
                @Remark,
                @CreatedCsv,
                @fSource,
                @fStatusPayment,
                @fRemark,
                @fCreatedCsv,
                @StatusPelaporan,
                @sNamaPelaporan
        ");

            sp.AddParameter("NoFakturPajak1", string.IsNullOrEmpty(noFaktur1) ? SqlString.Null : noFaktur1);
            sp.AddParameter("NoFakturPajak2", string.IsNullOrEmpty(noFaktur2) ? SqlString.Null : noFaktur2);
            sp.AddParameter("NpwpVendor", string.IsNullOrEmpty(npwpVendor) ? SqlString.Null : npwpVendor);
            sp.AddParameter("NamaVendor", string.IsNullOrEmpty(namaVendor) ? SqlString.Null : namaVendor);
            sp.AddParameter("MasaPajak", masaPajak.HasValue ? masaPajak.Value : SqlInt32.Null);
            sp.AddParameter("TahunPajak", tahunPajak.HasValue ? tahunPajak.Value : SqlInt32.Null);
            sp.AddParameter("Status", string.IsNullOrEmpty(status) ? SqlString.Null : status);
            sp.AddParameter("TglFakturStart", tglFakturStart.HasValue ? tglFakturStart.Value : SqlDateTime.Null);
            sp.AddParameter("TglFakturEnd", tglFakturEnd.HasValue ? tglFakturEnd.Value : SqlDateTime.Null);

            sp.AddParameter("sFormatedNpwpPenjual", string.IsNullOrEmpty(sNpwpPenjual) ? SqlString.Null : sNpwpPenjual);
            sp.AddParameter("sNamaPenjual", string.IsNullOrEmpty(sNamaPenjual) ? SqlString.Null : sNamaPenjual);
            sp.AddParameter("sFormatedNoFaktur", string.IsNullOrEmpty(sNoFaktur) ? SqlString.Null : sNoFaktur);
            sp.AddParameter("sTglFakturString", string.IsNullOrEmpty(sTglFaktur) ? SqlString.Null : sTglFaktur);
            sp.AddParameter("sMasaPajakName", string.IsNullOrEmpty(sMasaPajak) ? SqlString.Null : sMasaPajak);
            sp.AddParameter("sTahunPajak", string.IsNullOrEmpty(sTahunPajak) ? SqlString.Null : sTahunPajak);
            sp.AddParameter("sDPPString", string.IsNullOrEmpty(sDppString) ? SqlString.Null : sDppString);
            sp.AddParameter("sPPNString", string.IsNullOrEmpty(sPpnString) ? SqlString.Null : sPpnString);
            sp.AddParameter("sPPNBMString", string.IsNullOrEmpty(sPpnBmString) ? SqlString.Null : sPpnBmString);
            sp.AddParameter("sStatusFaktur", string.IsNullOrEmpty(sStatusFaktur) ? SqlString.Null : sStatusFaktur);
            sp.AddParameter("scanDateAwal", scanDateAwal.HasValue ? scanDateAwal.Value : SqlDateTime.Null);
            sp.AddParameter("scanDateAkhir", scanDateAkhir.HasValue ? scanDateAkhir.Value : SqlDateTime.Null);
            sp.AddParameter("dataType", iFpType);
            sp.AddParameter("fillingIndex", fillingIndex.HasValue ? fillingIndex.Value : SqlInt32.Null);
            sp.AddParameter("sFillingIndex", string.IsNullOrEmpty(sFillingIndex) ? SqlString.Null : sFillingIndex);
            sp.AddParameter("sUserName", string.IsNullOrEmpty(sUserName) ? SqlString.Null : sUserName);

            sp.AddParameter("Source", string.IsNullOrEmpty(source) ? SqlString.Null : source);
            sp.AddParameter("StatusPayment", string.IsNullOrEmpty(statusPayment) ? SqlString.Null : statusPayment);
            sp.AddParameter("Remark", string.IsNullOrEmpty(remark) ? SqlString.Null : remark);
            sp.AddParameter("CreatedCsv", !createdCsv.HasValue ? SqlBoolean.Null : createdCsv.Value);
            sp.AddParameter("fSource", string.IsNullOrEmpty(sSource) ? SqlString.Null : sSource);
            sp.AddParameter("fStatusPayment", string.IsNullOrEmpty(sStatusPayment) ? SqlString.Null : sStatusPayment);
            sp.AddParameter("fRemark", string.IsNullOrEmpty(sRemark) ? SqlString.Null : sRemark);
            sp.AddParameter("StatusPelaporan", string.IsNullOrEmpty(StatusPelaporan) ? SqlString.Null : StatusPelaporan);
            sp.AddParameter("sNamaPelaporan", string.IsNullOrEmpty(sNamaPelaporan) ? SqlString.Null : sNamaPelaporan);

            bool? isCreated = null;
            if (!string.IsNullOrEmpty(sCreatedCsv))
            {
                isCreated = sCreatedCsv.ToLowerInvariant() == "done";
            }

            sp.AddParameter("fCreatedCsv", !isCreated.HasValue ? SqlBoolean.Null : isCreated.Value);

            return GetApplicationCollection(sp);
        }
    }
}
