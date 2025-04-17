using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Collections
{
    public class ReportFakturPajakOutstandings : ApplicationCollection<ReportFakturPajakOutstanding, SpBase>
    {

        public static List<ReportFakturPajakOutstanding> GetListWithouPaging(string search, DateTime? postingDateStart, DateTime? postingDateEnd, string docSapStart, string docSapEnd)
        {
            var sp = new SpBase(@"EXEC	[dbo].[sp_ReportFakturPajakOutstandings_GetListWithouPaging]
		@postingDateStart = @postingDateStart,
		@postingDateEnd = @postingDateEnd,
		@docSapStart = @docSapStart,
		@docSapEnd = @docSapEnd,
		@Search = @Search");

            sp.AddParameter("postingDateStart", postingDateStart.HasValue ? postingDateStart.Value : SqlDateTime.Null);
            sp.AddParameter("postingDateEnd", postingDateEnd.HasValue ? postingDateEnd.Value : SqlDateTime.Null);
            sp.AddParameter("docSapStart", string.IsNullOrEmpty(docSapStart) ? SqlString.Null : docSapStart);
            sp.AddParameter("docSapEnd", string.IsNullOrEmpty(docSapEnd) ? SqlString.Null : docSapEnd);
            sp.AddParameter("Search", string.IsNullOrEmpty(search) ? SqlString.Null : search);

            return GetApplicationCollection(sp);
        }

        public static List<ReportFakturPajakOutstanding> GetList(Filter filter, out int totalItems, DateTime? postingDateStart, DateTime? postingDateEnd, string docSapStart, string docSapEnd)
        {
            if (filter.Search != null)
                filter.Search = filter.Search.Trim();
            var sortOrder = filter.SortOrderAsc ? "ASC" : "DESC";
            totalItems = 0;

            var sp = new SpBase(@"EXEC	[dbo].[sp_ReportFakturPajakOutstandings_GetList]
		@postingDateStart = @postingDateStart,
		@postingDateEnd = @postingDateEnd,
		@docSapStart = @docSapStart,
		@docSapEnd = @docSapEnd,
		@Search = @Search,
		@CurrentPage = @CurrentPage,
		@ItemPerPage = @ItemPerPage,
		@SortColumnName = @SortColumnName,
		@sortOrder = @sortOrder");

            sp.AddParameter("CurrentPage", filter.CurrentPage);
            sp.AddParameter("ItemPerPage", filter.ItemsPerPage);
            sp.AddParameter("SortColumnName", filter.SortColumnName);
            sp.AddParameter("sortOrder", sortOrder);

            sp.AddParameter("postingDateStart", postingDateStart.HasValue ? postingDateStart.Value : SqlDateTime.Null);
            sp.AddParameter("postingDateEnd", postingDateEnd.HasValue ? postingDateEnd.Value : SqlDateTime.Null);
            sp.AddParameter("docSapStart", string.IsNullOrEmpty(docSapStart) ? SqlString.Null : docSapStart);
            sp.AddParameter("docSapEnd", string.IsNullOrEmpty(docSapEnd) ? SqlString.Null : docSapEnd);
            sp.AddParameter("Search", string.IsNullOrEmpty(filter.Search) ? SqlString.Null : filter.Search);

            List<ReportFakturPajakOutstanding> data = GetApplicationCollection(sp);

            if (data.Count > 0)
                totalItems = data[0].TotalItems;

            if (totalItems == 0 && filter.CurrentPage > 1)
            {
                filter.CurrentPage--;
                data = GetList(filter, out totalItems, postingDateStart, postingDateEnd, docSapStart, docSapEnd);
            }
            else if (totalItems > 0 && totalItems < (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = (totalItems <= filter.ItemsPerPage) ? 1 : (totalItems / filter.ItemsPerPage);
                data = GetList(filter, out totalItems, postingDateStart, postingDateEnd, docSapStart, docSapEnd);
            }
            else if (totalItems > 0 && totalItems == (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = 1;
                data = GetList(filter, out totalItems, postingDateStart, postingDateEnd, docSapStart, docSapEnd);
            }

            return data;
        }

    }
}
