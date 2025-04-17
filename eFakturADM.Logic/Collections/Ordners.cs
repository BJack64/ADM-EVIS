using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Collections
{
    public class Ordners : ApplicationCollection<Ordner, SpBase>
    {
        public static List<Ordner> GetList(Filter filter, out int totalItems, string noFaktur1, string noFaktur2,
            DateTime? tglFakturStart, DateTime? tglFakturEnd, int? masaPajak, int? tahunPajak, string picEntry)
        {
            if (filter.Search != null)
                filter.Search = filter.Search.Trim();
            var sortOrder = filter.SortOrderAsc ? "ASC" : "DESC";
            totalItems = 0;
            var sp = new SpBase(string.Format(@"SELECT	fp.*
		                                                , COUNT(fp.FakturPajakId) OVER() AS TotalItems
		                                                , (SELECT COUNT(LogPrintId) FROM LogPrintFakturPajak WHERE IsDeleted = 0 AND FakturPajakId = fp.FakturPajakId) AS PrintCount
                                                FROM	dbo.View_FakturPajak fp
                                                WHERE	[IsDeleted] = 0
		                                                AND [CreatedBy] LIKE '%' + ISNULL(@picEntry, '') + '%'
		                                                AND [Status] = 2
		                                                AND FillingIndex IS NOT NULL
                                                        AND ((LOWER([NoFakturPajak]) BETWEEN LOWER(ISNULL(@NoFakturPajak1, [NoFakturPajak])) AND LOWER(ISNULL(@NoFakturPajak2, [NoFakturPajak]))) OR @NoFakturPajak1 IS NULL OR @NoFakturPajak2 IS NULL)
                                                        AND ((@MasaPajak IS NOT NULL AND [MasaPajak] = @MasaPajak) OR @MasaPajak IS NULL)
		                                                AND ((@TahunPajak IS NOT NULL AND [TahunPajak] = @TahunPajak) OR @TahunPajak IS NULL)
		                                                AND ((CAST([TglFaktur] AS DATE) BETWEEN CAST(ISNULL(@TglFakturStart, [TglFaktur]) AS DATE) 
				                                                AND CAST(ISNULL(@TglFakturEnd, [TglFaktur]) AS DATE)
	                                                          ) OR @TglFakturStart IS NULL OR @TglFakturEnd IS NULL)
		                                                AND (@Search IS NULL 
			                                                OR LOWER(FormatedNPWPLawanTransaksi) LIKE '%' + LOWER(@Search) + '%'
			                                                OR LOWER(NamaLawanTransaksi) LIKE '%' + LOWER(@Search) + '%'
			                                                OR FormatedNoFaktur LIKE '%' + @Search + '%'
			                                                OR CONVERT(varchar, TglFaktur, 103) LIKE '%' + LOWER(@Search) + '%'
			                                                OR CAST(MasaPajak as varchar) LIKE '%' + @Search + '%'
			                                                OR CAST(TahunPajak as varchar) LIKE '%' + @Search + '%'
			                                                OR CAST(JumlahDPP as varchar) LIKE '%' + @Search + '%'
			                                                OR CAST(JumlahPPN as varchar) LIKE '%' + @Search + '%'
			                                                OR CAST(JumlahPPNBM as varchar) LIKE '%' + @Search + '%'
			                                                OR FillingIndex LIKE '%' + @Search + '%'
			                                                OR FillingIndex LIKE '%' + @Search + '%'
		                                                )
                                                ORDER BY {0} {1}
                                                OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
                                                FETCH NEXT @ItemPerPage ROWS ONLY", filter.SortColumnName, sortOrder));
            sp.AddParameter("CurrentPage", filter.CurrentPage);
            sp.AddParameter("ItemPerPage", filter.ItemsPerPage);
            sp.AddParameter("NoFakturPajak1", string.IsNullOrEmpty(noFaktur1) ? SqlString.Null : noFaktur1);
            sp.AddParameter("NoFakturPajak2", string.IsNullOrEmpty(noFaktur2) ? SqlString.Null : noFaktur2);
            sp.AddParameter("MasaPajak", masaPajak.HasValue ? masaPajak.Value : SqlInt32.Null);
            sp.AddParameter("TahunPajak", tahunPajak.HasValue ? tahunPajak.Value : SqlInt32.Null);
            sp.AddParameter("TglFakturStart", tglFakturStart.HasValue ? tglFakturStart.Value : SqlDateTime.Null);
            sp.AddParameter("TglFakturEnd", tglFakturEnd.HasValue ? tglFakturEnd.Value : SqlDateTime.Null);
            sp.AddParameter("Search", string.IsNullOrEmpty(filter.Search) ? SqlString.Null : filter.Search);
            sp.AddParameter("picEntry", string.IsNullOrEmpty(picEntry) ? SqlString.Null : picEntry);

            var data = GetApplicationCollection(sp);

            if (data.Count > 0)
                totalItems = data[0].TotalItems;

            if (totalItems == 0 && filter.CurrentPage > 1)
            {
                filter.CurrentPage--;
                data = GetList(filter, out totalItems, noFaktur1, noFaktur2, tglFakturStart, tglFakturEnd,
                    masaPajak, tahunPajak, picEntry);
            }
            else if (totalItems > 0 && totalItems < (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = (totalItems <= filter.ItemsPerPage) ? 1 : (totalItems / filter.ItemsPerPage);
                data = GetList(filter, out totalItems, noFaktur1, noFaktur2, tglFakturStart, tglFakturEnd,
                    masaPajak, tahunPajak, picEntry);
            }
            else if (totalItems > 0 && totalItems == (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = 1;
                data = GetList(filter, out totalItems, noFaktur1, noFaktur2, tglFakturStart, tglFakturEnd,
                     masaPajak, tahunPajak, picEntry);
            }

            return data;
        }

        public static List<Ordner> GetListWithoutPaging(string search, string noFaktur1, string noFaktur2,
            DateTime? tglFakturStart, DateTime? tglFakturEnd, int? masaPajak, int? tahunPajak, string picEntry)
        {
            var sp = new SpBase(@"SELECT	fp.*
		, COUNT(fp.FakturPajakId) OVER() AS TotalItems
		, (SELECT COUNT(LogPrintId) FROM LogPrintFakturPajak WHERE IsDeleted = 0 AND FakturPajakId = fp.FakturPajakId) AS PrintCount
FROM	dbo.View_FakturPajak fp
WHERE	[IsDeleted] = 0
		AND [CreatedBy] LIKE '%' + ISNULL(@picEntry, '') + '%'
		AND [Status] = 2
		AND FillingIndex IS NOT NULL
        AND ((LOWER([NoFakturPajak]) BETWEEN LOWER(ISNULL(@NoFakturPajak1, [NoFakturPajak])) AND LOWER(ISNULL(@NoFakturPajak2, [NoFakturPajak]))) OR @NoFakturPajak1 IS NULL OR @NoFakturPajak2 IS NULL)
        AND ((@MasaPajak IS NOT NULL AND [MasaPajak] = @MasaPajak) OR @MasaPajak IS NULL)
		AND ((@TahunPajak IS NOT NULL AND [TahunPajak] = @TahunPajak) OR @TahunPajak IS NULL)
		AND ((CAST([TglFaktur] AS DATE) BETWEEN CAST(ISNULL(@TglFakturStart, [TglFaktur]) AS DATE) 
				AND CAST(ISNULL(@TglFakturEnd, [TglFaktur]) AS DATE)
	          ) OR @TglFakturStart IS NULL OR @TglFakturEnd IS NULL)
		AND (@Search IS NULL 
			OR LOWER(FormatedNPWPLawanTransaksi) LIKE '%' + LOWER(@Search) + '%'
			OR LOWER(NamaLawanTransaksi) LIKE '%' + LOWER(@Search) + '%'
			OR FormatedNoFaktur LIKE '%' + @Search + '%'
			OR CONVERT(varchar, TglFaktur, 103) LIKE '%' + LOWER(@Search) + '%'
			OR CAST(MasaPajak as varchar) LIKE '%' + @Search + '%'
			OR CAST(TahunPajak as varchar) LIKE '%' + @Search + '%'
			OR CAST(JumlahDPP as varchar) LIKE '%' + @Search + '%'
			OR CAST(JumlahPPN as varchar) LIKE '%' + @Search + '%'
			OR CAST(JumlahPPNBM as varchar) LIKE '%' + @Search + '%'
			OR FillingIndex LIKE '%' + @Search + '%'
			OR FillingIndex LIKE '%' + @Search + '%'
		)");

            sp.AddParameter("NoFakturPajak1", string.IsNullOrEmpty(noFaktur1) ? SqlString.Null : noFaktur1);
            sp.AddParameter("NoFakturPajak2", string.IsNullOrEmpty(noFaktur2) ? SqlString.Null : noFaktur2);
            sp.AddParameter("MasaPajak", masaPajak.HasValue ? masaPajak.Value : SqlInt32.Null);
            sp.AddParameter("TahunPajak", tahunPajak.HasValue ? tahunPajak.Value : SqlInt32.Null);
            sp.AddParameter("TglFakturStart", tglFakturStart.HasValue ? tglFakturStart.Value : SqlDateTime.Null);
            sp.AddParameter("TglFakturEnd", tglFakturEnd.HasValue ? tglFakturEnd.Value : SqlDateTime.Null);
            sp.AddParameter("Search", string.IsNullOrEmpty(search) ? SqlString.Null : search);
            sp.AddParameter("picEntry", string.IsNullOrEmpty(picEntry) ? SqlString.Null : picEntry);

            var data = GetApplicationCollection(sp);

            return data;

        }

        public static List<Ordner> GetByFakturPajakIds(string fakturPajakIds, string orderColumn, bool isAsc)
        {
            var sortOrder = isAsc ? "ASC" : "DESC";
            var sp = new SpBase(string.Format(@"SELECT	fp.*
		, COUNT(fp.FakturPajakId) OVER() AS TotalItems
		, (SELECT COUNT(LogPrintId) FROM LogPrintFakturPajak WHERE IsDeleted = 0 AND FakturPajakId = fp.FakturPajakId) AS PrintCount
FROM	dbo.View_FakturPajak fp
WHERE	[IsDeleted] = 0
		AND [Status] = 2
		AND [FillingIndex] IS NOT NULL
		AND fp.FakturPajakId IN (SELECT Data FROM dbo.Split(@fakturPajakIds))
ORDER BY {0} {1}", orderColumn, sortOrder));

            sp.AddParameter("fakturPajakIds", fakturPajakIds);

            return GetApplicationCollection(sp);

        } 

    }
}
