using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using eFakturADM.Data;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Logic.Utilities;

namespace eFakturADM.Logic.Collections
{
    public class CompEvisSaps : ApplicationCollection<CompEvisSap, SpBase>
    {

        public static List<CompEvisSap> GetCompareList(Filter filter, out int totalItems, DateTime? postingDateStart, DateTime? postingDateEnd
            , DateTime? tglFakturStart, DateTime? tglFakturEnd, string noFakturStart, string noFakturEnd, DateTime? scanDate, string userName
            , int? masaPajak, int? tahunPajak, int? statusId, int? statusPosting)
        {
            totalItems = 0;
            //string a = @"EXEC sp_CompEvisVsSapGetGenerate " + postingDateStart + "," + @postingDateEnd + "," + tglFakturStart + "," + tglFakturEnd + 
            //                    ",'" + noFakturStart + "','" + noFakturEnd + "','" + scanDate + "','" + userName + "'" +
            //                    "," + masaPajak + "," + tahunPajak + "," + statusId + "," + statusPosting + "," + filter.CurrentPage + "," + filter.ItemsPerPage;
            var sp =
            //new SpBase(a);

            new SpBase(
                    @"EXEC sp_CompEvisVsSapGetGenerate3_ori @postingDateStart = @postingDateStart,
                    @postingDateEnd = @postingDateEnd,
                    @tglFakturStart = @tglFakturStart,
                    @tglFakturEnd = @tglFakturEnd,
                    @noFakturStart = @noFakturStart,
                    @noFakturEnd = @noFakturEnd,
                    @scanDate = @scanDate,
                    @userName = @userName,
                    @masaPajak = @masaPajak,
                    @tahunPajak = @tahunPajak,
                    @statusId = @statusId,
                    @statusPosting = @statusPosting,
                    @CurrentPage = @CurrentPage,
                    @ItemPerPage = @ItemPerPage");

            sp.AddParameter("postingDateStart", postingDateStart.HasValue ? postingDateStart.Value : SqlDateTime.Null);
            sp.AddParameter("postingDateEnd", postingDateEnd.HasValue ? postingDateEnd.Value : SqlDateTime.Null);
            sp.AddParameter("tglFakturStart", tglFakturStart.HasValue ? tglFakturStart.Value : SqlDateTime.Null);
            sp.AddParameter("tglFakturEnd", tglFakturEnd.HasValue ? tglFakturEnd.Value : SqlDateTime.Null);
            sp.AddParameter("noFakturStart", string.IsNullOrEmpty(noFakturStart) ? SqlString.Null : noFakturStart.Replace('*','%'));
            sp.AddParameter("noFakturEnd", string.IsNullOrEmpty(noFakturEnd) ? SqlString.Null : noFakturEnd.Replace('*', '%'));
            sp.AddParameter("scanDate", scanDate.HasValue ? scanDate.Value : SqlDateTime.Null);
            sp.AddParameter("userName", string.IsNullOrEmpty(userName) ? SqlString.Null : userName);
            sp.AddParameter("masaPajak", masaPajak.HasValue ? masaPajak.Value : SqlInt32.Null);
            sp.AddParameter("tahunPajak", tahunPajak.HasValue ? tahunPajak.Value : SqlInt32.Null);
            sp.AddParameter("CurrentPage", filter.CurrentPage);
            sp.AddParameter("ItemPerPage", filter.ItemsPerPage);
            sp.AddParameter("statusId", statusId.HasValue ? statusId.Value : SqlInt32.Null);
            sp.AddParameter("statusPosting", statusPosting.HasValue ? statusPosting.Value : SqlInt32.Null);

            var data = GetApplicationCollection(sp);

            if (data.Count > 0)
                totalItems = data[0].TotalItems;

            if (totalItems == 0)
            {
                //force return jika tidak ada row
                return new List<CompEvisSap>();
            }

            if (totalItems == 0 && filter.CurrentPage > 1)
            {
                filter.CurrentPage--;
                data = GetCompareList(filter, out totalItems, postingDateStart, postingDateEnd, tglFakturStart,
                    tglFakturEnd, noFakturStart, noFakturEnd, scanDate, userName, masaPajak, tahunPajak, statusId, statusPosting);
            }
            else if (totalItems > 0 && totalItems < (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = (totalItems <= filter.ItemsPerPage) ? 1 : (totalItems / filter.ItemsPerPage);
                data = GetCompareList(filter, out totalItems, postingDateStart, postingDateEnd, tglFakturStart,
                    tglFakturEnd, noFakturStart, noFakturEnd, scanDate, userName, masaPajak, tahunPajak, statusId, statusPosting);
            }
            else if (totalItems > 0 && totalItems == (filter.CurrentPage - 1) * filter.ItemsPerPage)
            {
                filter.CurrentPage = 1;
                data = GetCompareList(filter, out totalItems, postingDateStart, postingDateEnd, tglFakturStart,
                    tglFakturEnd, noFakturStart, noFakturEnd, scanDate, userName, masaPajak, tahunPajak, statusId, statusPosting);
            }

            return data;

        }

        public static List<CompEvisSap> GetCompareListToDownload(DateTime? postingDateStart, DateTime? postingDateEnd
            , DateTime? tglFakturStart, DateTime? tglFakturEnd, string noFakturStart, string noFakturEnd, DateTime? scanDate, string userName
            , int? masaPajak, int? tahunPajak, int? statusId, int? statusPosting)
        {
            var sp =
                new SpBase(
                    @"EXEC sp_CompEvisVsSapGetGenerateToDownload @postingDateStart = @postingDateStart,
@postingDateEnd = @postingDateEnd,
@tglFakturStart = @tglFakturStart,
@tglFakturEnd = @tglFakturEnd,
@noFakturStart = @noFakturStart,
@noFakturEnd = @noFakturEnd,
@scanDate = @scanDate,
@userName = @userName,
@masaPajak = @masaPajak,
@tahunPajak = @tahunPajak,
@statusPosting = @statusPosting,
@statusId = @statusId");

            sp.AddParameter("postingDateStart", postingDateStart.HasValue ? postingDateStart.Value : SqlDateTime.Null);
            sp.AddParameter("postingDateEnd", postingDateEnd.HasValue ? postingDateEnd.Value : SqlDateTime.Null);
            sp.AddParameter("tglFakturStart", tglFakturStart.HasValue ? tglFakturStart.Value : SqlDateTime.Null);
            sp.AddParameter("tglFakturEnd", tglFakturEnd.HasValue ? tglFakturEnd.Value : SqlDateTime.Null);
            sp.AddParameter("noFakturStart", string.IsNullOrEmpty(noFakturStart) ? SqlString.Null : noFakturStart);
            sp.AddParameter("noFakturEnd", string.IsNullOrEmpty(noFakturEnd) ? SqlString.Null : noFakturEnd);
            sp.AddParameter("scanDate", scanDate.HasValue ? scanDate.Value : SqlDateTime.Null);
            sp.AddParameter("userName", string.IsNullOrEmpty(userName) ? SqlString.Null : userName);
            sp.AddParameter("masaPajak", masaPajak.HasValue ? masaPajak.Value : SqlInt32.Null);
            sp.AddParameter("tahunPajak", tahunPajak.HasValue ? tahunPajak.Value : SqlInt32.Null);
            sp.AddParameter("statusId", statusId.HasValue ? statusId.Value : SqlInt32.Null);
            sp.AddParameter("statusPosting", statusPosting.HasValue ? statusPosting.Value : SqlInt32.Null);

            var data = GetApplicationCollection(sp);

            return data;

        }

        public static List<CompEvisSap> GetByIds(string ids)
        {
            var sp = new SpBase(@"EXEC dbo.sp_CompEvisVsSapGetByIds @ids = @idsParam");
            sp.AddParameter("idsParam", ids);

            return GetApplicationCollection(sp);

        }

        public static List<long> Submit(List<CompEvisSap> dats, string userNameLogin, out string idNo)
        {
            var objToRet = new List<long>();
            idNo = Guid.NewGuid().ToString();

            //save Compare efis vs sap
            foreach (var item in dats)
            {
                item.Pembetulan = item.Pembetulan;
                var resultId = Insert(item, idNo, userNameLogin);
                objToRet.Add(resultId);
            }

            return objToRet;

        }

        public static void SaveSubmitBulk(List<CompEvisSap> dats, string userNameLogin, out string idNo) {            
            idNo = Guid.NewGuid().ToString();

            sqlsrv SQL = null;

            var dsParamT = new dsParamTable();
            foreach (var item in dats)
            {
                var dRow = dsParamT.CompEvisSapParamTable.NewCompEvisSapParamTableRow();
                if (item.PostingDate.HasValue)
                {
                    dRow.PostingDate = item.PostingDate.Value;
                }
                dRow.AccountingDocNo = item.AccountingDocNo;
                dRow.ItemNo = item.ItemNo;
                if (item.TglFaktur.HasValue)
                {
                    dRow.TglFaktur = item.TglFaktur.Value;
                }
                dRow.TaxInvoiceNumberEVIS = item.TaxInvoiceNumberEvis;
                dRow.TaxInvoiceNumberSAP = item.TaxInvoiceNumberSap;
                dRow.DocumentHeaderText = item.DocumentHeaderText;
                dRow.NPWP = item.Npwp;
                if (item.AmountEvis.HasValue)
                {
                    dRow.AmountEVIS = item.AmountEvis.Value;
                }
                if (item.AmountSap.HasValue)
                {
                    dRow.AmountSAP = item.AmountSap.Value;
                }
                if (item.AmountDiff.HasValue)
                {
                    dRow.AmountDiff = item.AmountDiff.Value;
                }
                dRow.StatusCompare = item.StatusCompare;
                dRow.Notes = item.Notes;
                dRow.CreatedBy = userNameLogin;
                dRow.IdNo = idNo;
                if (item.MasaPajak.HasValue)
                {
                    dRow.MasaPajak = item.MasaPajak.Value;
                }
                if (item.TahunPajak.HasValue)
                {
                    dRow.TahunPajak = item.TahunPajak.Value;
                }
                dRow.ItemText = item.ItemText;
                if (item.FiscalYearDebet.HasValue)
                {
                    dRow.FiscalYearDebet = item.FiscalYearDebet.Value;
                }
                dRow.GLAccount = item.GLAccount;
                dRow.NPWPPenjual = item.NPWPPenjual;
                dRow.StatusFaktur = item.StatusFaktur;
                dsParamT.CompEvisSapParamTable.AddCompEvisSapParamTableRow(dRow);
            }
            try
            {
                SQL = new sqlsrv();

                string spname;
                string tabletypename;

                spname = "sp_CompEvisVsSap_BulkInsert";
                tabletypename = "@data";
                SQL.InsertTable_cmd(spname, tabletypename, dsParamT.CompEvisSapParamTable);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                SQL.close_con();
            }
        }

        private static long Insert(CompEvisSap dat, string idNo, string userCreated)
        {

            var sp = new SpBase(@"EXEC sp_CompEvisVsSap_Insert @PostingDate = @PostingDateParam, 
@AccountingDocNo = @AccountingDocNoParam, 
@ItemNo = @ItemNoParam, 
@TglFaktur = @TglFakturParam,
@TaxInvoiceNumberEVIS = @TaxInvoiceNumberEVISParam,
@TaxInvoiceNumberSAP = @TaxInvoiceNumberSAPParam,
@DocumentHeaderText = @DocumentHeaderTextParam,
@NPWP = @NPWPParam,
@AmountEVIS = @AmountEVISParam,
@AmountSAP = @AmountSAPParam,
@AmountDiff = @AmountDiffParam,
@StatusCompare = @StatusCompareParam,
@Notes = @NotesParam,
@CreatedBy = @CreatedByParam,
@IdNo = @IdNoParam,
@MasaPajak = @MasaPajakParam,
@TahunPajak = @TahunPajakParam,
@ItemText = @ItemTextParam,
@FiscalYearDebet = @FiscalYearDebetParam,
@GLAccount = @GLAccountParam,
@NPWPPenjual = @NPWPPenjualParam,
@StatusFaktur = @StatusFakturParam,
@CompEvisSapId = @CompEvisSapIdParam OUTPUT");

            long compEvisSapId = 0;

            sp.AddParameter("CompEvisSapIdParam", compEvisSapId, ParameterDirection.Output);

            sp.AddParameter("PostingDateParam", dat.PostingDate.HasValue ? dat.PostingDate.Value : SqlDateTime.Null);
            sp.AddParameter("AccountingDocNoParam", string.IsNullOrEmpty(dat.AccountingDocNo) ? SqlString.Null : dat.AccountingDocNo);
            sp.AddParameter("ItemNoParam", string.IsNullOrEmpty(dat.ItemNo) ? SqlString.Null : dat.ItemNo);
            sp.AddParameter("TglFakturParam", dat.TglFaktur.HasValue ? dat.TglFaktur.Value : SqlDateTime.Null);
            sp.AddParameter("TaxInvoiceNumberEVISParam", string.IsNullOrEmpty(dat.TaxInvoiceNumberEvis) ? SqlString.Null : dat.TaxInvoiceNumberEvis);
            sp.AddParameter("TaxInvoiceNumberSAPParam", string.IsNullOrEmpty(dat.TaxInvoiceNumberSap) ? SqlString.Null : dat.TaxInvoiceNumberSap);
            sp.AddParameter("DocumentHeaderTextParam", string.IsNullOrEmpty(dat.DocumentHeaderText) ? SqlString.Null : dat.DocumentHeaderText);
            sp.AddParameter("NPWPParam", string.IsNullOrEmpty(dat.Npwp) ? SqlString.Null : dat.Npwp);
            sp.AddParameter("AmountEVISParam", dat.AmountEvis.HasValue ? dat.AmountEvis.Value : SqlDecimal.Null);
            sp.AddParameter("AmountSAPParam", dat.AmountSap.HasValue ? dat.AmountSap.Value : SqlDecimal.Null);
            sp.AddParameter("AmountDiffParam", dat.AmountDiff.HasValue ? dat.AmountDiff.Value : SqlDecimal.Null);
            sp.AddParameter("StatusCompareParam", string.IsNullOrEmpty(dat.StatusCompare) ? SqlString.Null : dat.StatusCompare);
            sp.AddParameter("NotesParam", string.IsNullOrEmpty(dat.Notes) ? SqlString.Null : dat.Notes);
            sp.AddParameter("CreatedByParam", userCreated);
            sp.AddParameter("IdNoParam", idNo);
            sp.AddParameter("ItemTextParam", string.IsNullOrEmpty(dat.ItemText) ? SqlString.Null : dat.ItemText);
            sp.AddParameter("MasaPajakParam", dat.MasaPajak.HasValue ? dat.MasaPajak.Value : SqlInt32.Null);
            sp.AddParameter("TahunPajakParam", dat.TahunPajak.HasValue ? dat.TahunPajak.Value : SqlInt32.Null);
            sp.AddParameter("FiscalYearDebetParam", dat.FiscalYearDebet.HasValue ? dat.FiscalYearDebet.Value : SqlInt32.Null);
            sp.AddParameter("GLAccountParam", string.IsNullOrEmpty(dat.GLAccount) ? SqlString.Null : dat.GLAccount);
            sp.AddParameter("NPWPPenjualParam", string.IsNullOrEmpty(dat.NPWPPenjual) ? SqlString.Null : dat.NPWPPenjual);
            sp.AddParameter("StatusFakturParam", string.IsNullOrEmpty(dat.StatusFaktur) ? SqlString.Null : dat.StatusFaktur);
            sp.ExecuteNonQuery();


            compEvisSapId = (long)sp.GetParameter("CompEvisSapIdParam");
            return compEvisSapId;
        }
        
        public static List<CompEvisSap> GetByIdNo(string idNo)
        {
            var sp = new SpBase(@"EXEC dbo.sp_CompEvisVsSapGetByIdNo @idNo=@idNoParam");
            sp.AddParameter("idNoParam", idNo);

            return GetApplicationCollection(sp);

        }

        public static int? GetStatusReconcileByKey(string accountingDocNo, string lineItem, DateTime postingDate, string itemText)
        {
            var sp =
                new SpBase(@"EXEC sp_MTDownloadPPN_GetStatusReconcileByKey @AccountingDocNo = @AccountingDocNoParam, 
@LineItem = @LineItemParam,
@PostingDate = @PostingDateParam,
@ItemText = @ItemTextParam,
@StatusReconcile = @StatusReconcileParam OUTPUT");

            sp.AddParameter("StatusReconcileParam", SqlDbType.Int, ParameterDirection.Output);
            sp.AddParameter("ItemTextParam", itemText);
            sp.AddParameter("PostingDateParam", postingDate);
            sp.AddParameter("AccountingDocNoParam", accountingDocNo);
            sp.AddParameter("LineItemParam", lineItem);
            sp.ExecuteNonQuery();

            var statusReconcile = sp.GetParameter("StatusReconcileParam");
            if (statusReconcile == null) return null;

            return (int?)statusReconcile;

        }

        public static CompEvisSap GetByIdNoAndTaxInvoiceNumberEvis(string idNo, string taxInvoiceNumberEvis,
            string accountingDocNo)
        {
            var sp = new SpBase(@"EXEC dbo.sp_CompEvisVsSap_GetByIdNoAndTaxInvoiceNumberEvis @IdNo = @IdNoParam, @TaxInvoiceNumberEvis = @TaxInvoiceNumberEvisParam, @AccountingDocNo = @AccountingDocNoParam");
            sp.AddParameter("IdNoParam", idNo);
            sp.AddParameter("TaxInvoiceNumberEvisParam", taxInvoiceNumberEvis);
            sp.AddParameter("AccountingDocNoParam", accountingDocNo);

            var dbData = GetApplicationObject(sp);
            return dbData == null || dbData.Id == 0 ? null : dbData;
        }

        public static void UpdateStatusReconcileByIdNo(string idNo, int newStatus, string userModifier)
        {
            var sp = new SpBase(@"EXEC dbo.sp_CompEvisVsSap_UpdateReconcile @idNo = @idNoParam, @newStatus = @newStatusparamm, @userModifier = @userModifierParam");
            sp.AddParameter("idNoParam", idNo);
            sp.AddParameter("newStatusparamm", newStatus);
            sp.AddParameter("userModifierParam", userModifier);

            sp.ExecuteNonQuery();
        }

    }
}
