using System;
using System.Collections.Generic;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;

namespace eFakturADM.Logic.Collections
{
    public class VwDataIWSReqEfises : ApplicationCollection<VwDataIWSReqEfis, SpBase>
    {
        public static List<VwDataIWSReqEfis> GetByRangReceivingDate(DateTime dtStart, DateTime dtEnd)
        {
            var sp = new SpBase(@"SELECT [INVOICEID]
      ,[RECEIVINGDATE]
      ,[STATUS]
      ,[ACTIVITY]
      ,[INVOICEDATE]
      ,[VENDORID]
      ,[VENDORNAME]
      ,[TAXVOUCHERNO]
      ,[INVOICENO]
      ,[PPN]
  FROM [vw_DataIWSReqEfis]
  where Convert(Date,[RECEIVINGDATE]) >= Convert(Date,@dtStart)  AND Convert(Date,[RECEIVINGDATE]) <= Convert(Date,@dtEnd)
OPTION (OPTIMIZE FOR UNKNOWN)");

            sp.AddParameter("dtStart", dtStart);
            sp.AddParameter("dtEnd", dtEnd);

            return GetApplicationCollection(sp);

        }

        public static int GetCountDataByReceivingDate(DateTime receivingDate)
        {
            var sp = new SpBase(@"SELECT COUNT(vw.[INVOICEID]) AS DataCount
FROM [vw_DataIWSReqEfis] vw
WHERE CAST(vw.[RECEIVINGDATE] as date) = CAST(@receivingDate as date)
OPTION (OPTIMIZE FOR UNKNOWN)");

            sp.AddParameter("receivingDate", receivingDate);

            var data = sp.ExecuteScalar().ToString();

            return !string.IsNullOrEmpty(data) ? int.Parse(data) : 0;
        }
        
    }
}
