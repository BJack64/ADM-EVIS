using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Logic.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace eFakturADM.Logic.Collections
{
    public class FakturPajakOutstandingCollections : ApplicationCollection<FakturPajak, SpBase>
    {
        public static List<FakturPajak> GetByFormatedNoFakturOutstanding(string formatedNoFaktur)
        {
            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakId) OVER() AS TotalItems
FROM	dbo.View_FakturPajak fp
WHERE fp.IsDeleted = 0 AND fp.FormatedNoFaktur = @formatedNoFaktur AND fp.IsOutstanding = 1");
            sp.AddParameter("formatedNoFaktur", formatedNoFaktur);
            var dbData = GetApplicationCollection(sp);
            return dbData;
        }
        public static List<FakturPajak> GetList(Filter filter, out int totalItems, string noFaktur1, string noFaktur2, DateTime? tglFakturStart, DateTime? tglFakturEnd,
           string npwpVendor, string namaVendor, int? status,
           string sNpwpPenjual, string sNamaPenjual, string sNoFaktur, string sTglFaktur, string sMasaPajak, string sTahunPajak,
           string sDppString, string sPpnString, string sPpnBmString, string sStatusFaktur, int? dataType, DateTime? scanDateAwal, DateTime? scanDateAkhir, string fillingIndex,
           string sFillingIndex, string sUserName)
        {
            if (filter.Search != null)
                filter.Search = filter.Search.Trim();
            var sortOrder = filter.SortOrderAsc ? "ASC" : "DESC";
            totalItems = 0;

           var sp = new SpBase(@"EXEC	[dbo].[sp_FakturPajak_GetList WHERE fp.IsOutstanding = 1 AND fp.Deletd = 0]

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
    }
    }
