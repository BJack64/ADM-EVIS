using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Collections
{
    public class ReportFakturPajakMasukans : ApplicationCollection<ReportFakturPajakMasukan, SpBase>
    {

        public static List<ReportFakturPajakMasukan> GetListWithouPaging(string search, DateTime? tglFakturStart, DateTime? tglFakturEnd, string picEntry, string fillingIndexStart, string fillingIndexEnd, int? masaPajak, int? tahunPajak)
        {
            var sp = new SpBase(@"EXEC	[dbo].[sp_ReportFakturPajakMasukans_GetListWithouPaging]
		@dTglFakturStart = @dTglFakturStartParam,
		@dTglFakturEnd = @dTglFakturEndParam,
		@picEntry = @picEntryParam,
		@Search = @SearchParam,
        @fillingIndexStart = @fillingIndexStartParam,
        @fillingIndexEnd = @fillingIndexEndParam,
        @masaPajak = @masaPajakParam,
        @tahunPajak = @tahunPajakParam");

            sp.AddParameter("dTglFakturStartParam", tglFakturStart.HasValue ? tglFakturStart.Value : SqlDateTime.Null);
            sp.AddParameter("dTglFakturEndParam", tglFakturEnd.HasValue ? tglFakturEnd.Value : SqlDateTime.Null);
            sp.AddParameter("picEntryParam", string.IsNullOrEmpty(picEntry) ? SqlString.Null : picEntry);
            sp.AddParameter("SearchParam", string.IsNullOrEmpty(search) ? SqlString.Null : search);
            sp.AddParameter("fillingIndexStartParam",
                string.IsNullOrEmpty(fillingIndexStart) ? SqlString.Null : fillingIndexStart);
            sp.AddParameter("fillingIndexEndParam",
                string.IsNullOrEmpty(fillingIndexEnd) ? SqlString.Null : fillingIndexEnd);
            sp.AddParameter("masaPajakParam", masaPajak.HasValue ? masaPajak.Value : SqlInt32.Null);
            sp.AddParameter("tahunPajakParam", tahunPajak.HasValue ? tahunPajak.Value : SqlInt32.Null);

            return GetApplicationCollection(sp);
        }

        public static List<ReportFakturPajakMasukan> GetList(Filter filter, out int totalItems, DateTime? tglFakturStart, DateTime? tglFakturEnd, string picEntry, string fillingIndexStart, string fillingIndexEnd, int? masaPajak, int? tahunPajak)
        {
            if (filter.Search != null)
                filter.Search = filter.Search.Trim();
            var sortOrder = filter.SortOrderAsc ? "ASC" : "DESC";
            totalItems = 0;

            var sp = new SpBase(@"EXEC	[dbo].[sp_ReportFakturPajakMasukans_GetList]
		@dTglFakturStart = @dTglFakturStartParam,
		@dTglFakturEnd = @dTglFakturEndParam,
		@picEntry = @picEntryParam,
		@Search = @SearchParam,
		@CurrentPage = @CurrentPageParam,
		@ItemPerPage = @ItemPerPageParam,
		@SortColumnName = @SortColumnNameParam,
		@sortOrder = @sortOrderParam,
        @fillingIndexStart = @fillingIndexStartParam,
        @fillingIndexEnd = @fillingIndexEndParam,
        @masaPajak = @masaPajakParam,
        @tahunPajak = @tahunPajakParam", filter.SortColumnName, sortOrder);

            sp.AddParameter("CurrentPageParam", filter.CurrentPage);
            sp.AddParameter("ItemPerPageParam", filter.ItemsPerPage);
            sp.AddParameter("SortColumnNameParam", filter.SortColumnName);
            sp.AddParameter("sortOrderParam", sortOrder);
            sp.AddParameter("dTglFakturStartParam", tglFakturStart.HasValue ? tglFakturStart.Value : SqlDateTime.Null);
            sp.AddParameter("dTglFakturEndParam", tglFakturEnd.HasValue ? tglFakturEnd.Value : SqlDateTime.Null);
            sp.AddParameter("picEntryParam", string.IsNullOrEmpty(picEntry) ? SqlString.Null : picEntry);
            sp.AddParameter("SearchParam", string.IsNullOrEmpty(filter.Search) ? SqlString.Null : filter.Search);
            sp.AddParameter("fillingIndexStartParam",
                string.IsNullOrEmpty(fillingIndexStart) ? SqlString.Null : fillingIndexStart);
            sp.AddParameter("fillingIndexEndParam",
                string.IsNullOrEmpty(fillingIndexEnd) ? SqlString.Null : fillingIndexEnd);

            sp.AddParameter("masaPajakParam", masaPajak.HasValue ? masaPajak.Value : SqlInt32.Null);
            sp.AddParameter("tahunPajakParam", tahunPajak.HasValue ? tahunPajak.Value : SqlInt32.Null);

            List<ReportFakturPajakMasukan> data = GetApplicationCollection(sp);

            if (data.Count > 0)
                totalItems = data[0].TotalItems;

            if (totalItems == 0 && filter.CurrentPage > 1)
            {
                filter.CurrentPage--;
                data = GetList(filter, out totalItems, tglFakturStart, tglFakturEnd, picEntry, fillingIndexStart, fillingIndexEnd, masaPajak, tahunPajak);
            }
            else if (totalItems > 0 && totalItems < (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = (totalItems <= filter.ItemsPerPage) ? 1 : (totalItems / filter.ItemsPerPage);
                data = GetList(filter, out totalItems, tglFakturStart, tglFakturEnd, picEntry, fillingIndexStart, fillingIndexEnd, masaPajak, tahunPajak);
            }
            else if (totalItems > 0 && totalItems == (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = 1;
                data = GetList(filter, out totalItems, tglFakturStart, tglFakturEnd, picEntry, fillingIndexStart, fillingIndexEnd, masaPajak, tahunPajak);
            }

            return data;
        }

    }
}
