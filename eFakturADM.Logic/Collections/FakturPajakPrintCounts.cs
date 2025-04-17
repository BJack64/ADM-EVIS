using System.Collections.Generic;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Shared.Utility;

namespace eFakturADM.Logic.Collections
{
    public class FakturPajakPrintCounts : ApplicationCollection<FakturPajakPrintCount, SpBase>
    {
        
        public static List<FakturPajakPrintCount> GetByFaktuPajakIds(string fakturPajakIds, ApplicationEnums.LogPrintType ePrintType)
        {
            var printType = EnumHelper.GetDescription(ePrintType);
            var sp =
                new SpBase(
                    @"SELECT	fp.FakturPajakId AS FakturPajakId, fp.NoFakturPajak, CASE WHEN lp.PrintCount IS NULL THEN 0 ELSE lp.PrintCount END AS PrintCount
FROM	dbo.Split(@fakturPajakIds) LEFT JOIN
		(
			SELECT	FakturPajakId, COUNT(LogPrintId) AS PrintCount
			FROM	dbo.LogPrintFakturPajak
			WHERE	IsDeleted = 0 AND [PrintType] = @printType
			GROUP BY FakturPajakId
		) lp ON LTRIM(RTRIM(Data)) = lp.FakturPajakId LEFT JOIN
		dbo.FakturPajak fp ON LTRIM(RTRIM(Data)) = fp.FakturPajakId");
            sp.AddParameter("fakturPajakIds", fakturPajakIds);
            sp.AddParameter("printType", printType);

            var dbData = GetApplicationCollection(sp);
            return dbData;

        }

    }
}
