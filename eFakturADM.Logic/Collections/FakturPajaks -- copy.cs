using System;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using System.Collections.Generic;
using eFakturADM.Logic.Utilities;

using static eFakturADM.Logic.Objects.FakturPajak;
using System.Collections;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Text;
using static eFakturADM.Logic.Core.ApplicationEnums;
using Microsoft.SqlServer.Server;
using System.Web.UI.WebControls;

namespace eFakturADM.Logic.Collections
{
    public class wFakturPajaks : ApplicationCollection<FakturPajak, SpBase>
    {

        public static List<FakturPajak> Get()
        {
            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakId) OVER() AS TotalItems
FROM	dbo.View_FakturPajak fp
WHERE	fp.IsDeleted = 0");
            return GetApplicationCollection(sp);
        }

        public static FakturPajak GetById(long fakturPajakId)
        {
            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakId) OVER() AS TotalItems
FROM	dbo.View_FakturPajak fp
WHERE fp.FakturPajakId = @FakturPajakId");
            sp.AddParameter("FakturPajakId", fakturPajakId);
            var dbData = GetApplicationObject(sp);

            return dbData == null || dbData.FakturPajakId == 0 ? null : dbData;
        }

        public static List<FakturPajak> GetByMultipleId(string fakturPajakIds)
        {
            var sp = new SpBase(@"SELECT	fp.*
		    , COUNT(fp.FakturPajakId) OVER() AS TotalItems
            FROM	dbo.View_FakturPajak fp
            WHERE fp.FakturPajakId  IN (SELECT Data From dbo.Split(@FakturPajakIds))");
            sp.AddParameter("FakturPajakIds", fakturPajakIds);
            var dbData = GetApplicationCollection(sp);

            return dbData;
        }



        public static List<FakturPajak> GetByFormatedNoFaktur(string formatedNoFaktur)
        {
            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakId) OVER() AS TotalItems
FROM	dbo.View_FakturPajak fp
WHERE fp.IsDeleted = 0 AND fp.FormatedNoFaktur = @formatedNoFaktur");
            sp.AddParameter("formatedNoFaktur", formatedNoFaktur);

            var dbData = GetApplicationCollection(sp);

            return dbData;

        }

        public static List<FakturPajak> GetByCertificateID(string CertificateID)
        {
            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakId) OVER() AS TotalItems
