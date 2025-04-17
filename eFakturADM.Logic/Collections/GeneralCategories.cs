using System.Collections.Generic;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;
using eFakturADM.Shared.Utility;

namespace eFakturADM.Logic.Collections
{
    public class GeneralCategories : ApplicationCollection<GeneralCategory, SpBase>
    {
        public static List<GeneralCategory> GetByCategories(ApplicationEnums.EnumGeneralCategory ecategory)
        {
            var scategory = EnumHelper.GetDescription(ecategory);
            var sp =
                new SpBase(
                    @"SELECT * FROM [dbo].[GeneralCategory] WHERE IsDeleted = 0 AND Category = @Category ORDER BY Code ASC");
            sp.AddParameter("Category", scategory);
            return GetApplicationCollection(sp);
        }
        public static List<GeneralCategory> GetByCategoriesWithExclude(ApplicationEnums.EnumGeneralCategory ecategory, string excludedcode)
        {
            var scategory = EnumHelper.GetDescription(ecategory);
            var sp =
                new SpBase(
                    @"SELECT * FROM [dbo].[GeneralCategory] WHERE IsDeleted = 0 AND Category = @Category AND Code NOT IN (SELECT CAST(items AS bigint) AS Id FROM [dbo].[SplitString](@Excludes, ',')) ORDER BY Code ASC");
            sp.AddParameter("Category", scategory);
            sp.AddParameter("Excludes", excludedcode);
            return GetApplicationCollection(sp);
        }
        
    }
}
