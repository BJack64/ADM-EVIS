using System.Collections.Generic;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;

namespace eFakturADM.Logic.Collections
{
    public class XmlUploadPpnCredits : ApplicationCollection<XmlUploadPpnCredit, SpBase>
    {
//        public static List<XmlUploadPpnCredit> GetByIds(string ids)
//        {
//            var sp = new SpBase(@"EXEC [dbo].[sp_CompEvisVsSap_GetXmlUploadPPNSubmit]
//		@ids = @idsParam");
//            sp.AddParameter("idsParam", ids);

//            var dbData = GetApplicationCollection(sp);
//            return dbData;

//        }

        public static List<XmlUploadPpnCredit> ByIdNo(string idNo)
        {
            var sp = new SpBase(@"EXEC [dbo].[sp_CompEvisVsSap_GetXmlUploadPPNSubmit_ByIdNo]
		@idNo = @idNoParam");
            sp.AddParameter("idNoParam", idNo);

            var dbData = GetApplicationCollection(sp);
            return dbData;
        }
    }
}