FROM	dbo.View_FakturPajak fp
WHERE fp.IsDeleted = 1 AND fp.CertificateID = @CertificateID");
            sp.AddParameter("CertificateID", CertificateID);

            var dbData = GetApplicationCollection(sp);

            return dbData;

        }


        public static FakturPajak GetFakturPajakNormal(string nomorFaktur)
        {
            var sp = new SpBase(@"SELECT	fp.*
		, CAST(1 AS int) AS TotalItems
FROM	dbo.View_FakturPajak fp
WHERE fp.IsDeleted = 0 AND fp.NoFakturPajak = @NoFakturPajak AND fp.FgPengganti = '0'");
            sp.AddParameter("NoFakturPajak", nomorFaktur);
            var d = GetApplicationObject(sp);
            return d;
        }


        public static FakturPajak GetFakturPajakNormalTerlapor(string nomorFaktur)
        {
            var sp = new SpBase(@"SELECT	fp.*
            , CAST(1 AS int) AS TotalItems
            FROM	dbo.View_FakturPajak fp
            WHERE fp.IsDeleted = 0 AND fp.NoFakturPajak = @NoFakturPajak AND fp.FgPengganti = '0' AND fp.FakturPajakTerlaporID IS NOT NULL");
            sp.AddParameter("NoFakturPajak", nomorFaktur);
            var d = GetApplicationObject(sp);
            return d;
        }

        public static FakturPajak GetFakturPajakDuplikasi(string nomorFaktur, string kdJenisTransaksi, string fgPengganti)
        {
            var sp = new SpBase(@"SELECT	fp.*
            , CAST(1 AS int) AS TotalItems
            FROM	dbo.View_FakturPajak fp
            WHERE fp.IsDeleted = 0 AND fp.NoFakturPajak = @NoFakturPajak AND fp.FgPengganti = @fgPengganti AND fp.KdJenisTransaksi = @kdJenisTransaksi");
            sp.AddParameter("NoFakturPajak", nomorFaktur);
            sp.AddParameter("kdJenisTransaksi", kdJenisTransaksi);
            sp.AddParameter("fgPengganti", fgPengganti);
            var d = GetApplicationObject(sp);
            return d;
        }

        public static FakturPajak GetFakturPajakNormalByNoFaktur(string nomorFaktur)
        {
            var sp = new SpBase(@"SELECT	fp.*
		, CAST(1 AS int) AS TotalItems
FROM	dbo.View_FakturPajak fp
WHERE fp.IsDeleted = 0 AND fp.FgPengganti = '0' AND (fp.NoFakturPajak = @NoFakturPajak OR fp.FormatedNoFaktur = @NoFakturPajak)");
            sp.AddParameter("NoFakturPajak", nomorFaktur);
            var d = GetApplicationObject(sp);
            return d;
        }

        public static FakturPajak GetFakturPajakByNoFaktur(string nomorFaktur)
        {
            var sp = new SpBase(@"SELECT	top 1 fp.*, CAST(1 AS int) AS TotalItems
                    FROM	dbo.View_FakturPajak fp
                    WHERE (fp.NoFakturPajak = @NoFakturPajak OR fp.FormatedNoFaktur = @NoFakturPajak) 
                    ORDER by fp.FakturPajakId desc");
            sp.AddParameter("NoFakturPajak", nomorFaktur);
            var d = GetApplicationObject(sp);
            return d;
        }


        public static List<FakturPajak> GetByFormatedNoFakturFpKhusus(string formatedNoFaktur)
        {
            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakId) OVER() AS TotalItems
FROM	dbo.View_FakturPajak fp
WHERE fp.IsDeleted = 0 AND fp.FPType = 3 AND fp.FormatedNoFaktur = @formatedNoFaktur");
            sp.AddParameter("formatedNoFaktur", formatedNoFaktur);

            var dbData = GetApplicationCollection(sp);

            return dbData;

        }

        public static List<FakturPajak> GetFpKhususPenggantiByNoFaktur(string formatedNoFaktur)
        {
            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakId) OVER() AS TotalItems
FROM	dbo.View_FakturPajak fp
WHERE fp.IsDeleted = 0 AND fp.FPType = 3 AND fp.NoFakturYangDiganti = @formatedNoFaktur");
            sp.AddParameter("formatedNoFaktur", formatedNoFaktur);

            var dbData = GetApplicationCollection(sp);

            return dbData;
        }

        public static List<FakturPajak> GetByOriginalNoFaktur(string originalNoFaktur)
        {
            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakId) OVER() AS TotalItems
FROM	dbo.View_FakturPajak fp
WHERE fp.IsDeleted = 0 AND fp.NoFakturPajak = @NoFakturPajak");
            sp.AddParameter("NoFakturPajak", originalNoFaktur);

            var dbData = GetApplicationCollection(sp);

            return dbData;
        }

        public static List<FakturPajak> GetSpecificFakturPajak(ApplicationEnums.FPType eFpType, string originalNoFaktur,
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

        public static List<FakturPajak> GetListToDownload()
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
        /// <param name="fakturPajakIds">Multiple Faktur Pajak Id, separated by comma</param>
        /// <returns></returns>
        public static List<FakturPajak> GetByIds(string fakturPajakIds)
        {
            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakId) OVER() AS TotalItems
FROM	dbo.View_FakturPajak fp
  WHERE fp.FakturPajakId IN (SELECT Data FROM dbo.Split(@fakturPajakIds))");

            sp.AddParameter("fakturPajakIds", fakturPajakIds);

            return GetApplicationCollection(sp);
        }

        public static List<FakturPajak> GetScanBulk(Filter filter, out int totalItems, ApplicationEnums.FPType eFpType,
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

        public static List<FakturPajak> GetScanBulkToSubmit(ApplicationEnums.FPType eFpType,
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

        public static FakturPajak GetByUrlScan(string urlScan)
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

        public static List<FakturPajak> GetList(Filter filter, out int totalItems, string noFaktur1, string noFaktur2, DateTime? tglFakturStart, DateTime? tglFakturEnd,
            string npwpVendor, string namaVendor, int? masaPajak, int? tahunPajak, string status, string source, string statusPayment, string remark, bool? createdCsv, string StatusPelaporan,
            string sNpwpPenjual, string sNamaPenjual, string sNoFaktur, string sTglFaktur, string sMasaPajak, string sTahunPajak,
            string sDppString, string sPpnString, string sPpnBmString, string sStatusFaktur, int? dataType, DateTime? scanDateAwal, DateTime? scanDateAkhir, int? fillingIndex,
            string sFillingIndex, string sUserName, string sSource, string sStatusPayment, string sRemark, string sCreatedCsv, string sNamaPelaporan, string sTglFaktur010)
        {
            if (filter.Search != null)
                filter.Search = filter.Search.Trim();
            var sortOrder = filter.SortOrderAsc ? "ASC" : "DESC";
            totalItems = 0;

            var sp = new SpBase(string.Format(@"
                SELECT	fp.*
		                , COUNT(fp.FakturPajakId) OVER() AS TotalItems
                FROM	dbo.View_FakturPajak fp
                WHERE fp.[IsDeleted] = 0
                AND [Status] = 2
                AND (
	                (@scanDateAwal IS NULL AND @scanDateAkhir IS NULL)
	                OR (@scanDateAwal IS NOT NULL AND @scanDateAkhir IS NULL AND CAST([Created] AS date) = CAST(@scanDateAwal AS date))
	                OR (@scanDateAwal IS NULL AND @scanDateAkhir IS NOT NULL AND CAST([Created] AS date) = CAST(@scanDateAkhir AS date))
	                OR (@scanDateAwal IS NOT NULL AND @scanDateAkhir IS NOT NULL AND CAST([Created] AS date) >= CAST(@scanDateAwal AS date) AND CAST([Created] AS date) <= CAST(@scanDateAkhir AS date))
                )
                AND (
		                (@dataType IS NOT NULL AND @dataType = 3 AND [FPType] = 3)
		                OR (@dataType IS NOT NULL AND @dataType <> 3 AND [FPType] = @dataType AND (
		                @Status IS NULL OR (@Status IS NOT NULL AND 
		                ((@Status = 0 AND LOWER(StatusApproval) <> 'faktur valid, sudah diapprove oleh djp')
		                OR (@Status = 1 AND LOWER(StatusApproval) = 'faktur valid, sudah diapprove oleh djp')
		                )))
		                )
		                OR (@dataType IS NULL AND @Status IS NULL)
		                OR (@dataType IS NULL AND @Status IS NOT NULL AND (FPType = 3 OR (FPType <> 3 AND 
		                ((@Status = 0 AND LOWER(StatusApproval) <> 'faktur valid, sudah diapprove oleh djp')
		                OR (@Status = 1 AND LOWER(StatusApproval) = 'faktur valid, sudah diapprove oleh djp')
		                )))
		                )
	                )
                AND (@fillingIndex IS NULL OR (@fillingIndex IS NOT NULL AND ((@fillingIndex = 0 AND FillingIndex IS NULL) OR (@fillingIndex = 1 AND FillingIndex IS NOT NULL))))
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
                AND ((@MasaPajak IS NOT NULL AND [MasaPajak] = @MasaPajak) OR @MasaPajak IS NULL)
                AND ((@TahunPajak IS NOT NULL AND [TahunPajak] = @TahunPajak) OR @TahunPajak IS NULL)
                AND [MasaPajak] IS NOT NULL AND [TahunPajak] IS NOT NULL
                AND ((
		                CAST([TglFaktur] AS DATE) BETWEEN CAST(ISNULL(@TglFakturStart, [TglFaktur]) AS DATE) AND CAST(ISNULL(@TglFakturEnd, [TglFaktur]) AS DATE)
	                ) OR @TglFakturStart IS NULL OR @TglFakturEnd IS NULL)
                AND (@sFormatedNpwpPenjual IS NULL OR (@sFormatedNpwpPenjual IS NOT NULL AND FormatedNpwpPenjual LIKE REPLACE(@sFormatedNpwpPenjual,'*','%')))
		        AND (@sNamaPenjual IS NULL OR (@sNamaPenjual IS NOT NULL AND NamaPenjual LIKE REPLACE(@sNamaPenjual,'*','%')))
		        AND (@sFormatedNoFaktur IS NULL OR (@sFormatedNoFaktur IS NOT NULL AND FormatedNoFaktur LIKE REPLACE(@sFormatedNoFaktur,'*','%')))
		        AND (@sTglFakturString IS NULL OR (@sTglFakturString IS NOT NULL AND CONVERT(VARCHAR,TglFaktur, 103) LIKE REPLACE(@sTglFakturString,'*','%')))
		        AND (@sMasaPajakName IS NULL OR (@sMasaPajakName IS NOT NULL AND MasaPajakName LIKE REPLACE(@sMasaPajakName, '*', '%')))
		        AND (@sTahunPajak IS NULL OR (@sTahunPajak IS NOT NULL AND CAST(TahunPajak AS nvarchar) LIKE REPLACE(@sTahunPajak, '*', '%')))
		        AND (@sDPPString IS NULL OR (@sDPPString IS NOT NULL AND CAST(JumlahDPP AS nvarchar) LIKE REPLACE(@sDPPString, '*','%')))
		        AND (@sPPNString IS NULL OR (@sPPNString IS NOT NULL AND CAST(JumlahPPN AS nvarchar) LIKE REPLACE(@sPPNString, '*', '%')))
		        AND (@sPPNBMString IS NULL OR (@sPPNBMString IS NOT NULL AND CAST(JumlahPPNBM AS nvarchar) LIKE REPLACE(@sPPNBMString, '*', '%')))
		        AND (@sStatusFaktur IS NULL OR (@sStatusFaktur IS NOT NULL AND StatusFaktur LIKE REPLACE(@sStatusFaktur,'*','%')))
                AND (@sFillingIndex IS NULL OR (@sFillingIndex IS NOT NULL AND FillingIndex LIKE REPLACE(@sFillingIndex,'*','%')))
                AND (@sUserName IS NULL OR (@sUserName IS NOT NULL AND CreatedBy LIKE REPLACE(@sUserName,'*','%')))
                AND (@sSource IS NULL OR (@sSource IS NOT NULL AND Source LIKE REPLACE(@sSource,'*','%')))
                AND (@sStatusPayment IS NULL OR (@sStatusPayment IS NOT NULL AND StatusPayment LIKE REPLACE(@sStatusPayment,'*','%')))
                AND (@sRemark IS NULL OR (@sRemark IS NOT NULL AND Remark LIKE REPLACE(@sRemark,'*','%')))
                AND (@sCreatedCsv IS NULL OR (@sCreatedCsv IS NOT NULL AND IsCreatedCSV = @sCreatedCsv))
                AND (@Source IS NULL OR (@Source IS NOT NULL AND Source LIKE REPLACE(@Source,'*','%')))
                AND (@StatusPayment IS NULL OR (@StatusPayment IS NOT NULL AND ISNULL(StatusPayment,'') LIKE REPLACE(REPLACE(@StatusPayment,'Blank',''),'All','%')))
                AND (@Remark IS NULL OR (@Remark IS NOT NULL AND Remark LIKE REPLACE(@Remark,'*','%')))
                AND (@CreatedCsv IS NULL OR (@CreatedCsv IS NOT NULL AND IsCreatedCSV = @CreatedCsv))
                AND (StatusFaktur IS NULL OR (StatusFaktur IS NOT NULL AND StatusFaktur <> 'Faktur Diganti' AND StatusFaktur <> 'Faktur Dibatalkan'))
                AND [IsOutstanding] = 0
                AND (@StatusPelaporan IS NULL OR (@StatusPelaporan IS NOT NULL AND StatusPelaporan LIKE REPLACE(@StatusPelaporan,'All','%')))
                AND (@sNamaPelaporan IS NULL OR (@sNamaPelaporan IS NOT NULL AND NamaPelaporan LIKE REPLACE(@sNamaPelaporan,'*','%')))
                AND (@sTglFakturString010 IS NULL OR (@sTglFakturString010 IS NOT NULL AND CONVERT(VARCHAR,TglFaktur010, 103) LIKE REPLACE(@sTglFakturString010,'*','%')))
                ORDER BY {0} {1}
                OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
                FETCH NEXT @ItemPerPage ROWS ONLY
                OPTION (OPTIMIZE FOR UNKNOWN)
", filter.SortColumnName, sortOrder));

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
            sp.AddParameter("Source", string.IsNullOrEmpty(source) ? SqlString.Null : source);
            sp.AddParameter("StatusPayment", string.IsNullOrEmpty(statusPayment) ? SqlString.Null : statusPayment);
            sp.AddParameter("Remark", string.IsNullOrEmpty(remark) ? SqlString.Null : remark);
            sp.AddParameter("CreatedCsv", !createdCsv.HasValue ? SqlBoolean.Null : createdCsv.Value);
            sp.AddParameter("StatusPelaporan", string.IsNullOrEmpty(StatusPelaporan) ? SqlString.Null : StatusPelaporan);

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
            sp.AddParameter("dataType", dataType.HasValue && dataType.Value != 0 ? dataType.Value : SqlInt32.Null);
            sp.AddParameter("fillingIndex", fillingIndex.HasValue ? fillingIndex.Value : SqlInt32.Null);
            sp.AddParameter("sFillingIndex", string.IsNullOrEmpty(sFillingIndex) ? SqlString.Null : sFillingIndex);
            sp.AddParameter("sUserName", string.IsNullOrEmpty(sUserName) ? SqlString.Null : sUserName);
            sp.AddParameter("sSource", string.IsNullOrEmpty(sSource) ? SqlString.Null : sSource);
            sp.AddParameter("sStatusPayment", string.IsNullOrEmpty(sStatusPayment) ? SqlString.Null : sStatusPayment);
            sp.AddParameter("sRemark", string.IsNullOrEmpty(sStatusPayment) ? SqlString.Null : sRemark);
            sp.AddParameter("sNamaPelaporan", string.IsNullOrEmpty(sNamaPelaporan) ? SqlString.Null : sNamaPelaporan);
            sp.AddParameter("sTglFakturString010", string.IsNullOrEmpty(sTglFaktur010) ? SqlString.Null : sTglFaktur010);
            bool? isCreated = null;
            if (!string.IsNullOrEmpty(sCreatedCsv))
            {
                isCreated = sCreatedCsv.ToLowerInvariant() == "done";
            }

            sp.AddParameter("sCreatedCsv", !isCreated.HasValue ? SqlBoolean.Null : isCreated.Value);

            List<FakturPajak> data = GetApplicationCollection(sp);

            if (data.Count > 0)
                totalItems = data[0].TotalItems;

            if (totalItems == 0 && filter.CurrentPage > 1)
            {
                filter.CurrentPage--;
                data = GetList(filter, out totalItems, noFaktur1, noFaktur2, tglFakturStart, tglFakturEnd, npwpVendor, namaVendor,
                    masaPajak, tahunPajak, status, source, statusPayment, remark, createdCsv, StatusPelaporan, sNpwpPenjual, sNamaPenjual, sNoFaktur, sTglFaktur, sMasaPajak, sTahunPajak,
                    sDppString, sPpnString, sPpnBmString, sStatusFaktur, dataType, scanDateAwal, scanDateAkhir, fillingIndex, sFillingIndex, sUserName, sSource, sStatusPayment, sRemark, sCreatedCsv, sNamaPelaporan, sTglFaktur010);
            }
            else if (totalItems > 0 && totalItems < (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = (totalItems <= filter.ItemsPerPage) ? 1 : (totalItems / filter.ItemsPerPage);
                data = GetList(filter, out totalItems, noFaktur1, noFaktur2, tglFakturStart, tglFakturEnd, npwpVendor, namaVendor,
                    masaPajak, tahunPajak, status, source, statusPayment, remark, createdCsv, StatusPelaporan, sNpwpPenjual, sNamaPenjual, sNoFaktur, sTglFaktur, sMasaPajak, sTahunPajak,
                    sDppString, sPpnString, sPpnBmString, sStatusFaktur, dataType, scanDateAwal, scanDateAkhir, fillingIndex, sFillingIndex, sUserName, sSource, sStatusPayment, sRemark, sCreatedCsv, sNamaPelaporan, sTglFaktur010);
            }
            else if (totalItems > 0 && totalItems == (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = 1;
                data = GetList(filter, out totalItems, noFaktur1, noFaktur2, tglFakturStart, tglFakturEnd, npwpVendor, namaVendor,
                    masaPajak, tahunPajak, status, source, statusPayment, remark, createdCsv, StatusPelaporan, sNpwpPenjual, sNamaPenjual, sNoFaktur, sTglFaktur, sMasaPajak, sTahunPajak,
                    sDppString, sPpnString, sPpnBmString, sStatusFaktur, dataType, scanDateAwal, scanDateAkhir, fillingIndex, sFillingIndex, sUserName, sSource, sStatusPayment, sRemark, sCreatedCsv, sNamaPelaporan, sTglFaktur010);
            }

            return data;

        }

        public static List<FakturPajak> GetListRequestFakturPajak(Filter filter, out int totalItems, string noFaktur1, string noFaktur2, DateTime? tglFakturStart, DateTime? tglFakturEnd,
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

            List<FakturPajak> data = GetApplicationCollection(sp);

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

        public static List<FakturPajak> GetListBrowse(Filter filter, out int totalItems, DateTime? tglFaktur, string npwpVendor, string namaVendor)
        {
            if (filter.Search != null)
                filter.Search = filter.Search.Trim();
            var sortOrder = filter.SortOrderAsc ? "ASC" : "DESC";
            totalItems = 0;

            var sp = new SpBase(string.Format(@"SELECT	fp.*
		, COUNT(fp.FakturPajakId) OVER() AS TotalItems
FROM	dbo.View_FakturPajak fp
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

            List<FakturPajak> data = GetApplicationCollection(sp);

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

        public static List<FakturPajak> GetListBrowseFpKhusus(Filter filter, out int totalItems, DateTime? tglFaktur, string npwpVendor, string namaVendor)
        {
            if (filter.Search != null)
                filter.Search = filter.Search.Trim();
            var sortOrder = filter.SortOrderAsc ? "ASC" : "DESC";
            totalItems = 0;

            var sp = new SpBase(string.Format(@"SELECT	fp.*
		, COUNT(fp.FakturPajakId) OVER() AS TotalItems
FROM	dbo.View_FakturPajak fp
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

            List<FakturPajak> data = GetApplicationCollection(sp);

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

        public static FakturPajak Save(FakturPajak data)
        {
            data.WasSaved = false;
            data.UIMessage = string.Empty;
            SpBase sp;

            string outMsgs = "";

            sp = new SpBase(@"
                exec SaveFakturPajakHeader       
                @FakturPajakId ,
                @FCode ,
                @UrlScan ,
                @KdJenisTransaksi ,
                @FgPengganti ,
                @NoFakturPajak ,
                @TglFaktur ,
                @NPWPPenjual ,
                @NamaPenjual ,
                @AlamatPenjual ,
                @VendorId ,
                @NPWPLawanTransaksi ,
                @NamaLawanTransaksi ,
                @AlamatLawanTransaksi,
                @JumlahDPP ,
                @JumlahPPN ,
                @JumlahPPNBM ,
                @StatusApproval ,
                @StatusFaktur ,
                @Dikreditkan ,
                @MasaPajak ,
                @TahunPajak ,
                @ReceivingDate ,
                @Pesan ,
                @FPType ,
                @FillingIndex ,
                @ScanType ,

                @IsDeleted ,
                @Created ,
                @Modified ,
                @CreatedBy ,
                @ModifiedBy ,

                @TglFakturString ,
                @ReceivingDateString ,
                @DPPString ,
                @PPNString ,
                @PPNBMString ,

                @FormatedNoFaktur ,
                @FormatedNpwpPenjual ,

                @FormatedNpwpLawanTransaksi ,
        
                @MasaPajakName ,
                @ErrorMessage ,

                @Status ,
                @StatusText ,

                @StatusReconcile ,

                @Referensi ,

                @JenisTransaksi ,
                @JenisDokumen ,
                @NoFakturYangDiganti ,
                @ObjectID ,
                @CertificateID ,
                @StatusPayment ,
                @Remark ,
                @Source ,
                @IsOutstanding ,
                @IsCreatedCSV ,
                @FakturPajakTerlaporID ,
                @StatusRegular ,
                @NamaPelaporan ,
                @StatusPelaporan ,
                @IsByPass ,
                @TglFaktur010 ,
                @TglFakturString010 ,
		        @outmsgs varchar(max) = ''
            ");

            //update
            sp.AddParameter("FakturPajakId", data.FakturPajakId);
            sp.AddParameter("ModifiedBy", data.ModifiedBy);
            sp.AddParameter("FillingIndex", data.FillingIndex);

            //insert
            sp.AddParameter("FakturPajakId", data.FakturPajakId, ParameterDirection.Output);
            sp.AddParameter("CreatedBy", data.CreatedBy);
            sp.AddParameter("outmsgs", outMsgs, ParameterDirection.Output, DbType.String, 255);

            //all
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
            sp.AddParameter("IsDeleted", data.IsDeleted);

            if (sp.ExecuteNonQuery() == 0)
                data.WasSaved = true;

            if (data.FakturPajakId <= 0)
            {
                if (sp.GetParameter("outmsgs") != null && sp.GetParameter("outmsgs") is string)
                {
                    outMsgs = (string)sp.GetParameter("outmsgs");
                }

                if (string.IsNullOrEmpty(outMsgs) && sp.GetParameter("FakturPajakId") is long)
                {
                    data.FakturPajakId = (long)sp.GetParameter("FakturPajakId");
                }
                else
                {
                    data.UIMessage = outMsgs;
                }
            }

            return data;
        }

        public static FakturPajakExpired SetMasaPajakMultiple(string Ids, int MasaPajak, int TahunPajak, string UserName, out bool isValid)
        {
            SpBase sp;
            var result = new FakturPajakExpired();
            result.ExpiredFakturPajak = 0;
            result.NotExpiredFakturPajak = 0;
            result.PajakSudahDilaporkan = 0;
            result.PajakTidakSesuai = 0;
            isValid = true;

            string msgError = "";

            sp = new SpBase(@"
                 EXEC [dbo].[sp_FakturPajak_SetMasaPajak]
	                @FakturPajakIds
	                ,@MasaPajak
	                ,@TahunPajak
	                ,@UserName
	                ,@IsValid OUT
	                ,@MsgError OUT
	                ,@PajakExpiredCount OUT
	                ,@PajakTidakSesuaiCount OUT
	                ,@PajakSudahDilaporkanCount OUT
	                ,@PajakNotExpiredCount OUT
            ");

            sp.AddParameter("FakturPajakIds", Ids);
            sp.AddParameter("UserName", UserName);
            sp.AddParameter("MasaPajak", MasaPajak);
            sp.AddParameter("TahunPajak", TahunPajak);
            sp.AddParameter("IsValid", isValid, ParameterDirection.Output, DbType.Boolean);
            sp.AddParameter("MsgError", msgError, ParameterDirection.Output, DbType.String);
            sp.AddParameter("PajakExpiredCount", result.ExpiredFakturPajak, ParameterDirection.Output, DbType.Int32);
            sp.AddParameter("PajakTidakSesuaiCount", result.PajakTidakSesuai, ParameterDirection.Output, DbType.Int32);
            sp.AddParameter("PajakSudahDilaporkanCount", result.PajakSudahDilaporkan, ParameterDirection.Output, DbType.Int32);
            sp.AddParameter("PajakNotExpiredCount", result.NotExpiredFakturPajak, ParameterDirection.Output, DbType.Int32);

            if (sp.ExecuteNonQuery() == 0)
            {
                if (sp.GetParameter("PajakExpiredCount") is int)
                    result.ExpiredFakturPajak = (Int32)sp.GetParameter("PajakExpiredCount");
                if (sp.GetParameter("PajakTidakSesuaiCount") is int)
                    result.PajakTidakSesuai = (Int32)sp.GetParameter("PajakTidakSesuaiCount");
                if (sp.GetParameter("PajakSudahDilaporkanCount") is int)
                    result.PajakSudahDilaporkan = (Int32)sp.GetParameter("PajakSudahDilaporkanCount");
                if (sp.GetParameter("PajakNotExpiredCount") is int)
                    result.NotExpiredFakturPajak = (Int32)sp.GetParameter("PajakNotExpiredCount");


            }
            else isValid = false;

            return result;
        }

        public static FakturPajak SaveFakturPajakKhusus(FakturPajak data)
        {
            data.WasSaved = false;
            SpBase sp;

            if (data.FakturPajakId > 0)
            {
                sp = new SpBase(@"UPDATE [dbo].[FakturPajak]
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
                                 WHERE [FakturPajakId] = @FakturPajakId");

                sp.AddParameter("FakturPajakId", data.FakturPajakId);
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

                sp.AddParameter("FakturPajakId", data.FakturPajakId, ParameterDirection.Output);
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

            if (data.FakturPajakId <= 0)
            {
                data.FakturPajakId = (long)sp.GetParameter("FakturPajakId");
            }

            return data;
        }

        public static FakturPajak SaveScanBulk(FakturPajak data)
        {
            var sp = new SpBase(@"INSERT INTO [dbo].[FakturPajak]
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
           ,@Status); SELECT @FakturPajakId = @@IDENTITY");

            sp.AddParameter("FakturPajakId", data.FakturPajakId, ParameterDirection.Output);
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

            if (data.FakturPajakId <= 0)
            {
                data.FakturPajakId = (long)sp.GetParameter("FakturPajakId");
            }

            return data;

        }

        public static bool Delete(long fakturPajakId, string modifiedBy)
        {
            var sp = new SpBase(string.Format(@"UPDATE dbo.FakturPajak SET IsDeleted = 1, Modified = GETDATE(), ModifiedBy = @ModifiedBy WHERE FakturPajakId = @FakturPajakId;"));
            sp.AddParameter("FakturPajakId", fakturPajakId);
            sp.AddParameter("ModifiedBy", modifiedBy);
            return sp.ExecuteNonQuery() == 0;
        }

        public static bool DeleteDaftarFakturPajak(long fakturPajakId, string modifiedBy)
        {
            var result = false;
            var sp = new SpBase(string.Format(@"EXEC [dbo].[sp_FakturPajak_Delete] @FakturPajakId = @FakturPajakId, @ModifiedBy = @ModifiedBy"));
            sp.AddParameter("FakturPajakId", fakturPajakId);
            sp.AddParameter("ModifiedBy", modifiedBy);
            sp.ExecuteNonQuery();
            result = true;
            return result;
        }

        public static bool DeleteByIds(string fakturPajakIds, string modifiedBy)
        {
            var sp = new SpBase(string.Format(@"UPDATE dbo.FakturPajak SET IsDeleted = 1, Modified = GETDATE(), ModifiedBy = @ModifiedBy 
WHERE FakturPajakId IN (SELECT Data FROM dbo.Split(@FakturPajakIds))"));
            sp.AddParameter("FakturPajakIds", fakturPajakIds);
            sp.AddParameter("ModifiedBy", modifiedBy);
            return sp.ExecuteNonQuery() == 0;
        }

        public static bool UpdateStatusById(long fakturPajakId, ApplicationEnums.StatusFakturPajak eStatus, string modifiedBy)
        {
            var iStatus = (int)eStatus;
            var sp = new SpBase(@"UPDATE FakturPajak
SET [Status] = @Status
	,Modified = GETDATE()
	,ModifiedBy = @ModifiedBy
WHERE	FakturPajakId = @fakturPajakId");

            sp.AddParameter("ModifiedBy", modifiedBy);
            sp.AddParameter("Status", iStatus);
            sp.AddParameter("fakturPajakId", fakturPajakId);

            return sp.ExecuteNonQuery() >= 0;
        }

        public static bool UpdateStatusFaktur(long fakturPajakId, string statusFaktur, string modifiedBy)
        {
            var sp = new SpBase(@"UPDATE FakturPajak
SET [StatusFaktur] = @StatusFaktur
	,Modified = GETDATE()
	,ModifiedBy = @ModifiedBy
WHERE	FakturPajakId = @fakturPajakId");

            sp.AddParameter("ModifiedBy", modifiedBy);
            sp.AddParameter("StatusFaktur", statusFaktur);
            sp.AddParameter("fakturPajakId", fakturPajakId);

            return sp.ExecuteNonQuery() >= 0;
        }

        public static bool UpdateStatusFakturNotDeleted(long fakturPajakId, string statusFaktur, string modifiedBy, bool isDeleted)
        {
            var sp = new SpBase(@"UPDATE FakturPajak
SET [StatusFaktur] = @StatusFaktur
	,Modified = GETDATE()
	,ModifiedBy = @ModifiedBy
WHERE	FakturPajakId = @fakturPajakId and IsDeleted = @isDeleted");

            sp.AddParameter("ModifiedBy", modifiedBy);
            sp.AddParameter("StatusFaktur", statusFaktur);
            sp.AddParameter("fakturPajakId", fakturPajakId);
            sp.AddParameter("isDeleted", Convert.ToInt32(isDeleted));

            return sp.ExecuteNonQuery() >= 0;
        }

        public static int GetCountByFpTypeAndReceivingDate(ApplicationEnums.FPType eFpType, DateTime receivingDate)
        {
            var iFpType = (int)eFpType;
            var sp = new SpBase(@"SELECT	COUNT(FakturPajakId) AS CountData
FROM	dbo.View_FakturPajak
WHERE	FPType = @fpType AND IsDeleted = 0 AND CAST(ReceivingDate as date) = CAST(@receivingDate as date)");
            sp.AddParameter("fpType", iFpType);
            sp.AddParameter("receivingDate", receivingDate);

            var data = sp.ExecuteScalar().ToString();

            return !string.IsNullOrEmpty(data) ? int.Parse(data) : 0;

        }

        public static List<FakturPajak> GetListToDownloadExcel(out int totalItems, string noFaktur1, string noFaktur2, DateTime? tglFakturStart, DateTime? tglFakturEnd,
            string npwpVendor, string namaVendor, int? masaPajak, int? tahunPajak, string status,
            string sNpwpPenjual, string sNamaPenjual, string sNoFaktur, string sTglFaktur, string sMasaPajak, string sTahunPajak,
            string sDppString, string sPpnString, string sPpnBmString, string sStatusFaktur, int? dataType, DateTime? scanDateAwal, DateTime? scanDateAkhir, int? fillingIndex,
            string sFillingIndex, string sUserName, string source, string statusPayment, string remark, bool? createdCsv, string sSource, string sStatusPayment, string sRemark, string sCreatedCsv, string StatusPelaporan, string fNamaPelaporan)
        {
            totalItems = 0;
            var sp = new SpBase(@"
            SELECT fp.*
		            , COUNT(fp.FakturPajakId) OVER() AS TotalItems
            FROM	dbo.View_FakturPajak fp
            WHERE fp.[IsDeleted] = 0
            AND (
                (@scanDateAwal IS NULL AND @scanDateAkhir IS NULL)
	            OR (@scanDateAwal IS NOT NULL AND @scanDateAkhir IS NULL AND CAST([Created] AS date) = CAST(@scanDateAwal AS date))
	            OR (@scanDateAwal IS NULL AND @scanDateAkhir IS NOT NULL AND CAST([Created] AS date) = CAST(@scanDateAkhir AS date))
	            OR (@scanDateAwal IS NOT NULL AND @scanDateAkhir IS NOT NULL AND CAST([Created] AS date) >= CAST(@scanDateAwal AS date) AND CAST([Created] AS date) <= CAST(@scanDateAkhir AS date))
	            )
            AND (
		            (@dataType IS NOT NULL AND @dataType = 3 AND [FPType] = 3)
		            OR (@dataType IS NOT NULL AND @dataType <> 3 AND [FPType] = @dataType AND (
		            @Status IS NULL OR (@Status IS NOT NULL AND 
		            ((@Status = 0 AND LOWER(StatusApproval) <> 'faktur valid, sudah diapprove oleh djp')
		            OR (@Status = 1 AND LOWER(StatusApproval) = 'faktur valid, sudah diapprove oleh djp')
		            )))
		            )
		            OR (@dataType IS NULL AND @Status IS NULL)
		            OR (@dataType IS NULL AND @Status IS NOT NULL AND (FPType = 3 OR (FPType <> 3 AND 
		            ((@Status = 0 AND LOWER(StatusApproval) <> 'faktur valid, sudah diapprove oleh djp')
		            OR (@Status = 1 AND LOWER(StatusApproval) = 'faktur valid, sudah diapprove oleh djp')
		            )))
		            )
	            )
            AND (@fillingIndex IS NULL OR (@fillingIndex IS NOT NULL AND ((@fillingIndex = 0 AND FillingIndex IS NULL) OR (@fillingIndex = 1 AND FillingIndex IS NOT NULL))))
            AND [Status] = 2
            AND (
	            (@NoFakturPajak1 IS NULL AND @NoFakturPajak2 IS NULL)
	            OR (@NoFakturPajak1 IS NULL AND @NoFakturPajak2 IS NOT NULL 
	            AND REPLACE(REPLACE([FormatedNoFaktur], '.', ''), '-','') LIKE REPLACE(REPLACE(REPLACE(@NoFakturPajak2, '-',''),'.',''),'*', '%'))
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
            AND [MasaPajak] IS NOT NULL AND [TahunPajak] IS NOT NULL
            AND ((
		            CAST([TglFaktur] AS DATE) BETWEEN CAST(ISNULL(@TglFakturStart, [TglFaktur]) AS DATE) AND CAST(ISNULL(@TglFakturEnd, [TglFaktur]) AS DATE)
	            ) OR @TglFakturStart IS NULL OR @TglFakturEnd IS NULL)
            AND (@sFormatedNpwpPenjual IS NULL OR (@sFormatedNpwpPenjual IS NOT NULL AND FormatedNpwpPenjual LIKE REPLACE(@sFormatedNpwpPenjual,'*','%')))
		    AND (@sNamaPenjual IS NULL OR (@sNamaPenjual IS NOT NULL AND NamaPenjual LIKE REPLACE(@sNamaPenjual,'*','%')))
		    AND (@sFormatedNoFaktur IS NULL OR (@sFormatedNoFaktur IS NOT NULL AND FormatedNoFaktur LIKE REPLACE(@sFormatedNoFaktur,'*','%')))
		    AND (@sTglFakturString IS NULL OR (@sTglFakturString IS NOT NULL AND CONVERT(VARCHAR,TglFaktur, 103) LIKE REPLACE(@sTglFakturString,'*','%')))
		    AND (@sMasaPajakName IS NULL OR (@sMasaPajakName IS NOT NULL AND MasaPajakName LIKE REPLACE(@sMasaPajakName, '*', '%')))
		    AND (@sTahunPajak IS NULL OR (@sTahunPajak IS NOT NULL AND CAST(TahunPajak AS nvarchar) LIKE REPLACE(@sTahunPajak, '*', '%')))
		    AND (@sDPPString IS NULL OR (@sDPPString IS NOT NULL AND CAST(JumlahDPP AS nvarchar) LIKE REPLACE(@sDPPString, '*','%')))
		    AND (@sPPNString IS NULL OR (@sPPNString IS NOT NULL AND CAST(JumlahPPN AS nvarchar) LIKE REPLACE(@sPPNString, '*', '%')))
		    AND (@sPPNBMString IS NULL OR (@sPPNBMString IS NOT NULL AND CAST(JumlahPPNBM AS nvarchar) LIKE REPLACE(@sPPNBMString, '*', '%')))
		    AND (@sStatusFaktur IS NULL OR (@sStatusFaktur IS NOT NULL AND StatusFaktur LIKE REPLACE(@sStatusFaktur,'*','%')))
            AND (@sFillingIndex IS NULL OR (@sFillingIndex IS NOT NULL AND FillingIndex LIKE REPLACE(@sFillingIndex,'*','%')))
            AND (@sUserName IS NULL OR (@sUserName IS NOT NULL AND CreatedBy LIKE REPLACE(@sUserName,'*','%')))
            AND (@sSource IS NULL OR (@sSource IS NOT NULL AND Source LIKE REPLACE(@sSource,'*','%')))
            AND (@sStatusPayment IS NULL OR (@sStatusPayment IS NOT NULL AND StatusPayment LIKE REPLACE(@sStatusPayment,'*','%')))
            AND (@sRemark IS NULL OR (@sRemark IS NOT NULL AND Remark LIKE REPLACE(@sRemark,'*','%')))
            AND (@sCreatedCsv IS NULL OR (@sCreatedCsv IS NOT NULL AND IsCreatedCSV = @sCreatedCsv))
            AND (@Source IS NULL OR (@Source IS NOT NULL AND Source LIKE REPLACE(@Source,'*','%')))
            AND (@StatusPayment IS NULL OR (@StatusPayment IS NOT NULL AND ISNULL(StatusPayment,'') LIKE REPLACE(@StatusPayment,'All','%')))
            AND (@Remark IS NULL OR (@Remark IS NOT NULL AND Remark LIKE REPLACE(@Remark,'*','%')))
            AND (@CreatedCsv IS NULL OR (@CreatedCsv IS NOT NULL AND IsCreatedCSV = @CreatedCsv))
            AND (StatusFaktur <> 'Faktur Diganti' OR StatusFaktur IS NULL)
            AND (@StatusPelaporan IS NULL OR (@StatusPelaporan IS NOT NULL AND StatusPelaporan LIKE REPLACE(@StatusPelaporan,'All','%')))
            AND (@sNamaPelaporan IS NULL OR (@sNamaPelaporan IS NOT NULL AND NamaPelaporan LIKE REPLACE(@sNamaPelaporan,'*','%')))
            AND [IsOutstanding] = 0
            ORDER BY fp.FakturPajakId ASC
            OPTION (OPTIMIZE FOR UNKNOWN)

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
            sp.AddParameter("Source", string.IsNullOrEmpty(source) ? SqlString.Null : source);
            sp.AddParameter("StatusPayment", string.IsNullOrEmpty(statusPayment) ? SqlString.Null : statusPayment);
            sp.AddParameter("Remark", string.IsNullOrEmpty(remark) ? SqlString.Null : remark);
            sp.AddParameter("CreatedCsv", !createdCsv.HasValue ? SqlBoolean.Null : createdCsv.Value);
            sp.AddParameter("StatusPelaporan", string.IsNullOrEmpty(StatusPelaporan) ? SqlString.Null : StatusPelaporan);

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
            sp.AddParameter("dataType", dataType.HasValue ? dataType.Value : SqlInt32.Null);
            sp.AddParameter("fillingIndex", fillingIndex.HasValue ? fillingIndex.Value : SqlInt32.Null);
            sp.AddParameter("sFillingIndex", string.IsNullOrEmpty(sFillingIndex) ? SqlString.Null : sFillingIndex);
            sp.AddParameter("sUserName", string.IsNullOrEmpty(sUserName) ? SqlString.Null : sUserName);
            sp.AddParameter("sSource", string.IsNullOrEmpty(sSource) ? SqlString.Null : sSource);
            sp.AddParameter("sStatusPayment", string.IsNullOrEmpty(sStatusPayment) ? SqlString.Null : sStatusPayment);
            sp.AddParameter("sRemark", string.IsNullOrEmpty(sStatusPayment) ? SqlString.Null : sRemark);
            sp.AddParameter("sNamaPelaporan", string.IsNullOrEmpty(fNamaPelaporan) ? SqlString.Null : fNamaPelaporan);

            bool? isCreated = null;
            if (!string.IsNullOrEmpty(sCreatedCsv))
            {
                isCreated = sCreatedCsv.ToLowerInvariant() == "done";
            }

            sp.AddParameter("sCreatedCsv", !isCreated.HasValue ? SqlBoolean.Null : isCreated.Value);

            List<FakturPajak> data = GetApplicationCollection(sp);

            if (data.Count > 0)
                totalItems = data[0].TotalItems;

            return data;
        }

        //Digunakan untuk scan Bulk dan Scan Satuan IWS dan NON-IWS
        public static string ValidateScanPengganti(string noFakturOrigin, string fpPengganti, ApplicationEnums.FPType eFpType)
        {
            var sp = new SpBase(@"EXEC sp_FakturPajak_ValidateScanPengganti @noFaktur = @noFakturParam, @fgPengganti= @fgPenggantiParam, @fpType = @fpTypeParam");
            sp.AddParameter("noFakturParam", noFakturOrigin);
            sp.AddParameter("fgPenggantiParam", fpPengganti);
            sp.AddParameter("fpTypeParam", (int)eFpType);
            var retScalar = sp.ExecuteScalar();
            if (retScalar != null)
            {
                return retScalar as string;
            }

            return null;
        }
        /// <summary>
        /// CR EVIS 
        /// Ada validasi tambahan ketika semua Faktur Pajak (apapun kode FP nya) yang di scan adalah “Faktur Diganti” maka akan muncul notifikasi “Faktur Pajak sudah diganti”
        /// Terkaita perubahan dari sisi eFaktur V2.0
        /// Created Date : 2018-04-19
        /// </summary>
        /// <param name="noFakturOrigin"></param>
        /// <param name="fpPengganti"></param>
        /// <param name="eFpType"></param>
        /// <returns></returns>
        public static string ValidateScanFpNormal(string noFakturOrigin, string fpPengganti,
            ApplicationEnums.FPType eFpType)
        {
            var sp = new SpBase(@"EXEC sp_FakturPajak_ValidateScanFpNormal @noFaktur = @noFakturParam, @fgPengganti= @fgPenggantiParam, @fpType = @fpTypeParam");
            sp.AddParameter("noFakturParam", noFakturOrigin);
            sp.AddParameter("fgPenggantiParam", fpPengganti);
            sp.AddParameter("fpTypeParam", (int)eFpType);
            var retScalar = sp.ExecuteScalar();
            if (retScalar != null)
            {
                return retScalar as string;
            }

            return null;
        }

        public static int? GetStatusReconcileByTaxInvoiceNumber(string taxInvoiceNumber)
        {
            var sp =
                new SpBase(
                    @"EXEC sp_FakturPajak_GetStatusReconcileByTaxInvoiceNumber @TaxInvoiceNumber = @TaxInvoiceNumberParam, @StatusReconcile = @StatusReconcileParam OUTPUT");

            sp.AddParameter("TaxInvoiceNumberParam", taxInvoiceNumber);
            sp.AddParameter("StatusReconcileParam", SqlDbType.Int, ParameterDirection.Output);
            sp.ExecuteNonQuery();

            var chkD = sp.GetParameter("StatusReconcileParam");
            if (chkD == null)
            {
                return null;
            }
            return (int?)chkD;
        }

        public static List<FakturPajak> GetSourceRequestDetailFakturPajak(int batchitem)
        {
            var sp = new SpBase(string.Format(@"SELECT DISTINCT TOP {0} fp.*, {0} AS TotalItems
FROM	[dbo].[View_FakturPajak] fp
		LEFT JOIN FakturPajakDetail fpd ON fpd.FakturPajakId = fp.FakturPajakId
			AND fpd.IsDeleted = 0
WHERE	fp.IsDeleted = 0 AND fpd.FakturPajakDetailId IS NULL AND fp.MasaPajak IS NOT NULL AND fp.TahunPajak IS NOT NULL
		AND fp.FPType != 3
ORDER BY fp.FakturPajakId ASC", batchitem));
            var d = GetApplicationCollection(sp);
            return d.ToList();
        }

        public static List<FakturPajak> GetSourceFpDigantiOutstandingService(int batchitem)
        {
            var sp = new SpBase(string.Format(@"SELECT DISTINCT TOP {0} fp.*, CAST({0} AS int) AS TotalItems
FROM	[dbo].[View_FakturPajak] fp
		LEFT JOIN FakturPajakDetail fpd ON fpd.FakturPajakId = fp.FakturPajakId
			AND fpd.IsDeleted = 0
		INNER JOIN OpenClosePeriod ocp ON ocp.IsDeleted = 0
			AND DATEFROMPARTS(ocp.TahunPajak, ocp.MasaPajak, 1) = DATEFROMPARTS(fp.TahunPajak, fp.MasaPajak, 1)
			AND ocp.StatusRegular = 1
WHERE	fp.IsDeleted = 0 AND fpd.FakturPajakDetailId IS NULL 
		AND fp.MasaPajak IS NOT NULL AND fp.TahunPajak IS NOT NULL
		AND fp.FPType != 3
		AND fp.StatusFaktur IN ('Faktur Pajak Normal', 'Faktur Pajak Normal-Pengganti')
        AND fp.FormatedNoFaktur NOT IN (SELECT FormatedNoFaktur FROM LogFPDigantiOutstanding WHERE CAST(ProcessDate AS date) = CAST(GETDATE() AS date))
ORDER BY fp.FakturPajakId ASC", batchitem));
            var d = GetApplicationCollection(sp);
            return d.ToList();
        }

        public static bool UpdateStatusFakturDelvi(string FormattedNoFaktur, string StatusFaktur)
        {
            var result = false;
            var sp = new SpBase(string.Format(@"EXEC ADM_DELVI.DSTDELVI.dbo.[sp_EVIS_ChangeFaktur] @FormattedNoFaktur, @StatusFaktur"));
            sp.AddParameter("FormattedNoFaktur", FormattedNoFaktur);
            sp.AddParameter("StatusFaktur", StatusFaktur);
            sp.ExecuteNonQuery();
            result = true;
            return result;
        }

        public static FakturPajak GetFakturPajakByNoFakturPajak(string NoFakturPajak)
        {
            var sp = new SpBase(@"SELECT	fp.*
		, CAST(1 AS int) AS TotalItems
FROM	dbo.View_FakturPajak fp
WHERE fp.IsDeleted = 0 AND fp.NoFakturPajak = @NoFakturPajak AND fp.StatusFaktur='Faktur Diganti' ");
            sp.AddParameter("NoFakturPajak", NoFakturPajak);
            var d = GetApplicationObject(sp);
            return d;
        }
    }
}
