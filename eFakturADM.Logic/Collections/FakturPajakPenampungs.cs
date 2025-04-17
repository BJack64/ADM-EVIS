using System;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using System.Collections.Generic;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Collections
{
    public class FakturPajakPenampungs : ApplicationCollection<FakturPajakPenampung, SpBase>
    {

        public static List<FakturPajakPenampung> Get()
        {
            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakPenampungId) OVER() AS TotalItems
FROM	dbo.View_FakturPajakPenampung fp
WHERE	fp.IsDeleted = 0");
            return GetApplicationCollection(sp);
        }

        public static FakturPajakPenampung GetById(long fakturPajakPenampungId)
        {
            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakPenampungId) OVER() AS TotalItems
FROM	dbo.View_FakturPajakPenampung fp
WHERE fp.FakturPajakPenampungId = @FakturPajakPenampungId");
            sp.AddParameter("FakturPajakPenampungId", fakturPajakPenampungId);
            var dbData = GetApplicationObject(sp);

            return dbData == null || dbData.FakturPajakPenampungId == 0 ? null : dbData;

        }

        public static List<FakturPajakPenampung> GetByFormatedNoFaktur(string formatedNoFaktur)
        {
            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakPenampungId) OVER() AS TotalItems
FROM	dbo.View_FakturPajakPenampung fp
WHERE fp.IsDeleted = 0 AND fp.FormatedNoFaktur = @formatedNoFaktur");
            sp.AddParameter("formatedNoFaktur", formatedNoFaktur);

            var dbData = GetApplicationCollection(sp);

            return dbData;

        }

        public static FakturPajakPenampung GetFakturPajakNormal(string nomorFaktur)
        {
            var sp = new SpBase(@"SELECT	fp.*
		, CAST(1 AS int) AS TotalItems
FROM	dbo.View_FakturPajakPenampung fp
WHERE fp.IsDeleted = 0 AND fp.NoFakturPajak = @NoFakturPajak AND fp.FgPengganti = '0'");
            sp.AddParameter("NoFakturPajak", nomorFaktur);
            var d = GetApplicationObject(sp);
            return d;
        }

        public static List<FakturPajakPenampung> GetStatusPayment()
        {
            var sp = new SpBase(@"SELECT fp.StatusPayment FROM dbo.View_FakturPajakPenampung fp WHERE fp.IsDeleted = 0 GROUP BY fp.StatusPayment");
            var d = GetApplicationCollection(sp);
            return d;
        }

        public static List<FakturPajakPenampung> GetByFormatedNoFakturFpKhusus(string formatedNoFaktur)
        {
            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakPenampungId) OVER() AS TotalItems
FROM	dbo.View_FakturPajakPenampung fp
WHERE fp.IsDeleted = 0 AND fp.FPType = 3 AND fp.FormatedNoFaktur = @formatedNoFaktur");
            sp.AddParameter("formatedNoFaktur", formatedNoFaktur);

            var dbData = GetApplicationCollection(sp);

            return dbData;

        }

        public static List<FakturPajakPenampung> GetFpKhususPenggantiByNoFaktur(string formatedNoFaktur)
        {
            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakPenampungId) OVER() AS TotalItems
FROM	dbo.View_FakturPajakPenampung fp
WHERE fp.IsDeleted = 0 AND fp.FPType = 3 AND fp.NoFakturYangDiganti = @formatedNoFaktur");
            sp.AddParameter("formatedNoFaktur", formatedNoFaktur);

            var dbData = GetApplicationCollection(sp);

            return dbData;
        }

        public static List<FakturPajakPenampung> GetByOriginalNoFaktur(string originalNoFaktur)
        {
            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakPenampungId) OVER() AS TotalItems
