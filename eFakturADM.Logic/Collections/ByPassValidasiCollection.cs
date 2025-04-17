using System;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using System.Collections.Generic;
using eFakturADM.Logic.Utilities;
using static eFakturADM.Logic.Objects.FakturPajak;

namespace eFakturADM.Logic.Collections
{
    public class ByPassValidasiCollection : ApplicationCollection<ByPassValidasi, SpBase>
    {

        public static List<ByPassValidasi> GetList(Filter filter, out int totalItems, 
            string noFaktur1, string noFaktur2,
            DateTime? tglFakturStart, DateTime? tglFakturEnd,
            DateTime? receivedDateAwal, DateTime? receivedDateAkhir,
            string Source, string CheckingStatus,
            string npwpVendor, string namaVendor, string Status,
            
            string search_NpwpPenjual, string search_NamaPenjual, 
            string search_NoFaktur, string search_TglFaktur, 
            string search_DppString, string search_PpnString, 
            string search_Source, string search_Status, 
            string search_CheckingStatus, string search_CheckingCount,
            string search_CheckingStart, string search_CheckingLast, string search_IsByPass)
        {
            if (filter.Search != null)
                filter.Search = filter.Search.Trim();
            var sortOrder = filter.SortOrderAsc ? "ASC" : "DESC";
            totalItems = 0;

            var sp = new SpBase(string.Format(@"
                EXEC	[dbo].[sp_ByPassValidasi_GetList]
		                @NoFakturPajak1 = @NoFakturPajak1Param, 
		                @NoFakturPajak2 = @NoFakturPajak2Param, 
		                @TglFakturStart = @TglFakturStartParam, 
     	                @TglFakturEnd = @TglFakturEndParam, 
		                @ReceivedDateAwal = @ReceivedDateAwalParam, 
		                @ReceivedDateAkhir = @ReceivedDateAkhirParam, 
                        @Sources = @SourceParam, 		                
                        @CheckingStatus = @CheckingStatusParam, 		                
                        @VendorNPWP = @VendorNPWPParam, 
		                @VendorName = @VendorNameParam, 
		                @Status = @StatusParam, 
                        @search_VendorNPWP = @search_VendorNPWPParam,
                        @search_VendorName = @search_VendorNameParam,
                        @search_NoFaktur = @search_NoFakturParam,
                        @search_TglFaktur = @search_TglFakturParam,
                        @search_DppString = @search_DppStringParam,
                        @search_PpnString = @search_PpnStringParam,
                        @search_Source = @search_SourceParam,
                        @search_Status = @search_StatusParam,
                        @search_CheckingCount = @search_CheckingCountParam,
                        @search_CheckingStatus = @search_CheckingStatus,
                        @search_CheckingStart = @search_CheckingStartParam,
                        @search_CheckingLast = @search_CheckingLastParam,
                        @search_IsByPass = @search_IsByPassParam,
		                @CurrentPage = @CurrentPageParam, 
		                @ItemPerPage = @ItemPerPageParam, 
		                @SortColumnName = @SortColumnNameParam, 
		                @sortOrder = @sortOrderParam 
            ", filter.SortColumnName, sortOrder));

            sp.AddParameter("NoFakturPajak1Param", string.IsNullOrEmpty(noFaktur1) ? SqlString.Null : noFaktur1);
            sp.AddParameter("NoFakturPajak2Param", string.IsNullOrEmpty(noFaktur2) ? SqlString.Null : noFaktur2);
            sp.AddParameter("ReceivedDateAwalParam", receivedDateAwal.HasValue ? receivedDateAwal.Value : SqlDateTime.Null);
            sp.AddParameter("ReceivedDateAkhirParam", receivedDateAkhir.HasValue ? receivedDateAkhir.Value : SqlDateTime.Null);
            sp.AddParameter("TglFakturStartParam", tglFakturStart.HasValue ? tglFakturStart.Value : SqlDateTime.Null);
            sp.AddParameter("TglFakturEndParam", tglFakturEnd.HasValue ? tglFakturEnd.Value : SqlDateTime.Null);
            sp.AddParameter("SourceParam", string.IsNullOrEmpty(Source) ? SqlString.Null : Source);
            sp.AddParameter("CheckingStatusParam", string.IsNullOrEmpty(CheckingStatus) ? SqlString.Null : CheckingStatus);
            sp.AddParameter("VendorNPWPParam", string.IsNullOrEmpty(npwpVendor) ? SqlString.Null : npwpVendor);
            sp.AddParameter("VendorNameParam", string.IsNullOrEmpty(namaVendor) ? SqlString.Null : namaVendor);
            sp.AddParameter("StatusParam", string.IsNullOrEmpty(Status) ? SqlString.Null : Status);


            sp.AddParameter("search_VendorNPWPParam", string.IsNullOrEmpty(search_NpwpPenjual) ? SqlString.Null : search_NpwpPenjual);
            sp.AddParameter("search_VendorNameParam", string.IsNullOrEmpty(search_NamaPenjual) ? SqlString.Null : search_NamaPenjual);
            sp.AddParameter("search_NoFakturParam", string.IsNullOrEmpty(search_NoFaktur) ? SqlString.Null : search_NoFaktur);
            sp.AddParameter("search_TglFakturParam", string.IsNullOrEmpty(search_TglFaktur) ? SqlString.Null : search_TglFaktur);
            sp.AddParameter("search_DppStringParam", string.IsNullOrEmpty(search_DppString) ? SqlString.Null : search_DppString);
            sp.AddParameter("search_PpnStringParam", string.IsNullOrEmpty(search_PpnString) ? SqlString.Null : search_PpnString);
            sp.AddParameter("search_SourceParam", string.IsNullOrEmpty(search_Source) ? SqlString.Null : search_Source);
            sp.AddParameter("search_StatusParam", string.IsNullOrEmpty(search_Status) ? SqlString.Null : search_Status);
            sp.AddParameter("search_CheckingStatus", string.IsNullOrEmpty(search_CheckingStatus) ? SqlString.Null : search_CheckingStatus);
            sp.AddParameter("search_CheckingCountParam", string.IsNullOrEmpty(search_CheckingCount) ? SqlString.Null : search_CheckingCount);
            sp.AddParameter("search_CheckingStartParam", string.IsNullOrEmpty(search_CheckingStart) ? SqlString.Null : search_CheckingStart);
            sp.AddParameter("search_CheckingLastParam", string.IsNullOrEmpty(search_CheckingLast) ? SqlString.Null : search_CheckingLast);
            sp.AddParameter("search_IsByPassParam", string.IsNullOrEmpty(search_IsByPass) ? SqlString.Null : search_IsByPass);

            sp.AddParameter("CurrentPageParam", filter.CurrentPage);
            sp.AddParameter("ItemPerPageParam", filter.ItemsPerPage);
            sp.AddParameter("SortColumnNameParam", filter.SortColumnName);
            sp.AddParameter("sortOrderParam", sortOrder);


            List<ByPassValidasi> data = GetApplicationCollection(sp);
            
            if (data.Count > 0)
                totalItems = data[0].TotalItems;

            if (totalItems == 0 && filter.CurrentPage > 1)
            {
                filter.CurrentPage--;
                data = GetList(filter, out totalItems,
                    noFaktur1,  noFaktur2, tglFakturStart,  tglFakturEnd, receivedDateAwal, receivedDateAkhir,
                    Source,  CheckingStatus,  npwpVendor,  namaVendor,  Status,
                    search_NpwpPenjual,  search_NamaPenjual, search_NoFaktur,  search_TglFaktur,
                    search_DppString,  search_PpnString, search_Source,  search_Status,
                    search_CheckingStatus,  search_CheckingCount, search_CheckingStart,  search_CheckingLast, search_IsByPass);
            }
            else if (totalItems > 0 && totalItems < (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = (totalItems <= filter.ItemsPerPage) ? 1 : (totalItems / filter.ItemsPerPage);
                data = GetList(filter, out totalItems,
                    noFaktur1, noFaktur2, tglFakturStart, tglFakturEnd, receivedDateAwal, receivedDateAkhir,
                    Source, CheckingStatus, npwpVendor, namaVendor, Status,
                    search_NpwpPenjual, search_NamaPenjual, search_NoFaktur, search_TglFaktur,
                    search_DppString, search_PpnString, search_Source, search_Status,
                    search_CheckingStatus, search_CheckingCount, search_CheckingStart, search_CheckingLast, search_IsByPass);

            }
            else if (totalItems > 0 && totalItems == (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = 1;
                data = GetList(filter, out totalItems,
                    noFaktur1, noFaktur2, tglFakturStart, tglFakturEnd, receivedDateAwal, receivedDateAkhir,
                    Source, CheckingStatus, npwpVendor, namaVendor, Status,
                    search_NpwpPenjual, search_NamaPenjual, search_NoFaktur, search_TglFaktur,
                    search_DppString, search_PpnString, search_Source, search_Status,
                    search_CheckingStatus, search_CheckingCount, search_CheckingStart, search_CheckingLast, search_IsByPass);
            }

            return data;

        }


        public static bool ByPassValidasiDelvi(string CollectionFPdjpID, string userName)
        {
            var result = false;
            var sp = new SpBase(string.Format(@"EXEC sp_ByPassDJPDelvi @CollectionFakturID"));
            sp.AddParameter("CollectionFakturID", CollectionFPdjpID);
            var execSp = sp.ExecuteNonQuery();

            if (execSp == 0)
                result = LogPostingTanggalLaporans.SetByPassLogPosting(CollectionFPdjpID, true,userName);

            if (result)
                result = LogPostingTanggalLaporans.SetByPassOnFakturPajak(CollectionFPdjpID, true, userName);


            return result;
        }

        public static string CheckingStatusPendingValidation(string CollectionFPdjpID)
        {
            var sp = new SpBase(@"
                SELECT  ISNULL(STUFF((
	                SELECT ', ' + FPdjpNumber FROM ADM_DELVI.DSTDELVI.dbo.T_TaxInvoice_FakturPajakDJP WHERE Confirm_Status NOT IN (3,2)
	                AND FPdjpID  IN (SELECT Data FROM dbo.Split(@CollectionFakturID)) FOR XML PATH('')
                ),1,1,''),'') AS Result
            ");
            sp.AddParameter("CollectionFakturID", CollectionFPdjpID);
            
            return sp.ExecuteScalar().ToString();
        }
    }
}
