using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Collections
{
    public class ReportDetailFakturPajaks : ApplicationCollection<ReportDetailFakturPajak, SpBase>
    {

        public static List<ReportDetailFakturPajak> GetListWithouPaging(string search, string noFaktur1, string noFaktur2, 
            string npwpVendor, string namaVendor, DateTime? tglFakturStart, DateTime? tglFakturEnd, int? masaPajak, int? tahunPajak, DateTime? scanDateAwal, DateTime? scanDateAkhir)
        {
            var sp = new SpBase(@"EXEC	[dbo].[sp_ReportDetailFakturPajak_GetListWithouPaging]
                @noFaktur1 = @noFaktur1,
                @noFaktur2 = @noFaktur2,
                @Search = @Search,
                @npwpvendor = @NpwpVendor,
                @namavendor = @NamaVendor,
                @masaPajak = @MasaPajak,
                @tahunPajak = @TahunPajak,
                @tglfakturfrom = @TglFakturStart,
                @tglfakturto = @TglFakturEnd,
                @scandatefrom = @scanDateAwal,
                @scandateto = @scanDateAkhir
");

            sp.AddParameter("noFaktur1", string.IsNullOrEmpty(noFaktur1) ? SqlString.Null : noFaktur1);
            sp.AddParameter("noFaktur2", string.IsNullOrEmpty(noFaktur2) ? SqlString.Null : noFaktur2);
            sp.AddParameter("Search", string.IsNullOrEmpty(search) ? SqlString.Null : search);
            sp.AddParameter("NpwpVendor", string.IsNullOrEmpty(npwpVendor) ? SqlString.Null : npwpVendor);
            sp.AddParameter("NamaVendor", string.IsNullOrEmpty(namaVendor) ? SqlString.Null : namaVendor);
            sp.AddParameter("MasaPajak", masaPajak.HasValue ? masaPajak.Value : SqlInt32.Null);
            sp.AddParameter("TahunPajak", tahunPajak.HasValue ? tahunPajak.Value : SqlInt32.Null);
            sp.AddParameter("TglFakturStart", tglFakturStart.HasValue ? tglFakturStart.Value : SqlDateTime.Null);
            sp.AddParameter("TglFakturEnd", tglFakturEnd.HasValue ? tglFakturEnd.Value : SqlDateTime.Null);
            sp.AddParameter("scanDateAwal", scanDateAwal.HasValue ? scanDateAwal.Value : SqlDateTime.Null);
            sp.AddParameter("scanDateAkhir", scanDateAkhir.HasValue ? scanDateAkhir.Value : SqlDateTime.Null);

            return GetApplicationCollection(sp);
        }

        public static List<ReportDetailFakturPajak> GetList(Filter filter, out int totalItems, string noFaktur1, string noFaktur2, DateTime? tglFakturStart, DateTime? tglFakturEnd,
            string npwpVendor, string namaVendor, int? masaPajak, int? tahunPajak, DateTime? scanDateAwal, DateTime? scanDateAkhir)
        {
            if (filter.Search != null)
                filter.Search = filter.Search.Trim();
            var sortOrder = filter.SortOrderAsc ? "ASC" : "DESC";
            totalItems = 0;

            var sp = new SpBase(@"EXEC	[dbo].[sp_ReportDetailFakturPajak_GetList]
		                            @noFaktur1 = @noFaktur1,
		                            @noFaktur2 = @noFaktur2,
		                            @Search = @Search,
                                    @npwpvendor = @NpwpVendor,
                                    @namavendor = @NamaVendor,
                                    @masaPajak = @MasaPajak,
                                    @tahunPajak = @TahunPajak,
                                    @tglfakturfrom = @TglFakturStart,
                                    @tglfakturto = @TglFakturEnd,
                                    @scandatefrom = @scanDateAwal,
                                    @scandateto = @scanDateAkhir,
		                            @CurrentPage = @CurrentPage,
		                            @ItemPerPage = @ItemPerPage,
		                            @SortColumnName = @SortColumnName,
		                            @sortOrder = @sortOrder");

            sp.AddParameter("CurrentPage", filter.CurrentPage);
            sp.AddParameter("ItemPerPage", filter.ItemsPerPage);
            sp.AddParameter("noFaktur1", string.IsNullOrEmpty(noFaktur1) ? SqlString.Null : noFaktur1);
            sp.AddParameter("noFaktur2", string.IsNullOrEmpty(noFaktur2) ? SqlString.Null : noFaktur2);
            sp.AddParameter("Search", string.IsNullOrEmpty(filter.Search) ? SqlString.Null : filter.Search);
            sp.AddParameter("SortColumnName", filter.SortColumnName);
            sp.AddParameter("sortOrder", sortOrder);
            sp.AddParameter("NpwpVendor", string.IsNullOrEmpty(npwpVendor) ? SqlString.Null : npwpVendor);
            sp.AddParameter("NamaVendor", string.IsNullOrEmpty(namaVendor) ? SqlString.Null : namaVendor);
            sp.AddParameter("MasaPajak", masaPajak.HasValue ? masaPajak.Value : SqlInt32.Null);
            sp.AddParameter("TahunPajak", tahunPajak.HasValue ? tahunPajak.Value : SqlInt32.Null);
            sp.AddParameter("TglFakturStart", tglFakturStart.HasValue ? tglFakturStart.Value : SqlDateTime.Null);
            sp.AddParameter("TglFakturEnd", tglFakturEnd.HasValue ? tglFakturEnd.Value : SqlDateTime.Null);
            sp.AddParameter("scanDateAwal", scanDateAwal.HasValue ? scanDateAwal.Value : SqlDateTime.Null);
            sp.AddParameter("scanDateAkhir", scanDateAkhir.HasValue ? scanDateAkhir.Value : SqlDateTime.Null);

            List<ReportDetailFakturPajak> data = GetApplicationCollection(sp);

            if (data.Count > 0)
                totalItems = data[0].TotalItems;

            if (totalItems == 0 && filter.CurrentPage > 1)
            {
                filter.CurrentPage--;
                data = GetList(filter, out totalItems, noFaktur1, noFaktur2, tglFakturStart, tglFakturEnd, npwpVendor, namaVendor,
                    masaPajak, tahunPajak, scanDateAwal, scanDateAkhir);
            }
            else if (totalItems > 0 && totalItems < (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = (totalItems <= filter.ItemsPerPage) ? 1 : (totalItems / filter.ItemsPerPage);
                data = GetList(filter, out totalItems, noFaktur1, noFaktur2, tglFakturStart, tglFakturEnd, npwpVendor, namaVendor,
          masaPajak, tahunPajak, scanDateAwal, scanDateAkhir);
            }
            else if (totalItems > 0 && totalItems == (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = 1;
                data = GetList(filter, out totalItems, noFaktur1, noFaktur2, tglFakturStart, tglFakturEnd, npwpVendor, namaVendor,
         masaPajak, tahunPajak, scanDateAwal, scanDateAkhir);
            }

            return data;
        }
        
    }
}
