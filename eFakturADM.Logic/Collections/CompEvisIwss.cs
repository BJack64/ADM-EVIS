using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Collections
{
    public class CompEvisIwss : ApplicationCollection<CompEvisIws, SpBase>
    {

        public static bool GenerateCompareByReceivingDate(DateTime receivingDateStart, DateTime receivingDateEnd, string userNameLogin, string scanUserName, DateTime? scanDate)
        {
            var sp = new SpBase(@"EXEC sp_CompEvisVsIwsGenerateByReceivingDate @receivingDateStart, @receivingDateEnd, @scanDate, @filterUser, @userNameLogin");
            sp.AddParameter("receivingDateStart", receivingDateStart);
            sp.AddParameter("receivingDateEnd", receivingDateEnd);
            sp.AddParameter("userNameLogin", userNameLogin);
            sp.AddParameter("scanDate", scanDate.HasValue ? scanDate.Value : SqlDateTime.Null);
            sp.AddParameter("filterUser", string.IsNullOrEmpty(scanUserName) ? SqlString.Null : scanUserName);

            return sp.ExecuteNonQuery() >= 0;

        }

        public static bool DeleteByTaxInvoiceNumberEvis(string taxNoEvis, string by)
        {
            var sp = new SpBase(@"UPDATE COMP_EVIS_IWS SET IsDeleted = 1, Modified = GETDATE(), ModifiedBy = @ModifiedBy WHERE IsDeleted = 0 AND TaxInvoiceNumberEVIS = @taxNoEvis");
            sp.AddParameter("taxNoEvis", taxNoEvis);
            sp.AddParameter("ModifiedBy", by);
            return sp.ExecuteNonQuery() == 0;
        }

        public static List<CompEvisIws> GetByReceivingDate(Filter filter, out int totalItems, DateTime receivingDateStart, DateTime receivingDateEnd, int? statusId, string scanUserName, DateTime? scanDate)
        {
            if (filter.Search != null)
                filter.Search = filter.Search.Trim();
            var sortOrder = filter.SortOrderAsc ? "ASC" : "DESC";
            totalItems = 0;

            var sp = new SpBase(string.Format(@"SELECT [Id]
		  ,[ReceivedDate]
		  ,[VendorCode]
		  ,[VendorName]
		  ,[TaxInvoiceNumberEVIS]
		  ,[TaxInvoiceNumberIWS]
		  ,[InvoiceNumber]
		  ,[VATAmountScanned]
		  ,[VATAmountIWS]
		  ,[VATAmountDiff]
		  ,[StatusDJP]
		  ,[StatusCompare]
		  ,[Notes]
		  ,[IsDeleted]
		  ,[Created]
		  ,[Modified]
		  ,[CreatedBy]
		  ,[ModifiedBy]
		  ,[LastUpdatedOnIws]
          ,COUNT([Id]) OVER() AS TotalItems
          ,ScanDate
		  ,ScanUserName
	FROM [dbo].[COMP_EVIS_IWS]
	WHERE [IsDeleted] = 0 AND CAST([ReceivedDate] as date) BETWEEN CAST(@receivingDateStart as date) AND CAST(@receivingDateEnd as date)
            AND (@scandate IS NULL OR (@scandate IS NOT NULL AND CONVERT(date, @scandate) = CONVERT(date, ScanDate)))
			AND (@scanUserName IS NULL OR (@scanUserName IS NOT NULL AND ScanUserName LIKE '%' + @scanUserName + '%'))
			AND (@statusId IS NULL 
				OR (@statusId IS NOT NULL AND @statusId = 1 AND LOWER([StatusCompare]) = 'ok')
				OR (@statusId IS NOT NULL AND @statusId = 2 AND LOWER([StatusCompare]) = 'ok' AND [Notes] IS NOT NULL)
				OR (@statusId IS NOT NULL AND @statusId = 3 AND LOWER([StatusCompare]) <> 'ok')
				)
			AND (@search IS NULL
				OR LOWER([VendorCode]) LIKE '%' + LOWER(@search) + '%'
				OR LOWER([VendorName]) LIKE '%' + LOWER(@search) + '%'
				OR LOWER([TaxInvoiceNumberEVIS]) LIKE '%' + LOWER(@search) + '%'
				OR LOWER([TaxInvoiceNumberIWS]) LIKE '%' + LOWER(@search) + '%'
				OR LOWER([InvoiceNumber]) LIKE '%' + LOWER(@search) + '%'
				OR LOWER([StatusDJP]) LIKE '%' + LOWER(@search) + '%'
				OR LOWER([StatusCompare]) LIKE '%' + LOWER(@search) + '%'
				OR LOWER([Notes]) LIKE '%' + LOWER(@search) + '%'
				)
	ORDER BY {0} {1}
	OFFSET (@CurrentPage-1)*@ItemPerPage ROWS
	FETCH NEXT @ItemPerPage ROWS ONLY", filter.SortColumnName, sortOrder));

            sp.AddParameter("receivingDateStart", receivingDateStart);
            sp.AddParameter("receivingDateEnd", receivingDateEnd);
            sp.AddParameter("statusId", statusId.HasValue ? statusId.Value : SqlInt32.Null);
            sp.AddParameter("CurrentPage", filter.CurrentPage);
            sp.AddParameter("ItemPerPage", filter.ItemsPerPage);
            sp.AddParameter("search", string.IsNullOrEmpty(filter.Search) ? SqlString.Null : filter.Search);
            sp.AddParameter("scandate", scanDate.HasValue ? scanDate.Value : SqlDateTime.Null);
            sp.AddParameter("scanUserName", string.IsNullOrEmpty(scanUserName) ? SqlString.Null : scanUserName);

            var dbData = GetApplicationCollection(sp);
            if (dbData.Count > 0)
            {
                totalItems = dbData.First().TotalItems;
            }

            if (totalItems == 0 && filter.CurrentPage > 1)
            {
                filter.CurrentPage--;
                dbData = GetByReceivingDate(filter, out totalItems, receivingDateStart, receivingDateEnd, statusId, scanUserName, scanDate);
            }
            else if (totalItems > 0 && totalItems < (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = (totalItems <= filter.ItemsPerPage) ? 1 : (totalItems / filter.ItemsPerPage);
                dbData = GetByReceivingDate(filter, out totalItems, receivingDateStart, receivingDateEnd, statusId, scanUserName, scanDate);
            }
            else if (totalItems > 0 && totalItems == (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = 1;
                dbData = GetByReceivingDate(filter, out totalItems, receivingDateStart, receivingDateEnd, statusId, scanUserName, scanDate);
            }

            return dbData;

        }

        public static List<CompEvisIws> GetByReceivingDateWithoutPaging(string sortColumnName, bool isAsc, DateTime receivingDateStart, DateTime receivingDateEnd, int? statusId, string scanUserName, DateTime? scanDate)
        {
            var sortOrder = isAsc ? "ASC" : "DESC";
            var sp = new SpBase(string.Format(@"SELECT [Id]
		  ,[ReceivedDate]
		  ,[VendorCode]
		  ,[VendorName]
		  ,[TaxInvoiceNumberEVIS]
		  ,[TaxInvoiceNumberIWS]
		  ,[InvoiceNumber]
		  ,[VATAmountScanned]
		  ,[VATAmountIWS]
		  ,[VATAmountDiff]
		  ,[StatusDJP]
		  ,[StatusCompare]
		  ,[Notes]
		  ,[IsDeleted]
		  ,[Created]
		  ,[Modified]
		  ,[CreatedBy]
		  ,[ModifiedBy]
		  ,[LastUpdatedOnIws]
          ,COUNT([Id]) OVER() AS TotalItems
          ,ScanDate
		  ,ScanUserName
	FROM [dbo].[COMP_EVIS_IWS]
	WHERE [IsDeleted] = 0 AND CAST([ReceivedDate] as date) BETWEEN CAST(@receivingDateStart as date) AND CAST(@receivingDateEnd as date)
            AND (@scandate IS NULL OR (@scandate IS NOT NULL AND CONVERT(date, @scandate) = CONVERT(date, ScanDate)))
			AND (@scanUserName IS NULL OR (@scanUserName IS NOT NULL AND ScanUserName LIKE '%' + @scanUserName + '%'))
			AND (@statusId IS NULL 
				OR (@statusId IS NOT NULL AND @statusId = 1 AND LOWER([StatusCompare]) = 'ok')
				OR (@statusId IS NOT NULL AND @statusId = 2 AND LOWER([StatusCompare]) = 'ok' AND [Notes] IS NOT NULL)
				OR (@statusId IS NOT NULL AND @statusId = 3 AND LOWER([StatusCompare]) <> 'ok')
				)
	ORDER BY {0} {1}", sortColumnName, sortOrder));

            sp.AddParameter("receivingDateStart", receivingDateStart);
            sp.AddParameter("receivingDateEnd", receivingDateEnd);
            sp.AddParameter("statusId", statusId.HasValue ? statusId.Value : SqlInt32.Null);
            sp.AddParameter("scandate", scanDate.HasValue ? scanDate.Value : SqlDateTime.Null);
            sp.AddParameter("scanUserName", string.IsNullOrEmpty(scanUserName) ? SqlString.Null : scanUserName);

            return GetApplicationCollection(sp);

        }

        public static bool UpdateNotesById(long id, string notes, string userModify)
        {
            var sp = new SpBase(@"UPDATE COMP_EVIS_IWS
SET Notes = @Notes
	, Modified = GETDATE()
	, ModifiedBy = @UserModify
WHERE [Id] = @Id");
            sp.AddParameter("Notes", notes);
            sp.AddParameter("UserModify", userModify);
            sp.AddParameter("Id", id);
            return sp.ExecuteNonQuery() >= 0;
        }

        public static CompEvisIws GetById(long id)
        {
            var sp = new SpBase(@"SELECT [Id]
		  ,[ReceivedDate]
		  ,[VendorCode]
		  ,[VendorName]
		  ,[TaxInvoiceNumberEVIS]
		  ,[TaxInvoiceNumberIWS]
		  ,[InvoiceNumber]
		  ,[VATAmountScanned]
		  ,[VATAmountIWS]
		  ,[VATAmountDiff]
		  ,[StatusDJP]
		  ,[StatusCompare]
		  ,[Notes]
		  ,[IsDeleted]
		  ,[Created]
		  ,[Modified]
		  ,[CreatedBy]
		  ,[ModifiedBy]
		  ,[LastUpdatedOnIws]
          ,COUNT([Id]) OVER() AS TotalItems
          ,ScanDate
		  ,ScanUserName
	FROM [dbo].[COMP_EVIS_IWS]
	WHERE [Id] = @Id");

            sp.AddParameter("Id", id);
            var dbData = GetApplicationObject(sp);
            return dbData == null || dbData.Id != id ? null : dbData;

        }

        public static CompEvisIws GetByNoFakturPajak(string formatedNoFaktur)
        {
            var sp = new SpBase(@"SELECT [Id]
		  ,[ReceivedDate]
		  ,[VendorCode]
		  ,[VendorName]
		  ,[TaxInvoiceNumberEVIS]
		  ,[TaxInvoiceNumberIWS]
		  ,[InvoiceNumber]
		  ,[VATAmountScanned]
		  ,[VATAmountIWS]
		  ,[VATAmountDiff]
		  ,[StatusDJP]
		  ,[StatusCompare]
		  ,[Notes]
		  ,[IsDeleted]
		  ,[Created]
		  ,[Modified]
		  ,[CreatedBy]
		  ,[ModifiedBy]
		  ,[LastUpdatedOnIws]
          ,COUNT([Id]) OVER() AS TotalItems
          ,ScanDate
		  ,ScanUserName
	FROM [dbo].[COMP_EVIS_IWS]
WHERE TaxInvoiceNumberEVIS = @formatedNoFaktur AND IsDeleted = 0");

            sp.AddParameter("formatedNoFaktur", formatedNoFaktur);

            var dbData = GetApplicationObject(sp);
            return dbData == null || dbData.Id != 0 ? null : dbData;

        }

    }
}
