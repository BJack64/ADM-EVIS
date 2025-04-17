using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Collections
{
    public class ReportFakturPajakBelumDiJurnals : ApplicationCollection<ReportFakturPajakBelumDiJurnal, SpBase>
    {
        public static List<ReportFakturPajakBelumDiJurnal> GetListWithoutPaging(string search, DateTime? tglFakturStart, DateTime? tglFakturEnd, string noFakturStart, string noFakturEnd)
        {
            var sp = new SpBase(@"EXEC	[dbo].[sp_ReportFakturPajakBelumDiJurnals_GetListWithoutPaging]
		@tglFakturStart = @tglFakturStart,
		@tglFakturEnd = @tglFakturEnd,
		@noFakturStart = @noFakturStart,
		@noFakturEnd = @noFakturEnd,
		@Search = @Search");

            sp.AddParameter("tglFakturStart", tglFakturStart.HasValue ? tglFakturStart.Value : SqlDateTime.Null);
            sp.AddParameter("tglFakturEnd", tglFakturEnd.HasValue ? tglFakturEnd.Value : SqlDateTime.Null);
            sp.AddParameter("noFakturStart", string.IsNullOrEmpty(noFakturStart) ? SqlString.Null : noFakturStart);
            sp.AddParameter("noFakturEnd", string.IsNullOrEmpty(noFakturEnd) ? SqlString.Null : noFakturEnd);
            sp.AddParameter("Search", string.IsNullOrEmpty(search) ? SqlString.Null : search);

            return GetApplicationCollection(sp);

        }

        public static List<ReportFakturPajakBelumDiJurnal> GetList(Filter filter, out int totalItems, DateTime? tglFakturStart, DateTime? tglFakturEnd, string noFakturStart, string noFakturEnd)
        {
            if (filter.Search != null)
                filter.Search = filter.Search.Trim();
            var sortOrder = filter.SortOrderAsc ? "ASC" : "DESC";
            totalItems = 0;

            var sp = new SpBase(@"EXEC	[dbo].[sp_ReportFakturPajakBelumDiJurnals_GetList]
		@tglFakturStart = @tglFakturStart,
		@tglFakturEnd = @tglFakturEnd,
		@noFakturStart = @noFakturStart,
		@noFakturEnd = @noFakturEnd,
		@Search = @Search,
		@CurrentPage = @CurrentPage,
		@ItemPerPage = @ItemPerPage,
		@SortColumnName = @SortColumnName,
		@sortOrder =@sortOrder");

            sp.AddParameter("CurrentPage", filter.CurrentPage);
            sp.AddParameter("ItemPerPage", filter.ItemsPerPage);
            sp.AddParameter("SortColumnName", filter.SortColumnName);
            sp.AddParameter("sortOrder", sortOrder);

            sp.AddParameter("tglFakturStart", tglFakturStart.HasValue ? tglFakturStart.Value : SqlDateTime.Null);
            sp.AddParameter("tglFakturEnd", tglFakturEnd.HasValue ? tglFakturEnd.Value : SqlDateTime.Null);
            sp.AddParameter("noFakturStart", string.IsNullOrEmpty(noFakturStart) ? SqlString.Null : noFakturStart);
            sp.AddParameter("noFakturEnd", string.IsNullOrEmpty(noFakturEnd) ? SqlString.Null : noFakturEnd);
            sp.AddParameter("Search", string.IsNullOrEmpty(filter.Search) ? SqlString.Null : filter.Search);

            List<ReportFakturPajakBelumDiJurnal> data = GetApplicationCollection(sp);

            if (data.Count > 0)
                totalItems = data[0].TotalItems;

            if (totalItems == 0 && filter.CurrentPage > 1)
            {
                filter.CurrentPage--;
                data = GetList(filter, out totalItems, tglFakturStart, tglFakturEnd, noFakturStart, noFakturEnd);
            }
            else if (totalItems > 0 && totalItems < (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = (totalItems <= filter.ItemsPerPage) ? 1 : (totalItems / filter.ItemsPerPage);
                data = GetList(filter, out totalItems, tglFakturStart, tglFakturEnd, noFakturStart, noFakturEnd);
            }
            else if (totalItems > 0 && totalItems == (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = 1;
                data = GetList(filter, out totalItems, tglFakturStart, tglFakturEnd, noFakturStart, noFakturEnd);
            }

            return data;

        }

    }
}
