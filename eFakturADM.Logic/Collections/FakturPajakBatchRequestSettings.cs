using System.Collections.Generic;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;

namespace eFakturADM.Logic.Collections
{
    public class FakturPajakBatchRequestSettings : ApplicationCollection<FakturPajakBatchRequestSetting, SpBase>
    {
        public static List<FakturPajakBatchRequestSetting> GetByBatchOrder(int batchorder)
        {
            var sp = new SpBase(@"SELECT [Id]
      ,[UrlScan]
      ,[FormatedNoFaktur]
      ,[BatchDate]
      ,[BatchOrder]
      ,[ProcessStatus]
  FROM [dbo].[FakturPajakBatchRequestSetting]
WHERE BatchOrder = @BatchOrder AND ProcessStatus IS NULL");

            sp.AddParameter("BatchOrder", batchorder);
            return GetApplicationCollection(sp);

        }

        public static bool UpdateStatus(int id, int status)
        {
            var sp =
                new SpBase(
                    @"UPDATE [dbo].[FakturPajakBatchRequestSetting] SET ProcessStatus = @ProcessStatus WHERE Id = @Id");
            sp.AddParameter("ProcessStatus", status);
            sp.AddParameter("Id", id);
            sp.ExecuteNonQuery();
            return true;
        }

    }
}
