using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;

namespace eFakturADM.Logic.Collections
{
    public class LogFPDigantiOutstandings : ApplicationCollection<LogFPDigantiOutstanding, SpBase>
    {
        public static bool DeleteOldLog()
        {
            var sp =
                new SpBase(
                    @"DELETE FROM LogFPDigantiOutstanding WHERE CAST(ProcessDate AS date) < CAST(GETDATE() AS date)");
            return sp.ExecuteNonQuery() >= 0;
        }

        public static bool Add(LogFPDigantiOutstanding dat)
        {
            var sp = new SpBase(@"INSERT INTO LogFPDigantiOutstanding(FormatedNoFaktur, ProcessDate)
VALUES(@FormatedNoFaktur, CAST(GETDATE() AS date))");
            sp.AddParameter("FormatedNoFaktur", dat.FormatedNoFaktur);
            return sp.ExecuteNonQuery() >= 0;
        }
    }
}
