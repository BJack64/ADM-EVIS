using System.Collections.Generic;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;

namespace eFakturADM.Logic.Collections
{
    public class FormatingDomains : ApplicationCollection<FormatingDomain, SpBase>
    {
        public static List<FormatingDomain> GetFormatNpwp(List<string> originalNpwp)
        {
            var sp = new SpBase(@"SELECT	Data AS OriginalField, dbo.FormatNpwp(Data) AS FormattedField
FROM	dbo.Split(@originalNpwp)");
            var originalNpwps = string.Join(",", originalNpwp);
            sp.AddParameter("originalNpwp", originalNpwps);
            return GetApplicationCollection(sp);
        }

        public static FormatingDomain GetFormatNoFaktur(ApplicationEnums.FPType eFpType, string kdJenistransaksi, string fgPengganti, string noFakturPajak)
        {
            var sp = new SpBase(@"SELECT @noFakturPajak AS OriginalField, dbo.FormatNoFaktur(@fpType, @noFakturPajak, @kdJenistransaksi, @fgPengganti) AS FormattedField");

            sp.AddParameter("noFakturPajak", noFakturPajak);
            sp.AddParameter("kdJenistransaksi", kdJenistransaksi);
            sp.AddParameter("fgPengganti", fgPengganti);
            sp.AddParameter("fpType", (int) eFpType);

            return GetApplicationObject(sp);

        }

    }
}
