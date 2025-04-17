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
    public class FakturPajakOutstandings : ApplicationCollection<FakturPajakOutstanding, SpBase>
    {

        public static List<FakturPajakOutstanding> Get()
        {
            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakId) OVER() AS TotalItems
FROM	dbo.View_FakturPajak fp
WHERE	fp.IsDeleted = 0");
            return GetApplicationCollection(sp);
        }

        public static FakturPajakOutstanding GetById(long FakturPajakId)
        {
            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakId) OVER() AS TotalItems
FROM	dbo.View_FakturPajak fp
WHERE fp.FakturPajakId = @FakturPajakId");
            sp.AddParameter("FakturPajakId", FakturPajakId);
            var dbData = GetApplicationObject(sp);

            return dbData == null || dbData.FakturPajakId == 0 ? null : dbData;

        }
     
        public static List<FakturPajakOutstanding> GetByFormatedNoFaktur(string formatedNoFaktur)
        {
            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakId) OVER() AS TotalItems
FROM	dbo.View_FakturPajak fp
WHERE fp.IsDeleted = 0 AND fp.FormatedNoFaktur = @formatedNoFaktur");
            sp.AddParameter("formatedNoFaktur", formatedNoFaktur);

            var dbData = GetApplicationCollection(sp);

            return dbData;

        }

        public static FakturPajakOutstanding GetFakturPajakNormal(string nomorFaktur)
        {
            var sp = new SpBase(@"SELECT	fp.*
		, CAST(1 AS int) AS TotalItems
FROM	dbo.View_FakturPajak fp
WHERE fp.IsDeleted = 0 AND fp.NoFakturPajak = @NoFakturPajak AND fp.FgPengganti = '0'");
            sp.AddParameter("NoFakturPajak", nomorFaktur);
            var d = GetApplicationObject(sp);
            return d;
        }

        public static List<FakturPajakOutstanding> GetStatusPayment()
        {
            var sp = new SpBase(@"SELECT fp.StatusPayment FROM dbo.View_FakturPajak fp WHERE fp.IsDeleted = 0 GROUP BY fp.StatusPayment");
            var d = GetApplicationCollection(sp);
            return d;
        }

        public static List<FakturPajakOutstanding> GetByFormatedNoFakturFpKhusus(string formatedNoFaktur)
        {
            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakId) OVER() AS TotalItems
FROM	dbo.View_FakturPajak fp
WHERE fp.IsDeleted = 0 AND fp.FPType = 3 AND fp.FormatedNoFaktur = @formatedNoFaktur");
            sp.AddParameter("formatedNoFaktur", formatedNoFaktur);

            var dbData = GetApplicationCollection(sp);

            return dbData;

        }

        public static List<FakturPajakOutstanding> GetFpKhususPenggantiByNoFaktur(string formatedNoFaktur)
        {
            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakId) OVER() AS TotalItems
FROM	dbo.View_FakturPajak fp
WHERE fp.IsDeleted = 0 AND fp.FPType = 3 AND fp.NoFakturYangDiganti = @formatedNoFaktur");
            sp.AddParameter("formatedNoFaktur", formatedNoFaktur);

            var dbData = GetApplicationCollection(sp);

            return dbData;
        }

        public static List<FakturPajakOutstanding> GetByOriginalNoFaktur(string originalNoFaktur)
        {
            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakId) OVER() AS TotalItems