FROM	dbo.View_FakturPajakPenampung fp
WHERE fp.IsDeleted = 0 AND fp.NoFakturPajak = @NoFakturPajak");
            sp.AddParameter("NoFakturPajak", originalNoFaktur);

            var dbData = GetApplicationCollection(sp);

            return dbData;
        }

        public static List<FakturPajakPenampung> GetSpecificFakturPajak(ApplicationEnums.FPType eFpType, string originalNoFaktur,
            string kdJenisTransaksi, string fgPengganti)
        {
            var sp = new SpBase(@"SELECT	fp.*, COUNT(fp.FakturPajakPenampungId) OVER() AS TotalItems
FROM	dbo.View_FakturPajakPenampung fp
WHERE	IsDeleted = 0 AND FormatedNoFaktur = dbo.FormatNoFaktur(@fpType, @originalNoFaktur, @kdJenisTransaksi, @fgPengganti)
ORDER BY fp.FakturPajakPenampungId DESC");

            sp.AddParameter("fpType", (int)eFpType);
            sp.AddParameter("originalNoFaktur", originalNoFaktur);
            sp.AddParameter("kdJenisTransaksi", kdJenisTransaksi);
            sp.AddParameter("fgPengganti", fgPengganti);

            var dbData = GetApplicationCollection(sp);
            return dbData;
        }

        public static List<FakturPajakPenampung> GetListToDownload()
        {
            //Status Faktur Pajak yang bisa didownload kan oleh service hanya untuk status 1 (Scanned) & 3 (Error Request)
            var lstStats = new List<int>
            {
                (int) ApplicationEnums.StatusFakturPajak.Scanned,
                (int) ApplicationEnums.StatusFakturPajak.ErrorRequest
            };

            var stats = string.Join(",", lstStats);

            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakPenampungId) OVER() AS TotalItems
FROM	dbo.View_FakturPajakPenampung fp
WHERE fp.IsDeleted = 0 AND fp.UrlScan IS NOT NULL AND fp.[Status] IN (SELECT Data FROM dbo.Split(@stats))");

            sp.AddParameter("stats", stats);

            return GetApplicationCollection(sp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fakturPajakPenampungIds">Multiple Faktur Pajak Id, separated by comma</param>
        /// <returns></returns>
        public static List<FakturPajakPenampung> GetByIds(string fakturPajakPenampungIds)
        {
            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakPenampungId) OVER() AS TotalItems
FROM	dbo.View_FakturPajakPenampung fp
  WHERE fp.FakturPajakPenampungId IN (SELECT Data FROM dbo.Split(@fakturPajakPenampungIds))");

            sp.AddParameter("fakturPajakPenampungIds", fakturPajakPenampungIds);

            return GetApplicationCollection(sp);
        }

        public static List<FakturPajakPenampung> GetScanBulk(Filter filter, out int totalItems, ApplicationEnums.FPType eFpType,
            int masaPajak, int tahunPajak, DateTime? receivingDate)
        {
            var iFpType = (int)eFpType;
            if (filter.Search != null)
                filter.Search = filter.Search.Trim();
            var sortOrder = filter.SortOrderAsc ? "ASC" : "DESC";
            totalItems = 0;

            var sp = new SpBase(string.Format(@"SELECT	fp.*
		, COUNT(fp.FakturPajakPenampungId) OVER() AS TotalItems
FROM	dbo.View_FakturPajakPenampung fp
  WHERE fp.[Status] <> 2 AND fp.[IsDeleted] = 0 AND [ScanType] = 2
		AND [FPType] = @fpType 
		AND	[MasaPajak] = @masaPajak
		AND [TahunPajak] = @tahunPajak
        AND (@receivingDate IS NULL OR (@receivingDate IS NOT NULL AND CAST([ReceivingDate] as date) = CAST(@receivingDate as date)))
  ORDER BY {0} {1}
OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
FETCH NEXT @ItemPerPage ROWS ONLY", filter.SortColumnName, sortOrder));

            sp.AddParameter("fpType", iFpType);
            sp.AddParameter("masaPajak", masaPajak);
            sp.AddParameter("tahunPajak", tahunPajak);
            sp.AddParameter("CurrentPage", filter.CurrentPage);
            sp.AddParameter("ItemPerPage", filter.ItemsPerPage);
            sp.AddParameter("receivingDate", receivingDate.HasValue ? receivingDate.Value : SqlDateTime.Null);
            //sp.AddParameter("createdBy", createdBy);

            var data = GetApplicationCollection(sp);

            if (data.Count > 0)
                totalItems = data[0].TotalItems;

            if (totalItems == 0 && filter.CurrentPage > 1)
            {
                filter.CurrentPage--;
                data = GetScanBulk(filter, out totalItems, eFpType, masaPajak, tahunPajak, receivingDate);
            }
            else if (totalItems > 0 && totalItems < (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = (totalItems <= filter.ItemsPerPage) ? 1 : (totalItems / filter.ItemsPerPage);
                data = GetScanBulk(filter, out totalItems, eFpType, masaPajak, tahunPajak, receivingDate);
            }
            else if (totalItems > 0 && totalItems == (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = 1;
                data = GetScanBulk(filter, out totalItems, eFpType, masaPajak, tahunPajak, receivingDate);
            }

            return data;
        }

        public static List<FakturPajakPenampung> GetScanBulkToSubmit(ApplicationEnums.FPType eFpType,
            int masaPajak, int tahunPajak, DateTime? receivingDate)
        {
            var iFpType = (int)eFpType;
            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakPenampungId) OVER() AS TotalItems
FROM	dbo.View_FakturPajakPenampung fp
  WHERE fp.[Status] <> 2 AND fp.[IsDeleted] = 0 AND [ScanType] = 2
		AND [FPType] = @fpType 
		AND	[MasaPajak] = @masaPajak
		AND [TahunPajak] = @tahunPajak
        AND (@receivingDate IS NULL OR (@receivingDate IS NOT NULL AND CAST([ReceivingDate] as date) = CAST(@receivingDate as date)))");

            sp.AddParameter("fpType", iFpType);
            sp.AddParameter("masaPajak", masaPajak);
            sp.AddParameter("tahunPajak", tahunPajak);
            sp.AddParameter("receivingDate", receivingDate.HasValue ? receivingDate.Value : SqlDateTime.Null);
            //sp.AddParameter("createdBy", createdBy);

            var data = GetApplicationCollection(sp);

            return data;
        }

        //        public static FakturPajak GetByUrlScanSuccess(string urlScan)
        //        {
        //            var sp = new SpBase(@"SELECT	fp.*
        //		, COUNT(fp.FakturPajakId) OVER() AS TotalItems
        //FROM	dbo.View_FakturPajak fp
        //  WHERE LOWER(fp.[UrlScan]) = LOWER(@UrlScan) AND IsDeleted = 0 AND [Status] = 2");
        //            sp.AddParameter("UrlScan", urlScan);
        //            var dbData = GetApplicationObject(sp);
        //            if (dbData == null || dbData.FakturPajakId == 0) return null;
        //            return dbData;
        //        }

        public static FakturPajakPenampung GetByUrlScan(string urlScan)
        {
            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakPenampungId) OVER() AS TotalItems
FROM	dbo.View_FakturPajakPenampung fp
  WHERE LTRIM(RTRIM(LOWER(fp.[UrlScan]))) = LTRIM(RTRIM(LOWER(@UrlScan))) AND IsDeleted = 0");
            sp.AddParameter("UrlScan", urlScan);
            var dbData = GetApplicationObject(sp);
            if (dbData == null || dbData.FakturPajakPenampungId == 0) return null;
            return dbData;
        }

        //        public static FakturPajak GetScannedDataByUrlScan(string urlScan)
        //        {
        //            var sp = new SpBase(@"SELECT	fp.*
        //		, COUNT(fp.FakturPajakId) OVER() AS TotalItems
        //FROM	dbo.View_FakturPajak fp
        //  WHERE LOWER(fp.[UrlScan]) = LOWER(@UrlScan) AND IsDeleted = 0 AND [Status] = 1");
        //            sp.AddParameter("UrlScan", urlScan);
        //            var dbData = GetApplicationObject(sp);
        //            if (dbData == null || dbData.FakturPajakId == 0) return null;
        //            return dbData;
        //        }

        public static List<FakturPajakPenampung> GetList(Filter filter, out int totalItems, string noFaktur1, string noFaktur2, string npwpVendor, string namaVendor,
            DateTime? tglFakturStart, DateTime? tglFakturEnd, string status, DateTime? scanDateAwal, DateTime? scanDateAkhir, string source, string remark,
            string sNpwpPenjual, string sNamaPenjual, string sNoFaktur, string sTglFaktur ,
            string sDppString, string sPpnString, string sPpnBmString, string sStatusFaktur, string sUserName, string sSource, string sStatusPayment, string sRemark)
        {
            if (filter.Search != null)
                filter.Search = filter.Search.Trim();
            var sortOrder = filter.SortOrderAsc ? "ASC" : "DESC";
            totalItems = 0;

            var sp = new SpBase(string.Format(@"SELECT	fp.*
		, COUNT(fp.FakturPajakPenampungId) OVER() AS TotalItems
FROM	dbo.View_FakturPajakPenampung fp
WHERE fp.[IsDeleted] = 0
AND [Status] = 2
AND (
	(@scanDateAwal IS NULL AND @scanDateAkhir IS NULL)
	OR (@scanDateAwal IS NOT NULL AND @scanDateAkhir IS NULL AND CAST([Created] AS date) = CAST(@scanDateAwal AS date))
	OR (@scanDateAwal IS NULL AND @scanDateAkhir IS NOT NULL AND CAST([Created] AS date) = CAST(@scanDateAkhir AS date))
	OR (@scanDateAwal IS NOT NULL AND @scanDateAkhir IS NOT NULL AND CAST([Created] AS date) >= CAST(@scanDateAwal AS date) AND CAST([Created] AS date) <= CAST(@scanDateAkhir AS date))
)
AND (
	(@NoFakturPajak1 IS NULL AND @NoFakturPajak2 IS NULL)
	OR (@NoFakturPajak1 IS NULL AND @NoFakturPajak2 IS NOT NULL 
	AND REPLACE(REPLACE([FormatedNoFaktur], '.', ''), '-','') LIKE REPLACE(REPLACE(REPLACE(@NoFakturPajak2, '-',''),'.',''),'*','%'))
	OR (@NoFakturPajak1 IS NOT NULL AND @NoFakturPajak2 IS NULL AND REPLACE(REPLACE([FormatedNoFaktur], '.', ''), '-','') LIKE REPLACE(REPLACE(REPLACE(@NoFakturPajak1, '-',''),'.',''), '*', '%'))
	OR (@NoFakturPajak1 IS NOT NULL AND @NoFakturPajak2 IS NOT NULL 
	AND REPLACE(REPLACE([FormatedNoFaktur], '.', ''), '-','') >= REPLACE(REPLACE(@NoFakturPajak1, '-',''),'.','') 
	AND REPLACE(REPLACE([FormatedNoFaktur], '.', ''), '-','') <= REPLACE(REPLACE(@NoFakturPajak2, '-',''),'.',''))
)
AND (@NpwpVendor IS NULL OR
	(@NpwpVendor IS NOT NULL AND LOWER(REPLACE(REPLACE([FormatedNpwpPenjual], '.', ''), '-', '')) LIKE REPLACE(LOWER(REPLACE(REPLACE(@NpwpVendor, '.',''),'-','')), '*', '%'))
	)
AND (
	(@NamaVendor IS NOT NULL AND LOWER([NamaPenjual]) LIKE REPLACE(LOWER(@NamaVendor), '*', '%'))
	OR @NamaVendor IS NULL
	)
AND ((
		CAST([TglFaktur] AS DATE) BETWEEN CAST(ISNULL(@TglFakturStart, [TglFaktur]) AS DATE) AND CAST(ISNULL(@TglFakturEnd, [TglFaktur]) AS DATE)
	) OR @TglFakturStart IS NULL OR @TglFakturEnd IS NULL)
AND (@sFormatedNpwpPenjual IS NULL OR (@sFormatedNpwpPenjual IS NOT NULL AND FormatedNpwpPenjual LIKE REPLACE(@sFormatedNpwpPenjual,'*','%')))
		AND (@sNamaPenjual IS NULL OR (@sNamaPenjual IS NOT NULL AND NamaPenjual LIKE REPLACE(@sNamaPenjual,'*','%')))
		AND (@sFormatedNoFaktur IS NULL OR (@sFormatedNoFaktur IS NOT NULL AND FormatedNoFaktur LIKE REPLACE(@sFormatedNoFaktur,'*','%')))
		AND (@sTglFakturString IS NULL OR (@sTglFakturString IS NOT NULL AND CONVERT(VARCHAR,TglFaktur, 103) LIKE REPLACE(@sTglFakturString,'*','%')))
		AND (@sDPPString IS NULL OR (@sDPPString IS NOT NULL AND CAST(JumlahDPP AS nvarchar) LIKE REPLACE(@sDPPString, '*','%')))
		AND (@sPPNString IS NULL OR (@sPPNString IS NOT NULL AND CAST(JumlahPPN AS nvarchar) LIKE REPLACE(@sPPNString, '*', '%')))
		AND (@sPPNBMString IS NULL OR (@sPPNBMString IS NOT NULL AND CAST(JumlahPPNBM AS nvarchar) LIKE REPLACE(@sPPNBMString, '*', '%')))
		AND (@sStatusFaktur IS NULL OR (@sStatusFaktur IS NOT NULL AND StatusFaktur LIKE REPLACE(@sStatusFaktur,'*','%')))
        AND (@sUserName IS NULL OR (@sUserName IS NOT NULL AND CreatedBy LIKE REPLACE(@sUserName,'*','%')))
        AND (@sSource IS NULL OR (@sSource IS NOT NULL AND Source LIKE REPLACE(@sSource,'*','%')))
        AND (@Source IS NULL OR (@Source IS NOT NULL AND Source LIKE REPLACE(@Source,'All','%')))
        AND (@sStatusPayment IS NULL OR (@sStatusPayment IS NOT NULL AND StatusPayment LIKE REPLACE(@sStatusPayment,'*','%')))
        AND (@sRemark IS NULL OR (@sRemark IS NOT NULL AND Remark LIKE REPLACE(@sRemark,'*','%')))
        AND (StatusFaktur <> 'Faktur Diganti' OR StatusFaktur IS NULL)
ORDER BY {0} {1}
OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
FETCH NEXT @ItemPerPage ROWS ONLY
OPTION (OPTIMIZE FOR UNKNOWN)", filter.SortColumnName, sortOrder));

            sp.AddParameter("CurrentPage", filter.CurrentPage);
            sp.AddParameter("ItemPerPage", filter.ItemsPerPage);
            sp.AddParameter("NoFakturPajak1", string.IsNullOrEmpty(noFaktur1) ? SqlString.Null : noFaktur1);
            sp.AddParameter("NoFakturPajak2", string.IsNullOrEmpty(noFaktur2) ? SqlString.Null : noFaktur2);
            sp.AddParameter("NpwpVendor", string.IsNullOrEmpty(npwpVendor) ? SqlString.Null : npwpVendor);
            sp.AddParameter("NamaVendor", string.IsNullOrEmpty(namaVendor) ? SqlString.Null : namaVendor);
            sp.AddParameter("TglFakturStart", tglFakturStart.HasValue ? tglFakturStart.Value : SqlDateTime.Null);
            sp.AddParameter("TglFakturEnd", tglFakturEnd.HasValue ? tglFakturEnd.Value : SqlDateTime.Null);
            sp.AddParameter("Status", string.IsNullOrEmpty(status) ? SqlString.Null : status);
            sp.AddParameter("scanDateAwal", scanDateAwal.HasValue ? scanDateAwal.Value : SqlDateTime.Null);
            sp.AddParameter("scanDateAkhir", scanDateAkhir.HasValue ? scanDateAkhir.Value : SqlDateTime.Null);
            sp.AddParameter("Source", string.IsNullOrEmpty(source) ? SqlString.Null : source);
            sp.AddParameter("Remark", string.IsNullOrEmpty(remark) ? SqlString.Null : source);

            sp.AddParameter("sFormatedNpwpPenjual", string.IsNullOrEmpty(sNpwpPenjual) ? SqlString.Null : sNpwpPenjual);
            sp.AddParameter("sNamaPenjual", string.IsNullOrEmpty(sNamaPenjual) ? SqlString.Null : sNamaPenjual);
            sp.AddParameter("sFormatedNoFaktur", string.IsNullOrEmpty(sNoFaktur) ? SqlString.Null : sNoFaktur);
            sp.AddParameter("sTglFakturString", string.IsNullOrEmpty(sTglFaktur) ? SqlString.Null : sTglFaktur);
            sp.AddParameter("sDPPString", string.IsNullOrEmpty(sDppString) ? SqlString.Null : sDppString);
            sp.AddParameter("sPPNString", string.IsNullOrEmpty(sPpnString) ? SqlString.Null : sPpnString);
            sp.AddParameter("sPPNBMString", string.IsNullOrEmpty(sPpnBmString) ? SqlString.Null : sPpnBmString);
            sp.AddParameter("sStatusFaktur", string.IsNullOrEmpty(sStatusFaktur) ? SqlString.Null : sStatusFaktur);
            sp.AddParameter("sUserName", string.IsNullOrEmpty(sUserName) ? SqlString.Null : sUserName);
            sp.AddParameter("sSource", string.IsNullOrEmpty(sSource) ? SqlString.Null : sSource);
            sp.AddParameter("sStatusPayment", string.IsNullOrEmpty(sStatusPayment) ? SqlString.Null : sStatusPayment);
            sp.AddParameter("sRemark", string.IsNullOrEmpty(sRemark) ? SqlString.Null : sRemark);

            List<FakturPajakPenampung> data = GetApplicationCollection(sp);

            if (data.Count > 0)
                totalItems = data[0].TotalItems;

            if (totalItems == 0 && filter.CurrentPage > 1)
            {
                filter.CurrentPage--;
                data = GetList(filter, out totalItems,noFaktur1, noFaktur2,  npwpVendor,  namaVendor,
                tglFakturStart, tglFakturEnd,  status, scanDateAwal, scanDateAkhir,  source,  remark,
                sNpwpPenjual,  sNamaPenjual,  sNoFaktur,  sTglFaktur,
                sDppString,  sPpnString,  sPpnBmString,  sStatusFaktur,  sUserName,  sSource,  sStatusPayment,  sRemark);
            }
            else if (totalItems > 0 && totalItems < (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = (totalItems <= filter.ItemsPerPage) ? 1 : (totalItems / filter.ItemsPerPage);
                data = GetList(filter, out totalItems, noFaktur1, noFaktur2, npwpVendor, namaVendor,
                tglFakturStart, tglFakturEnd, status, scanDateAwal, scanDateAkhir, source, remark,
                sNpwpPenjual, sNamaPenjual, sNoFaktur, sTglFaktur,
                sDppString, sPpnString, sPpnBmString, sStatusFaktur, sUserName, sSource, sStatusPayment, sRemark);
            }
            else if (totalItems > 0 && totalItems == (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = 1;
                data = GetList(filter, out totalItems, noFaktur1, noFaktur2, npwpVendor, namaVendor,
                tglFakturStart, tglFakturEnd, status, scanDateAwal, scanDateAkhir, source, remark,
                sNpwpPenjual, sNamaPenjual, sNoFaktur, sTglFaktur,
                sDppString, sPpnString, sPpnBmString, sStatusFaktur, sUserName, sSource, sStatusPayment, sRemark);
            }

            return data;

        }

        public static bool Restore(string fakturPajakPenampungIds, string createdBy, string noFaktur1, string noFaktur2, string npwp,
            string nama, string tglStart, string tglEnd,
            string status, string scanDateAwal, string scanDateAkhir, string source, string remark,
            string fNpwpPenjual, string fNamaPenjual,
            string fNoFaktur, string fTglFaktur, string fDppString, string fPpnString,
            string fPpnBmString, string fStatusFaktur, string fUserName, string fSource, string fStatusPayment, string fRemarks)
        {
            var sp = new SpBase(@"EXEC [dbo].[sp_RestoreFakturPajakPenampung] @FakturPajakPenampungIds, @CreatedBy,
            @NoFakturPajak1, @NoFakturPajak2, @NpwpVendor, @NamaVendor, @TglFakturStart, @TglFakturEnd, @Status, @scanDateAwal, @scanDateAkhir,
            @Source, @Remark, @sFormatedNpwpPenjual, @sNamaPenjual, @sFormatedNoFaktur, @sTglFakturString, @sDPPString,
            @sPPNString, @sPPNBMString, @sStatusFaktur, @sUserName, @sSource, @sStatusPayment, @sRemark,
            @IsValid, @MsgError");
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

            sp.AddParameter("FakturPajakPenampungIds", string.IsNullOrEmpty(fakturPajakPenampungIds) ? fakturPajakPenampungIds : SqlString.Null);
            sp.AddParameter("CreatedBy", createdBy);
            sp.AddParameter("NoFakturPajak1", string.IsNullOrEmpty(noFaktur1) ? SqlString.Null : noFaktur1);
            sp.AddParameter("NoFakturPajak2", string.IsNullOrEmpty(noFaktur2) ? SqlString.Null : noFaktur2);
            sp.AddParameter("NpwpVendor", string.IsNullOrEmpty(npwp) ? SqlString.Null : npwp);
            sp.AddParameter("NamaVendor", string.IsNullOrEmpty(nama) ? SqlString.Null : nama);
            sp.AddParameter("TglFakturStart", tglFakturStart.HasValue ? tglFakturEnd.Value : SqlDateTime.Null);
            sp.AddParameter("TglFakturEnd", tglFakturEnd.HasValue ? tglFakturEnd.Value : SqlDateTime.Null);
            sp.AddParameter("Status", string.IsNullOrEmpty(status) ? SqlString.Null : status);
            sp.AddParameter("scanDateAwal", dscanDateAwal.HasValue ? dscanDateAwal.Value : SqlDateTime.Null);
            sp.AddParameter("scanDateAkhir", dscanDateAkhir.HasValue ? dscanDateAkhir.Value : SqlDateTime.Null);
            sp.AddParameter("Source", string.IsNullOrEmpty(source) ? SqlString.Null : source);
            sp.AddParameter("Remark", string.IsNullOrEmpty(remark) ? SqlString.Null : source);
            sp.AddParameter("sFormatedNpwpPenjual", string.IsNullOrEmpty(fNpwpPenjual) ? SqlString.Null : fNpwpPenjual);
            sp.AddParameter("sNamaPenjual", string.IsNullOrEmpty(fNamaPenjual) ? SqlString.Null : fNamaPenjual);
            sp.AddParameter("sFormatedNoFaktur", string.IsNullOrEmpty(fNoFaktur) ? SqlString.Null : fNoFaktur);
            sp.AddParameter("sTglFakturString", string.IsNullOrEmpty(fTglFaktur) ? SqlString.Null : fTglFaktur);
            sp.AddParameter("sDPPString", string.IsNullOrEmpty(fDppString) ? SqlString.Null : fDppString);
            sp.AddParameter("sPPNString", string.IsNullOrEmpty(fPpnString) ? SqlString.Null : fPpnString);
            sp.AddParameter("sPPNBMString", string.IsNullOrEmpty(fPpnBmString) ? SqlString.Null : fPpnBmString);
            sp.AddParameter("sStatusFaktur", string.IsNullOrEmpty(fStatusFaktur) ? SqlString.Null : fStatusFaktur);
            sp.AddParameter("sUserName", string.IsNullOrEmpty(fUserName) ? SqlString.Null : fUserName);
            sp.AddParameter("sSource", string.IsNullOrEmpty(fSource) ? SqlString.Null : fSource);
            sp.AddParameter("sStatusPayment", string.IsNullOrEmpty(fStatusPayment) ? SqlString.Null : fStatusPayment);
            sp.AddParameter("sRemark", string.IsNullOrEmpty(fRemarks) ? SqlString.Null : fRemarks);

            sp.AddParameter("IsValid", 1);
            sp.AddParameter("MsgError", "");

            return sp.ExecuteNonQuery() >= 0;
        }

        public static List<FakturPajakPenampung> GetListRequestFakturPajak(Filter filter, out int totalItems, string noFaktur1, string noFaktur2, DateTime? tglFakturStart, DateTime? tglFakturEnd,
            string npwpVendor, string namaVendor, int? masaPajak, int? tahunPajak, string status)
        {
            if (filter.Search != null)
                filter.Search = filter.Search.Trim();
            var sortOrder = filter.SortOrderAsc ? "ASC" : "DESC";
            totalItems = 0;
            var sp = new SpBase(string.Format(@"SELECT	fp.* , COUNT(fp.FakturPajakPenampungId) OVER() AS TotalItems
                                                FROM	dbo.View_FakturPajakPenampung fp
                                                WHERE [IsDeleted] = 0
                                                        AND [FPType] <> 3 AND [Status] = 2
                                                        AND (
	                                                        (@NoFakturPajak1 IS NULL AND @NoFakturPajak2 IS NULL)
	                                                        OR (@NoFakturPajak1 IS NULL AND @NoFakturPajak2 IS NOT NULL 
	                                                        AND REPLACE(REPLACE([FormatedNoFaktur], '.', ''), '-','') LIKE REPLACE(REPLACE(REPLACE(@NoFakturPajak2, '-',''),'.',''), '*', '%'))
	                                                        OR (@NoFakturPajak1 IS NOT NULL AND @NoFakturPajak2 IS NULL AND REPLACE(REPLACE([FormatedNoFaktur], '.', ''), '-','') LIKE REPLACE(REPLACE(REPLACE(@NoFakturPajak1, '-',''),'.',''), '*', '%'))
	                                                        OR (@NoFakturPajak1 IS NOT NULL AND @NoFakturPajak2 IS NOT NULL 
	                                                        AND REPLACE(REPLACE([FormatedNoFaktur], '.', ''), '-','') >= REPLACE(REPLACE(@NoFakturPajak1, '-',''),'.','') 
	                                                        AND REPLACE(REPLACE([FormatedNoFaktur], '.', ''), '-','') <= REPLACE(REPLACE(@NoFakturPajak2, '-',''),'.',''))
                                                        )
                                                        AND (@NpwpVendor IS NULL OR
	                                                        (@NpwpVendor IS NOT NULL AND LOWER(REPLACE(REPLACE([FormatedNpwpPenjual], '.', ''), '-', '')) LIKE REPLACE(LOWER(REPLACE(REPLACE(@NpwpVendor, '.',''),'-','')), '*', '%'))
	                                                        )
                                                        AND (
	                                                        (@NamaVendor IS NOT NULL AND LOWER([NamaPenjual]) LIKE REPLACE(LOWER(@NamaVendor), '*', '%'))
	                                                        OR @NamaVendor IS NULL
	                                                        )
                                                        AND ((@MasaPajak IS NOT NULL AND [MasaPajak] = @MasaPajak) OR @MasaPajak IS NULL)
                                                        AND ((@TahunPajak IS NOT NULL AND [TahunPajak] = @TahunPajak) OR @TahunPajak IS NULL)
                                                        AND ((@Status IS NOT NULL AND LOWER([StatusApproval]) LIKE '%' + LOWER(@Status) + '%') OR @Status IS NULL)
                                                        AND ((
		                                                        CAST([TglFaktur] AS DATE) BETWEEN CAST(ISNULL(@TglFakturStart, [TglFaktur]) AS DATE) AND CAST(ISNULL(@TglFakturEnd, [TglFaktur]) AS DATE)
	                                                        ) OR @TglFakturStart IS NULL OR @TglFakturEnd IS NULL)
                                                        AND (LOWER(FormatedNoFaktur) LIKE LOWER(REPLACE(@Search, '*', '%'))
	                                                        OR LOWER(UrlScan) LIKE LOWER(REPLACE(@Search, '*', '%'))
	                                                        OR CONVERT(varchar, TglFaktur, 103) LIKE LOWER(REPLACE(@Search, '*', '%'))
	                                                        OR LOWER([FormatedNPWPLawanTransaksi]) LIKE LOWER(REPLACE(@Search, '*', '%'))
	                                                        OR LOWER([FormatedNpwpPenjual]) LIKE LOWER(REPLACE(@Search, '*', '%'))
	                                                        OR LOWER([NamaPenjual]) LIKE LOWER(REPLACE(@Search, '*', '%'))
	                                                        OR LOWER([NamaLawanTransaksi]) LIKE LOWER(REPLACE(@Search, '*', '%'))
	                                                        OR LOWER([StatusFaktur]) LIKE LOWER(REPLACE(@Search, '*', '%'))
	                                                        OR @Search IS NULL
	                                                        )
                                                        ORDER BY {0} {1}
                                                        OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
                                                        FETCH NEXT @ItemPerPage ROWS ONLY
                                                        OPTION (OPTIMIZE FOR UNKNOWN)", filter.SortColumnName, sortOrder));

            sp.AddParameter("CurrentPage", filter.CurrentPage);
            sp.AddParameter("ItemPerPage", filter.ItemsPerPage);
            sp.AddParameter("NoFakturPajak1", string.IsNullOrEmpty(noFaktur1) ? SqlString.Null : noFaktur1);
            sp.AddParameter("NoFakturPajak2", string.IsNullOrEmpty(noFaktur2) ? SqlString.Null : noFaktur2);
            sp.AddParameter("NpwpVendor", string.IsNullOrEmpty(npwpVendor) ? SqlString.Null : npwpVendor);
            sp.AddParameter("NamaVendor", string.IsNullOrEmpty(namaVendor) ? SqlString.Null : namaVendor);
            sp.AddParameter("MasaPajak", masaPajak.HasValue ? masaPajak.Value : SqlInt32.Null);
            sp.AddParameter("TahunPajak", tahunPajak.HasValue ? tahunPajak.Value : SqlInt32.Null);
            sp.AddParameter("Status", string.IsNullOrEmpty(status) ? SqlString.Null : status);
            sp.AddParameter("TglFakturStart", tglFakturStart.HasValue ? tglFakturStart.Value : SqlDateTime.Null);
            sp.AddParameter("TglFakturEnd", tglFakturEnd.HasValue ? tglFakturEnd.Value : SqlDateTime.Null);
            sp.AddParameter("Search", string.IsNullOrEmpty(filter.Search) ? SqlString.Null : filter.Search);

            List<FakturPajakPenampung> data = GetApplicationCollection(sp);

            if (data.Count > 0)
                totalItems = data[0].TotalItems;

            if (totalItems == 0 && filter.CurrentPage > 1)
            {
                filter.CurrentPage--;
                data = GetListRequestFakturPajak(filter, out totalItems, noFaktur1, noFaktur2, tglFakturStart, tglFakturEnd, npwpVendor, namaVendor,
                    masaPajak, tahunPajak, status);
            }
            else if (totalItems > 0 && totalItems < (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = (totalItems <= filter.ItemsPerPage) ? 1 : (totalItems / filter.ItemsPerPage);
                data = GetListRequestFakturPajak(filter, out totalItems, noFaktur1, noFaktur2, tglFakturStart, tglFakturEnd, npwpVendor, namaVendor,
                    masaPajak, tahunPajak, status);
            }
            else if (totalItems > 0 && totalItems == (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = 1;
                data = GetListRequestFakturPajak(filter, out totalItems, noFaktur1, noFaktur2, tglFakturStart, tglFakturEnd, npwpVendor, namaVendor,
                    masaPajak, tahunPajak, status);
            }

            return data;
        }

        public static List<FakturPajakPenampung> GetListBrowse(Filter filter, out int totalItems, DateTime? tglFaktur, string npwpVendor, string namaVendor)
        {
            if (filter.Search != null)
                filter.Search = filter.Search.Trim();
            var sortOrder = filter.SortOrderAsc ? "ASC" : "DESC";
            totalItems = 0;

            var sp = new SpBase(string.Format(@"SELECT	fp.*
		, COUNT(fp.FakturPajakPenampungId) OVER() AS TotalItems
FROM	dbo.View_FakturPajakPenampung fp
WHERE [IsDeleted] = 0 AND [Status] = 2
AND (@NpwpVendor IS NULL OR
	(@NpwpVendor IS NOT NULL AND LOWER(REPLACE(REPLACE([FormatedNpwpPenjual], '.', ''), '-', '')) LIKE REPLACE(LOWER(REPLACE(REPLACE(@NpwpVendor, '.',''),'-','')), '*', '%'))
	)
AND (
	(@NamaVendor IS NOT NULL AND LOWER([NamaPenjual]) LIKE REPLACE(LOWER(@NamaVendor), '*', '%'))
	OR @NamaVendor IS NULL
	)
AND (
	(@TglFaktur IS NOT NULL AND [TglFaktur] = @TglFaktur )
	OR @TglFaktur IS NULL
	)

AND (
	LOWER([NoFakturPajak]) LIKE LOWER(REPLACE(@Search, '*', '%'))
    OR LOWER([FormatedNoFaktur]) LIKE LOWER(REPLACE(@Search, '*', '%'))
	OR LOWER([NPWPLawanTransaksi]) LIKE LOWER(REPLACE(@Search, '*', '%'))
	OR LOWER([NamaLawanTransaksi]) LIKE LOWER(REPLACE(@Search, '*', '%'))
    OR [MasaPajak] LIKE LOWER(REPLACE(@Search, '*', '%'))
    OR [TahunPajak] LIKE LOWER(REPLACE(@Search, '*', '%'))
    OR [JumlahDPP] LIKE LOWER(REPLACE(@Search, '*', '%'))
    OR [JumlahPPN] LIKE LOWER(REPLACE(@Search, '*', '%'))
    OR [JumlahPPNBM] LIKE LOWER(REPLACE(@Search, '*', '%'))
    OR CONVERT(VARCHAR, [TglFaktur], 103) LIKE LOWER(REPLACE(@Search, '*', '%'))
	OR @Search IS NULL
	)
ORDER BY {0} {1}
OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
FETCH NEXT @ItemPerPage ROWS ONLY
OPTION (OPTIMIZE FOR UNKNOWN)", filter.SortColumnName, sortOrder));

            sp.AddParameter("CurrentPage", filter.CurrentPage);
            sp.AddParameter("ItemPerPage", filter.ItemsPerPage);
            sp.AddParameter("NpwpVendor", string.IsNullOrEmpty(npwpVendor) ? SqlString.Null : npwpVendor);
            sp.AddParameter("NamaVendor", string.IsNullOrEmpty(namaVendor) ? SqlString.Null : namaVendor);
            sp.AddParameter("TglFaktur", tglFaktur.HasValue ? tglFaktur.Value : SqlDateTime.Null);
            sp.AddParameter("Search", string.IsNullOrEmpty(filter.Search) ? SqlString.Null : filter.Search);

            List<FakturPajakPenampung> data = GetApplicationCollection(sp);

            if (data.Count > 0)
                totalItems = data[0].TotalItems;

            if (totalItems == 0 && filter.CurrentPage > 1)
            {
                filter.CurrentPage--;
                data = GetListBrowse(filter, out totalItems, tglFaktur, npwpVendor, namaVendor);
            }
            else if (totalItems > 0 && totalItems < (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = (totalItems <= filter.ItemsPerPage) ? 1 : (totalItems / filter.ItemsPerPage);
                data = GetListBrowse(filter, out totalItems, tglFaktur, npwpVendor, namaVendor);
            }
            else if (totalItems > 0 && totalItems == (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = 1;
                data = GetListBrowse(filter, out totalItems, tglFaktur, npwpVendor, namaVendor);
            }

            return data;

        }

        public static List<FakturPajakPenampung> GetListBrowseFpKhusus(Filter filter, out int totalItems, DateTime? tglFaktur, string npwpVendor, string namaVendor)
        {
            if (filter.Search != null)
                filter.Search = filter.Search.Trim();
            var sortOrder = filter.SortOrderAsc ? "ASC" : "DESC";
            totalItems = 0;

            var sp = new SpBase(string.Format(@"SELECT	fp.*
		, COUNT(fp.FakturPajakPenampungId) OVER() AS TotalItems
FROM	dbo.View_FakturPajakPenampung fp
WHERE [IsDeleted] = 0 AND [Status] = 2 AND [FPType] = 3
AND (@NpwpVendor IS NULL OR
	(@NpwpVendor IS NOT NULL AND LOWER(REPLACE(REPLACE([FormatedNpwpPenjual], '.', ''), '-', '')) LIKE REPLACE(LOWER(REPLACE(REPLACE(@NpwpVendor, '.',''),'-','')), '*', '%'))
	)
AND (
	(@NamaVendor IS NOT NULL AND LOWER([NamaPenjual]) LIKE REPLACE(LOWER(@NamaVendor), '*', '%'))
	OR @NamaVendor IS NULL
	)
AND (
	(@TglFaktur IS NOT NULL AND [TglFaktur] = @TglFaktur )
	OR @TglFaktur IS NULL
	)

AND (
	LOWER([NoFakturPajak]) LIKE LOWER(REPLACE(@Search, '*', '%'))
    OR LOWER([FormatedNoFaktur]) LIKE LOWER(REPLACE(@Search, '*', '%'))
	OR LOWER([NPWPLawanTransaksi]) LIKE LOWER(REPLACE(@Search, '*', '%'))
	OR LOWER([NamaLawanTransaksi]) LIKE LOWER(REPLACE(@Search, '*', '%'))
    OR [MasaPajak] LIKE LOWER(REPLACE(@Search, '*', '%'))
    OR [TahunPajak] LIKE LOWER(REPLACE(@Search, '*', '%'))
    OR [JumlahDPP] LIKE LOWER(REPLACE(@Search, '*', '%'))
    OR [JumlahPPN] LIKE LOWER(REPLACE(@Search, '*', '%'))
    OR [JumlahPPNBM] LIKE LOWER(REPLACE(@Search, '*', '%'))
    OR CONVERT(VARCHAR, [TglFaktur], 103) LIKE LOWER(REPLACE(@Search, '*', '%'))
	OR @Search IS NULL
	)
ORDER BY {0} {1}
OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
FETCH NEXT @ItemPerPage ROWS ONLY
OPTION (OPTIMIZE FOR UNKNOWN)", filter.SortColumnName, sortOrder));

            sp.AddParameter("CurrentPage", filter.CurrentPage);
            sp.AddParameter("ItemPerPage", filter.ItemsPerPage);
            sp.AddParameter("NpwpVendor", string.IsNullOrEmpty(npwpVendor) ? SqlString.Null : npwpVendor);
            sp.AddParameter("NamaVendor", string.IsNullOrEmpty(namaVendor) ? SqlString.Null : namaVendor);
            sp.AddParameter("TglFaktur", tglFaktur.HasValue ? tglFaktur.Value : SqlDateTime.Null);
            sp.AddParameter("Search", string.IsNullOrEmpty(filter.Search) ? SqlString.Null : filter.Search);

            List<FakturPajakPenampung> data = GetApplicationCollection(sp);

            if (data.Count > 0)
                totalItems = data[0].TotalItems;

            if (totalItems == 0 && filter.CurrentPage > 1)
            {
                filter.CurrentPage--;
                data = GetListBrowse(filter, out totalItems, tglFaktur, npwpVendor, namaVendor);
            }
            else if (totalItems > 0 && totalItems < (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = (totalItems <= filter.ItemsPerPage) ? 1 : (totalItems / filter.ItemsPerPage);
                data = GetListBrowse(filter, out totalItems, tglFaktur, npwpVendor, namaVendor);
            }
            else if (totalItems > 0 && totalItems == (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = 1;
                data = GetListBrowse(filter, out totalItems, tglFaktur, npwpVendor, namaVendor);
            }

            return data;

        }

        public static FakturPajakPenampung Save(FakturPajakPenampung data)
        {
            data.WasSaved = false;
            data.UIMessage = string.Empty;
            SpBase sp;

            string outMsgs = "";

            if (data.FakturPajakPenampungId > 0)
            {
                sp = new SpBase(@"UPDATE [dbo].[FakturPajakPenampung]
                                       SET
                                           [FCode] = @FCode 
                                          ,[UrlScan] = LTRIM(RTRIM(@UrlScan))
                                          ,[KdJenisTransaksi] = @KdJenisTransaksi
                                          ,[FgPengganti] = @FgPengganti
                                          ,[NoFakturPajak] = @NoFakturPajak
                                          ,[TglFaktur] = @TglFaktur
                                          ,[NPWPPenjual] = @NPWPPenjual
                                          ,[NamaPenjual] = @NamaPenjual
                                          ,[AlamatPenjual] = @AlamatPenjual
                                          ,[VendorId] = @VendorId
                                          ,[NPWPLawanTransaksi] = @NPWPLawanTransaksi
                                          ,[NamaLawanTransaksi] = @NamaLawanTransaksi
                                          ,[AlamatLawanTransaksi] = @AlamatLawanTransaksi
                                          ,[JumlahDPP] = @JumlahDPP
                                          ,[JumlahPPN] = @JumlahPPN
                                          ,[JumlahPPNBM] = @JumlahPPNBM
                                          ,[StatusApproval] = @StatusApproval
                                          ,[StatusFaktur] = @StatusFaktur
                                          ,[Dikreditkan] = @Dikreditkan
                                          ,[MasaPajak] = @MasaPajak
                                          ,[TahunPajak] = @TahunPajak
                                          ,[ReceivingDate] = @ReceivingDate
                                          ,[Pesan] = @Pesan
                                          ,[Status] = @Status
                                          ,[FPType] = @FPType
                                          ,[FillingIndex] = @FillingIndex
                                          ,[ScanType] = @ScanType
                                          ,[Modified] = GETDATE()
                                          ,[ModifiedBy] = @ModifiedBy
                                          ,[FormatedNoFaktur] = dbo.FormatNoFaktur(@FPType, @NoFakturPajak, @KdJenisTransaksi, @FgPengganti)
                                          ,[FormatedNpwpPenjual] = dbo.FormatNpwp(@NPWPPenjual)
                                          ,[FormatedNpwpLawanTransaksi] = dbo.FormatNpwp(@NPWPLawanTransaksi)
                                          ,[Referensi] = @Referensi
                                          ,[ObjectID] = @ObjectID
                                          ,[CertificateID] = @CertificateID
                                          ,[StatusPayment] = @StatusPayment
                                          ,[Remark] = @Remark
                                          ,[Source] = @Source
                                          ,[IsOutstanding] = @IsOutstanding
                                          ,[IsCreatedCSV] = @IsCreatedCSV
                                 WHERE [FakturPajakPenampungId] = @FakturPajakPenampungId");

                sp.AddParameter("FakturPajakPenampungId", data.FakturPajakPenampungId);
                sp.AddParameter("ModifiedBy", data.ModifiedBy);
                sp.AddParameter("FillingIndex", data.FillingIndex);
            }
            else
            {
                sp = new SpBase(@"IF EXISTS(SELECT * FROM FakturPajak fp WITH (NOLOCK) WHERE LTRIM(RTRIM(LOWER(fp.[UrlScan]))) = LTRIM(RTRIM(LOWER(@UrlScan))) AND IsDeleted = 0 AND Created >= CONVERT(DATE, Dateadd(month, -24, Getdate())))
BEGIN
	SET @outmsgs = (
		SELECT	'Sudah discan pada ' + CONVERT(varchar, fp.Created, 13) + ' oleh ' 
				+ fp.CreatedBy + '. No FP ' + fp.FormatedNoFaktur + ' sudah ada di Masa Pajak ' + fp.MasaPajakName + ' ' + CAST(fp.TahunPajak AS varchar(4))
				+ ', Nomor Filling Index ' + fp.FillingIndex + ' (Save)'
		FROM	View_FakturPajak fp 
		WHERE	LTRIM(RTRIM(LOWER(fp.[UrlScan]))) = LTRIM(RTRIM(LOWER(@UrlScan))) AND IsDeleted = 0
	)
END
ELSE
BEGIN
    SET @outmsgs = '';
	INSERT INTO [dbo].[FakturPajak]
           ([UrlScan]
           ,[KdJenisTransaksi]
           ,[FgPengganti]
           ,[NoFakturPajak]
           ,[TglFaktur]
           ,[NPWPPenjual]
           ,[NamaPenjual]
           ,[AlamatPenjual]
           ,[NPWPLawanTransaksi]
           ,[NamaLawanTransaksi]
           ,[AlamatLawanTransaksi]
           ,[JumlahDPP]
           ,[JumlahPPN]
           ,[JumlahPPNBM]
           ,[StatusApproval]
           ,[StatusFaktur]
           ,[Dikreditkan]
           ,[MasaPajak]
           ,[TahunPajak]
           ,[ReceivingDate]
           ,[Pesan]           
           ,[CreatedBy]
           ,[FCode]
           ,[VendorId]
           ,[FPType]
           ,[ScanType]
           ,[FillingIndex]
           ,[FormatedNoFaktur]
           ,[FormatedNpwpPenjual]
           ,[FormatedNpwpLawanTransaksi]
           ,[Status]
           ,[Referensi]
           ,[ObjectID]
           ,[CertificateID]
           ,[StatusPayment]
           ,[Remark]
           ,[Source]
           ,[IsOutstanding]
           ,[IsCreatedCSV]
           )
     VALUES
           (LTRIM(RTRIM(@UrlScan))
           ,@KdJenisTransaksi
           ,@FgPengganti
           ,@NoFakturPajak
           ,@TglFaktur
           ,@NPWPPenjual
           ,@NamaPenjual
           ,@AlamatPenjual
           ,@NPWPLawanTransaksi
           ,@NamaLawanTransaksi
           ,@AlamatLawanTransaksi
           ,@JumlahDPP
           ,@JumlahPPN
           ,@JumlahPPNBM
           ,@StatusApproval
           ,@StatusFaktur
           ,@Dikreditkan
           ,@MasaPajak
           ,@TahunPajak
           ,@ReceivingDate
           ,@Pesan                    
           ,@CreatedBy
           ,@FCode
           ,@VendorId
           ,@FPType           
           ,@ScanType
           ,dbo.fnGenerateFillingIndexByCreatedBy(@ScanType,@StatusApproval,@TahunPajak, @MasaPajak, @CreatedBy)
           ,dbo.FormatNoFaktur(@FPType, @NoFakturPajak, @KdJenisTransaksi, @FgPengganti)
           ,dbo.FormatNpwp(@NPWPPenjual)
           ,dbo.FormatNpwp(@NPWPLawanTransaksi)
           ,@Status
           ,@Referensi
           ,@ObjectID
           ,@CertificateID
           ,@StatusPayment
           ,@Remark
           ,@Source
           ,@IsOutstanding
           ,@IsCreatedCSV
           ); SELECT @FakturPajakId = @@IDENTITY;
END");
                sp.AddParameter("FakturPajakId", data.FakturPajakPenampungId, ParameterDirection.Output);
                sp.AddParameter("CreatedBy", data.CreatedBy);
                sp.AddParameter("outmsgs", outMsgs, ParameterDirection.Output, DbType.String, 255);

            }

            sp.AddParameter("UrlScan", string.IsNullOrEmpty(data.UrlScan) ? SqlString.Null : data.UrlScan.Trim());
            sp.AddParameter("KdJenisTransaksi", string.IsNullOrEmpty(data.KdJenisTransaksi) ? SqlString.Null : data.KdJenisTransaksi);
            sp.AddParameter("FgPengganti", string.IsNullOrEmpty(data.FgPengganti) ? SqlString.Null : data.FgPengganti);
            sp.AddParameter("NoFakturPajak", string.IsNullOrEmpty(data.NoFakturPajak) ? SqlString.Null : data.NoFakturPajak);
            sp.AddParameter("TglFaktur", data.TglFaktur.HasValue ? data.TglFaktur.Value : SqlDateTime.Null);
            sp.AddParameter("NPWPPenjual", string.IsNullOrEmpty(data.NPWPPenjual) ? SqlString.Null : data.NPWPPenjual);
            sp.AddParameter("NamaPenjual", string.IsNullOrEmpty(data.NamaPenjual) ? SqlString.Null : data.NamaPenjual);
            sp.AddParameter("AlamatPenjual", string.IsNullOrEmpty(data.AlamatPenjual) ? SqlString.Null : data.AlamatPenjual);
            sp.AddParameter("NPWPLawanTransaksi", string.IsNullOrEmpty(data.NPWPLawanTransaksi) ? SqlString.Null : data.NPWPLawanTransaksi);
            sp.AddParameter("NamaLawanTransaksi", string.IsNullOrEmpty(data.NamaLawanTransaksi) ? SqlString.Null : data.NamaLawanTransaksi);
            sp.AddParameter("AlamatLawanTransaksi", string.IsNullOrEmpty(data.AlamatLawanTransaksi) ? SqlString.Null : data.AlamatLawanTransaksi);
            sp.AddParameter("JumlahDPP", data.JumlahDPP.HasValue ? data.JumlahDPP.Value : SqlDecimal.Null);
            sp.AddParameter("JumlahPPN", data.JumlahPPN.HasValue ? data.JumlahPPN.Value : SqlDecimal.Null);
            sp.AddParameter("JumlahPPNBM", data.JumlahPPNBM.HasValue ? data.JumlahPPNBM.Value : SqlDecimal.Null);
            sp.AddParameter("StatusApproval", string.IsNullOrEmpty(data.StatusApproval) ? SqlString.Null : data.StatusApproval);
            sp.AddParameter("StatusFaktur", string.IsNullOrEmpty(data.StatusFaktur) ? SqlString.Null : data.StatusFaktur);
            sp.AddParameter("Dikreditkan", !data.Dikreditkan.HasValue ? SqlBoolean.Null : data.Dikreditkan.Value);
            sp.AddParameter("MasaPajak", data.MasaPajak.HasValue ? data.MasaPajak.Value : SqlInt32.Null);
            sp.AddParameter("TahunPajak", data.TahunPajak.HasValue ? data.TahunPajak.Value : SqlInt32.Null);
            sp.AddParameter("ReceivingDate", data.ReceivingDate.HasValue ? data.ReceivingDate.Value : SqlDateTime.Null);
            sp.AddParameter("Pesan", string.IsNullOrEmpty(data.Pesan) ? SqlString.Null : data.Pesan);
            sp.AddParameter("Status", data.Status);
            sp.AddParameter("FCode", string.IsNullOrEmpty(data.FCode) ? SqlString.Null : data.FCode);
            sp.AddParameter("VendorId", data.VendorId.HasValue ? data.VendorId.Value : SqlInt32.Null);
            sp.AddParameter("FPType", data.FPType);
            sp.AddParameter("ScanType", data.ScanType);
            sp.AddParameter("Referensi", string.IsNullOrEmpty(data.Referensi) ? SqlString.Null : data.Referensi);
            sp.AddParameter("ObjectID", !data.ObjectID.HasValue ? SqlInt64.Null : data.ObjectID.Value);
            sp.AddParameter("CertificateID", string.IsNullOrEmpty(data.CertificateID) ? SqlString.Null : data.CertificateID);
            sp.AddParameter("StatusPayment", string.IsNullOrEmpty(data.StatusPayment) ? SqlString.Null : data.StatusPayment);
            sp.AddParameter("Remark", string.IsNullOrEmpty(data.Remark) ? SqlString.Null : data.Remark);
            sp.AddParameter("Source", string.IsNullOrEmpty(data.Source) ? SqlString.Null : data.Source);
            sp.AddParameter("IsOutstanding", data.IsOutstanding);
            sp.AddParameter("IsCreatedCSV", data.IsCreatedCSV);

            if (sp.ExecuteNonQuery() == 0)
                data.WasSaved = true;

            if (data.FakturPajakPenampungId <= 0)
            {
                if (sp.GetParameter("outmsgs") != null)
                {
                    outMsgs = (string)sp.GetParameter("outmsgs");
                }

                if (string.IsNullOrEmpty(outMsgs))
                {
                    data.FakturPajakPenampungId = (long)sp.GetParameter("FakturPajakPenampungId");
                }
                else
                {
                    data.UIMessage = outMsgs;
                }

            }

            return data;
        }

        public static FakturPajakPenampung SaveFakturPajakKhusus(FakturPajakPenampung data)
        {
            data.WasSaved = false;
            SpBase sp;

            if (data.FakturPajakPenampungId > 0)
            {
                sp = new SpBase(@"UPDATE [dbo].[FakturPajakPenampung]
                                       SET
                                           [FCode] = @FCode 
                                          ,[KdJenisTransaksi] = @KdJenisTransaksi
                                          ,[FgPengganti] = @FgPengganti
                                          ,[NoFakturPajak] = @NoFakturPajak
                                          ,[TglFaktur] = @TglFaktur
                                          ,[NPWPPenjual] = @NPWPPenjual
                                          ,[NamaPenjual] = @NamaPenjual
                                          ,[AlamatPenjual] = @AlamatPenjual
                                          ,[VendorId] = @VendorId
                                          ,[JumlahDPP] = @JumlahDPP
                                          ,[JumlahPPN] = @JumlahPPN
                                          ,[JumlahPPNBM] = @JumlahPPNBM
                                          ,[MasaPajak] = @MasaPajak
                                          ,[TahunPajak] = @TahunPajak
                                          ,[Pesan] = @Pesan
                                          ,[Status] = @Status
                                          ,[FPType] = @FPType
                                          ,[FillingIndex] = @FillingIndex
                                          ,[ScanType] = @ScanType
                                          ,[Modified] = GETDATE()
                                          ,[ModifiedBy] = @ModifiedBy
                                          ,[FormatedNoFaktur] = dbo.FormatNoFaktur(@FPType, @NoFakturPajak, @KdJenisTransaksi, @FgPengganti)
                                          ,[FormatedNpwpPenjual] = dbo.FormatNpwp(@NPWPPenjual)
                                          ,[JenisTransaksi] = @JenisTransaksi
                                          ,[JenisDokumen] = @JenisDokumen
                                          ,[NoFakturYangDiganti] = @NoFakturYangDiganti
                                 WHERE [FakturPajakPenampungId] = @FakturPajakPenampungId");

                sp.AddParameter("FakturPajakPenampungId", data.FakturPajakPenampungId);
                sp.AddParameter("ModifiedBy", data.ModifiedBy);
                sp.AddParameter("FillingIndex", data.FillingIndex);
            }
            else
            {
                sp = new SpBase(@"INSERT INTO [dbo].[FakturPajak]
           ([KdJenisTransaksi]
           ,[FgPengganti]
           ,[NoFakturPajak]
           ,[TglFaktur]
           ,[NPWPPenjual]
           ,[NamaPenjual]
           ,[AlamatPenjual]
           ,[JumlahDPP]
           ,[JumlahPPN]
           ,[JumlahPPNBM]
           ,[MasaPajak]
           ,[TahunPajak]
           ,[Pesan]           
           ,[CreatedBy]
           ,[FCode]
           ,[VendorId]
           ,[FPType]
           ,[ScanType]
           ,[FillingIndex]
           ,[FormatedNoFaktur]
           ,[FormatedNpwpPenjual]
           ,[Status]
           ,[JenisTransaksi]
           ,[JenisDokumen]
           ,[NoFakturYangDiganti])
     VALUES
           (@KdJenisTransaksi
           ,@FgPengganti
           ,@NoFakturPajak
           ,@TglFaktur
           ,@NPWPPenjual
           ,@NamaPenjual
           ,@AlamatPenjual
           ,@JumlahDPP
           ,@JumlahPPN
           ,@JumlahPPNBM
           ,@MasaPajak
           ,@TahunPajak
           ,@Pesan                    
           ,@CreatedBy
           ,@FCode
           ,@VendorId
           ,@FPType           
           ,@ScanType
           ,dbo.fnGenerateFillingIndexByCreatedBy(@ScanType,@StatusApproval,@TahunPajak, @MasaPajak, @CreatedBy)
           ,dbo.FormatNoFaktur(@FPType, @NoFakturPajak, @KdJenisTransaksi, @FgPengganti)
           ,dbo.FormatNpwp(@NPWPPenjual)
           ,@Status
           ,@JenisTransaksi
           ,@JenisDokumen
           ,@NoFakturYangDiganti
           ); SELECT @FakturPajakId = @@IDENTITY");

                sp.AddParameter("FakturPajakId", data.FakturPajakPenampungId, ParameterDirection.Output);
                sp.AddParameter("CreatedBy", data.CreatedBy);
                sp.AddParameter("StatusApproval", SqlString.Null); //formalitas, tidak terpakai

            }
            sp.AddParameter("JenisTransaksi",
                string.IsNullOrEmpty(data.JenisTransaksi) ? SqlString.Null : data.JenisTransaksi);
            sp.AddParameter("JenisDokumen",
                string.IsNullOrEmpty(data.JenisDokumen) ? SqlString.Null : data.JenisDokumen);
            sp.AddParameter("NoFakturYangDiganti",
                string.IsNullOrEmpty(data.NoFakturYangDiganti) ? SqlString.Null : data.NoFakturYangDiganti);
            sp.AddParameter("KdJenisTransaksi", string.IsNullOrEmpty(data.KdJenisTransaksi) ? SqlString.Null : data.KdJenisTransaksi);
            sp.AddParameter("FgPengganti", string.IsNullOrEmpty(data.FgPengganti) ? SqlString.Null : data.FgPengganti);
            sp.AddParameter("NoFakturPajak", string.IsNullOrEmpty(data.NoFakturPajak) ? SqlString.Null : data.NoFakturPajak);
            sp.AddParameter("TglFaktur", data.TglFaktur.HasValue ? data.TglFaktur.Value : SqlDateTime.Null);
            sp.AddParameter("NPWPPenjual", string.IsNullOrEmpty(data.NPWPPenjual) ? SqlString.Null : data.NPWPPenjual);
            sp.AddParameter("NamaPenjual", string.IsNullOrEmpty(data.NamaPenjual) ? SqlString.Null : data.NamaPenjual);
            sp.AddParameter("AlamatPenjual", string.IsNullOrEmpty(data.AlamatPenjual) ? SqlString.Null : data.AlamatPenjual);
            sp.AddParameter("JumlahDPP", data.JumlahDPP.HasValue ? data.JumlahDPP.Value : SqlDecimal.Null);
            sp.AddParameter("JumlahPPN", data.JumlahPPN.HasValue ? data.JumlahPPN.Value : SqlDecimal.Null);
            sp.AddParameter("JumlahPPNBM", data.JumlahPPNBM.HasValue ? data.JumlahPPNBM.Value : SqlDecimal.Null);
            sp.AddParameter("MasaPajak", data.MasaPajak);
            sp.AddParameter("TahunPajak", data.TahunPajak);
            sp.AddParameter("Pesan", string.IsNullOrEmpty(data.Pesan) ? SqlString.Null : data.Pesan);
            sp.AddParameter("Status", data.Status);
            sp.AddParameter("FCode", data.FCode);
            sp.AddParameter("VendorId", data.VendorId.HasValue ? data.VendorId.Value : SqlInt32.Null);
            sp.AddParameter("FPType", data.FPType);
            sp.AddParameter("ScanType", data.ScanType);

            if (sp.ExecuteNonQuery() == 0)
                data.WasSaved = true;

            if (data.FakturPajakPenampungId <= 0)
            {
                data.FakturPajakPenampungId = (long)sp.GetParameter("FakturPajakPenampungId");
            }

            return data;
        }

        public static FakturPajakPenampung SaveScanBulk(FakturPajakPenampung data)
        {
            var sp = new SpBase(@"INSERT INTO [dbo].[FakturPajakPenampung]
           ([FCode]
           ,[UrlScan]
           ,[Dikreditkan]
           ,[MasaPajak]
           ,[TahunPajak]
           ,[ReceivingDate]
           ,[FPType]
           ,[FillingIndex]
           ,[ScanType]
           ,[CreatedBy]
           ,[Status])
     VALUES
           (@FCode
           ,LTRIM(RTRIM(@UrlScan))
           ,@Dikreditkan
           ,@MasaPajak
           ,@TahunPajak
           ,@ReceivingDate
           ,@FPType
           ,dbo.fnGenerateFillingIndexByCreatedBy(@ScanType,'',@TahunPajak, @MasaPajak, @CreatedBy)
           ,@ScanType
           ,@CreatedBy
           ,@Status); SELECT @FakturPajakPenampungId = @@IDENTITY");

            sp.AddParameter("FakturPajakPenampungId", data.FakturPajakPenampungId, ParameterDirection.Output);
            sp.AddParameter("CreatedBy", data.CreatedBy);

            sp.AddParameter("UrlScan", string.IsNullOrEmpty(data.UrlScan) ? SqlString.Null : data.UrlScan.Trim());

            sp.AddParameter("Dikreditkan", data.Dikreditkan);
            sp.AddParameter("MasaPajak", data.MasaPajak);
            sp.AddParameter("TahunPajak", data.TahunPajak);
            sp.AddParameter("ReceivingDate", data.ReceivingDate.HasValue ? data.ReceivingDate.Value : SqlDateTime.Null);
            sp.AddParameter("Status", data.Status);
            sp.AddParameter("FCode", data.FCode);
            sp.AddParameter("FPType", data.FPType);
            sp.AddParameter("ScanType", data.ScanType);

            if (sp.ExecuteNonQuery() == 0)
                data.WasSaved = true;

            if (data.FakturPajakPenampungId <= 0)
            {
                data.FakturPajakPenampungId = (long)sp.GetParameter("FakturPajakId");
            }

            return data;

        }

        public static bool Delete(long fakturPajakPenampungId, string modifiedBy)
        {
            var sp = new SpBase(string.Format(@"UPDATE dbo.FakturPajakPenampung SET IsDeleted = 1, Modified = GETDATE(), ModifiedBy = @ModifiedBy WHERE FakturPajakPenampungId = @FakturPajakPenampungId;"));
            sp.AddParameter("FakturPajakPenampungId", fakturPajakPenampungId);
            sp.AddParameter("ModifiedBy", modifiedBy);
            return sp.ExecuteNonQuery() == 0;
        }

        public static bool DeleteDaftarFakturPajak(long fakturPajakPenampungId, string modifiedBy)
        {
            var result = false;
            var sp = new SpBase(string.Format(@"EXEC [dbo].[sp_FakturPajakPenampung_Delete] @FakturPajakPenampungId = @FakturPajakPenampungId, @ModifiedBy = @ModifiedBy"));
            sp.AddParameter("FakturPajakPenampungId", fakturPajakPenampungId);
            sp.AddParameter("ModifiedBy", modifiedBy);
            sp.ExecuteNonQuery();
            result = true;
            return result;
        }

        public static bool DeleteByIds(string fakturPajakPenampungIds, string modifiedBy)
        {
            var sp = new SpBase(string.Format(@"UPDATE dbo.FakturPajakPenampung SET IsDeleted = 1, Modified = GETDATE(), ModifiedBy = @ModifiedBy 
WHERE FakturPajakPenampungId IN (SELECT Data FROM dbo.Split(@FakturPajakPenampungIds))"));
            sp.AddParameter("FakturPajakPenampungIds", fakturPajakPenampungIds);
            sp.AddParameter("ModifiedBy", modifiedBy);
            return sp.ExecuteNonQuery() == 0;
        }

        public static bool UpdateStatusById(long fakturPajakPenampungId, ApplicationEnums.StatusFakturPajak eStatus, string modifiedBy)
        {
            var iStatus = (int)eStatus;
            var sp = new SpBase(@"UPDATE FakturPajakPenampung
SET [Status] = @Status
	,Modified = GETDATE()
	,ModifiedBy = @ModifiedBy
WHERE	FakturPajakPenampungId = @fakturPajakPenampungId");

            sp.AddParameter("ModifiedBy", modifiedBy);
            sp.AddParameter("Status", iStatus);
            sp.AddParameter("fakturPajakPenampungId", fakturPajakPenampungId);

            return sp.ExecuteNonQuery() >= 0;
        }

        public static bool UpdateStatusFaktur(long fakturPajakPenampungId, string statusFaktur, string modifiedBy)
        {
            var sp = new SpBase(@"UPDATE FakturPajakPenampung
SET [StatusFaktur] = @StatusFaktur
	,Modified = GETDATE()
	,ModifiedBy = @ModifiedBy
WHERE	FakturPajakPenampungId = @fakturPajakPenampungId");

            sp.AddParameter("ModifiedBy", modifiedBy);
            sp.AddParameter("StatusFaktur", statusFaktur);
            sp.AddParameter("fakturPajakPenampungId", fakturPajakPenampungId);

            return sp.ExecuteNonQuery() >= 0;
        }

        public static bool UpdateStatusFakturNotDeleted(long fakturPajakPenampungId, string statusFaktur, string modifiedBy, bool isDeleted)
        {
            var sp = new SpBase(@"UPDATE FakturPajakPenampung
SET [StatusFaktur] = @StatusFaktur
	,Modified = GETDATE()
	,ModifiedBy = @ModifiedBy
WHERE	FakturPajakPenampungId = @fakturPajakPenampungId and IsDeleted = @isDeleted");

            sp.AddParameter("ModifiedBy", modifiedBy);
            sp.AddParameter("StatusFaktur", statusFaktur);
            sp.AddParameter("fakturPajakPenampungId", fakturPajakPenampungId);
            sp.AddParameter("isDeleted", Convert.ToInt32(isDeleted));

            return sp.ExecuteNonQuery() >= 0;
        }

        public static int GetCountByFpTypeAndReceivingDate(ApplicationEnums.FPType eFpType, DateTime receivingDate)
        {
            var iFpType = (int)eFpType;
            var sp = new SpBase(@"SELECT	COUNT(FakturPajakPenampungId) AS CountData
FROM	dbo.View_FakturPajakPenampung
WHERE	FPType = @fpType AND IsDeleted = 0 AND CAST(ReceivingDate as date) = CAST(@receivingDate as date)");
            sp.AddParameter("fpType", iFpType);
            sp.AddParameter("receivingDate", receivingDate);

            var data = sp.ExecuteScalar().ToString();

            return !string.IsNullOrEmpty(data) ? int.Parse(data) : 0;

        }

        public static List<FakturPajakPenampung> GetListToDownloadExcel(out int totalItems, string noFaktur1, string noFaktur2, string npwpVendor, string namaVendor,
            DateTime? tglFakturStart, DateTime? tglFakturEnd, string status, DateTime? scanDateAwal, DateTime? scanDateAkhir, string source, string remark,
            string sNpwpPenjual, string sNamaPenjual, string sNoFaktur, string sTglFaktur,
            string sDppString, string sPpnString, string sPpnBmString, string sStatusFaktur, string sUserName, string sSource, string sStatusPayment, string sRemark, string fpIds)
        {
            totalItems = 0;
            var sp = new SpBase(string.Format(@"SELECT	fp.*
		, COUNT(fp.FakturPajakPenampungId) OVER() AS TotalItems
FROM	dbo.View_FakturPajakPenampung fp
WHERE fp.[IsDeleted] = 0
AND [Status] = 2
AND (
	(@scanDateAwal IS NULL AND @scanDateAkhir IS NULL)
	OR (@scanDateAwal IS NOT NULL AND @scanDateAkhir IS NULL AND CAST([Created] AS date) = CAST(@scanDateAwal AS date))
	OR (@scanDateAwal IS NULL AND @scanDateAkhir IS NOT NULL AND CAST([Created] AS date) = CAST(@scanDateAkhir AS date))
	OR (@scanDateAwal IS NOT NULL AND @scanDateAkhir IS NOT NULL AND CAST([Created] AS date) >= CAST(@scanDateAwal AS date) AND CAST([Created] AS date) <= CAST(@scanDateAkhir AS date))
)
AND (
	(@NoFakturPajak1 IS NULL AND @NoFakturPajak2 IS NULL)
	OR (@NoFakturPajak1 IS NULL AND @NoFakturPajak2 IS NOT NULL 
	AND REPLACE(REPLACE([FormatedNoFaktur], '.', ''), '-','') LIKE REPLACE(REPLACE(REPLACE(@NoFakturPajak2, '-',''),'.',''),'*','%'))
	OR (@NoFakturPajak1 IS NOT NULL AND @NoFakturPajak2 IS NULL AND REPLACE(REPLACE([FormatedNoFaktur], '.', ''), '-','') LIKE REPLACE(REPLACE(REPLACE(@NoFakturPajak1, '-',''),'.',''), '*', '%'))
	OR (@NoFakturPajak1 IS NOT NULL AND @NoFakturPajak2 IS NOT NULL 
	AND REPLACE(REPLACE([FormatedNoFaktur], '.', ''), '-','') >= REPLACE(REPLACE(@NoFakturPajak1, '-',''),'.','') 
	AND REPLACE(REPLACE([FormatedNoFaktur], '.', ''), '-','') <= REPLACE(REPLACE(@NoFakturPajak2, '-',''),'.',''))
)
AND (@NpwpVendor IS NULL OR
	(@NpwpVendor IS NOT NULL AND LOWER(REPLACE(REPLACE([FormatedNpwpPenjual], '.', ''), '-', '')) LIKE REPLACE(LOWER(REPLACE(REPLACE(@NpwpVendor, '.',''),'-','')), '*', '%'))
	)
AND (
	(@NamaVendor IS NOT NULL AND LOWER([NamaPenjual]) LIKE REPLACE(LOWER(@NamaVendor), '*', '%'))
	OR @NamaVendor IS NULL
	)
AND ((
		CAST([TglFaktur] AS DATE) BETWEEN CAST(ISNULL(@TglFakturStart, [TglFaktur]) AS DATE) AND CAST(ISNULL(@TglFakturEnd, [TglFaktur]) AS DATE)
	) OR @TglFakturStart IS NULL OR @TglFakturEnd IS NULL)
AND (@sFormatedNpwpPenjual IS NULL OR (@sFormatedNpwpPenjual IS NOT NULL AND FormatedNpwpPenjual LIKE REPLACE(@sFormatedNpwpPenjual,'*','%')))
		AND (@sNamaPenjual IS NULL OR (@sNamaPenjual IS NOT NULL AND NamaPenjual LIKE REPLACE(@sNamaPenjual,'*','%')))
		AND (@sFormatedNoFaktur IS NULL OR (@sFormatedNoFaktur IS NOT NULL AND FormatedNoFaktur LIKE REPLACE(@sFormatedNoFaktur,'*','%')))
		AND (@sTglFakturString IS NULL OR (@sTglFakturString IS NOT NULL AND CONVERT(VARCHAR,TglFaktur, 103) LIKE REPLACE(@sTglFakturString,'*','%')))
		AND (@sDPPString IS NULL OR (@sDPPString IS NOT NULL AND CAST(JumlahDPP AS nvarchar) LIKE REPLACE(@sDPPString, '*','%')))
		AND (@sPPNString IS NULL OR (@sPPNString IS NOT NULL AND CAST(JumlahPPN AS nvarchar) LIKE REPLACE(@sPPNString, '*', '%')))
		AND (@sPPNBMString IS NULL OR (@sPPNBMString IS NOT NULL AND CAST(JumlahPPNBM AS nvarchar) LIKE REPLACE(@sPPNBMString, '*', '%')))
		AND (@sStatusFaktur IS NULL OR (@sStatusFaktur IS NOT NULL AND StatusFaktur LIKE REPLACE(@sStatusFaktur,'*','%')))
        AND (@sUserName IS NULL OR (@sUserName IS NOT NULL AND CreatedBy LIKE REPLACE(@sUserName,'*','%')))
        AND (@sSource IS NULL OR (@sSource IS NOT NULL AND Source LIKE REPLACE(@sSource,'*','%')))
        AND (@sStatusPayment IS NULL OR (@sStatusPayment IS NOT NULL AND StatusPayment LIKE REPLACE(@sStatusPayment,'*','%')))
        AND (@sRemark IS NULL OR (@sRemark IS NOT NULL AND Remark LIKE REPLACE(@sRemark,'*','%')))
        AND (@fpIds IS NULL OR (@fpIds IS NOT NULL AND FakturPajakPenampungId IN (SELECT Data From dbo.Split(@fpIds))))
        AND (StatusFaktur <> 'Faktur Diganti' OR StatusFaktur IS NULL)
ORDER BY fp.FakturPajakPenampungId ASC
OPTION (OPTIMIZE FOR UNKNOWN)"));
            
            sp.AddParameter("NoFakturPajak1", string.IsNullOrEmpty(noFaktur1) ? SqlString.Null : noFaktur1);
            sp.AddParameter("NoFakturPajak2", string.IsNullOrEmpty(noFaktur2) ? SqlString.Null : noFaktur2);
            sp.AddParameter("NpwpVendor", string.IsNullOrEmpty(npwpVendor) ? SqlString.Null : npwpVendor);
            sp.AddParameter("NamaVendor", string.IsNullOrEmpty(namaVendor) ? SqlString.Null : namaVendor);
            sp.AddParameter("TglFakturStart", tglFakturStart.HasValue ? tglFakturStart.Value : SqlDateTime.Null);
            sp.AddParameter("TglFakturEnd", tglFakturEnd.HasValue ? tglFakturEnd.Value : SqlDateTime.Null);
            sp.AddParameter("Status", string.IsNullOrEmpty(status) ? SqlString.Null : status);
            sp.AddParameter("scanDateAwal", scanDateAwal.HasValue ? scanDateAwal.Value : SqlDateTime.Null);
            sp.AddParameter("scanDateAkhir", scanDateAkhir.HasValue ? scanDateAkhir.Value : SqlDateTime.Null);
            sp.AddParameter("Source", string.IsNullOrEmpty(source) ? SqlString.Null : source);
            sp.AddParameter("Remark", string.IsNullOrEmpty(remark) ? SqlString.Null : source);

            sp.AddParameter("sFormatedNpwpPenjual", string.IsNullOrEmpty(sNpwpPenjual) ? SqlString.Null : sNpwpPenjual);
            sp.AddParameter("sNamaPenjual", string.IsNullOrEmpty(sNamaPenjual) ? SqlString.Null : sNamaPenjual);
            sp.AddParameter("sFormatedNoFaktur", string.IsNullOrEmpty(sNoFaktur) ? SqlString.Null : sNoFaktur);
            sp.AddParameter("sTglFakturString", string.IsNullOrEmpty(sTglFaktur) ? SqlString.Null : sTglFaktur);
            sp.AddParameter("sDPPString", string.IsNullOrEmpty(sDppString) ? SqlString.Null : sDppString);
            sp.AddParameter("sPPNString", string.IsNullOrEmpty(sPpnString) ? SqlString.Null : sPpnString);
            sp.AddParameter("sPPNBMString", string.IsNullOrEmpty(sPpnBmString) ? SqlString.Null : sPpnBmString);
            sp.AddParameter("sStatusFaktur", string.IsNullOrEmpty(sStatusFaktur) ? SqlString.Null : sStatusFaktur);
            sp.AddParameter("sUserName", string.IsNullOrEmpty(sUserName) ? SqlString.Null : sUserName);
            sp.AddParameter("sSource", string.IsNullOrEmpty(sSource) ? SqlString.Null : sSource);
            sp.AddParameter("sStatusPayment", string.IsNullOrEmpty(sStatusPayment) ? SqlString.Null : sStatusPayment);
            sp.AddParameter("sRemark", string.IsNullOrEmpty(sRemark) ? SqlString.Null : sRemark);
            sp.AddParameter("fpIds", string.IsNullOrEmpty(fpIds) ? SqlString.Null : fpIds);

            List<FakturPajakPenampung> data = GetApplicationCollection(sp);

            if (data.Count > 0)
                totalItems = data[0].TotalItems;

            return data;
        }

        public static List<FakturPajakPenampung> GetSourceRequestDetailFakturPajak(int batchitem)
        {
            var sp = new SpBase(string.Format(@"SELECT DISTINCT TOP {0} fp.*, {0} AS TotalItems
FROM	[dbo].[View_FakturPajakPenampung] fp
		LEFT JOIN FakturPajakPenampungDetail fpd ON fpd.FakturPajakPenampungId = fp.FakturPajakPenampungId
			AND fpd.IsDeleted = 0
WHERE	fp.IsDeleted = 0 AND fpd.FakturPajakDetailId IS NULL AND fp.MasaPajak IS NOT NULL AND fp.TahunPajak IS NOT NULL
		AND fp.FPType != 3
ORDER BY fp.FakturPajakPenampungId ASC", batchitem));
            var d = GetApplicationCollection(sp);
            return d.ToList();
        }

        public static List<FakturPajakPenampung> GetSourceFpDigantiOutstandingService(int batchitem)
        {
            var sp = new SpBase(string.Format(@"SELECT DISTINCT TOP {0} fp.*, CAST({0} AS int) AS TotalItems
FROM	[dbo].[View_FakturPajakPenampung] fp
		LEFT JOIN FakturPajakPenampungDetail fpd ON fpd.FakturPajakPenampungId = fp.FakturPajakPenampungId
			AND fpd.IsDeleted = 0
		INNER JOIN OpenClosePeriod ocp ON ocp.IsDeleted = 0
			AND DATEFROMPARTS(ocp.TahunPajak, ocp.MasaPajak, 1) = DATEFROMPARTS(fp.TahunPajak, fp.MasaPajak, 1)
			AND ocp.StatusRegular = 1
WHERE	fp.IsDeleted = 0 AND fpd.FakturPajakPenampungDetailId IS NULL 
		AND fp.MasaPajak IS NOT NULL AND fp.TahunPajak IS NOT NULL
		AND fp.FPType != 3
		AND fp.StatusFaktur IN ('Faktur Pajak Normal', 'Faktur Pajak Normal-Pengganti')
        AND fp.FormatedNoFaktur NOT IN (SELECT FormatedNoFaktur FROM LogFPDigantiOutstanding WHERE CAST(ProcessDate AS date) = CAST(GETDATE() AS date))
ORDER BY fp.FakturPajakPenampungId ASC", batchitem));
            var d = GetApplicationCollection(sp);
            return d.ToList();
        }

    }
}
