using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Collections
{
    public class FakturPajakDigantiOutstandings : ApplicationCollection<FakturPajakDigantiOutstanding, SpBase>
    {
        public static FakturPajakDigantiOutstanding Save(FakturPajakDigantiOutstanding data)
        {
            data.WasSaved = false;

            SpBase sp;
            if (data.Id > 0)
            {
                //Update
                sp = new SpBase(@"UPDATE [dbo].[FakturPajakDigantiOutstanding]
   SET [FormatedNoFaktur] = CASE WHEN @FormatedNoFaktur IS NULL THEN [FormatedNoFaktur] ELSE @FormatedNoFaktur END
      ,[MasaPajak] = CASE WHEN @MasaPajak = 0 THEN [MasaPajak] ELSE @MasaPajak END
      ,[TahunPajak] =  CASE WHEN @TahunPajak = 0 THEN [TahunPajak] ELSE @TahunPajak END  
      ,[StatusFaktur] = CASE WHEN @StatusFaktur IS NULL THEN [StatusFaktur] ELSE @StatusFaktur END    
      ,[StatusApproval] = CASE WHEN @StatusApproval IS NULL THEN [StatusApproval] ELSE @StatusApproval END
      ,[KeteranganDjp] = CASE WHEN @KeteranganDjp IS NULL THEN [KeteranganDjp] ELSE @KeteranganDjp END 
      ,[Keterangan] = CASE WHEN @Keterangan IS NULL THEN [Keterangan] ELSE @Keterangan END 
      ,[StatusOutstanding] = @StatusOutstanding
      ,[Modified] = GETDATE()
      ,[ModifiedBy] = @ModifiedBy
 WHERE Id = @Id");
                sp.AddParameter("Id", data.Id);
                sp.AddParameter("ModifiedBy", data.ModifiedBy);
            }
            else
            {
                //Insert
                sp = new SpBase(@"INSERT INTO [dbo].[FakturPajakDigantiOutstanding]
           ([FormatedNoFaktur]
           ,[MasaPajak]
           ,[TahunPajak]
           ,[StatusFaktur]
           ,[StatusApproval]
           ,[Keterangan]
           ,[KeteranganDjp]
           ,[StatusOutstanding]
           ,[CreatedBy])
     VALUES
           (@FormatedNoFaktur
           ,@MasaPajak
           ,@TahunPajak
           ,@StatusFaktur
           ,@StatusApproval
           ,@Keterangan
           ,@KeteranganDjp
           ,@StatusOutstanding
           ,@CreatedBy); SELECT @Id = @@IDENTITY");

                sp.AddParameter("Id", data.Id, ParameterDirection.Output);
                sp.AddParameter("CreatedBy", data.CreatedBy);
            }

            sp.AddParameter("FormatedNoFaktur", string.IsNullOrEmpty(data.FormatedNoFaktur) ? SqlString.Null: data.FormatedNoFaktur);
            sp.AddParameter("MasaPajak", data.MasaPajak == null ? 0 : data.MasaPajak);
            sp.AddParameter("TahunPajak", data.TahunPajak == null ? 0 : data.TahunPajak);
            sp.AddParameter("StatusFaktur", string.IsNullOrEmpty(data.StatusFaktur) ? SqlString.Null : data.StatusFaktur);
            sp.AddParameter("StatusApproval", string.IsNullOrEmpty(data.StatusApproval) ? SqlString.Null : data.StatusApproval);
            sp.AddParameter("Keterangan", string.IsNullOrEmpty(data.Keterangan) ? SqlString.Null : data.Keterangan);
            sp.AddParameter("StatusOutstanding", data.StatusOutstanding);
            sp.AddParameter("KeteranganDjp", string.IsNullOrEmpty(data.KeteranganDjp) ? SqlString.Null : data.KeteranganDjp);

            if (sp.ExecuteNonQuery() == 0)
                data.WasSaved = true;

            if (data.Id <= 0)
            {
                data.Id = (long)sp.GetParameter("Id");
            }

            return data;
        }

        public static bool Delete(long id, string modifiedBy)
        {
            var sp = new SpBase(string.Format(@"UPDATE dbo.FakturPajakDigantiOutstanding SET IsDeleted = 1, Modified = GETDATE(), ModifiedBy = @ModifiedBy WHERE Id = @Id;"));
            sp.AddParameter("Id", id);
            sp.AddParameter("ModifiedBy", modifiedBy);
            return sp.ExecuteNonQuery() == 0;
        }

        public static bool UpdateStatusByFormatedNoFaktur(string formatedNoFaktur, ApplicationEnums.StatusDigantiOutstanding eStatus, string keterangan, string modifiedBy)
        {
            var iStatus = (int)eStatus;
            var sp = new SpBase(@"UPDATE FakturPajakDigantiOutstanding
SET [StatusOutstanding] = @Status
    ,[Keterangan] = @Keterangan
	,Modified = GETDATE()
	,ModifiedBy = @ModifiedBy
WHERE	FormatedNoFaktur = @FormatedNoFaktur AND IsDeleted = 0");

            sp.AddParameter("ModifiedBy", modifiedBy);
            sp.AddParameter("Status", iStatus);
            sp.AddParameter("Keterangan", string.IsNullOrEmpty(keterangan) ? SqlString.Null : keterangan);
            sp.AddParameter("FormatedNoFaktur", formatedNoFaktur);

            return sp.ExecuteNonQuery() >= 0;
        }

        public static void UpdateKeterangan(long id, string newketerangan, string by)
        {
            var sp =
                new SpBase(
                    @"UPDATE FakturPajakDigantiOutstanding SET Keterangan = @Keterangan, Modified = GETDATE(), ModifiedBy = @By WHERE Id = @Id");
            sp.AddParameter("Keterangan", string.IsNullOrEmpty(newketerangan) ? SqlString.Null : newketerangan);
            sp.AddParameter("By", by);
            sp.AddParameter("Id", id);
            sp.ExecuteNonQuery();
        }

        public static void UpdateKeteranganByIds(string ids, string newketerangan, string by)
        {
            var sp =
                new SpBase(
                    @"UPDATE FakturPajakDigantiOutstanding SET Keterangan = @Keterangan, Modified = GETDATE(), ModifiedBy = @By WHERE Id IN (SELECT CAST(items AS bigint) AS Id FROM [dbo].[SplitString](@Ids, ','))");
            sp.AddParameter("Keterangan", string.IsNullOrEmpty(newketerangan) ? SqlString.Null : newketerangan);
            sp.AddParameter("By", by);
            sp.AddParameter("Ids", ids);
            sp.ExecuteNonQuery();
        }

        public static FakturPajakDigantiOutstanding GetByFormatedNoFaktur(string formatedNoFaktur, int isDeleted = 0)
        {
            var sp = new SpBase(@"SELECT TOP 1 CAST(1 AS int) AS VSequenceNumber
		    ,fpdo.Id
			,fp.NoFakturPajak
			,fpdo.FormatedNoFaktur
			,fpdo.MasaPajak
			,fpdo.TahunPajak
			,fp.NPWPPenjual
			,fp.NamaPenjual
			,fp.TglFaktur
			,fp.[JumlahDPP]
			,fp.[JumlahPPN]
			,fp.[JumlahPPNBM]
			,fpdo.StatusFaktur
			,fpdo.StatusApproval
			,fpdo.Keterangan
			,fpdo.KeteranganDjp
			,fpdo.StatusOutstanding
			,gc.[Name] AS StatusOutstandingName
			,fp.CreatedBy AS ScanByUsername
			,u.UserInitial AS ScanByUserInitial
			,fp.[FormatedNpwpPenjual]
			,tm.[MonthName] AS MasaPajakName
			,fp.FillingIndex
			,fpdo.IsDeleted
			,fpdo.Created
			,fpdo.CreatedBy
			,fpdo.Modified
			,fpdo.ModifiedBy
            ,CAST(1 AS int) AS TotalItems
            ,fp.TahunPajak AS OriginTahunPajak
			,fp.MasaPajak AS OriginMasaPajak
			,tm2.[MonthName] AS OriginMasaPajakName
FROM	[dbo].[FakturPajakDigantiOutstanding] fpdo
		INNER JOIN [dbo].[FakturPajak] fp ON fp.IsDeleted = @IsDeleted AND fp.FormatedNoFaktur = fpdo.FormatedNoFaktur
		INNER JOIN [dbo].[User] u ON u.IsDeleted = 0 AND u.UserName = fp.CreatedBy
		INNER JOIN [dbo].[TMonth] tm ON tm.MonthNumber = fpdo.MasaPajak
		LEFT JOIN GeneralCategory gc ON gc.IsDeleted = 0 AND gc.Category = 'FpDigantiOutstandingStatus'
						AND fpdo.StatusOutstanding = CAST(gc.Code AS int) 
        LEFT JOIN [dbo].[TMonth] tm2 ON tm2.MonthNumber = fp.MasaPajak
WHERE	fp.IsDeleted =  @IsDeleted AND fpdo.[FormatedNoFaktur] = @FormatedNoFaktur AND fpdo.IsDeleted = 0");
            sp.AddParameter("FormatedNoFaktur", formatedNoFaktur);
            sp.AddParameter("IsDeleted", isDeleted);
            var d = GetApplicationObject(sp);
            return d == null || d.Id == 0 ? null : d;
        }

        public static List<FakturPajakDigantiOutstanding> GetByOriginalNoFaktur(string nofaktur)
        {
            var sp = new SpBase(@"EXEC sp_FakturPajakDigantiOutstanding_GetByOriginalNoFaktur @NoFakturPajak = @NoFakturPajakParam");
            sp.AddParameter("NoFakturPajakParam", nofaktur);
            return GetApplicationCollection(sp);
        }

        public static void SetAllExpired(string by)
        {
            var sp = new SpBase(@"EXEC sp_FakturDigantiOutstanding_SetExpired @actor");
            sp.AddParameter("actor", by);
            sp.ExecuteNonQuery();
        }

        public static List<FakturPajakDigantiOutstanding> GetList(Filter filter, out int totalItems, string noFaktur1, string noFaktur2, DateTime? tglFakturStart, DateTime? tglFakturEnd,
            string npwpVendor, string namaVendor, int? status,
            string sNpwpPenjual, string sNamaPenjual, string sNoFaktur, string sTglFaktur, string sMasaPajak, string sTahunPajak,
            string sDppString, string sPpnString, string sPpnBmString, string sStatusFaktur, int? dataType, DateTime? scanDateAwal, DateTime? scanDateAkhir, string fillingIndex,
            string sFillingIndex, string sUserName)
        {
            if (filter.Search != null)
                filter.Search = filter.Search.Trim();
            var sortOrder = filter.SortOrderAsc ? "ASC" : "DESC";
            totalItems = 0;

            var sp = new SpBase(@"EXEC	[dbo].[sp_FakturPajakDigantiOutstanding_GetList]
		@NoFakturPajak1 = @NoFakturPajak1Param,
		@NoFakturPajak2 = @NoFakturPajak2Param,
		@TglFakturStart = @TglFakturStartParam,
		@TglFakturEnd = @TglFakturEndParam,
		@scanDateAwal = @scanDateAwalParam,
		@scanDateAkhir = @scanDateAkhirParam,
		@NpwpVendor = @NpwpVendorParam,
		@NamaVendor = @NamaVendorParam,
		@fillingIndex = @fillingIndexParam,
		@dataType = @dataTypeParam,
		@status = @statusParam,
		@CurrentPage = @CurrentPageParam,
		@ItemPerPage = @ItemPerPageParam,
		@SortColumnName = @SortColumnNameParam,
		@sortOrder = @sortOrderParam,
		@sFormatedNpwpPenjual = @sFormatedNpwpPenjualParam,
		@sNamaPenjual = @sNamaPenjualParam,
		@sFormatedNoFaktur = @sFormatedNoFakturParam,
		@sTglFakturString = @sTglFakturStringParam,
		@sMasaPajakName = @sMasaPajakNameParam,
		@sTahunPajak = @sTahunPajakParam,
		@sDPPString = @sDPPStringParam,
		@sPPNString = @sPPNStringParam,
		@sPPNBMString = @sPPNBMStringParam,
		@sStatusFaktur = @sStatusFakturParam,
		@sFillingIndex = @sFillingIndexParam,
		@sUserName = @sUserNameParam");

            
            sp.AddParameter("NoFakturPajak1Param", string.IsNullOrEmpty(noFaktur1) ? SqlString.Null : noFaktur1);
            sp.AddParameter("NoFakturPajak2Param", string.IsNullOrEmpty(noFaktur2) ? SqlString.Null : noFaktur2);
            sp.AddParameter("TglFakturStartParam", tglFakturStart.HasValue ? tglFakturStart.Value : SqlDateTime.Null);
            sp.AddParameter("TglFakturEndParam", tglFakturEnd.HasValue ? tglFakturEnd.Value : SqlDateTime.Null);
            sp.AddParameter("scanDateAwalParam", scanDateAwal.HasValue ? scanDateAwal.Value : SqlDateTime.Null);
            sp.AddParameter("scanDateAkhirParam", scanDateAkhir.HasValue ? scanDateAkhir.Value : SqlDateTime.Null);
            sp.AddParameter("NpwpVendorParam", string.IsNullOrEmpty(npwpVendor) ? SqlString.Null : npwpVendor);
            sp.AddParameter("NamaVendorParam", string.IsNullOrEmpty(namaVendor) ? SqlString.Null : namaVendor);
            sp.AddParameter("fillingIndexParam", string.IsNullOrEmpty(fillingIndex) ? SqlString.Null : fillingIndex);
            sp.AddParameter("dataTypeParam", dataType.HasValue ? dataType.Value : SqlInt32.Null);
            sp.AddParameter("statusParam", status.HasValue ? status.Value : SqlInt32.Null);
            sp.AddParameter("CurrentPageParam", filter.CurrentPage);
            sp.AddParameter("ItemPerPageParam", filter.ItemsPerPage);
            sp.AddParameter("SortColumnNameParam", filter.SortColumnName);
            sp.AddParameter("sortOrderParam", sortOrder);

            sp.AddParameter("sFormatedNpwpPenjualParam", string.IsNullOrEmpty(sNpwpPenjual) ? SqlString.Null : sNpwpPenjual);
            sp.AddParameter("sNamaPenjualParam", string.IsNullOrEmpty(sNamaPenjual) ? SqlString.Null : sNamaPenjual);
            sp.AddParameter("sFormatedNoFakturParam", string.IsNullOrEmpty(sNoFaktur) ? SqlString.Null : sNoFaktur);
            sp.AddParameter("sTglFakturStringParam", string.IsNullOrEmpty(sTglFaktur) ? SqlString.Null : sTglFaktur);
            sp.AddParameter("sMasaPajakNameParam", string.IsNullOrEmpty(sMasaPajak) ? SqlString.Null : sMasaPajak);
            sp.AddParameter("sTahunPajakParam", string.IsNullOrEmpty(sTahunPajak) ? SqlString.Null : sTahunPajak);
            sp.AddParameter("sDPPStringParam", string.IsNullOrEmpty(sDppString) ? SqlString.Null : sDppString);
            sp.AddParameter("sPPNStringParam", string.IsNullOrEmpty(sPpnString) ? SqlString.Null : sPpnString);
            sp.AddParameter("sPPNBMStringParam", string.IsNullOrEmpty(sPpnBmString) ? SqlString.Null : sPpnBmString);
            sp.AddParameter("sStatusFakturParam", string.IsNullOrEmpty(sStatusFaktur) ? SqlString.Null : sStatusFaktur);
            sp.AddParameter("sFillingIndexParam", string.IsNullOrEmpty(sFillingIndex) ? SqlString.Null : sFillingIndex);
            sp.AddParameter("sUserNameParam", string.IsNullOrEmpty(sUserName) ? SqlString.Null : sUserName);

            var data = GetApplicationCollection(sp);

            if (data.Count > 0)
                totalItems = data[0].TotalItems;

            if (totalItems == 0 && filter.CurrentPage > 1)
            {
                filter.CurrentPage--;
                data = GetList(filter, out totalItems, noFaktur1, noFaktur2, tglFakturStart, tglFakturEnd, npwpVendor, namaVendor,
                    status, sNpwpPenjual, sNamaPenjual, sNoFaktur, sTglFaktur, sMasaPajak, sTahunPajak,
                    sDppString, sPpnString, sPpnBmString, sStatusFaktur, dataType, scanDateAwal, scanDateAkhir, fillingIndex, sFillingIndex, sUserName);
            }
            else if (totalItems > 0 && totalItems < (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = (totalItems <= filter.ItemsPerPage) ? 1 : (totalItems / filter.ItemsPerPage);
                data = GetList(filter, out totalItems, noFaktur1, noFaktur2, tglFakturStart, tglFakturEnd, npwpVendor, namaVendor,
                    status, sNpwpPenjual, sNamaPenjual, sNoFaktur, sTglFaktur, sMasaPajak, sTahunPajak,
                    sDppString, sPpnString, sPpnBmString, sStatusFaktur, dataType, scanDateAwal, scanDateAkhir, fillingIndex, sFillingIndex, sUserName);
            }
            else if (totalItems > 0 && totalItems == (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = 1;
                data = GetList(filter, out totalItems, noFaktur1, noFaktur2, tglFakturStart, tglFakturEnd, npwpVendor, namaVendor,
                    status, sNpwpPenjual, sNamaPenjual, sNoFaktur, sTglFaktur, sMasaPajak, sTahunPajak,
                    sDppString, sPpnString, sPpnBmString, sStatusFaktur, dataType, scanDateAwal, scanDateAkhir, fillingIndex, sFillingIndex, sUserName);
            }

            return data;
        }

        public static List<FakturPajakDigantiOutstanding> GetListWithoutPaging(out int totalItems, string noFaktur1, string noFaktur2, DateTime? tglFakturStart, DateTime? tglFakturEnd,
            string npwpVendor, string namaVendor, int? status,
            string sNpwpPenjual, string sNamaPenjual, string sNoFaktur, string sTglFaktur, string sMasaPajak, string sTahunPajak,
            string sDppString, string sPpnString, string sPpnBmString, string sStatusFaktur, int? dataType, DateTime? scanDateAwal, DateTime? scanDateAkhir, string fillingIndex,
            string sFillingIndex, string sUserName)
        {
            totalItems = 0;

            var sp = new SpBase(@"EXEC	[dbo].[sp_FakturPajakDigantiOutstanding_GetListWithoutPaging]
		@NoFakturPajak1 = @NoFakturPajak1Param,
		@NoFakturPajak2 = @NoFakturPajak2Param,
		@TglFakturStart = @TglFakturStartParam,
		@TglFakturEnd = @TglFakturEndParam,
		@scanDateAwal = @scanDateAwalParam,
		@scanDateAkhir = @scanDateAkhirParam,
		@NpwpVendor = @NpwpVendorParam,
		@NamaVendor = @NamaVendorParam,
		@fillingIndex = @fillingIndexParam,
		@dataType = @dataTypeParam,
		@status = @statusParam,		
		@sFormatedNpwpPenjual = @sFormatedNpwpPenjualParam,
		@sNamaPenjual = @sNamaPenjualParam,
		@sFormatedNoFaktur = @sFormatedNoFakturParam,
		@sTglFakturString = @sTglFakturStringParam,
		@sMasaPajakName = @sMasaPajakNameParam,
		@sTahunPajak = @sTahunPajakParam,
		@sDPPString = @sDPPStringParam,
		@sPPNString = @sPPNStringParam,
		@sPPNBMString = @sPPNBMStringParam,
		@sStatusFaktur = @sStatusFakturParam,
		@sFillingIndex = @sFillingIndexParam,
		@sUserName = @sUserNameParam");


            sp.AddParameter("NoFakturPajak1Param", string.IsNullOrEmpty(noFaktur1) ? SqlString.Null : noFaktur1);
            sp.AddParameter("NoFakturPajak2Param", string.IsNullOrEmpty(noFaktur2) ? SqlString.Null : noFaktur2);
            sp.AddParameter("TglFakturStartParam", tglFakturStart.HasValue ? tglFakturStart.Value : SqlDateTime.Null);
            sp.AddParameter("TglFakturEndParam", tglFakturEnd.HasValue ? tglFakturEnd.Value : SqlDateTime.Null);
            sp.AddParameter("scanDateAwalParam", scanDateAwal.HasValue ? scanDateAwal.Value : SqlDateTime.Null);
            sp.AddParameter("scanDateAkhirParam", scanDateAkhir.HasValue ? scanDateAkhir.Value : SqlDateTime.Null);
            sp.AddParameter("NpwpVendorParam", string.IsNullOrEmpty(npwpVendor) ? SqlString.Null : npwpVendor);
            sp.AddParameter("NamaVendorParam", string.IsNullOrEmpty(namaVendor) ? SqlString.Null : namaVendor);
            sp.AddParameter("fillingIndexParam", string.IsNullOrEmpty(fillingIndex) ? SqlString.Null : fillingIndex);
            sp.AddParameter("dataTypeParam", dataType.HasValue ? dataType.Value : SqlInt32.Null);
            sp.AddParameter("statusParam", status.HasValue ? status.Value : SqlInt32.Null);

            sp.AddParameter("sFormatedNpwpPenjualParam", string.IsNullOrEmpty(sNpwpPenjual) ? SqlString.Null : sNpwpPenjual);
            sp.AddParameter("sNamaPenjualParam", string.IsNullOrEmpty(sNamaPenjual) ? SqlString.Null : sNamaPenjual);
            sp.AddParameter("sFormatedNoFakturParam", string.IsNullOrEmpty(sNoFaktur) ? SqlString.Null : sNoFaktur);
            sp.AddParameter("sTglFakturStringParam", string.IsNullOrEmpty(sTglFaktur) ? SqlString.Null : sTglFaktur);
            sp.AddParameter("sMasaPajakNameParam", string.IsNullOrEmpty(sMasaPajak) ? SqlString.Null : sMasaPajak);
            sp.AddParameter("sTahunPajakParam", string.IsNullOrEmpty(sTahunPajak) ? SqlString.Null : sTahunPajak);
            sp.AddParameter("sDPPStringParam", string.IsNullOrEmpty(sDppString) ? SqlString.Null : sDppString);
            sp.AddParameter("sPPNStringParam", string.IsNullOrEmpty(sPpnString) ? SqlString.Null : sPpnString);
            sp.AddParameter("sPPNBMStringParam", string.IsNullOrEmpty(sPpnBmString) ? SqlString.Null : sPpnBmString);
            sp.AddParameter("sStatusFakturParam", string.IsNullOrEmpty(sStatusFaktur) ? SqlString.Null : sStatusFaktur);
            sp.AddParameter("sFillingIndexParam", string.IsNullOrEmpty(sFillingIndex) ? SqlString.Null : sFillingIndex);
            sp.AddParameter("sUserNameParam", string.IsNullOrEmpty(sUserName) ? SqlString.Null : sUserName);

            var data = GetApplicationCollection(sp);

            if (data.Count > 0)
                totalItems = data[0].TotalItems;
            return data;
        }

        public static List<FakturPajakDigantiOutstanding> GetByMasaPajak(int tahunpajak, int masapajak)
        {
            var sp =
                new SpBase(
                    @"EXEC sp_FakturPajakDigantiOutstanding_GetByMasaPajak @tahunpajak = @tahunpajakParam, @masapajak = @masapajakParam");

            sp.AddParameter("tahunpajakParam", tahunpajak);
            sp.AddParameter("masapajakParam", masapajak);

            return GetApplicationCollection(sp);
        }

        public static List<FakturPajakDigantiOutstanding> GetByDateRange(DateTime dtmin, DateTime dtmax)
        {
            var sp =
                new SpBase(
                    @"EXEC sp_FakturPajakDigantiOutstanding_GetByDateRange @dtmin = @dtminParam, @dtmax = @dtmaxParam");

            sp.AddParameter("dtminParam", dtmin);
            sp.AddParameter("dtmaxParam", dtmax);

            return GetApplicationCollection(sp);

        }

    }
}