FROM	dbo.View_FakturPajak fp
WHERE fp.IsDeleted = 0 AND fp.NoFakturPajak = @NoFakturPajak");
            sp.AddParameter("NoFakturPajak", originalNoFaktur);

            var dbData = GetApplicationCollection(sp);

            return dbData;
        }

        public static List<FakturPajakOutstanding> GetSpecificFakturPajak(ApplicationEnums.FPType eFpType, string originalNoFaktur,
            string kdJenisTransaksi, string fgPengganti)
        {
            var sp = new SpBase(@"SELECT	fp.*, COUNT(fp.FakturPajakId) OVER() AS TotalItems
FROM	dbo.View_FakturPajak fp
WHERE	IsDeleted = 0 AND FormatedNoFaktur = dbo.FormatNoFaktur(@fpType, @originalNoFaktur, @kdJenisTransaksi, @fgPengganti)
ORDER BY fp.FakturPajakId DESC");

            sp.AddParameter("fpType", (int)eFpType);
            sp.AddParameter("originalNoFaktur", originalNoFaktur);
            sp.AddParameter("kdJenisTransaksi", kdJenisTransaksi);
            sp.AddParameter("fgPengganti", fgPengganti);

            var dbData = GetApplicationCollection(sp);
            return dbData;
        }

        public static List<FakturPajakOutstanding> GetListToDownload()
        {
            //Status Faktur Pajak yang bisa didownload kan oleh service hanya untuk status 1 (Scanned) & 3 (Error Request)
            var lstStats = new List<int>
            {
                (int) ApplicationEnums.StatusFakturPajak.Scanned,
                (int) ApplicationEnums.StatusFakturPajak.ErrorRequest
            };

            var stats = string.Join(",", lstStats);

            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakId) OVER() AS TotalItems
FROM	dbo.View_FakturPajak fp
WHERE fp.IsDeleted = 0 AND fp.UrlScan IS NOT NULL AND fp.[Status] IN (SELECT Data FROM dbo.Split(@stats))");

            sp.AddParameter("stats", stats);

            return GetApplicationCollection(sp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FakturPajakIds">Multiple Faktur Pajak Id, separated by comma</param>
        /// <returns></returns>
        public static List<FakturPajakOutstanding> GetByIds(string FakturPajakIds)
        {
            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakId) OVER() AS TotalItems
FROM	dbo.View_FakturPajak fp
  WHERE fp.FakturPajakId IN (SELECT Data FROM dbo.Split(@FakturPajakIds))");

            sp.AddParameter("FakturPajakIds", FakturPajakIds);

            return GetApplicationCollection(sp);
        }

        public static List<FakturPajakOutstanding> GetScanBulk(Filter filter, out int totalItems, ApplicationEnums.FPType eFpType,
            int masaPajak, int tahunPajak, DateTime? receivingDate)
        {
            var iFpType = (int)eFpType;
            if (filter.Search != null)
                filter.Search = filter.Search.Trim();
            var sortOrder = filter.SortOrderAsc ? "ASC" : "DESC";
            totalItems = 0;

            var sp = new SpBase(string.Format(@"SELECT	fp.*
		, COUNT(fp.FakturPajakId) OVER() AS TotalItems
FROM	dbo.View_FakturPajak fp
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

        public static List<FakturPajakOutstanding> GetScanBulkToSubmit(ApplicationEnums.FPType eFpType,
            int masaPajak, int tahunPajak, DateTime? receivingDate)
        {
            var iFpType = (int)eFpType;
            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakId) OVER() AS TotalItems
FROM	dbo.View_FakturPajak fp
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

        public static FakturPajakOutstanding GetByUrlScan(string urlScan)
        {
            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakId) OVER() AS TotalItems
FROM	dbo.View_FakturPajak fp
  WHERE LTRIM(RTRIM(LOWER(fp.[UrlScan]))) = LTRIM(RTRIM(LOWER(@UrlScan))) AND IsDeleted = 0");
            sp.AddParameter("UrlScan", urlScan);
            var dbData = GetApplicationObject(sp);
            if (dbData == null || dbData.FakturPajakId == 0) return null;
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

        public static List<FakturPajakOutstanding> GetList(Filter filter, out int totalItems, string noFaktur1, string noFaktur2, string npwpVendor, string namaVendor,
            DateTime? tglFakturStart, DateTime? tglFakturEnd, string status, DateTime? scanDateAwal, DateTime? scanDateAkhir, DateTime? receivedStart, DateTime? receivedEnd, string source, string statusFaktur, string remark, string isByPass,
            string sNpwpPenjual, string sNamaPenjual, string sNoFaktur, string sTglFaktur,
            string sDppString, string sPpnString, string sPpnBmString, string sStatusFaktur, string sSource, string sStatusPayment, string sRemark, string sTglFaktur010)
        {
            if (filter.Search != null)
                filter.Search = filter.Search.Trim();
            var sortOrder = filter.SortOrderAsc ? "ASC" : "DESC";
            totalItems = 0;

            var sp = new SpBase(string.Format(@"SELECT	fp.*
		, COUNT(fp.FakturPajakId) OVER() AS TotalItems
FROM	dbo.View_FakturPajak fp
WHERE fp.[IsDeleted] = 0
AND [Status] = 2
AND (StatusFaktur IS NULL OR (StatusFaktur IS NOT NULL AND StatusFaktur IN ('Faktur Pajak Normal','Faktur Pajak Normal-Pengganti','Pending Verification')))
AND [IsOutstanding] = 1
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
        AND (@sTglFakturString010 IS NULL OR (@sTglFakturString010 IS NOT NULL AND CONVERT(VARCHAR,TglFaktur010, 103) LIKE REPLACE(@sTglFakturString010,'*','%')))
		AND (@sDPPString IS NULL OR (@sDPPString IS NOT NULL AND CAST(JumlahDPP AS nvarchar) LIKE REPLACE(@sDPPString, '*','%')))
		AND (@sPPNString IS NULL OR (@sPPNString IS NOT NULL AND CAST(JumlahPPN AS nvarchar) LIKE REPLACE(@sPPNString, '*', '%')))
		AND (@sPPNBMString IS NULL OR (@sPPNBMString IS NOT NULL AND CAST(JumlahPPNBM AS nvarchar) LIKE REPLACE(@sPPNBMString, '*', '%')))
		AND (@sStatusFaktur IS NULL OR (@sStatusFaktur IS NOT NULL AND StatusFaktur LIKE REPLACE(@sStatusFaktur,'*','%')))
        AND (@sUserName IS NULL OR (@sUserName IS NOT NULL AND CreatedBy LIKE REPLACE(@sUserName,'*','%')))
        AND (@sSource IS NULL OR (@sSource IS NOT NULL AND Source LIKE REPLACE(@sSource,'*','%')))
        AND (@sStatusPayment IS NULL OR (@sStatusPayment IS NOT NULL AND StatusPayment LIKE REPLACE(@sStatusPayment,'*','%')))
        AND (@sRemark IS NULL OR (@sRemark IS NOT NULL AND Remark LIKE REPLACE(@sRemark,'*','%')))
        AND (@StatusFaktur IS NULL OR (@StatusFaktur IS NOT NULL AND  ISNULL(StatusFaktur,'Blank') LIKE CASE WHEN @StatusFaktur = 'All' THEN '%' WHEN @StatusFaktur = 'Normal Pengganti' THEN 'Faktur Pajak Normal-Pengganti' WHEN @StatusFaktur = 'Normal' THEN 'Faktur Pajak Normal' ELSE @StatusFaktur END))
       AND (@isByPass IS NULL OR (@isByPass IS NOT NULL AND IsByPass = (CASE WHEN @isByPass = '' THEN IsByPass ELSE CAST(@isByPass AS bit) END) ))
        AND (
	        (@receivedStart IS NULL AND @receivedEnd IS NULL)
	        OR (@receivedStart IS NOT NULL AND @receivedEnd IS NULL AND CAST([Created] AS date) = CAST(@receivedStart AS date))
	        OR (@receivedStart IS NULL AND @receivedEnd IS NOT NULL AND CAST([Created] AS date) = CAST(@receivedEnd AS date))
	        OR (@receivedStart IS NOT NULL AND @receivedEnd IS NOT NULL AND CAST([Created] AS date) >= CAST(@receivedStart AS date) AND CAST([Created] AS date) <= CAST(@receivedEnd AS date))
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
            sp.AddParameter("TglFakturStart", tglFakturStart.HasValue ? tglFakturStart.Value : SqlDateTime.Null);
            sp.AddParameter("TglFakturEnd", tglFakturEnd.HasValue ? tglFakturEnd.Value : SqlDateTime.Null);
            sp.AddParameter("Status", string.IsNullOrEmpty(status) ? SqlString.Null : status);
            sp.AddParameter("scanDateAwal", scanDateAwal.HasValue ? scanDateAwal.Value : SqlDateTime.Null);
            sp.AddParameter("scanDateAkhir", scanDateAkhir.HasValue ? scanDateAkhir.Value : SqlDateTime.Null);
            sp.AddParameter("receivedStart", receivedStart.HasValue ? receivedStart.Value : SqlDateTime.Null);
            sp.AddParameter("receivedEnd", receivedEnd.HasValue ? receivedEnd.Value : SqlDateTime.Null);

            sp.AddParameter("Source", string.IsNullOrEmpty(source) ? SqlString.Null : source);
            sp.AddParameter("StatusFaktur", string.IsNullOrEmpty(statusFaktur) ? SqlString.Null : statusFaktur);
            sp.AddParameter("Remark", string.IsNullOrEmpty(remark) ? SqlString.Null : remark);
            sp.AddParameter("isByPass", string.IsNullOrEmpty(isByPass) ? SqlString.Null : isByPass);
            var sUserName = "*";//sementara ga dijadikan parameter
            sp.AddParameter("sFormatedNpwpPenjual", string.IsNullOrEmpty(sNpwpPenjual) ? SqlString.Null : sNpwpPenjual);
            sp.AddParameter("sNamaPenjual", string.IsNullOrEmpty(sNamaPenjual) ? SqlString.Null : sNamaPenjual);
            sp.AddParameter("sFormatedNoFaktur", string.IsNullOrEmpty(sNoFaktur) ? SqlString.Null : sNoFaktur);
            sp.AddParameter("sTglFakturString", string.IsNullOrEmpty(sTglFaktur) ? SqlString.Null : sTglFaktur);
            sp.AddParameter("sDPPString", string.IsNullOrEmpty(sDppString) ? SqlString.Null : sDppString);
            sp.AddParameter("sPPNString", string.IsNullOrEmpty(sPpnString) ? SqlString.Null : sPpnString);
            sp.AddParameter("sPPNBMString", string.IsNullOrEmpty(sPpnBmString) ? SqlString.Null : sPpnBmString);
            sp.AddParameter("sStatusFaktur", string.IsNullOrEmpty(sStatusFaktur) ? SqlString.Null : sStatusFaktur);
            sp.AddParameter("sUserName", string.IsNullOrEmpty(sUserName) ? SqlString.Null : sUserName);
            sp.AddParameter("sSource", string.IsNullOrEmpty(source) ? SqlString.Null : source);
            sp.AddParameter("sStatusPayment", string.IsNullOrEmpty(status) ? SqlString.Null : status.Replace("All","%"));
            sp.AddParameter("sRemark", string.IsNullOrEmpty(remark) ? SqlString.Null : remark);
            sp.AddParameter("sTglFakturString010", string.IsNullOrEmpty(sTglFaktur010) ? SqlString.Null : sTglFaktur010);

            List<FakturPajakOutstanding> data = GetApplicationCollection(sp);

            if (data.Count > 0)
                totalItems = data[0].TotalItems;

            if (totalItems == 0 && filter.CurrentPage > 1)
            {
                filter.CurrentPage--;
                data = GetList(filter, out totalItems, noFaktur1, noFaktur2, npwpVendor, namaVendor,
                tglFakturStart, tglFakturEnd, status, scanDateAwal, scanDateAkhir,receivedStart,receivedEnd, source,statusFaktur, remark,isByPass,
                sNpwpPenjual, sNamaPenjual, sNoFaktur, sTglFaktur,
                sDppString, sPpnString, sPpnBmString, sStatusFaktur, sSource, sStatusPayment, sRemark, sTglFaktur010);
            }
            else if (totalItems > 0 && totalItems < (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = (totalItems <= filter.ItemsPerPage) ? 1 : (totalItems / filter.ItemsPerPage);
                data = GetList(filter, out totalItems, noFaktur1, noFaktur2, npwpVendor, namaVendor,
                tglFakturStart, tglFakturEnd, status, scanDateAwal, scanDateAkhir, receivedStart, receivedEnd, source, statusFaktur, remark, isByPass,
                sNpwpPenjual, sNamaPenjual, sNoFaktur, sTglFaktur,
                sDppString, sPpnString, sPpnBmString, sStatusFaktur,  sSource, sStatusPayment, sRemark, sTglFaktur010);
            }
            else if (totalItems > 0 && totalItems == (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = 1;
                data = GetList(filter, out totalItems, noFaktur1, noFaktur2, npwpVendor, namaVendor,
                tglFakturStart, tglFakturEnd, status, scanDateAwal, scanDateAkhir, receivedStart, receivedEnd, source, statusFaktur, remark, isByPass,
                sNpwpPenjual, sNamaPenjual, sNoFaktur, sTglFaktur,
                sDppString, sPpnString, sPpnBmString, sStatusFaktur,  sSource, sStatusPayment, sRemark, sTglFaktur010);
            }

            return data;

        }




        public static List<FakturPajakOutstanding> GetListToDownloadExcel(out int totalItems, string noFaktur1, string noFaktur2, string npwpVendor, string namaVendor,
            DateTime? tglFakturStart, DateTime? tglFakturEnd, string status, DateTime? scanDateAwal, DateTime? scanDateAkhir, DateTime? receivedStart, DateTime? receivedEnd, string source, string statusFaktur, string remark, string isByPass,
            string sNpwpPenjual, string sNamaPenjual, string sNoFaktur, string sTglFaktur,
            string sDppString, string sPpnString, string sPpnBmString, string sStatusFaktur, string sSource, string sStatusPayment, string sRemark, string sTglFaktur010)
        {
            totalItems = 0;

            var sp = new SpBase(string.Format(@"SELECT	fp.*
		, COUNT(fp.FakturPajakId) OVER() AS TotalItems
FROM	dbo.View_FakturPajak fp
WHERE fp.[IsDeleted] = 0
AND [Status] = 2
AND (StatusFaktur IS NULL OR (StatusFaktur IS NOT NULL AND StatusFaktur IN ('Faktur Pajak Normal','Faktur Pajak Normal-Pengganti','Pending Verification')))
AND [IsOutstanding] = 1
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
        AND (@sTglFakturString010 IS NULL OR (@sTglFakturString010 IS NOT NULL AND CONVERT(VARCHAR,TglFaktur010, 103) LIKE REPLACE(@sTglFakturString010,'*','%')))
		AND (@sDPPString IS NULL OR (@sDPPString IS NOT NULL AND CAST(JumlahDPP AS nvarchar) LIKE REPLACE(@sDPPString, '*','%')))
		AND (@sPPNString IS NULL OR (@sPPNString IS NOT NULL AND CAST(JumlahPPN AS nvarchar) LIKE REPLACE(@sPPNString, '*', '%')))
		AND (@sPPNBMString IS NULL OR (@sPPNBMString IS NOT NULL AND CAST(JumlahPPNBM AS nvarchar) LIKE REPLACE(@sPPNBMString, '*', '%')))
		AND (@sStatusFaktur IS NULL OR (@sStatusFaktur IS NOT NULL AND StatusFaktur LIKE REPLACE(@sStatusFaktur,'*','%')))
        AND (@sUserName IS NULL OR (@sUserName IS NOT NULL AND CreatedBy LIKE REPLACE(@sUserName,'*','%')))
        AND (@sSource IS NULL OR (@sSource IS NOT NULL AND Source LIKE REPLACE(@sSource,'*','%')))
        AND (@sStatusPayment IS NULL OR (@sStatusPayment IS NOT NULL AND StatusPayment LIKE REPLACE(@sStatusPayment,'*','%')))
        AND (@sRemark IS NULL OR (@sRemark IS NOT NULL AND Remark LIKE REPLACE(@sRemark,'*','%')))
        AND (@StatusFaktur IS NULL OR (@StatusFaktur IS NOT NULL AND  ISNULL(StatusFaktur,'Blank') LIKE CASE WHEN @StatusFaktur = 'All' THEN '%' WHEN @StatusFaktur = 'Normal Pengganti' THEN 'Faktur Pajak Normal-Pengganti' WHEN @StatusFaktur = 'Normal' THEN 'Faktur Pajak Normal' ELSE @StatusFaktur END))
       AND (@isByPass IS NULL OR (@isByPass IS NOT NULL AND IsByPass = (CASE WHEN @isByPass = '' THEN IsByPass ELSE CAST(@isByPass AS bit) END) ))
        AND (
	        (@receivedStart IS NULL AND @receivedEnd IS NULL)
	        OR (@receivedStart IS NOT NULL AND @receivedEnd IS NULL AND CAST([Created] AS date) = CAST(@receivedStart AS date))
	        OR (@receivedStart IS NULL AND @receivedEnd IS NOT NULL AND CAST([Created] AS date) = CAST(@receivedEnd AS date))
	        OR (@receivedStart IS NOT NULL AND @receivedEnd IS NOT NULL AND CAST([Created] AS date) >= CAST(@receivedStart AS date) AND CAST([Created] AS date) <= CAST(@receivedEnd AS date))
        )
ORDER BY fp.FakturPajakId ASC
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
            sp.AddParameter("receivedStart", receivedStart.HasValue ? receivedStart.Value : SqlDateTime.Null);
            sp.AddParameter("receivedEnd", receivedEnd.HasValue ? receivedEnd.Value : SqlDateTime.Null);

            sp.AddParameter("Source", string.IsNullOrEmpty(source) ? SqlString.Null : source);
            sp.AddParameter("StatusFaktur", string.IsNullOrEmpty(statusFaktur) ? SqlString.Null : statusFaktur);
            sp.AddParameter("Remark", string.IsNullOrEmpty(remark) ? SqlString.Null : remark);
            sp.AddParameter("isByPass", string.IsNullOrEmpty(isByPass) ? SqlString.Null : isByPass);
            var sUserName = "*";//sementara ga dijadikan parameter
            sp.AddParameter("sFormatedNpwpPenjual", string.IsNullOrEmpty(sNpwpPenjual) ? SqlString.Null : sNpwpPenjual);
            sp.AddParameter("sNamaPenjual", string.IsNullOrEmpty(sNamaPenjual) ? SqlString.Null : sNamaPenjual);
            sp.AddParameter("sFormatedNoFaktur", string.IsNullOrEmpty(sNoFaktur) ? SqlString.Null : sNoFaktur);
            sp.AddParameter("sTglFakturString", string.IsNullOrEmpty(sTglFaktur) ? SqlString.Null : sTglFaktur);
            sp.AddParameter("sDPPString", string.IsNullOrEmpty(sDppString) ? SqlString.Null : sDppString);
            sp.AddParameter("sPPNString", string.IsNullOrEmpty(sPpnString) ? SqlString.Null : sPpnString);
            sp.AddParameter("sPPNBMString", string.IsNullOrEmpty(sPpnBmString) ? SqlString.Null : sPpnBmString);
            sp.AddParameter("sStatusFaktur", string.IsNullOrEmpty(sStatusFaktur) ? SqlString.Null : sStatusFaktur);
            sp.AddParameter("sUserName", string.IsNullOrEmpty(sUserName) ? SqlString.Null : sUserName);
            sp.AddParameter("sSource", string.IsNullOrEmpty(source) ? SqlString.Null : source);
            sp.AddParameter("sStatusPayment", string.IsNullOrEmpty(status) ? SqlString.Null : status.Replace("All", "%"));
            sp.AddParameter("sRemark", string.IsNullOrEmpty(remark) ? SqlString.Null : remark);
            sp.AddParameter("sTglFakturString010", string.IsNullOrEmpty(sTglFaktur010) ? SqlString.Null : sTglFaktur010);

            List<FakturPajakOutstanding> data = GetApplicationCollection(sp);

            if (data.Count > 0)
                totalItems = data[0].TotalItems;

            return data;

        }


        public static bool Delete(string FakturPajakIds, string CreatedBy, string noFaktur1, string noFaktur2, string npwpVendor, string namaVendor,
            DateTime? tglFakturStart, DateTime? tglFakturEnd, string status, DateTime? scanDateAwal, DateTime? scanDateAkhir, DateTime? receivedStart, DateTime? receivedEnd, string source, string statusFaktur, string remark,
            string sNpwpPenjual, string sNamaPenjual, string sNoFaktur, string sTglFaktur,
            string sDppString, string sPpnString, string sPpnBmString, string sStatusFaktur, string sUserName, string sSource, string sStatusPayment, string sRemark)
        {
            var sp = new SpBase(@"EXEC [dbo].[sp_DeleteFakturPajakOutstanding] @FakturPajakIds, @CreatedBy, @IsValid, @MsgError
            ,@NoFakturPajak1 = @NoFakturPajak1
            ,@NoFakturPajak2 = @NoFakturPajak2
            ,@NpwpVendor = @NpwpVendor
            ,@NamaVendor = @NamaVendor
            ,@TglFakturStart = @TglFakturStart
            ,@TglFakturEnd = @TglFakturEnd
            ,@Status = @Status
            ,@scanDateAwal = @scanDateAwal
            ,@scanDateAkhir = @scanDateAkhir
            ,@Source = @Source
            ,@Remark = @Remark
            ,@sFormatedNpwpPenjual = @sFormatedNpwpPenjual
            ,@sNamaPenjual = @sNamaPenjual
            ,@sFormatedNoFaktur = @sFormatedNoFaktur
            ,@sTglFakturString = @sTglFakturString
            ,@sDPPString = @sDPPString
            ,@sPPNString = @sPPNString
            ,@sPPNBMString = @sPPNBMString
            ,@sStatusFaktur = @sStatusFaktur
            ,@sUserName = @sUserName
            ,@sSource = @sSource
            ,@sStatusPayment = @sStatusPayment
            ,@sRemark = @sRemark
            ,@StatusFaktur = @StatusFaktur
            ,@receivedStart = @receivedStart
            ,@receivedEnd = @receivedEnd");
            sp.AddParameter("FakturPajakIds", string.IsNullOrEmpty(FakturPajakIds) ? SqlString.Null : FakturPajakIds);
            sp.AddParameter("CreatedBy", CreatedBy);
            sp.AddParameter("IsValid", 1);
            sp.AddParameter("MsgError", "");
            sp.AddParameter("NoFakturPajak1", string.IsNullOrEmpty(noFaktur1) ? SqlString.Null : noFaktur1);
            sp.AddParameter("NoFakturPajak2", string.IsNullOrEmpty(noFaktur2) ? SqlString.Null : noFaktur2);
            sp.AddParameter("NpwpVendor", string.IsNullOrEmpty(npwpVendor) ? SqlString.Null : npwpVendor);
            sp.AddParameter("NamaVendor", string.IsNullOrEmpty(namaVendor) ? SqlString.Null : namaVendor);
            sp.AddParameter("TglFakturStart", tglFakturStart.HasValue ? tglFakturStart.Value : SqlDateTime.Null);
            sp.AddParameter("TglFakturEnd", tglFakturEnd.HasValue ? tglFakturEnd.Value : SqlDateTime.Null);
            sp.AddParameter("Status", string.IsNullOrEmpty(status) ? SqlString.Null : status);
            sp.AddParameter("scanDateAwal", scanDateAwal.HasValue ? scanDateAwal.Value : SqlDateTime.Null);
            sp.AddParameter("scanDateAkhir", scanDateAkhir.HasValue ? scanDateAkhir.Value : SqlDateTime.Null);
            sp.AddParameter("receivedStart", receivedStart.HasValue ? receivedStart.Value : SqlDateTime.Null);
            sp.AddParameter("receivedEnd", receivedEnd.HasValue ? receivedEnd.Value : SqlDateTime.Null);

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
            sp.AddParameter("StatusFaktur", string.IsNullOrEmpty(statusFaktur) ? SqlString.Null : statusFaktur);
            return sp.ExecuteNonQuery() >= 0;
        }
        public static bool GetByIdSetMasa(string FakturPajakIds)
        {
            var sp = new SpBase(@"EXEC [dbo].[sp_getid] @FakturPajakIds");
            sp.AddParameter("FakturPajakIds", FakturPajakIds);
            return sp.ExecuteNonQuery() >= 0;
  
        }

        [Obsolete("Untuk updatenya dipindah ke sp_getExpiredAndNotFakturPajak")]
        public static bool OutstandingValidasi(string FakturPajakIds, int BulanMasaPajak,int TahunMasaPajak, int KreditPajak,
            string noFaktur1, string noFaktur2, string npwpVendor, string namaVendor,
            DateTime? tglFakturStart, DateTime? tglFakturEnd, string status, DateTime? scanDateAwal, DateTime? scanDateAkhir, DateTime? receivedStart, DateTime? receivedEnd, string source, string remark,
            string sNpwpPenjual, string sNamaPenjual, string sNoFaktur, string sTglFaktur,
            string sDppString, string sPpnString, string sPpnBmString, string sStatusFaktur, string sUserName, string sSource, string sStatusPayment, string sRemark, bool isForceMasaPajak = false)
        {
            var sp = new SpBase(@"EXEC [dbo].[sp_FakturPajakOutstanding_Validasi] @FakturPajakIds = @FakturPajakIds, @BulanMasaPajak = @BulanMasaPajak ,@TahunMasaPajak = @TahunMasaPajak,
            @KreditPajak=@KreditPajak,@IsValid=@IsValid,@MsgError=@MsgError
            ,@NoFakturPajak1 = @NoFakturPajak1
            ,@NoFakturPajak2 = @NoFakturPajak2
            ,@NpwpVendor = @NpwpVendor
            ,@NamaVendor = @NamaVendor
            ,@TglFakturStart = @TglFakturStart
            ,@TglFakturEnd = @TglFakturEnd
            ,@Status = @Status
            ,@scanDateAwal = @scanDateAwal
            ,@scanDateAkhir = @scanDateAkhir
            ,@receivedStart = @receivedStart
            ,@receivedEnd = @receivedEnd
            ,@Source = @Source
            ,@Remark = @Remark
            ,@sFormatedNpwpPenjual = @sFormatedNpwpPenjual
            ,@sNamaPenjual = @sNamaPenjual
            ,@sFormatedNoFaktur = @sFormatedNoFaktur
            ,@sTglFakturString = @sTglFakturString
            ,@sDPPString = @sDPPString
            ,@sPPNString = @sPPNString
            ,@sPPNBMString = @sPPNBMString
            ,@sStatusFaktur = @sStatusFaktur
            ,@sUserName = @sUserName
            ,@sSource = @sSource
            ,@sStatusPayment = @sStatusPayment
            ,@sRemark = @sRemark
            ,@IsForceMasaPajak = @IsForceMasaPajak
            ");
            sp.AddParameter("FakturPajakIds", string.IsNullOrEmpty(FakturPajakIds) ? SqlString.Null : FakturPajakIds);
            sp.AddParameter("BulanMasaPajak", BulanMasaPajak);
            sp.AddParameter("TahunMasaPajak", TahunMasaPajak);
            sp.AddParameter("KreditPajak", KreditPajak);
            sp.AddParameter("IsValid", 1);
            sp.AddParameter("MsgError", "");
            sp.AddParameter("NoFakturPajak1", string.IsNullOrEmpty(noFaktur1) ? SqlString.Null : noFaktur1);
            sp.AddParameter("NoFakturPajak2", string.IsNullOrEmpty(noFaktur2) ? SqlString.Null : noFaktur2);
            sp.AddParameter("NpwpVendor", string.IsNullOrEmpty(npwpVendor) ? SqlString.Null : npwpVendor);
            sp.AddParameter("NamaVendor", string.IsNullOrEmpty(namaVendor) ? SqlString.Null : namaVendor);
            sp.AddParameter("TglFakturStart", tglFakturStart.HasValue ? tglFakturStart.Value : SqlDateTime.Null);
            sp.AddParameter("TglFakturEnd", tglFakturEnd.HasValue ? tglFakturEnd.Value : SqlDateTime.Null);
            sp.AddParameter("Status", string.IsNullOrEmpty(status) ? SqlString.Null : status);
            sp.AddParameter("scanDateAwal", scanDateAwal.HasValue ? scanDateAwal.Value : SqlDateTime.Null);
            sp.AddParameter("scanDateAkhir", scanDateAkhir.HasValue ? scanDateAkhir.Value : SqlDateTime.Null);
            sp.AddParameter("receivedStart", receivedStart.HasValue ? receivedStart.Value : SqlDateTime.Null);
            sp.AddParameter("receivedEnd", receivedEnd.HasValue ? receivedEnd.Value : SqlDateTime.Null);
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
            sp.AddParameter("IsForceMasaPajak", isForceMasaPajak);
            return sp.ExecuteNonQuery() >= 0;
        }


        public static FakturPajakOutstandingExpired GetExpiredData(string FakturPajakIds, int BulanMasaPajak, int TahunMasaPajak, 
            string noFaktur1, string noFaktur2, string npwpVendor, string namaVendor,
            DateTime? tglFakturStart, DateTime? tglFakturEnd, string status, DateTime? scanDateAwal, DateTime? scanDateAkhir, DateTime? receivedStart, DateTime? receivedEnd, string source, string remark,
            string sNpwpPenjual, string sNamaPenjual, string sNoFaktur, string sTglFaktur,
            string sDppString, string sPpnString, string sPpnBmString, string sStatusFaktur, string sUserName, string sSource, string sStatusPayment, string sRemark, bool isSetForceMasaPajak = false,int KreditPajak = 0, string TglFakturString010 = "")
        {
            var sp = new SpBase(@"EXEC [dbo].[sp_getExpiredAndNotFakturPajak] @FakturPajakIds = @FakturPajakIds, @MasaPajak = @MasaPajak, @TahunPajak= @TahunPajak
            ,@NoFakturPajak1 = @NoFakturPajak1
            ,@NoFakturPajak2 = @NoFakturPajak2
            ,@NpwpVendor = @NpwpVendor
            ,@NamaVendor = @NamaVendor
            ,@TglFakturStart = @TglFakturStart
            ,@TglFakturEnd = @TglFakturEnd
            ,@Status = @Status
            ,@scanDateAwal = @scanDateAwal
            ,@scanDateAkhir = @scanDateAkhir
            ,@receivedStart = @receivedStart
            ,@receivedEnd = @receivedEnd
            ,@Source = @Source
            ,@Remark = @Remark
            ,@sFormatedNpwpPenjual = @sFormatedNpwpPenjual
            ,@sNamaPenjual = @sNamaPenjual
            ,@sFormatedNoFaktur = @sFormatedNoFaktur
            ,@sTglFakturString = @sTglFakturString
            ,@sDPPString = @sDPPString
            ,@sPPNString = @sPPNString
            ,@sPPNBMString = @sPPNBMString
            ,@sStatusFaktur = @sStatusFaktur
            ,@sUserName = @sUserName
            ,@sSource = @sSource
            ,@sStatusPayment = @sStatusPayment
            ,@sRemark = @sRemark
            ,@IsForceMasaPajak = @IsForceMasaPajak
            ,@KreditPajak=@KreditPajak
            ,@sTglFakturString010 = @sTglFakturString010
");
            sp.AddParameter("FakturPajakIds", string.IsNullOrEmpty(FakturPajakIds) ? SqlString.Null : FakturPajakIds);
            sp.AddParameter("MasaPajak", BulanMasaPajak);
            sp.AddParameter("TahunPajak", TahunMasaPajak);
            sp.AddParameter("NoFakturPajak1", string.IsNullOrEmpty(noFaktur1) ? SqlString.Null : noFaktur1);
            sp.AddParameter("NoFakturPajak2", string.IsNullOrEmpty(noFaktur2) ? SqlString.Null : noFaktur2);
            sp.AddParameter("NpwpVendor", string.IsNullOrEmpty(npwpVendor) ? SqlString.Null : npwpVendor);
            sp.AddParameter("NamaVendor", string.IsNullOrEmpty(namaVendor) ? SqlString.Null : namaVendor);
            sp.AddParameter("TglFakturStart", tglFakturStart.HasValue ? tglFakturStart.Value : SqlDateTime.Null);
            sp.AddParameter("TglFakturEnd", tglFakturEnd.HasValue ? tglFakturEnd.Value : SqlDateTime.Null);
            sp.AddParameter("Status", string.IsNullOrEmpty(status) ? SqlString.Null : status);
            sp.AddParameter("scanDateAwal", scanDateAwal.HasValue ? scanDateAwal.Value : SqlDateTime.Null);
            sp.AddParameter("scanDateAkhir", scanDateAkhir.HasValue ? scanDateAkhir.Value : SqlDateTime.Null);
            sp.AddParameter("receivedStart", receivedStart.HasValue ? receivedStart.Value : SqlDateTime.Null);
            sp.AddParameter("receivedEnd", receivedEnd.HasValue ? receivedEnd.Value : SqlDateTime.Null);
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
            sp.AddParameter("sTglFakturString010", string.IsNullOrEmpty(TglFakturString010) ? SqlString.Null : TglFakturString010);
            sp.AddParameter("sSource", string.IsNullOrEmpty(sSource) ? SqlString.Null : sSource);
            sp.AddParameter("sStatusPayment", string.IsNullOrEmpty(sStatusPayment) ? SqlString.Null : sStatusPayment);
            sp.AddParameter("sRemark", string.IsNullOrEmpty(sRemark) ? SqlString.Null : sRemark);
            sp.AddParameter("IsForceMasaPajak", isSetForceMasaPajak);
            sp.AddParameter("KreditPajak", KreditPajak);

            var dbData = GetExpiredObject(sp);
            if (dbData == null) return null;

            return dbData;
        }


        public static FakturPajakOutstandingExpired GetExpiredObject(SpBase Sp)
        {
            FakturPajakOutstandingExpired _object = new FakturPajakOutstandingExpired();


            using (IDataReader _dr = Sp.ExecuteReader())
            {
                if (_dr.Read())
                {
                    _object.ExpiredFakturPajak = DBUtil.GetIntField(_dr, "ExpiredFakturPajak");
                    _object.NotExpiredFakturPajak = DBUtil.GetIntField(_dr, "NotExpiredFakturPajak");
                    _object.PajakTidakSesuai = DBUtil.GetIntField(_dr, "PajakTidakSesuai");
                    _object.PajakValidasiStatus = DBUtil.GetIntField(_dr, "PajakValidasiStatus");
                    _object.PajakValidasiStatusBlankDanBelumBypass = DBUtil.GetIntField(_dr, "PajakValidasiStatusBlankDanBelumBypass");
                }


                if (_dr != null)
                    _dr.Close();


                Sp.Close();
            }


            return _object;
        }


        public static List<FakturPajakOutstanding> GetListRequestFakturPajak(Filter filter, out int totalItems, string noFaktur1, string noFaktur2, DateTime? tglFakturStart, DateTime? tglFakturEnd,
           string npwpVendor, string namaVendor, int? masaPajak, int? tahunPajak, string status)
        {
            if (filter.Search != null)
                filter.Search = filter.Search.Trim();
            var sortOrder = filter.SortOrderAsc ? "ASC" : "DESC";
            totalItems = 0;
            var sp = new SpBase(string.Format(@"SELECT	fp.* , COUNT(fp.FakturPajakId) OVER() AS TotalItems
                                                FROM	dbo.View_FakturPajak fp
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

            List<FakturPajakOutstanding> data = GetApplicationCollection(sp);

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


    }
}
