using System.Collections.Generic;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;

namespace eFakturADM.Logic.Collections
{
    public class TahunPajaks : ApplicationCollection<TahunPajak, SpBase>
    {
        public static List<TahunPajak> GetAll()
        {
            var sp = new SpBase(@"SELECT	DISTINCT TahunPajak AS [Year]
FROM	dbo.OpenClosePeriod ocp
WHERE	IsDeleted = 0");
            return GetApplicationCollection(sp);
        }

        public static List<TahunPajak> GetOpenRegularByMasaPajak(int month)
        {
            //var sp = new SpBase(@"exec sp_m_GetTahunPajakForDropdown @month");
            //sp.AddParameter("month", month);

            var sp = new SpBase(@"exec sp_m_GetTahunPajakForDropdown " + month);


            //var sp = new SpBase(@"SELECT	DISTINCT a.TahunPajak AS [Year]
            //from (
            //select top 12 * from dbo.OpenClosePeriod 
            //where IsDeleted = 0 
            //order by OpenClosePeriodId desc) a
            //where a.MasaPajak = @month");
            //sp.AddParameter("month", month);


            //            var sp = new SpBase(@"SELECT	DISTINCT TahunPajak AS [Year]
            //FROM	dbo.OpenClosePeriod ocp
            //WHERE	IsDeleted = 0 AND MasaPajak = @month
            //		AND StatusRegular = 1");
            //sp.AddParameter("month", month);

            return GetApplicationCollection(sp);
        }

        public static List<TahunPajak> GetCloseRegularByMasaPajak(int month)
        {
            var sp = new SpBase(@"SELECT	DISTINCT TahunPajak AS [Year]
FROM	dbo.OpenClosePeriod ocp
WHERE	IsDeleted = 0 AND MasaPajak = @month
		AND (StatusRegular = 0 OR StatusRegular IS NULL)");

            sp.AddParameter("month", month);

            return GetApplicationCollection(sp);
        }

        public static List<TahunPajak> GetByMasaPajak(int month)
        {
            var sp = new SpBase(@"SELECT	DISTINCT TahunPajak AS [Year]
FROM	dbo.OpenClosePeriod ocp
WHERE	IsDeleted = 0 AND MasaPajak = @month");

            sp.AddParameter("month", month);

            return GetApplicationCollection(sp);
        }

    }
}
