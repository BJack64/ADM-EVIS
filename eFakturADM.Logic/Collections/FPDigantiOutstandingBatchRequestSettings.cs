using System.Collections.Generic;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;

namespace eFakturADM.Logic.Collections
{
    public class FPDigantiOutstandingBatchRequestSettings : ApplicationCollection<FPDigantiOutstandingBatchRequestSetting, SpBase>
    {
        public static List<FPDigantiOutstandingBatchRequestSetting> GetByBatchOrder(int batchorder)
        {
            var sp = new SpBase(@"SELECT [Id]
      ,[UrlScan]
      ,[FormatedNoFaktur]
      ,[BatchDate]
      ,[BatchOrder]
      ,[ProcessStatus]
  FROM [dbo].[FPDigantiOutstandingBatchRequestSetting]
WHERE BatchOrder = @BatchOrder AND ProcessStatus IS NULL");

            sp.AddParameter("BatchOrder", batchorder);
            return GetApplicationCollection(sp);

        }

        public static bool UpdateStatus(int id, int status)
        {
            var sp =
                new SpBase(
                    @"UPDATE [dbo].[FPDigantiOutstandingBatchRequestSetting] SET ProcessStatus = @ProcessStatus WHERE Id = @Id");
            sp.AddParameter("ProcessStatus", status);
            sp.AddParameter("Id", id);
            sp.ExecuteNonQuery();
            return true;
        }
    }
}
