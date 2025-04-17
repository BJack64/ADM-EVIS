using System.Collections.Generic;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;

namespace eFakturADM.Logic.Collections
{
    public class MasaPajaks : ApplicationCollection<MasaPajak, SpBase>
    {
        public static List<MasaPajak> GetAll()
        {

            var sp = new SpBase(@"SELECT	*
FROM	dbo.GetMonth()");

            return GetApplicationCollection(sp);

        }

        public static List<MasaPajak> GetOpenRegular()
        {
            var sp = new SpBase(@"SELECT	DISTINCT m.MonthNumber, m.MonthName
FROM	dbo.GetMonth() m INNER JOIN
		dbo.OpenClosePeriod ocp ON m.MonthNumber = ocp.MasaPajak
WHERE	ocp.IsDeleted = 0 AND ocp.StatusRegular = 1
ORDER BY m.MonthNumber");

            return GetApplicationCollection(sp);

        }

        public static List<MasaPajak> GetCloseRegular()
        {
            var sp = new SpBase(@"SELECT	DISTINCT m.MonthNumber, m.MonthName
FROM	dbo.GetMonth() m INNER JOIN
		dbo.OpenClosePeriod ocp ON m.MonthNumber = ocp.MasaPajak
WHERE	ocp.IsDeleted = 0 AND (ocp.StatusRegular = 0 OR ocp.StatusRegular IS NULL)
ORDER BY m.MonthNumber");

            return GetApplicationCollection(sp);

        }

        public static MasaPajak GetByMonthNumber(int iMonthNumber)
        {
            var sp = new SpBase(@"SELECT	DISTINCT m.MonthNumber, m.MonthName
FROM	dbo.GetMonth() m 
WHERE	m.MonthNumber = @MonthNumber");

            sp.AddParameter("MonthNumber", iMonthNumber);

            return GetApplicationObject(sp);

        }

        
    }
}
